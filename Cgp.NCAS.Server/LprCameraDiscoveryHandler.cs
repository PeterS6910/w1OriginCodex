using CDKDOTNET;
using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Contal.Cgp.NCAS.Server
{
    internal sealed class LprCameraDiscoveryHandler
    {
        private static readonly Lazy<LprCameraDiscoveryHandler> _instance =
            new Lazy<LprCameraDiscoveryHandler>(() => new LprCameraDiscoveryHandler());

        private readonly HashSet<Guid> _lookupingClients;
        private readonly object _syncRoot;

        private bool _isLookupRunning;

        private static readonly TimeSpan DefaultLookupTimeout = TimeSpan.FromSeconds(5);

        private LprCameraDiscoveryHandler()
        {
            _lookupingClients = new HashSet<Guid>();
            _syncRoot = new object();
        }

        public static LprCameraDiscoveryHandler Singleton => _instance.Value;

        public void Lookup(Guid clientId)
        {
            lock (_syncRoot)
            {
                _lookupingClients.Add(clientId);

                if (_isLookupRunning)
                    return;

                _isLookupRunning = true;
            }

            ThreadPool.QueueUserWorkItem(_ => PerformLookup());
        }

        private void PerformLookup()
        {
            ICollection<Guid> clients = null;
            try
            {                               
                lock (_syncRoot)
                {
                    clients = _lookupingClients.ToList();
                    _lookupingClients.Clear();
                    _isLookupRunning = false;
                }

                if (clients.Count == 0)
                    return;

                var cameras = DiscoverAllCameraTypes(DefaultLookupTimeout, CancellationToken.None);
                NotifyClients(cameras, clients);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);

                lock (_syncRoot)
                {
                    clients = _lookupingClients.ToList();
                    _lookupingClients.Clear();
                    _isLookupRunning = false;
                }

                if (clients.Count == 0)
                    return;

                NotifyClients(new List<LookupedLprCamera>(), clients);
            }
        }

        private static List<LookupedLprCamera> DiscoverAllCameraTypes( TimeSpan timeout, CancellationToken cancellationToken)
        {
            var aggregatedCameras = new Dictionary<string, LookupedLprCamera>(StringComparer.OrdinalIgnoreCase);
            var nanopackDiscovery = new Nanopack5LprCameraDiscoveryStrategy();

            DiscoverWith(
                () => nanopackDiscovery.Discover(timeout, cancellationToken),
                aggregatedCameras);

            return aggregatedCameras.Values.ToList();
        }

        private static void DiscoverWith(
            Func<IEnumerable<LookupedLprCamera>> discovery,
            IDictionary<string, LookupedLprCamera> aggregated)
        {
            try
            {
                foreach (var camera in discovery() ?? Enumerable.Empty<LookupedLprCamera>())
                {
                    if (camera == null || string.IsNullOrWhiteSpace(camera.IpAddress))
                        continue;

                    aggregated[camera.IpAddress.Trim()] = camera;
                }
            }
            catch (BadImageFormatException badImage)
            {
                HandledExceptionAdapter.Examine(badImage);
            }
            catch (DllNotFoundException dllNotFound)
            {
                HandledExceptionAdapter.Examine(dllNotFound);
            }
            catch (Exception discoveryError)
            {
                HandledExceptionAdapter.Examine(discoveryError);
            }
        }

        private static void NotifyClients(
            ICollection<LookupedLprCamera> cameras,
            ICollection<Guid> clients)
        {
            try
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    CCUCallbackRunner.RunLprCameraLookupFinished,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        cameras,
                        clients
                    });
            }
            catch (Exception notifyError)
            {
                HandledExceptionAdapter.Examine(notifyError);
            }
        }
    }

    internal sealed class Nanopack5LprCameraDiscoveryStrategy
    {
        private const int DefaultMaxDevices = 16;
        private static readonly TimeSpan DefaultResponseDelay = TimeSpan.FromSeconds(2);

        private readonly int _maxDevices;
        private readonly TimeSpan _responseDelay;

        public Nanopack5LprCameraDiscoveryStrategy()
            : this(DefaultMaxDevices, DefaultResponseDelay)
        {
        }

        public Nanopack5LprCameraDiscoveryStrategy(int maxDevices, TimeSpan responseDelay)
        {
            _maxDevices = maxDevices;
            _responseDelay = responseDelay;
        }

        public IEnumerable<LookupedLprCamera> Discover(TimeSpan timeout, CancellationToken ct)
        {
            var ctx = CDKDiscover.CDKDiscoverCreate();
            if (ctx == IntPtr.Zero) return Array.Empty<LookupedLprCamera>();

            var results = new List<LookupedLprCamera>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var started = false;

            try
            {
                if (CDKDiscover.CDKDiscoverStart(ctx) == 0) return results;
                started = true;

                // základné čakanie (warm-up) – kratšie z timeout a _responseDelay
                var initialDelay = (_responseDelay > TimeSpan.Zero &&
                                   (timeout == TimeSpan.Zero || _responseDelay < timeout))
                                   ? _responseDelay : timeout;
                if (initialDelay > TimeSpan.Zero)
                {
                    if (ct.WaitHandle.WaitOne(initialDelay)) return results;
                }

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var poll = TimeSpan.FromMilliseconds(250);
                bool rebroadcasted = false;

                while ((timeout == TimeSpan.Zero || sw.Elapsed < timeout) && !ct.IsCancellationRequested)
                {
                    var messages = new IntPtr[_maxDevices];
                    var ok = CDKDiscover.CDKDiscoverGetDiscovered(ctx, messages, out var count);

                    if (ok != 0 && count > 0)
                    {
                        for (int i = 0; i < count && i < messages.Length; i++)
                        {
                            var msg = messages[i];
                            if (msg == IntPtr.Zero) continue;

                            try
                            {
                                var el = CDKMsg.CDKMsgChild(msg);
                                if (el == IntPtr.Zero) continue;

                                var cam = new LookupedLprCamera
                                {
                                    UniqueKey = CDKMsg.CDKMsgElementAttributeValue(el, "uniqueKey"),
                                    InterfaceSource = CDKMsg.CDKMsgElementAttributeValue(el, "interfaceSource"),
                                    Name = CDKMsg.CDKMsgElementAttributeValue(el, "name"),
                                    Port = CDKMsg.CDKMsgElementAttributeValue(el, "port"),
                                    PortSsl = CDKMsg.CDKMsgElementAttributeValue(el, "portSSL"),
                                    Equipment = CDKMsg.CDKMsgElementAttributeValue(el, "equipment"),
                                    Version = CDKMsg.CDKMsgElementAttributeValue(el, "version"),
                                    Locked = CDKMsg.CDKMsgElementAttributeValue(el, "locked"),
                                    LockingClientIp = CDKMsg.CDKMsgElementAttributeValue(el, "lockingClientIP"),
                                    MacAddress = CDKMsg.CDKMsgElementAttributeValue(el, "macAddress"),
                                    Serial = CDKMsg.CDKMsgElementAttributeValue(el, "serial"),
                                    IpAddress = CDKMsg.CDKMsgElementAttributeValue(el, "ipAddress"),
                                    Model = CDKMsg.CDKMsgElementAttributeValue(el, "model"),
                                    Type = CDKMsg.CDKMsgElementAttributeValue(el, "type"),
                                    Build = CDKMsg.CDKMsgElementAttributeValue(el, "build")
                                };

                                var key = string.IsNullOrWhiteSpace(cam.UniqueKey) ? cam.IpAddress : cam.UniqueKey;
                                if (!string.IsNullOrWhiteSpace(cam.IpAddress) && seen.Add(key))
                                    results.Add(cam);
                            }
                            finally
                            {
                                try { CDKMsg.CDKMsgDestroy(msg); } catch { }
                            }
                        }

                        // ak už máme aspoň jednu, môžeme skončiť (alebo zbierať do timeoutu – podľa potreby)
                        if (results.Count > 0) break;
                    }

                    // jednorazový re-broadcast po ~1 s, keď nič nechodí
                    if (!rebroadcasted && sw.Elapsed > TimeSpan.FromSeconds(1))
                    {
                        try { CDKDiscover.CDKDiscoverStop(ctx); } catch { }
                        if (CDKDiscover.CDKDiscoverStart(ctx) != 0) rebroadcasted = true;
                    }

                    // čakaj ďalšie kolo
                    var remaining = (timeout == TimeSpan.Zero) ? poll : TimeSpan.FromMilliseconds(
                        Math.Max(0, (timeout - sw.Elapsed).TotalMilliseconds));
                    if (remaining <= TimeSpan.Zero) break;
                    ct.WaitHandle.WaitOne(remaining > poll ? poll : remaining);
                }
            }
            finally
            {
                StopAndDestroyDiscoverContext(ctx, started);
            }

            return results;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private static void StopAndDestroyDiscoverContext(IntPtr ctx, bool started)
        {
            if (ctx == IntPtr.Zero)
                return;

            if (started)
            {
                try
                {
                    CDKDiscover.CDKDiscoverStop(ctx);
                }
                catch (DllNotFoundException dllError)
                {
                    HandledExceptionAdapter.Examine(dllError);
                }
                catch (BadImageFormatException badImage)
                {
                    HandledExceptionAdapter.Examine(badImage);
                }
                catch (SEHException seh)
                {
                    HandledExceptionAdapter.Examine(seh);
                }
                catch (AccessViolationException accessViolation)
                {
                    HandledExceptionAdapter.Examine(accessViolation);
                    return;
                }
            }

            try
            {
                CDKDiscover.CDKDiscoverDestroy(ctx);
            }
            catch (DllNotFoundException dllError)
            {
                HandledExceptionAdapter.Examine(dllError);
            }
            catch (BadImageFormatException badImage)
            {
                HandledExceptionAdapter.Examine(badImage);
            }
            catch (SEHException seh)
            {
                HandledExceptionAdapter.Examine(seh);
            }
            catch (AccessViolationException accessViolation)
            {
                HandledExceptionAdapter.Examine(accessViolation);
            }
        }
    }
}
