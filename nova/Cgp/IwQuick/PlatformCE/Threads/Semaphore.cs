using System;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Threads
{
    public class Semaphore : IDisposable
    {
        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr CreateSemaphore(
            IntPtr lpSemaphoreAttributes,
            Int32 lInitialCount,
            Int32 lMaximumCount,
            string lpName);

        [DllImport("coredll.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReleaseSemaphore(
            IntPtr handle,
            Int32 lReleaseCount,
            out Int32 previousCount);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern Int32 WaitForSingleObject(
            IntPtr hHandle,
            Int32 dwMilliseconds);

        //Handle
        [DllImport("coredll.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        private readonly IntPtr _handle;

        public Semaphore()
        {
            _handle = CreateSemaphore(
                IntPtr.Zero,
                0,
                Int32.MaxValue,
                null);
        }

        public Semaphore(
            int initialCount,
            int maximalCount)
        {
            _handle = CreateSemaphore(
                IntPtr.Zero,
                initialCount,
                maximalCount,
                null);
        }

        ~Semaphore()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            CloseHandle(_handle);
        }

        public int Release(int i)
        {
            int result;

            ReleaseSemaphore(
                _handle,
                i,
                out result);

            return result;
        }

        public void WaitOne()
        {
            if (WaitForSingleObject(
                _handle,
                -1) == 0xffffffff)
            {
                throw new ObjectDisposedException("Semaphore");
            }
        }
    }
}
