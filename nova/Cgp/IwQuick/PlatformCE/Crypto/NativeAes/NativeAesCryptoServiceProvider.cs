using System.Security.Cryptography;

using Contal.IwQuick.Crypto.Microsoft;

namespace Contal.IwQuick.Crypto.NativeAes
{
    public class NativeAesCryptoServiceProvider : NativeRijndael
    {
        internal abstract class CryptoTransform : AesCryptoTransform
        {
            protected readonly int ContextId;

            protected CryptoTransform(byte[] rgbKey, byte[] rgbIv)
            {
                int resultCode;

                ContextId = 
                    NativeAES.NativeSuperAes_InitStream(
                        rgbKey, 
                        rgbKey.Length, 
                        rgbIv, 
                        rgbIv.Length, 
                        out resultCode);
            }

            public override void Dispose()
            {
                int resultCode;

                NativeAES.NativeSuperAes_FinalizeStream(ContextId, out resultCode);
            }
        }

        public override ICryptoTransform CreateEncryptor(
            byte[] rgbKey, byte[] rgbIV)
        {
            return new NativeAesEncryptor(rgbKey, rgbIV);
        }

        public override ICryptoTransform CreateDecryptor(
            byte[] rgbKey, byte[] rgbIV)
        {
            return new NativeAesDecryptor(rgbKey, rgbIV);
        }
    }
}