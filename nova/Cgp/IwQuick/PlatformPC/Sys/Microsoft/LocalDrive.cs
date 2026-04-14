using System.IO;

namespace Contal.IwQuick.Sys.Microsoft
{
    public class LocalDrive
    {
        public static string GetFree()
        {
            DriveInfo[] arDrives = DriveInfo.GetDrives();
            DriveInfo aLastDrive = null;
            string strDrive = null;
            foreach (DriveInfo aDrive in arDrives)
            {
                if (null == aDrive)
                    continue;

                if (null != aLastDrive &&
                    aDrive.Name[0] - aLastDrive.Name[0] > 1)
                {
                    strDrive = new string(
                        (char)(aLastDrive.Name[0]+1), 1) ;
                    break;
                }

                aLastDrive = aDrive;
            }

            if (null == strDrive &&
                null != aLastDrive)
            {
                string strUpper = aLastDrive.Name.ToUpper();
                if (strUpper[0] < 'Z')
                {
                    strDrive = new string((char)(strUpper[0]+1), 1);
                }
            }

            return strDrive;
        }
        
        

    }
}
