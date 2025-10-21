using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Security.Principal;
using System.Security.AccessControl;

namespace Contal.IwQuick.Net.Microsoft
{
    public class NetShare
    {
        public enum NetShareResult
        {
            Success = 0,
            AccessDenied = 2,
            UnknownFailure = 8,
            InvalidName = 9,
            InvalidLevel = 10,
            InvalidParameter = 21,
            DuplicateShare = 22,
            RedirectedPath = 23,
            UnknownDeviceOrDirectory = 24,
            NetNameNotFound = 25
        }

        public static uint CreateSharedFolder(string folderPath, string shareName, string description, bool allowEveryone)
        {
            Validator.CheckDirectoryExists(folderPath);

            //ManagementClass aShareMC = new ManagementClass("Win32_Share");

            //// Process parameters
            //ManagementBaseObject aCreateParametersMB = aShareMC.GetMethodParameters("Create");
            //ManagementBaseObject aCreateResultMB;

            //// Parameters on mbIn are key to the properties to be
            //// changed
            //aCreateParametersMB["Type"] = 0x0; // This is a hard disk
            //aCreateParametersMB["Path"] = folderPath;
            //aCreateParametersMB["Description"] = description;
            //aCreateParametersMB["Name"] = folderName;

            //// Create Invoke to the out object.
            //aCreateResultMB = aShareMC.InvokeMethod("Create", aCreateParametersMB, null);
            //UInt32 iReturn = (UInt32)aCreateResultMB["returnValue"];
            //return iReturn;

            // Create a ManagementClass object
            ManagementClass managementClass = new ManagementClass("Win32_Share");

            // Create ManagementBaseObjects for in and out parameters
            ManagementBaseObject inParams = managementClass.GetMethodParameters("Create");
            ManagementBaseObject outParams;
            UInt32 result;

            // Set the input parameters
            inParams["Description"] = description;
            inParams["Name"] = shareName;
            inParams["Path"] = folderPath;
            inParams["Type"] = 0x0; // Disk Drive
            //Another Type:
            //        DISK_DRIVE = 0x0
            //        PRINT_QUEUE = 0x1
            //        DEVICE = 0x2
            //        IPC = 0x3
            //        DISK_DRIVE_ADMIN = 0x80000000
            //        PRINT_QUEUE_ADMIN = 0x80000001
            //        DEVICE_ADMIN = 0x80000002
            //        IPC_ADMIN = 0x8000003
            if (!allowEveryone)
            {
                outParams = managementClass.InvokeMethod("Create", inParams, null);

                result = (UInt32)(outParams.Properties["ReturnValue"].Value);
                return result;
            }
            else
            {
                inParams["MaximumAllowed"] = null;
                inParams["Password"] = null;
                inParams["Access"] = null; // Make Everyone has full control access.                
                //inParams["MaximumAllowed"] = int maxConnectionsNum;

                // Invoke the method on the ManagementClass object
                outParams = managementClass.InvokeMethod("Create", inParams, null);
                // Check to see if the method invocation was successful
                
                result = (UInt32)(outParams.Properties["ReturnValue"].Value);
                //if (result != 0)
                //{
                    //Console.WriteLine("Error sharing MMS folders. Please make sure you install as Administrator.", "Error!");
                    //throw new Exception("Error sharing MMS folders. Please make sure you install as Administrator.");
                    //return result;
                //}

                //user selection
                NTAccount ntAccount = new NTAccount("Everyone");

                //SID
                SecurityIdentifier userSID = (SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier));
                byte[] utenteSIDArray = new byte[userSID.BinaryLength];
                userSID.GetBinaryForm(utenteSIDArray, 0);

                //Trustee
                ManagementObject userTrustee = new ManagementClass(new ManagementPath("Win32_Trustee"), null);
                userTrustee["Name"] = "Everyone";
                userTrustee["SID"] = utenteSIDArray;

                //ACE
                ManagementObject userACE = new ManagementClass(new ManagementPath("Win32_Ace"), null);
                userACE["AccessMask"] = 2032127;                                 //Full access
                userACE["AceFlags"] = AceFlags.ObjectInherit | AceFlags.ContainerInherit;
                userACE["AceType"] = AceType.AccessAllowed;
                userACE["Trustee"] = userTrustee;

                ManagementObject userSecurityDescriptor = new ManagementClass(new ManagementPath("Win32_SecurityDescriptor"), null);
                userSecurityDescriptor["ControlFlags"] = 4; //SE_DACL_PRESENT 
                userSecurityDescriptor["DACL"] = new object[] { userACE };
                //can declare share either way, where "ShareName" is the name used to share the folder
                //ManagementPath path = new ManagementPath("Win32_Share.Name='" + ShareName + "'");
                //ManagementObject share = new ManagementObject(path);
                ManagementObject share = new ManagementObject(managementClass.Path + ".Name='" + shareName + "'");

                result = (UInt32)share.InvokeMethod("SetShareInfo", new object[] { Int32.MaxValue, description, userSecurityDescriptor });

                return result;
            }
        }
    }
}
