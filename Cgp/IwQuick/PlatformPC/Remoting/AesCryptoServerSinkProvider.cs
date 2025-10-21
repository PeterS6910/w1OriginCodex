using System.Runtime.Remoting.Channels;
using System.Collections;

using Contal.IwQuick.Crypto;
using Contal.IwQuick.Net;
using JetBrains.Annotations;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// Class for encryption server sink provider
    /// </summary>
    public class AesCryptoServerSinkProvider : IServerChannelSinkProvider
    {
        private const string AES_SETTINGS_KEY = "aesSettings";

        private readonly CryptoProcessor _cryptoProcessor;

        /// <summary>
        /// Create EncryptionServerSinkProvider
        /// </summary>
        /// <param name="properties">if this constructor used, the key and IV parameters must be provided as AESSettings instance keyed under "aesSettings"</param>
        /// <param name="providerData">provider data</param>
        public AesCryptoServerSinkProvider(
            IDictionary properties,
            [PublicAPI] ICollection providerData)
// ReSharper disable once AssignNullToNotNullAttribute
            : this(properties[AES_SETTINGS_KEY] as AESSettings)
        {
        }

        public AesCryptoServerSinkProvider(
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
        public IServerChannelSinkProvider Next { get; set; }

        /// <summary>
        /// Create server channel sink
        /// </summary>
        /// <param name="channel">Input channel</param>
        /// <returns>Return new EncryptionServerSink</returns>
        public IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            // create other sinks in the chain
            IServerChannelSink nextSink =
                Next.CreateSink(channel);

            return
                new AesCryptoServerSink(
                    nextSink,
                    _cryptoProcessor);
        }

        public void GetChannelData(IChannelDataStore channelData)
        {
        }
    }
}
