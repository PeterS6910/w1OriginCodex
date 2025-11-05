using System;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using SmartLprConsole.Models;

namespace SmartLprConsole
{
    internal sealed class SmartLprClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly DataContractJsonSerializerSettings _serializerSettings;
        private bool _disposed;

        public SmartLprClient(SmartLprOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var handler = new HttpClientHandler
            {
                UseCookies = false
            };

            if (options.IgnoreSslErrors)
            {
                handler.ServerCertificateCustomValidationCallback = (message, certificate2, chain, errors) => true;
            }

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = options.BaseAddress,
                Timeout = options.Timeout
            };

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(options.Username + ":" + options.Password));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

            _serializerSettings = new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            };
        }

        public Task<DeviceStatus> GetCameraStatusAsync()
        {
            return GetAsync<DeviceStatus>("api/v2/status");
        }

        public Task<RecognitionStatistics> GetRecognitionStatisticsAsync()
        {
            return GetAsync<RecognitionStatistics>("api/v2/statistics");
        }

        public Task<AlarmCollection> GetAlarmsAsync()
        {
            return GetAsync<AlarmCollection>("api/v2/alarms");
        }

        private async Task<T> GetAsync<T>(string relativePath)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(SmartLprClient));
            }

            using (var response = await _httpClient.GetAsync(relativePath).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var serializer = new DataContractJsonSerializer(typeof(T), _serializerSettings);
                    var result = serializer.ReadObject(stream);
                    return (T)result;
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _httpClient.Dispose();
            _disposed = true;
        }
    }
}
