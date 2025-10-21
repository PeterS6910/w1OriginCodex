using System.Collections.Generic;
using System.IO;
using System.Linq;

using Contal.SLA.Client;
using Contal.SLA.Client.Interfaces;

namespace Contal.Cgp.Globals.PlatformPC
{
    public static class LicenseValidator
    {
        public const string PLUGIN_FILE_KEYWORD = ".plugin";

        private static readonly string[] _requiredLicenseProperties =
        {
            "Edition",
            "ConnectionCount",
            "CISIntegration",
            "OfflineImport",
            "MajorVersion",
            "MaxSubsiteCount",
            "DoorEnvironmentCount",
            "Graphics",
            "CCU40MaxDsm",
            "CAT12CEMaxDsm",
            "CCU05MaxDsm",
            "Cat12ComboCount"           
        };

        public static bool CheckLicense(
            string filePath, 
            int requiredMajorVersion,
            out Dictionary<string, bool> availablePlugins,
            out ISLAClientLicenseBlock licenseBlock,
            out string[] missingRequiredProperties)
        {
            availablePlugins = null;
            licenseBlock = null;
            missingRequiredProperties = null;

            if (!File.Exists(filePath))
                return false;

            try
            {
                licenseBlock = SLAClientModule.Singleton.DecryptKey(filePath);

                if (!licenseBlock.GetLicenceBlockProperties()
                    .ContainsKey(RequiredLicenceProperties.MajorVersion.ToString())
                    ||
                    licenseBlock.GetLicenceBlockProperties()[RequiredLicenceProperties.MajorVersion.ToString()] !=
                    requiredMajorVersion.ToString())
                {
                    return false;
                }

                availablePlugins = licenseBlock.LicenseBlockProperties.Where
                    (property => property.Name.Contains(PLUGIN_FILE_KEYWORD))
                    .ToDictionary(obj => obj.Name, obj => bool.Parse(obj.Value.ToString()));

                var licenseProperties = licenseBlock.GetLicenceBlockProperties();

                missingRequiredProperties = _requiredLicenseProperties.Where(
                    requiredPropertyName => !licenseProperties.ContainsKey(requiredPropertyName)).ToArray();

                if (missingRequiredProperties.Length > 0)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
