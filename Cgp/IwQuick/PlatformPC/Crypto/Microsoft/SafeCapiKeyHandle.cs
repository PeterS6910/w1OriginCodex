using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Contal.IwQuick.Crypto.Microsoft
{
	[SecurityCritical(SecurityCriticalScope.Everything)]
	internal sealed class SafeCapiKeyHandle : SafeCapiHandleBase
	{
		internal static SafeCapiKeyHandle InvalidHandle
		{
			get
			{
				var safeCapiKeyHandle = new SafeCapiKeyHandle();
				safeCapiKeyHandle.SetHandle(IntPtr.Zero);
				return safeCapiKeyHandle;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptDestroyKey(IntPtr hKey);

		protected override bool ReleaseCapiChildHandle()
		{
			return CryptDestroyKey(handle);
		}

		private SafeCapiKeyHandle()
		{
		}

		internal SafeCapiKeyHandle Duplicate()
		{
			SafeCapiKeyHandle safeCapiKeyHandle = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			    if (!CapiNative.UnsafeNativeMethods.CryptDuplicateKey(this, IntPtr.Zero, 0, out safeCapiKeyHandle))
			        throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			finally
			{
			    if (safeCapiKeyHandle != null && !safeCapiKeyHandle.IsInvalid && ParentCsp != IntPtr.Zero)
			        safeCapiKeyHandle.ParentCsp = ParentCsp;
			}

			return safeCapiKeyHandle;
		}
	}
}
