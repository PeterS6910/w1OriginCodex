using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace Contal.IwQuick.Crypto.Microsoft
{
	[SecurityCritical(SecurityCriticalScope.Everything)]
	internal sealed class SafeCapiHashHandle : SafeCapiHandleBase
	{
		public static SafeCapiHashHandle InvalidHandle
		{
			get
			{
				var safeCapiHashHandle = new SafeCapiHashHandle();
				safeCapiHashHandle.SetHandle(IntPtr.Zero);
				return safeCapiHashHandle;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptDestroyHash(IntPtr hHash);

		protected override bool ReleaseCapiChildHandle()
		{
			return CryptDestroyHash(handle);
		}

		private SafeCapiHashHandle()
		{
		}
	}
}
