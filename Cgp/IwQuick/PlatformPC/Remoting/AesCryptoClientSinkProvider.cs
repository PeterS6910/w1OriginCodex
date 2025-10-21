using System.Runtime.Remoting.Channels;
using System.Collections;
using System.Security.Cryptography;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Net;
using JetBrains.Annotations;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// Class for encryption client sink provider
    /// </summary>
    public class AesCryptoClientSinkProvider : IClientChannelSinkProvider
    {
        private readonly CryptoProcessor _cryptoProcessor;

        private const string AES_SETTINGS_KEY = "aesSettings";

        /// <summary>
        /// Create EncryptionClientSinkProvider
        /// </summary>
        /// <param name="properties">Properties must have path to file with key in key and path to file with iv in iv</param>
        /// <param name="providerData">Provider data</param>
        public AesCryptoClientSinkProvider(
            IDictionary properties,
            ICollection providerData)
            : this(properties[AES_SETTINGS_KEY] as AESSettings)
        {
        }

        /// <summary>
        /// Create EncryptionClientSinkProvider
        /// </summary>
        /// <param name="aesSettings"></param>
        public AesCryptoClientSinkProvider(
            [NotNull] AESSettings aesSettings)
        {
            Validator.CheckForNull(aesSettings,"aesSettings");

            _cryptoProcessor = 
                new CryptoProcessor(
                    new AesProvider
                    {
                        Key = aesSettings.Aes256Key, 
                        IV = aesSettings.Aes256IV
                    });
        }

        /// <summary>
        /// Get or set _nextProvider
        /// </summary>
        public IClientChannelSinkProvider Next { get; set; }

        /// <summary>
        /// Create client channel sink
        /// </summary>
        /// <param name="channel">Input channel</param>
        /// <param name="url">Url to the well know service</param>
        /// <param name="remoteChannelData">Data</param>
        /// <returns>Return new EncryptionClientSink</returns>
        public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            // create other sinks in the chain

            return 
                new AesCryptoClientSink(
                    Next.CreateSink(
                        channel,
                        url,
                        remoteChannelData),
                    _cryptoProcessor);
        }
    }
}
