using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CDKDOTNET;

namespace Contal.Cgp.NCAS.Server
{
    internal interface ILprCameraDiscoveryStrategy
    {
        string Name { get; }

        IEnumerable<LookupedLprCamera> Discover(TimeSpan timeout, CancellationToken cancellationToken);
    }

    internal sealed class LprCameraDiscoveryHandler
    {
        private static readonly Lazy<LprCameraDiscoveryHandler> _instance =
            new Lazy<LprCameraDiscoveryHandler>(() => new LprCameraDiscoveryHandler());

        private readonly List<ILprCameraDiscoveryStrategy> _strategies;
        private readonly HashSet<Guid> _lookupingClients;
        private readonly object _syncRoot;

        private bool _isLookupRunning;

        private static readonly TimeSpan DefaultLookupTimeout = TimeSpan.FromSeconds(5);

        private LprCameraDiscoveryHandler()
        {
            _strategies = new List<ILprCameraDiscoveryStrategy>();
            _lookupingClients = new HashSet<Guid>();
            _syncRoot = new object();

            try
            {
                RegisterStrategy(new Nanopack5LprCameraDiscoveryStrategy());
            }
            catch (BadImageFormatException badImage)
            {
                HandledExceptionAdapter.Examine(badImage);
            }
            catch (DllNotFoundException dllNotFound)
            {
                HandledExceptionAdapter.Examine(dllNotFound);
            }
        }

        public static LprCameraDiscoveryHandler Singleton => _instance.Value;

        public void RegisterStrategy(ILprCameraDiscoveryStrategy strategy)
        {
            if (strategy == null)
                return;

            lock (_strategies)
            {
                if (_strategies.Any(existing => existing.GetType() == strategy.GetType()))
                    return;

                _strategies.Add(strategy);
            }
        }

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
                var aggregated = new Dictionary<string, LookupedLprCamera>(StringComparer.OrdinalIgnoreCase);

                List<ILprCameraDiscoveryStrategy> strategies;
                lock (_strategies)
                {
                    strategies = _strategies.ToList();
                }

                foreach (var strategy in strategies)
                {
                    try
                    {
                        var discoveredCameras = strategy.Discover(DefaultLookupTimeout, CancellationToken.None)
                       ?? Array.Empty<LookupedLprCamera>();
                        foreach (var camera in strategy.Discover(DefaultLookupTimeout, CancellationToken.None)
                                 ?? Enumerable.Empty<LookupedLprCamera>())
                        {
                            if (camera == null || string.IsNullOrWhiteSpace(camera.IpAddress))
                                continue;

                            aggregated[camera.IpAddress.Trim()] = camera;
                        }
                    }
                    catch (Exception discoveryError)
                    {
                        HandledExceptionAdapter.Examine(discoveryError);
                    }
                }

                lock (_syncRoot)
                {
                    clients = _lookupingClients.ToList();
                    _lookupingClients.Clear();
                    _isLookupRunning = false;
                }

                if (clients.Count == 0)
                    return;

                NotifyClients(aggregated.Values.ToList(), clients);
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

    internal sealed class Nanopack5LprCameraDiscoveryStrategy : ILprCameraDiscoveryStrategy
    {
        private const int DefaultMaxDevices = 32;
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

        public string Name => "Nanopack 5";

        public IEnumerable<LookupedLprCamera> Discover(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var context = CDKDiscover.CDKDiscoverCreate();
            if (context == IntPtr.Zero)
                return Enumerable.Empty<LookupedLprCamera>();

            var results = new List<LookupedLprCamera>();
            var started = false;

            try
            {
                if (CDKDiscover.CDKDiscoverStart(context) == 0)
                    return results;

                started = true;

                var effectiveDelay = _responseDelay;
                if (timeout > TimeSpan.Zero && timeout < _responseDelay)
                    effectiveDelay = timeout;

                if (effectiveDelay > TimeSpan.Zero)
                {
                    if (cancellationToken.WaitHandle.WaitOne(effectiveDelay))
                        return results;
                }

                var messages = new IntPtr[_maxDevices];
                var discoveredResult = CDKDiscover.CDKDiscoverGetDiscovered(context, messages, out var discoveredCount);

                if (discoveredResult == 0 || discoveredCount <= 0)
                    return results;

                for (var index = 0; index < discoveredCount && index < messages.Length; index++)
                {
                    var message = messages[index];
                    if (message == IntPtr.Zero)
                        continue;

                    try
                    {
                        var element = CDKMsg.CDKMsgChild(message);
                        if (element == IntPtr.Zero)
                            continue;

                        var camera = new LookupedLprCamera
                        {
                            UniqueKey = CDKMsg.CDKMsgElementAttributeValue(element, "uniqueKey"),
                            InterfaceSource = CDKMsg.CDKMsgElementAttributeValue(element, "interfaceSource"),
                            Name = CDKMsg.CDKMsgElementAttributeValue(element, "name"),
                            Port = CDKMsg.CDKMsgElementAttributeValue(element, "port"),
                            PortSsl = CDKMsg.CDKMsgElementAttributeValue(element, "portSSL"),
                            Equipment = CDKMsg.CDKMsgElementAttributeValue(element, "equipment"),
                            Version = CDKMsg.CDKMsgElementAttributeValue(element, "version"),
                            Locked = CDKMsg.CDKMsgElementAttributeValue(element, "locked"),
                            LockingClientIp = CDKMsg.CDKMsgElementAttributeValue(element, "lockingClientIP"),
                            MacAddress = CDKMsg.CDKMsgElementAttributeValue(element, "macAddress"),
                            Serial = CDKMsg.CDKMsgElementAttributeValue(element, "serial"),
                            IpAddress = CDKMsg.CDKMsgElementAttributeValue(element, "ipAddress"),
                            Model = CDKMsg.CDKMsgElementAttributeValue(element, "model"),
                            Type = CDKMsg.CDKMsgElementAttributeValue(element, "type"),
                            Build = CDKMsg.CDKMsgElementAttributeValue(element, "build")
                        };

                        if (!string.IsNullOrWhiteSpace(camera.IpAddress))
                            results.Add(camera);
                    }
                    finally
                    {
                        try
                        {
                            CDKMsg.CDKMsgDestroy(message);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            finally
            {
                if (started)
                {
                    try
                    {
                        CDKDiscover.CDKDiscoverStop(context);
                    }
                    catch
                    {
                    }
                }

                try
                {
                    CDKDiscover.CDKDiscoverDestroy(context);
                }
                catch
                {
                }
            }

            return results;
        }
    }
}
