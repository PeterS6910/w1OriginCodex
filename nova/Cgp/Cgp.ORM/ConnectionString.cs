using System;

using Microsoft.Win32;
using Contal.IwQuick;
using System.Data.SqlClient;
using Contal.Cgp.Globals;
using System.Security.Cryptography;
using System.IO;

namespace Contal.Cgp.ORM
{
    /// <summary>
    /// class for explicit connection string configuration
    /// </summary>
    public class ConnectionString
    {
        public const int DEFAULT_CONNECTION_TIMEOUT = 60;

        private string _databaseServer;
        private string _databaseName;
        private string _databaseLogin;
        private string _databaseLogPassword;

        /// <summary>
        /// implicit database name
        /// </summary>
        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                Validator.CheckNullString(value);
                _databaseName = value;
            }
        }

        /// <summary>
        /// implicit database host , alternatively with instance specification (e.g. (local)\SQLEXPRESS
        /// </summary>
        public string DatabaseServer
        {
            get { return _databaseServer; }
            set
            {
                Validator.CheckNullString(value);
                _databaseServer = value;
            }
        }

        /// <summary>
        /// implicit database login
        /// </summary>
        public string DatabaseLogin
        {
            get { return _databaseLogin; }
            set
            {
                Validator.CheckNullString(value);
                _databaseLogin = value;
            }
        }

        /// <summary>
        /// implicit database login password
        /// </summary>
        public string DatabaseLogPassword
        {
            get { return _databaseLogPassword; }
            set
            {
                Validator.CheckNullString(value);
                _databaseLogPassword = value;
            }
        }

        /// <summary>
        /// implicit using integrated security
        /// </summary>
        public bool IntegratedSecurity { get; private set; }

        /// <summary>
        /// returns true, if either explicit or implicit values are present and valid
        /// </summary>
        public bool IsValid()
        {
            if (!Validator.IsNotNullString(_databaseName) ||
                    !Validator.IsNotNullString(_databaseServer))
                return false;

            if (IntegratedSecurity)
                return true;

            return
                Validator.IsNotNullString(_databaseLogin) &&
                Validator.IsNotNullString(_databaseLogPassword);
        }

        private static volatile ConnectionString _singleton;
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// singleton instance , usually used by ORM tools
        /// </summary>
        public static ConnectionString Singleton
        {
            get
            {
                if (null != _singleton)
                    return _singleton;

                lock (SyncRoot)
                    if (_singleton == null)
                        _singleton = new ConnectionString();

                return _singleton;
            }
        }

        /// <summary>
        /// instantiates invalid connection string
        /// </summary>
        public ConnectionString()
        {
            _databaseName = String.Empty;
            _databaseServer = String.Empty;

            _databaseLogin = String.Empty;
            _databaseLogPassword = String.Empty;

            IntegratedSecurity = false;
        }

        #region Registry based load & store

        private const string DATABASE_SERVER = "DatabaseServer";
        private const string DATABASE_NAME = "DatabaseName";
        private const string DATABASE_LOGIN = "DatabaseLogin";
        private const string DATABASE_LOGPASSWORD = "DatabaseLogPassword";
        private const string DATABASE_LOGIN_PASSWORD = "DatabaseLoginPassword";
        private const string DATABASE_INTEGRATEDSECURITY = "DatabaseIntegratedSecurity";

        /// <summary>
        /// tries to load the settings from the registry key
        /// </summary>
        /// <param name="regisrtyKey"></param>
        public ConnectionString(RegistryKey regisrtyKey)
        {
            LoadFromRegistry(regisrtyKey);
        }

        /// <summary>
        /// loads the settings from registry
        /// </summary>
        /// <param name="registryKey">instance of the registry key; can be null</param>
        public void LoadFromRegistry(RegistryKey registryKey)
        {
            try
            {
                // do via properties , to preserve constraints
                DatabaseServer = (string)registryKey.GetValue(DATABASE_SERVER);
            }
            catch
            {
                _databaseServer = String.Empty;
            }

            try
            {
                DatabaseName = (string)registryKey.GetValue(DATABASE_NAME);
            }
            catch
            {
                _databaseName = String.Empty;
            }

            try
            {
                DatabaseLogin = (string)registryKey.GetValue(DATABASE_LOGIN);
            }
            catch
            {
                _databaseLogin = String.Empty;
            }

            try
            {
                object cipherPassword = 
                    registryKey.GetValue(DATABASE_LOGIN_PASSWORD);

                DatabaseLogPassword = 
                    DecryptFunction(
                        (byte[])cipherPassword, 
                        CgpServerGlobals.DATABASE_KEY, 
                        CgpServerGlobals.DATABASE_SALT);
            }
            catch
            {
                _databaseLogPassword = String.Empty;
            }

            try
            {
                var intIntegratedSecurity = 
                    (int)registryKey.GetValue(DATABASE_INTEGRATEDSECURITY);

                IntegratedSecurity = intIntegratedSecurity != 0;
            }
            catch
            {
                // don't throw, validity can be checked by the IsValid property
                //throw new ArgumentException("Invalid connection string data at "+registryKey.ToString());
                IntegratedSecurity = false;
            }
        }

        /// <summary>
        /// stores the internal values (implicit) into the registry key
        /// </summary>
        /// <param name="registryKey"></param>
        public void SaveToRegistry(String registryKey)
        {
            Validator.CheckNullString(registryKey);

            RegistryKey rk = 
                IwQuick.Sys.Microsoft.RegistryHelper
                    .GetOrAddKey(registryKey);

            if (rk == null)
                throw new DoesNotExistException(registryKey);

            rk.SetValue(
                DATABASE_SERVER,
                _databaseServer ?? "", 
                RegistryValueKind.String);

            rk.SetValue(
                DATABASE_NAME, 
                _databaseName ?? "", 
                RegistryValueKind.String);

            rk.SetValue(
                DATABASE_LOGIN, 
                _databaseLogin ?? "", 
                RegistryValueKind.String);

            byte[] cryptedPassword = 
                EncryptFunction(
                    _databaseLogPassword, 
                    CgpServerGlobals.DATABASE_KEY, 
                    CgpServerGlobals.DATABASE_SALT);

            rk.SetValue(
                DATABASE_LOGIN_PASSWORD, 
                cryptedPassword, 
                RegistryValueKind.Binary);

            rk.SetValue(
                DATABASE_INTEGRATEDSECURITY, 
                IntegratedSecurity, 
                RegistryValueKind.DWord);
        }

        /// <summary>
        ///Saves login and password to CgpServerGlobals.REGISTRY_CONNECTION_STRING registry key
        /// </summary>
        public void SaveToRegistryLoginPassword()
        {
            Validator.CheckNullString(CgpServerGlobals.REGISTRY_CONNECTION_STRING);

            RegistryKey rk = 
                IwQuick.Sys.Microsoft.RegistryHelper
                    .GetOrAddKey(CgpServerGlobals.REGISTRY_CONNECTION_STRING);


            if (null == rk)
                throw new DoesNotExistException(CgpServerGlobals.REGISTRY_CONNECTION_STRING);

            rk.SetValue(
                DATABASE_LOGIN,
                _databaseLogin ?? "",
                RegistryValueKind.String);

            byte[] cryptedPassword = 
                EncryptFunction(
                _databaseLogPassword, 
                CgpServerGlobals.DATABASE_KEY,
                CgpServerGlobals.DATABASE_SALT);

            rk.SetValue(
                DATABASE_LOGIN_PASSWORD, 
                cryptedPassword, 
                RegistryValueKind.Binary);
        }

        #endregion

        /// <summary>
        /// instantiates connection string with explicit settings
        /// </summary>
        /// <param name="databaseServer"></param>
        /// <param name="databaseName"></param>
        /// <param name="databaseLogin"></param>
        /// <param name="databaseLogPassword"></param>
        /// <param name="integratedSecurity"></param>
        public ConnectionString(
            string databaseServer, 
            string databaseName, 
            string databaseLogin, 
            string databaseLogPassword, 
            bool integratedSecurity)
        {
            Validator.CheckNullString(databaseName);
            Validator.CheckNullString(databaseServer);

            if (!integratedSecurity)
            {
                Validator.CheckNullString(databaseLogin);
                Validator.CheckNullString(databaseLogPassword);
            }

            _databaseServer = databaseServer;
            _databaseName = databaseName;

            _databaseLogin = databaseLogin;
            _databaseLogPassword = databaseLogPassword;

            IntegratedSecurity = integratedSecurity;
        }

        public string ToString(int connectionTimeout, bool disablePooling)
        {
            if (!IsValid())
                return string.Empty;

            var sqlConnectionStringBuilder =
                new SqlConnectionStringBuilder
                {
                    PersistSecurityInfo = true,
                    InitialCatalog = _databaseName,
                    DataSource = _databaseServer,
                    IntegratedSecurity = IntegratedSecurity
                };

            if (!IntegratedSecurity)
            {
                sqlConnectionStringBuilder.UserID = _databaseLogin;
                sqlConnectionStringBuilder.Password = _databaseLogPassword;
            }

            sqlConnectionStringBuilder.ConnectTimeout = connectionTimeout;

            if (disablePooling)
                sqlConnectionStringBuilder.Pooling = false;

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public override string ToString()
        {
            return ToString(
                DEFAULT_CONNECTION_TIMEOUT,
                false);
        }

        /// <summary>
        /// creates instance from the registry key if possible
        /// </summary>
        /// <param name="registryKey"></param>
        /// <returns></returns>
        public static ConnectionString LoadFromRegistry(string registryKey)
        {
            try
            {

                Validator.CheckNullString(registryKey);

                RegistryKey rk;

                return 
                    IwQuick.Sys.Microsoft.RegistryHelper.TryParseKey(
                            registryKey, 
                            true, 
                            out rk) 
                        ? new ConnectionString(rk)
                        : new ConnectionString();
            }
            catch
            {
                return null;
            }
        }

        public void Clear()
        {
            _databaseServer = string.Empty;
            _databaseName = string.Empty;
            _databaseLogin = string.Empty;
            _databaseLogPassword = string.Empty;
        }

        public static void EnsureEncryptedDatabaseLoginPassword()
        {
            if (ExistsDatabaseLoginPassword(CgpServerGlobals.REGISTRY_CONNECTION_STRING_EXTERN_DATABASE))
                DeleteOldPlainDbsLogPassword(CgpServerGlobals.REGISTRY_CONNECTION_STRING_EXTERN_DATABASE);
            else
                CreateNewEncryptPassword(CgpServerGlobals.REGISTRY_CONNECTION_STRING_EXTERN_DATABASE);

            if (ExistsDatabaseLoginPassword(CgpServerGlobals.REGISTRY_CONNECTION_STRING))
                DeleteOldPlainDbsLogPassword(CgpServerGlobals.REGISTRY_CONNECTION_STRING);
            else
                CreateNewEncryptPassword(CgpServerGlobals.REGISTRY_CONNECTION_STRING);
        }

        public static bool IsDatabaseLoginPasswordEncrypted()
        {
            if (!ExistsDatabaseLoginPassword(CgpServerGlobals.REGISTRY_CONNECTION_STRING_EXTERN_DATABASE) && 
                    ExistsOldPlainDbsLogPassword(CgpServerGlobals.REGISTRY_CONNECTION_STRING_EXTERN_DATABASE))
            {
                return false;
            }

            return ExistsDatabaseLoginPassword(CgpServerGlobals.REGISTRY_CONNECTION_STRING);
        }

        private static void CreateNewEncryptPassword(string regPath)
        {
            if (!ExistsOldPlainDbsLogPassword(regPath)) 
                return;

            if (SaveCryptedDatabaseLoginPassword(
                    regPath, 
                    GetOldPlainDbsLogPassword(regPath)))
            {
                DeleteOldPlainDbsLogPassword(regPath);
            }
        }

        private static bool ExistsDatabaseLoginPassword(string regPath)
        {
            if (string.IsNullOrEmpty(regPath)) 
                return false;

            try
            {
                RegistryKey registryKey;

                return 
                    IwQuick.Sys.Microsoft.RegistryHelper.TryParseKey(
                        regPath,
                        true,
                        out registryKey) && 
                    registryKey.GetValue(DATABASE_LOGIN_PASSWORD) != null;
            }
            catch
            {
                return false;
            }
        }


        private static bool SaveCryptedDatabaseLoginPassword(string regPath, string plainPassword)
        {
            if (string.IsNullOrEmpty(plainPassword))
                return false;

            if (string.IsNullOrEmpty(regPath))
                return false;

            try
            {
                RegistryKey registryKey = 
                    IwQuick.Sys.Microsoft.RegistryHelper
                        .GetOrAddKey(regPath);

                if (registryKey == null)
                    return false;

                byte[] cypherPassword = 
                    EncryptFunction(
                        plainPassword, 
                        CgpServerGlobals.DATABASE_KEY, 
                        CgpServerGlobals.DATABASE_SALT);

                registryKey.SetValue(
                    DATABASE_LOGIN_PASSWORD, 
                    cypherPassword, 
                    RegistryValueKind.Binary);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool ExistsOldPlainDbsLogPassword(string regPath)
        {
            if (string.IsNullOrEmpty(regPath)) 
                return false;

            try
            {
                RegistryKey registryKey;

                return 
                    IwQuick.Sys.Microsoft.RegistryHelper.TryParseKey(
                        regPath,
                        true,
                        out registryKey) && 
                    registryKey.GetValue(DATABASE_LOGPASSWORD) != null;
            }
            catch
            {
                return false;
            }
        }

        private static string GetOldPlainDbsLogPassword(string regPath)
        {
            if (string.IsNullOrEmpty(regPath)) 
                return string.Empty;

            try
            {
                RegistryKey registryKey;

                if (!IwQuick.Sys.Microsoft.RegistryHelper.TryParseKey(
                    regPath,
                    true,
                    out registryKey))
                {
                    return string.Empty;
                }

                return (string)registryKey.GetValue(DATABASE_LOGPASSWORD) ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void DeleteOldPlainDbsLogPassword(string regPath)
        {
            if (string.IsNullOrEmpty(regPath)) 
                return;

            try
            {
                RegistryKey rk = 
                    IwQuick.Sys.Microsoft.RegistryHelper
                        .GetOrAddKey(regPath);

                if (rk != null)
                    rk.DeleteValue(DATABASE_LOGPASSWORD, false);
            }
            catch { }
        }

        private static byte[] EncryptFunction(string plainText, byte[] key, byte[] IV)
        {
            RijndaelManaged crypto = null;
            MemoryStream memStream;

            // I crypto transform is used to perform the actual decryption vs encryption, 
            // hash function are also a version of crypto transform.
            // Crypto streams allow for encryption in memory.
            CryptoStream cryptoStream = null;

            var byteTransform = new System.Text.UTF8Encoding();

            // Just grabbing the bytes since most crypto functions need bytes.
            byte[] plainBytes = byteTransform.GetBytes(plainText);

            try
            {
                crypto = new RijndaelManaged
                {
                    Key = key,
                    IV = IV
                };

                memStream = new MemoryStream();

                // Calling the method create encryptor method. Needs both the Key and IV 
                // these have to be from the original Rijndael call
                // If these are changed nothing will work right.

                ICryptoTransform encryptor = 
                    crypto.CreateEncryptor(
                        crypto.Key, 
                        crypto.IV);

                // The big parameter here is the cryptomode.write, 
                // you are writing the data to memory to perform the transformation

                cryptoStream = 
                    new CryptoStream(
                        memStream, 
                        encryptor, 
                        CryptoStreamMode.Write);

                // The method write takes three params the data to be written (in bytes) 
                // the offset value (int) and the length of the stream (int)

                cryptoStream.Write(plainBytes, 0, plainBytes.Length);

            }
            finally
            {
                // if the crypto blocks are not clear lets make sure the data is gone
                if (crypto != null)
                    crypto.Clear();

                // Close because of my need to close things when done.
                if (cryptoStream != null)
                    cryptoStream.Close();
            }
            // Return the memory byte array
            return memStream.ToArray();
        }

        private static string DecryptFunction(byte[] cipherText, byte[] key, byte[] IV)
        {
            if (cipherText == null) 
                return string.Empty;

            RijndaelManaged crypto = null;
            MemoryStream memStream = null;

            string plainText;

            try
            {
                crypto = new RijndaelManaged
                {
                    Key = key,
                    IV = IV
                };

                memStream = new MemoryStream(cipherText);

                // Create Decryptor make sure if you are decrypting that this is here 
                // and you did not copy paste encryptor.

                ICryptoTransform decryptor = 
                    crypto.CreateDecryptor(crypto.Key, crypto.IV);

                // This is different from the encryption 
                // look at the mode make sure you are reading from the stream.

                var cryptoStream = 
                    new CryptoStream(
                        memStream, 
                        decryptor, 
                        CryptoStreamMode.Read);

                // I used the stream reader here because the ReadToEnd method is easy
                // and because it return a string, also easy.

                plainText = new StreamReader(cryptoStream).ReadToEnd();
            }
            finally
            {
                if (crypto != null)
                    crypto.Clear();

                if (memStream != null)
                {
                    memStream.Flush();
                    memStream.Close();
                }
            }

            return plainText;
        }
    }
}
