using CDKDOTNET;
using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

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

        private static List<LookupedLprCamera> DiscoverAllCameraTypes(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var aggregatedCameras = new Dictionary<string, LookupedLprCamera>(StringComparer.OrdinalIgnoreCase);
            var nanopackDiscovery = new Nanopack5LprCameraDiscoveryStrategy();
            var smartDiscovery = new SmartLprCameraDiscoveryStrategy();
            var lprCameraCounter = 1;

            DiscoverWith(
                                () =>
                                {
                                    var cameras = nanopackDiscovery.Discover(timeout, cancellationToken)?.ToList();
                                    AssignSequentialNames(cameras, "nanopack5", ref lprCameraCounter);
                                    return cameras;
                                },
                aggregatedCameras);

            DiscoverWith(
                                () =>
                                {
                                    var cameras = smartDiscovery.Discover(timeout, cancellationToken)?.ToList();
                                    AssignSequentialNames(cameras, "quertec", ref lprCameraCounter);
                                    return cameras;
                                },
                aggregatedCameras);

            return aggregatedCameras.Values.ToList();
        }

        private static void AssignSequentialNames(IList<LookupedLprCamera> cameras, string prefix, ref int counter)
        {
            if (cameras == null)
                return;

            foreach (var camera in cameras)
            {
                if (camera == null)
                    continue;

                camera.Name = string.Format("{0} {1}", prefix, counter++);
            }
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

    internal sealed class SmartLprCameraDiscoveryStrategy
    {
        private const string AddressPrefix = "192.168.1.2";
        private const int AddressStartSuffix = 0;
        private const int AddressEndSuffix = 54;
        private const int RestPort = 8080;
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromMilliseconds(200);
        private static readonly AuthenticationHeaderValue AuthorizationHeader =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:quercus2")));
        private static readonly string[] RequiredResponseTokens = { "\"global\"", "\"lamp\"", "\"temperature\"" };
        private const X509ChainStatusFlags AllowedChainStatuses =
    X509ChainStatusFlags.UntrustedRoot |
    X509ChainStatusFlags.PartialChain |
    X509ChainStatusFlags.RevocationStatusUnknown |
    X509ChainStatusFlags.OfflineRevocation;

        public IEnumerable<LookupedLprCamera> Discover(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var discovered = new List<LookupedLprCamera>();

            EnsureSecurityProtocols();

            using (var handler = CreateHttpClientHandler())
            {
                using (var httpClient = new HttpClient(handler, disposeHandler: true))
                {
                    httpClient.Timeout = RequestTimeout;
                    httpClient.DefaultRequestHeaders.Authorization = AuthorizationHeader;

                    for (var suffix = AddressStartSuffix; suffix <= AddressEndSuffix; suffix++)
                    {
                        var address = string.Format("{0}{1:D2}", AddressPrefix, suffix);

                        if (!IPAddress.TryParse(address, out _))
                            continue;

                        if (TryDiscoverCamera(httpClient, address, cancellationToken, out var camera))
                        {
                            discovered.Add(camera);
                        }
                    }
                }
            }

            return discovered;
        }

        private static bool TryDiscoverCamera(HttpClient httpClient, string address, CancellationToken cancellationToken, out LookupedLprCamera camera)
        {
            camera = TryDiscoverCameraAsync(httpClient, address, cancellationToken)
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();


            return camera != null;
        }

        private static async Task<LookupedLprCamera> TryDiscoverCameraAsync(HttpClient httpClient,
                                                        string address, CancellationToken cancellationToken)
        {
            var requestUri = string.Format("https://{0}:{1}/api/v2/status", address, RestPort);

            try
            {
                using (var response = await httpClient.GetAsync(requestUri))
                {
                    if (!response.IsSuccessStatusCode)
                        return null;
                    var payload = await response.Content
                                                .ReadAsStringAsync()
                                                .ConfigureAwait(false);
                    if (!IsSmartLprResponse(payload))
                        return null;

                    return new LookupedLprCamera
                    {
                        IpAddress = address,
                        Name = string.Format("SmartLpr {0}", address),
                        Port = RestPort.ToString(),
                        PortSsl = RestPort.ToString(),
                        MacAddress = TryGetMacAddress(address),
                        Type = "SmartLpr",
                        InterfaceSource = "SmartLpr",
                        UniqueKey = string.Format("quercus:{0}", address)
                    };
                }
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return null;
            }
        }

        private static bool IsSmartLprResponse(string payload)
        {
            if (string.IsNullOrEmpty(payload))
                return false;

            foreach (var token in RequiredResponseTokens)
            {
                if (payload.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0)
                    return false;
            }

            return true;
        }

        private static HttpClientHandler CreateHttpClientHandler()
        {
            return new ValidationAwareHttpClientHandler();
        }

        private static string TryGetMacAddress(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return null;

            try
            {
                using (var ping = new Ping())
                {
                    try
                    {
                        ping.Send(ipAddress, 1000);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "arp",
                    Arguments = "-a " + ipAddress,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    if (process == null)
                        return null;

                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!line.Contains(ipAddress))
                            continue;

                        var parts = Regex.Split(line.Trim(), @"\s+");
                        if (parts.Length >= 2)
                            return parts[1];
                    }
                }
            }
            catch
            {
            }

            return null;
        }
        private sealed class ValidationAwareHttpClientHandler : HttpClientHandler
        {
            private readonly RemoteCertificateValidationCallback _previousCallback;
            private readonly RemoteCertificateValidationCallback _currentCallback;
            private readonly bool _usesGlobalValidation;

            public ValidationAwareHttpClientHandler()
            {
                var property = typeof(HttpClientHandler).GetProperty(
                    "ServerCertificateCustomValidationCallback",
                    BindingFlags.Instance | BindingFlags.Public);

                if (property != null)
                {
                    var callback = (Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>)ValidateCertificate;
                    property.SetValue(this, callback, null);
                }
                else
                {
                    _previousCallback = ServicePointManager.ServerCertificateValidationCallback;
                    _currentCallback = (sender, certificate, chain, errors) =>
                    {
                        if (ValidateCertificate((sender as HttpWebRequest)?.RequestUri?.Host,
                                                certificate,
                                                chain,
                                                errors))
                        {
                            return true;
                        }

                        return _previousCallback?.Invoke(sender, certificate, chain, errors) ?? false;
                    };

                    ServicePointManager.ServerCertificateValidationCallback = _currentCallback;
                    _usesGlobalValidation = true;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (_usesGlobalValidation &&
                    ServicePointManager.ServerCertificateValidationCallback == _currentCallback)
                {
                    ServicePointManager.ServerCertificateValidationCallback = _previousCallback;
                }

                base.Dispose(disposing);
            }
        }

        private static bool ValidateCertificate(HttpRequestMessage message,
                                                X509Certificate2 certificate,
                                                X509Chain chain,
                                                SslPolicyErrors errors)
        {
            var host = message?.RequestUri?.Host;
            return ValidateCertificateCore(host, certificate, chain, errors);
        }

        private static bool ValidateCertificate(string requestHost,
                                                X509Certificate certificate,
                                                X509Chain chain,
                                                SslPolicyErrors errors)
        {
            var certificate2 = certificate as X509Certificate2 ??
                                (certificate != null ? new X509Certificate2(certificate) : null);
            return ValidateCertificateCore(requestHost, certificate2, chain, errors);
        }

        private static bool ValidateCertificateCore(string requestHost,
                                                    X509Certificate2 certificate,
                                                    X509Chain chain,
                                                    SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;

            if (certificate == null)
                return false;

            if ((errors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0 &&
                                !string.IsNullOrEmpty(requestHost) &&
                                IPAddress.TryParse(requestHost, out _))
            {
                errors &= ~SslPolicyErrors.RemoteCertificateNameMismatch;
            }

            if ((errors & SslPolicyErrors.RemoteCertificateChainErrors) != 0 &&
                IsChainAcceptable(chain))
            {
                errors &= ~SslPolicyErrors.RemoteCertificateChainErrors;
            }

            return errors == SslPolicyErrors.None;
        }

        private static bool IsChainAcceptable(X509Chain chain)
        {
            if (chain == null)
                return true;

            foreach (var status in chain.ChainStatus)
            {
                if ((status.Status & AllowedChainStatuses) == status.Status)
                    continue;

                return false;
            }

            return true;
        }

        private static void EnsureSecurityProtocols()
        {
            try
            {
                ServicePointManager.SecurityProtocol |=
                    SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11 |
                    SecurityProtocolType.Tls12;
            }
            catch (NotSupportedException)
            {

            }
        }
    }
}
