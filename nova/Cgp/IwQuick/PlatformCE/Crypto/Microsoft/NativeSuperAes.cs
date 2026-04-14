using System.Runtime.InteropServices;

namespace Contal.IwQuick.Crypto.Microsoft
{
    public enum AESResult
    {
        Ok = 0,
        WrongKeySize = 1,
        WrongIVSize = 2,
        OutputBufferTooSmall = 3,
        NotCompleteInput = 4,
        PaddingMismatch = 5,
        ExceptionThrown = 6
    }

    public class NativeAES
    {
        private const string DLL_PATH = "NativeSuperAes.dll";

        [DllImport(DLL_PATH)]
        public static extern int NativeSuperAes_Decrypt(byte[] key, int keyLength, byte[] iv, int ivLength,
                                      byte[] encryptedData, int encryptedBufferLength, int encryptedOffset, int encryptedLength,
                                      byte[] plainDataOutput, int maxDecryptedLength, out int resultCode);

        [DllImport(DLL_PATH)]
        public static extern int NativeSuperAes_Encrypt(byte[] key, int keyLength, byte[] iv, int ivLength,
                                      byte[] plainData, int plainBufferLength, int plainOffset, int plainLength,
                                      byte[] encryptedDataOutput, int encryptedBufferLength, out int resultCode);

        [DllImport(DLL_PATH)]
        public static extern int NativeSuperAes_InitStream(byte[] key, int keyLength, byte[] iv, int ivLength, out int resultCode);

        [DllImport(DLL_PATH)]
        public static extern int NativeSuperAes_ResetKey(int contextId, out int resultCode);

        [DllImport(DLL_PATH)]
        public static extern int NativeSuperAes_EncryptBlock(int contextId,
                                           byte[] plainData, int plainBufferLength, int plainOffset, int plainLength,
                                           byte[] encryptedDataOutput, int encryptedBufferLength, out int resultCode);

        [DllImport(DLL_PATH)]
        public static extern int NativeSuperAes_EncryptLastBlock(int contextId,
                                           byte[] plainData, int plainBufferLength, int plainOffset, int plainLength,
                                           byte[] encryptedDataOutput, int encryptedBufferLength, out int resultCode);

        [DllImport(DLL_PATH)]
        public static extern int NativeSuperAes_DecryptBlock(int contextId,
                                           byte[] encryptedData, int encryptedBufferLength, int encryptedOffset, int encryptedLength,
                                           byte[] plainDataOutput, int plainBufferLength, out int resultCode);

        [DllImport(DLL_PATH)]
        public static extern int NativeSuperAes_DecryptLastBlock(int contextId,
                                           byte[] encryptedData, int encryptedBufferLength, int ecryptedOffset, int ecryptedLength,
                                           byte[] plainDataOutput, int plainBufferLength, out int resultCode);

        [DllImport(DLL_PATH)]
        public static extern int NativeSuperAes_FinalizeStream(int contextId, out int resultCode);
    }

}
