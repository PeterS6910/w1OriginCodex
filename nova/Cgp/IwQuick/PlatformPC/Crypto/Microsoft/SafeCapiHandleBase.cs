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
	internal abstract class SafeCapiHandleBase : SafeHandleZeroOrMinusOneIsInvalid
	{
		private IntPtr _csp;

		protected IntPtr ParentCsp
		{
			get
			{
				return _csp;
			}

			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			set
			{
				int num = 0;
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
				    if (CryptContextAddRef(value, IntPtr.Zero, 0))
				        _csp = value;
				    else
				        num = Marshal.GetLastWin32Error();
				}

			    if (num != 0)
			        throw new CryptographicException(num);
			}
		}

		internal SafeCapiHandleBase() : base(true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptContextAddRef(IntPtr hProv, IntPtr pdwReserved, int dwFlags);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptReleaseContext(IntPtr hProv, int dwFlags);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal void SetParentCsp(SafeCspHandle parentCsp)
		{
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				parentCsp.DangerousAddRef(ref flag);
				IntPtr parentCsp2 = parentCsp.DangerousGetHandle();
				ParentCsp = parentCsp2;
			}
			finally
			{
				if (flag)
				{
					parentCsp.DangerousRelease();
				}
			}
		}

		protected abstract bool ReleaseCapiChildHandle();

		protected sealed override bool ReleaseHandle()
		{
			bool flag = ReleaseCapiChildHandle();
			bool flag2 = true;
			if (_csp != IntPtr.Zero)
			{
				flag2 = CryptReleaseContext(_csp, 0);
			}
			return flag && flag2;
		}
	}
}
