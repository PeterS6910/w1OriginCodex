using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace Contal.IwQuick.Crypto.Microsoft
{
	[SecurityCritical(SecurityCriticalScope.Everything)]
	internal sealed class SafeCspHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeCspHandle() : base(true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptContextAddRef(SafeCspHandle hProv, IntPtr pdwReserved, int dwFlags);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptReleaseContext(IntPtr hProv, int dwFlags);

		public SafeCspHandle Duplicate()
		{
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			SafeCspHandle result;
			try
			{
				base.DangerousAddRef(ref flag);
				IntPtr handle = base.DangerousGetHandle();
				int num = 0;
				SafeCspHandle safeCspHandle = new SafeCspHandle();
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					if (!SafeCspHandle.CryptContextAddRef(this, IntPtr.Zero, 0))
					{
						num = Marshal.GetLastWin32Error();
					}
					else
					{
						safeCspHandle.SetHandle(handle);
					}
				}
				if (num != 0)
				{
					safeCspHandle.Dispose();
					throw new CryptographicException(num);
				}
				result = safeCspHandle;
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return result;
		}

		protected override bool ReleaseHandle()
		{
			return SafeCspHandle.CryptReleaseContext(this.handle, 0);
		}
	}
}
