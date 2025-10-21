using System;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Crypto.Microsoft
{
    public class NativeSuperAesV2
    {
        private const string DLL_PATH = "NativeSuperAesV2.dll";

        [DllImport(DLL_PATH, EntryPoint = "NativeSuperAes2_CreateKey")]
        public static extern IntPtr CreateKey(
            [MarshalAs(UnmanagedType.LPArray)]
            [In]byte[] key,
            int keyLength,
            [MarshalAs(UnmanagedType.LPArray)]
            [In]byte[] iv);

        [DllImport(DLL_PATH, EntryPoint = "NativeSuperAes2_ReleaseKey")]
        public static extern void ReleaseKey(IntPtr aesKey);

        [DllImport(DLL_PATH, EntryPoint = "NativeSuperAes2_Encrypt")]
        public static extern bool Encrypt(
            IntPtr aesKey,
            bool final,
            [MarshalAs(UnmanagedType.LPArray)]
            [In, Out] byte[] data,
            int dataOffset,
            [In, Out] ref int dataLength,
            int bufLen);

        [DllImport(DLL_PATH, EntryPoint = "NativeSuperAes2_Decrypt")]
        public static extern bool Decrypt(
            IntPtr aesKey,
            bool final,
            [MarshalAs(UnmanagedType.LPArray)]
            [In] byte[] inputData,
            int inputOffset,
            [MarshalAs(UnmanagedType.LPArray)]
            [Out] byte[] outputData,
            int outputOffset,
            [In, Out] ref int dataLength);
    }
}
