using System;
using System.IO;
using System.DirectoryServices;
using System.Security.AccessControl;


namespace Contal.IwQuick.Sys.Microsoft
{
    public class LocalUsers
    {
        [Flags]
        public enum ActiveDirectoryUserFlags
        {
            Script = 1,                          // 0x1
            AccountDisabled = 2,                 // 0x2
            HomeDirectoryRequired = 8,           // 0x8 
            AccountLockedOut = 16,               // 0x10
            PasswordNotRequired = 32,            // 0x20
            PasswordCannotChange = 64,           // 0x40
            EncryptedTextPasswordAllowed = 128,  // 0x80
            TempDuplicateAccount = 256,          // 0x100
            NormalAccount = 512,                 // 0x200
            InterDomainTrustAccount = 2048,      // 0x800
            WorkstationTrustAccount = 4096,      // 0x1000
            ServerTrustAccount = 8192,           // 0x2000
            PasswordDoesNotExpire = 65536,       // 0x10000
            MnsLogonAccount = 131072,            // 0x20000
            SmartCardRequired = 262144,          // 0x40000
            TrustedForDelegation = 524288,       // 0x80000
            AccountNotDelegated = 1048576,       // 0x100000
            UseDesKeyOnly = 2097152,              // 0x200000
            DontRequirePreauth = 4194304,         // 0x400000
            PasswordExpired = 8388608,           // 0x800000
            TrustedToAuthenticateForDelegation = 16777216, // 0x1000000
            NoAuthDataRequired = 33554432        // 0x2000000
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool UserExists(string username)
        {
            if (Validator.IsNullString(username))
                return false;

            try
            {
                DirectoryEntry aDirectoryEntry = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                DirectoryEntry aEntry = aDirectoryEntry.Children.Find(username, "user");
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
                return aEntry != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="cannotChangePassword"></param>
        /// <param name="passwordNeverExpires"></param>
        /// <param name="accountIsDisabled"></param>
        public static void CreateUser(string username,string password,bool cannotChangePassword,bool passwordNeverExpires,bool accountIsDisabled)
        {
            Validator.CheckNullString(username);

            DirectoryEntry aDirectoryEntry = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");

            DirectoryEntry aUserEntry;

            ActiveDirectoryUserFlags aUserFlags;
            try
            {
                aUserEntry = aDirectoryEntry.Children.Find(username, "user");
                aUserFlags = (ActiveDirectoryUserFlags)aUserEntry.Properties["UserFlags"].Value;
            }
            catch
            {
                aUserEntry = aDirectoryEntry.Children.Add(username, "User");
                aUserFlags = ActiveDirectoryUserFlags.NormalAccount;
            }

            aUserEntry.Invoke("SetPassword", password);


            if (cannotChangePassword)
                aUserFlags |= ActiveDirectoryUserFlags.PasswordCannotChange;

            if (passwordNeverExpires)
                aUserFlags |= ActiveDirectoryUserFlags.PasswordDoesNotExpire;

            if (accountIsDisabled)
                aUserFlags |= ActiveDirectoryUserFlags.AccountDisabled;

            aUserEntry.Properties["UserFlags"].Value = aUserFlags;
           
            aUserEntry.CommitChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="directoryPath"></param>
        /// <param name="rights"></param>
        /// <param name="allow"></param>
        public static void SetAccessTo(String identity, String directoryPath,FileSystemRights rights,bool allow)
        {
            Validator.CheckDirectoryExists(directoryPath);

            // Create a new DirectoryInfo object.
            DirectoryInfo aDirInfo = new DirectoryInfo(directoryPath);

            // Get a DirectorySecurity object that represents the 
            // current security settings.
            DirectorySecurity aDirSecurity = aDirInfo.GetAccessControl();

            aDirSecurity.SetAccessRuleProtection(true, false);
            // Add the FileSystemAccessRule to the security settings. 
            
            aDirSecurity.AddAccessRule(
                new FileSystemAccessRule(
                identity, 
                rights,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.None,
                allow ? AccessControlType.Allow : AccessControlType.Deny
                 ));


            // Set the new access settings.
            aDirInfo.SetAccessControl(aDirSecurity);

        }
    }
}
