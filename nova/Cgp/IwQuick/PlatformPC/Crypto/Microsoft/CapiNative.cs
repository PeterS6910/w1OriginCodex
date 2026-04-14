using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Contal.IwQuick.Crypto.Microsoft
{
	internal static class CapiNative
	{
		internal enum AlgorithmId
		{
			None,
			Aes128 = 26126,
			Aes192,
			Aes256,
			MD5 = 32771,
			Sha1,
			Sha256 = 32780,
			Sha384,
			Sha512
		}

		internal enum ProviderParameter
		{
			None,
			EnumerateAlgorithms
		}

		[Flags]
		internal enum ProviderParameterFlags
		{
			None = 0,
			RestartEnumeration = 1
		}

        [Flags]
        internal enum KeyParameterFlags
        {
            None = 0
        }

		[SecurityCritical(SecurityCriticalScope.Everything), SuppressUnmanagedCodeSecurity]
		internal static class UnsafeNativeMethods
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity]
			[DllImport("advapi32")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptDuplicateKey(SafeCapiKeyHandle hKey, IntPtr pdwReserved, int dwFlags, out SafeCapiKeyHandle phKey);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptGetProvParam(SafeCspHandle hProv, ProviderParameter dwParam, IntPtr pbData, [In] [Out] ref int pdwDataLen, ProviderParameterFlags dwFlags);

            [DllImport("advapi32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptGetKeyParam(SafeCapiKeyHandle hKey, KeyParameter dwParam, IntPtr pbData, [In] [Out] ref int pdwDataLen, KeyParameterFlags dwFlags);

			[DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptAcquireContext(out SafeCspHandle phProv, string pszContainer, string pszProvider, ProviderType dwProvType, CryptAcquireContextFlags dwFlags);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptCreateHash(SafeCspHandle hProv, AlgorithmId Algid, SafeCapiKeyHandle hKey, int dwFlags, out SafeCapiHashHandle phHash);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptDecrypt(SafeCapiKeyHandle hKey, SafeCapiHashHandle hHash, [MarshalAs(UnmanagedType.Bool)] bool Final, int dwFlags, IntPtr pbData, [In] [Out] ref int pdwDataLen);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptEncrypt(SafeCapiKeyHandle hKey, SafeCapiHashHandle hHash, [MarshalAs(UnmanagedType.Bool)] bool Final, int dwFlags, IntPtr pbData, [In] [Out] ref int pdwDataLen, int dwBufLen);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptExportKey(SafeCapiKeyHandle hKey, SafeCapiKeyHandle hExpKey, int dwBlobType, int dwExportFlags, [MarshalAs(UnmanagedType.LPArray)] [Out] byte[] pbData, [In] [Out] ref int pdwDataLen);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptGenKey(SafeCspHandle hProv, AlgorithmId Algid, KeyFlags dwFlags, out SafeCapiKeyHandle phKey);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptGenRandom(SafeCspHandle hProv, int dwLen, [MarshalAs(UnmanagedType.LPArray)] [Out] byte[] pbBuffer);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptGetHashParam(SafeCapiHashHandle hHash, HashParameter dwParam, [MarshalAs(UnmanagedType.LPArray)] [Out] byte[] pbData, [In] [Out] ref int pdwDataLen, int dwFlags);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptHashData(SafeCapiHashHandle hHash, [MarshalAs(UnmanagedType.LPArray)] byte[] pbData, int dwDataLen, int dwFlags);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptImportKey(SafeCspHandle hProv, [MarshalAs(UnmanagedType.LPArray)] byte[] pbData, int dwDataLen, SafeCapiKeyHandle hPubKey, KeyFlags dwFlags, out SafeCapiKeyHandle phKey);

			[DllImport("advapi32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CryptSetKeyParam(SafeCapiKeyHandle hKey, KeyParameter dwParam, [MarshalAs(UnmanagedType.LPArray)] byte[] pbData, int dwFlags);
		}

		internal enum AlgorithmClass
		{
			DataEncryption = 24576,
			Hash = 32768
		}

		internal enum AlgorithmType
		{
			Any,
			Block = 1536
		}

		internal enum AlgorithmSubId
		{
			MD5 = 3,
			Sha1,
			Sha256 = 12,
			Sha384,
			Sha512,
			Aes128 = 14,
			Aes192,
			Aes256
		}

		[Flags]
		internal enum CryptAcquireContextFlags
		{
			None = 0,
			VerifyContext = -268435456
		}

		internal enum ErrorCode
		{
			Success,
			MoreData = 234,
			NoMoreItems = 259,
			BadData = -2146893819,
			BadAlgorithmId = -2146893816,
			ProviderTypeNotDefined = -2146893801,
			KeysetNotDefined = -2146893799
		}

		internal enum HashParameter
		{
			None,
			AlgorithmId,
			HashValue,
			HashSize = 4
		}

		internal enum KeyBlobType : byte
		{
			PlainText = 8
		}

		[Flags]
		internal enum KeyFlags
		{
			None = 0,
			Exportable = 1
		}

		internal enum KeyParameter
		{
			None,
			IV,
			Mode = 4,
			ModeBits,
            AlgId = 7,
            Blocklen,
            Keylen
		}

		internal static class ProviderNames
		{
			public const string MicrosoftEnhancedRsaAes = "Microsoft Enhanced RSA and AES Cryptographic Provider";
			public const string MicrosoftEnhancedRsaAesPrototype = "Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)";
		}

		internal enum ProviderType
		{
			None,
			RsaAes = 24
		}

		[SecurityCritical]
		internal static T GetProviderParameterStruct<T>(SafeCspHandle provider, ProviderParameter parameter, ProviderParameterFlags flags) 
            where T : struct
		{
			int cb = 0;
			IntPtr intPtr = IntPtr.Zero;
			if (!UnsafeNativeMethods.CryptGetProvParam(provider, parameter, intPtr, ref cb, flags))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == 259)
				{
					return default(T);
				}
				if (lastWin32Error != 234)
				{
					throw new CryptographicException(lastWin32Error);
				}
			}
			RuntimeHelpers.PrepareConstrainedRegions();
			T result;
			try
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					intPtr = Marshal.AllocCoTaskMem(cb);
				}
				if (!UnsafeNativeMethods.CryptGetProvParam(provider, parameter, intPtr, ref cb, flags))
				{
					throw new CryptographicException(Marshal.GetLastWin32Error());
				}
				result = (T)Marshal.PtrToStructure(intPtr, typeof(T));
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(intPtr);
				}
			}
			return result;
		}

		[SecurityCritical]
		internal static SafeCspHandle AcquireCsp(string keyContainer, string providerName, ProviderType providerType, CryptAcquireContextFlags flags, bool throwPlatformException)
		{
			SafeCspHandle result;
			if (UnsafeNativeMethods.CryptAcquireContext(out result, keyContainer, providerName, providerType, flags))
			{
				return result;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (throwPlatformException && (lastWin32Error == -2146893801 || lastWin32Error == -2146893799))
			{
				throw new PlatformNotSupportedException("Cryptography_PlatformNotSupported");
			}
			throw new CryptographicException(lastWin32Error);
		}

		[SecurityCritical]
		internal static byte[] ExportSymmetricKey(SafeCapiKeyHandle key)
		{
			int num = 0;
			if (!UnsafeNativeMethods.CryptExportKey(key, SafeCapiKeyHandle.InvalidHandle, 8, 0, null, ref num))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 234)
				{
					throw new CryptographicException(lastWin32Error);
				}
			}
			byte[] array = new byte[num];
			if (!UnsafeNativeMethods.CryptExportKey(key, SafeCapiKeyHandle.InvalidHandle, 8, 0, array, ref num))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}

			int num2 = BitConverter.ToInt32(array, 8);
			byte[] array2 = new byte[num2];
			Buffer.BlockCopy(array, 12, array2, 0, array2.Length);
			return array2;
		}

		internal static string GetAlgorithmName(AlgorithmId algorithm)
		{
			return algorithm.ToString().ToUpper(CultureInfo.InvariantCulture);
		}

		[SecurityCritical]
		internal static byte[] GetHashParameter(SafeCapiHashHandle hashHandle, HashParameter parameter)
		{
			int num = 0;
			if (!UnsafeNativeMethods.CryptGetHashParam(hashHandle, parameter, null, ref num, 0))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			byte[] array = new byte[num];
			if (!UnsafeNativeMethods.CryptGetHashParam(hashHandle, parameter, array, ref num, 0))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			if (num != array.Length)
			{
				byte[] array2 = new byte[num];
				Buffer.BlockCopy(array, 0, array2, 0, num);
				array = array2;
			}
			return array;
		}

		internal static int HResultForVerificationResult(SignatureVerificationResult verificationResult)
		{
			switch (verificationResult)
			{
			case SignatureVerificationResult.AssemblyIdentityMismatch:
			case SignatureVerificationResult.PublicKeyTokenMismatch:
			case SignatureVerificationResult.PublisherMismatch:
				return -2146762749;
			case SignatureVerificationResult.ContainingSignatureInvalid:
				return -2146869232;
			default:
				return (int)verificationResult;
			}
		}

		[SecurityCritical]
		internal static SafeCapiKeyHandle ImportSymmetricKey(SafeCspHandle provider, AlgorithmId algorithm, byte[] key)
		{
            byte[] buffer = new byte[12 + key.Length];

		    buffer[0] = (byte) KeyBlobType.PlainText;
		    buffer[1] = 2;
            Buffer.BlockCopy(BitConverter.GetBytes((int)algorithm), 0, buffer, 4, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(key.Length), 0, buffer, 8, sizeof(int));
            Buffer.BlockCopy(key, 0, buffer, 12, key.Length);

			SafeCapiKeyHandle safeCapiKeyHandle = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				if (!UnsafeNativeMethods.CryptImportKey(provider, buffer, buffer.Length, SafeCapiKeyHandle.InvalidHandle, KeyFlags.Exportable, out safeCapiKeyHandle))
				{
					throw new CryptographicException(Marshal.GetLastWin32Error());
				}
			}
			finally
			{
				if (safeCapiKeyHandle != null && !safeCapiKeyHandle.IsInvalid)
				{
					safeCapiKeyHandle.SetParentCsp(provider);
				}
			}
			return safeCapiKeyHandle;
		}

		[SecurityCritical]
		internal static void SetKeyParameter(SafeCapiKeyHandle key, KeyParameter parameter, int value)
		{
			SetKeyParameter(key, parameter, BitConverter.GetBytes(value));
		}

		[SecurityCritical]
		internal static void SetKeyParameter(SafeCapiKeyHandle key, KeyParameter parameter, byte[] value)
		{
			if (!UnsafeNativeMethods.CryptSetKeyParam(key, parameter, value, 0))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
		}
    }
}
