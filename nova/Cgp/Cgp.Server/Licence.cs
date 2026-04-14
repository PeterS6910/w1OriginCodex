using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Contal.Cgp.Globals.PlatformPC;
using Contal.IwQuick.Sys;
using Contal.SLA.Client.Interfaces;
using Contal.Cgp.Server.DB;

namespace Contal.Cgp.Server
{
    public enum LicenseValidity
    {
        Invalid = 0,
        Valid = 1,
        Demo = 2
    }

    public class LicenseHelper
    {
        public const string PLUGIN_FILE_KEYWORD = ".plugin";
        public static int DEMO_MINUTES_WORKING = 120;
        public const string MISSING_LICENCE_PROPERTY = "Missing licence property";

        private static volatile LicenseHelper _singleton;
        private static readonly object SyncRoot = new object();

        public static LicenseHelper Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;

                lock (SyncRoot)
                    if (_singleton == null)
                        _singleton = new LicenseHelper();

                return _singleton;
            }
        }

        private Dictionary<string, bool> _plugins = 
            new Dictionary<string, bool>();

        public IDictionary<string, bool> GetPlugins()
        {
            return _plugins;
        }

        public string Path
        {
            get
            {
                return GeneralOptions.Singleton.LicencePath;
            }
            set
            {
                GeneralOptions.Singleton.LicencePath = value;
            }
        }


        private bool _isLocated;

        public bool IsLocated
        {
            get
            {
                _isLocated = Path != "";
                return _isLocated;
            }
            set
            {
                _isLocated = value;
            }
        }

        public bool DemoLicence
        {
            get
            {
                string fileName = System.IO.Path.GetFileName(Path);

                return 
                    fileName != null && 
                    fileName.ToLower() == "demo.lkey" && 
                    File.Exists(Path);
            }
        }

        public bool IsValid { get; private set; }

        public ISLAClientLicenseBlock LicenceBlock { get; set; }

        /// <summary>
        /// Checks if licence is valid  
        /// </summary>

        public void CheckLicence()
        {
            ISLAClientLicenseBlock licenseBlock;
            string[] missingRequiredProperties;

            IsValid = LicenseValidator.CheckLicense(Path,
                CgpServer.Singleton.Version.Major,
                out _plugins,
                out licenseBlock,
                out missingRequiredProperties);

            if (IsValid)
                LicenceBlock = licenseBlock;
            else if (missingRequiredProperties != null)
            {
                Eventlogs.Singleton.InsertEvent(
                    MISSING_LICENCE_PROPERTY,
                    GetType().Assembly.GetName().Name,
                    null,
                    "License does not contain required properties. " +
                        CreateMissingPropertyMessage(missingRequiredProperties));
            }

            CgpServer.Singleton.DemoLicense = false;
        }

        public bool SetServerAndPluginsRequiredLicenceProperties()
        {
            if (LicenceBlock == null || LicenceBlock.LicenseBlockProperties == null)
                return false;

            var allRequiredProperties =
                CgpServer.Singleton.GetRequiredLicencePropertiesServerAndAllPlugins();

            var propertiesToSet = 
                allRequiredProperties.Keys
                    .ToDictionary<string, string, object>(
                        property => property, 
                        property => null);

            foreach (var property in LicenceBlock.LicenseBlockProperties)
            {
                if (!propertiesToSet.ContainsKey(property.Name) ||
                    !allRequiredProperties.ContainsKey(property.Name))
                {
                    continue;
                }

                try
                {
                    propertiesToSet[property.Name] = GetPropertyValue(allRequiredProperties[property.Name],
                        property.Value);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }

            foreach (var notSetProperty in propertiesToSet.Where(property => property.Value == null).ToArray())
            {
                propertiesToSet[notSetProperty.Key] = GetPropertyValue(allRequiredProperties[notSetProperty.Key],
                    Activator.CreateInstance(allRequiredProperties[notSetProperty.Key]));
            }

            CgpServer.Singleton.SetServerAndPluginsRequiredLicenceProperties(propertiesToSet);

            if (!propertiesToSet.ContainsValue(null))
                return true;

            CgpServer.Singleton.StopProcessing();
            CgpServer.Singleton.StopServiceIfRunning();

            return false;
        }

        private object GetPropertyValue(Type propertyType, object value)
        {
            object propertyValue;

            if (propertyType == typeof(string))
                propertyValue = value.ToString();
            else if (propertyType == typeof(bool))
                propertyValue = bool.Parse(value.ToString());
            else if (propertyType == typeof(int))
                propertyValue = int.Parse(value.ToString());
            else if (propertyType == typeof(byte))
                propertyValue = byte.Parse(value.ToString());
            else if (propertyType == typeof(short))
                propertyValue = short.Parse(value.ToString());
            else propertyValue = null;

            return propertyValue;
        }

        /// <summary>
        /// Checks if licence is valid
        /// </summary>
        /// <param name="licencePath">Licence file path</param>
        public void CheckLicence(string licencePath)
        {
            Path = licencePath;
            CheckLicence();
        }

        private string CreateMissingPropertyMessage(string[] properties)
        {
            var message = new StringBuilder("Missing properties are: ");

            for (int i = 0; i < properties.Length; i++)
            {
                if (i != 0)
                    message.Append(", ");

                message.Append(properties[i]);
            }
            return message.ToString();
        }
    }
}
