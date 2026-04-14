using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;
using Contal.IwQuick;
using Contal.Cgp.Globals.PlatformPC;
using Contal.SLA.Client;
using Contal.SLA.Client.Interfaces;

namespace Contal.Cgp.ContalNovaLicenseManager
{
    public partial class ContalNovaLicenseManager : Form
    {
        public const string PropertyMajorVersion = "MajorVersion";
        public const string PropertyEdition = "Edition";
        public const string PropertyConnectionCount = "ConnectionCount";
        public const string PropertyDoorEnvironmentCount = "DoorEnvironmentCount";
        public const string PropertyCISIntegration = "CISIntegration";
        public const string PropertyOfflineImport = "OfflineImport";
        public const string PropertyCgpNCASServerplugin = "Cgp.NCAS.Server.plugin";
        public const string PropertyIdManagement = "IdManagement";
        public const string PropertyGraphics = "Graphics";
        public const string PropertyCCU40MaxDsm = "CCU40MaxDsm";
        public const string PropertyCAT12CEMaxDsm = "CAT12CEMaxDsm";

        public const string PropertyCCU05MaxDsm = "CCU05MaxDsm";

        public const string PropertyMaxSubsiteCount = "MaxSubsiteCount";

        public const string PropertyCat12ComboCount = "Cat12ComboCount";

        public const string PropertyTimetecMaster = "TimetecMaster";

        /// <summary>
        /// value is the description
        /// </summary>
        private readonly Dictionary<string,string> _requiredProperties = new Dictionary<string, string>()
        {
            {PropertyMajorVersion,"Major version"},
            {PropertyEdition,PropertyEdition},
            {PropertyConnectionCount,"Maximum number of connected clients"},
            {PropertyDoorEnvironmentCount, "Maximum number of door environments"},
            {PropertyCISIntegration,"Enable CIS integration"},
            {PropertyOfflineImport,"Enable import from CSV/TSV"},
            {PropertyCgpNCASServerplugin,"Enable Access plugin"} ,
            {PropertyIdManagement,"Enable ID/Batch management"} ,
            {PropertyGraphics,"Enable graphics"} ,
            {PropertyCCU40MaxDsm,"Maximum of door environments for CCU40"} ,
            {PropertyCAT12CEMaxDsm,"Maximum of door environments for CCU12" } ,
            {PropertyCCU05MaxDsm,"Maximum of door environments for CCU05"} ,
            {PropertyMaxSubsiteCount,"Maximum subsite count"} ,
            {PropertyCat12ComboCount,"Maximumber number of CCU12/CAT12CE units"},
            {PropertyTimetecMaster,"Enable Timetec master"}
        };

        private const string LICENSE_NOT_SUITABLE = "License can not be used with Contal Nova";
        private const string LICENSE_SUITABLE = "License can be used with Contal Nova";
        private readonly Log _logLicenseManager = new Log("License manager", false, true, false);

        private readonly ExtendedVersion _version = new ExtendedVersion(
            typeof(ContalNovaLicenseManager),
            true,
#if DEBUG
            DevelopmentStage.Testing
#else
            DevelopmentStage.Internal
#endif
            );

        public ContalNovaLicenseManager()
        {
            InitializeComponent();
        }

        private void ContalNovaLicenseManager_Load(object sender, EventArgs e)
        {
            Text += " [ " + _version + " ]";

            ShowAvailableLicenses();
        }

        private void ShowAvailableLicenses()
        {
            _lbAvailableLicenceFiles.Items.Clear();
            foreach (string fullFileName in Directory.GetFiles(QuickPath.AssemblyStartupPath))
            {
                if (Path.GetExtension(fullFileName) == ".lkey" && Path.GetFileNameWithoutExtension(fullFileName).ToLower() != "demo")
                    _lbAvailableLicenceFiles.Items.Add(Path.GetFileNameWithoutExtension(fullFileName));
            }
        }

        
        private void ShowFunctionalPropertiesAndPlugins(ISLAPropertyRecord[] licenseProperties)
        {
            _epgFunctionalProperties.Items.Clear();
            foreach (ISLAPropertyRecord property in licenseProperties)
            {
                string tmp = property.Name.ToLower();

                if (tmp.Contains("plugin") ||
                    property.Name == PropertyCCU40MaxDsm ||
                    property.Name == PropertyCAT12CEMaxDsm ||
                    property.Name == PropertyCCU05MaxDsm)
                {
                    continue;
                }

                _epgFunctionalProperties.Items.Add(
                    _requiredProperties.ContainsKey(property.Name)
                        ? _requiredProperties[property.Name]
                        : property.Name,
                    property.Value.ToString(),
                    true,
                    "Properties",
                    "",
                    true);
            }
            foreach (ISLAPropertyRecord property in licenseProperties)
            {
                if (!property.Name.ToLower().Contains("plugin"))
                {
                    continue;
                }

                _epgFunctionalProperties.Items.Add(
                    _requiredProperties.ContainsKey(property.Name)
                        ? _requiredProperties[property.Name]
                        : property.Name,
                    property.Value.ToString(),
                    true,
                    "Plugins",
                    "",
                    true);
            }
            _epgFunctionalProperties.Refresh();
        }

        private Dictionary<string, string> LoadPluginsFromLicenseFile(IEnumerable<ISLAPropertyRecord> licenseProperties)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (ISLAPropertyRecord property in licenseProperties)
            {
                if (property.Name.ToLower().Contains("plugin"))
                    result.Add(property.Name, property.Value.ToString());
            }

            return result;
        }

        private bool ValidateProperties(IEnumerable<ISLAPropertyRecord> licenseProperties, string[] requiredProperties)
        {
            List<string> licensePropertiesNames = new List<string>();
            foreach (ISLAPropertyRecord property in licenseProperties)
            {
                licensePropertiesNames.Add(property.Name);
            }

            if (licensePropertiesNames.Count != requiredProperties.Length)
                return false;

            foreach (string requiredProperty in requiredProperties)
            {
                if (!licensePropertiesNames.Contains(requiredProperty))
                    return false;
            }

            foreach (string licenseProperty in licensePropertiesNames)
            {
                if (!requiredProperties.Contains(licenseProperty))
                    return false;
            }
            return true;
        }

        private void OnCreateRequestClick(object sender, EventArgs e)
        {
            string publisherName = null;
            string publisherPublicKey = null;
            string commonName = null;
            Dictionary<int, string> propertyNames = null;// new Dictionary<int, string>();
            bool publisherLoaded = false;
            bool descriptiveFileLoaded = false;

            int crtCount = 0;

            foreach (string filePath in Directory.GetFiles(QuickPath.AssemblyStartupPath,"*.*crt"))
            {
                if (!publisherLoaded)
                    publisherLoaded =
                    SLAClientModule.Singleton.IsValidCertificateFile(filePath, out publisherName, out publisherPublicKey);

                crtCount++;
            }

            if (!publisherLoaded)
            {
                Dialog.Error("No valid certificate found");
                return;
            }
            
if (crtCount > 1)
            {
                Dialog.Error("There should not be more than one *.crt or *.lcrt file.\n"+
                             "Erase the old certificate");
                return;
            }

            bool propertiesCorrect = false;

            foreach (string filePath in Directory.GetFiles(QuickPath.AssemblyStartupPath,"*.ldsc"))
            {
                Dictionary<int, string> propertyTypes; // Dictionary<int, string>();
                Dictionary<int, string> propertyValues; // new Dictionary<int, string>();
                Dictionary<int, string> propertyDescriptions; // new Dictionary<int, string>();
                descriptiveFileLoaded = SLAClientModule.Singleton.IsValidDescriptiveFile(
                    filePath, 
                    publisherName, 
                    out commonName, 
                    out propertyNames, 
                    out propertyTypes, 
                    out propertyValues, 
                    out propertyDescriptions);

                if (descriptiveFileLoaded)
                {
                    if (ArePropertiesCorrect(propertyNames, _requiredProperties))
                    {
                        propertiesCorrect = true;
                        break;
                    }
                }
            
            }
            if (!descriptiveFileLoaded)
            {
                Dialog.Error("No valid descriptive file (*.ldsc) for this version found");
                return;
            }

            if (!propertiesCorrect)
            {
                Dialog.Error("None of descriptive files found had properties required by current version of Contal Nova requires.\n"+
                    "See system eventlog for details.");
                return;
            }

            

            LicenseRequest lrForm = new LicenseRequest(propertyNames, publisherName, publisherPublicKey, commonName);
            lrForm.ShowDialog();

        }

        private bool ArePropertiesCorrect(
            Dictionary<int, string> licensePropertyNames,
            IDictionary<string,string> requiredProperties)
        {           
            bool result = true;

            StringBuilder uselessProperties = new StringBuilder();
            foreach (string licenseProperty in licensePropertyNames.Values)
            {
                if (!requiredProperties.ContainsKey(licenseProperty))
                {
                    if (uselessProperties.Length != 0)
                        uselessProperties.Append(",");
                    uselessProperties.Append(licenseProperty);
                    result = false;
                }
            }

            StringBuilder missingProperties = new StringBuilder();
            foreach (string requiredProperty in requiredProperties.Keys)
            {
                if (!licensePropertyNames.ContainsValue(requiredProperty))
                {
                    if (missingProperties.Length != 0)
                        missingProperties.Append(",");
                    missingProperties.Append(requiredProperty);
                    result = false;
                }
            }

            if (result == false)
                _logLicenseManager.Error("Descriptive file is invalid. Missing properties are " + 
                    missingProperties + ". Useless properties are " + uselessProperties + ".");


            return result;
        }

        private void _lbAvailableLicenceFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_lAvailableLicenseFiles.Text))
                return;

            string licPath = Path.Combine(QuickPath.AssemblyStartupPath, _lbAvailableLicenceFiles.Text + ".lkey");

            ISLAClientLicenseBlock licenseBlock;
            Dictionary<string, bool> availablePlugins;
            string[] missingRequiredProperties;

            if (LicenseValidator.CheckLicense(licPath,
                Assembly.GetExecutingAssembly().GetName().Version.Major,
                out availablePlugins,
                out licenseBlock,
                out missingRequiredProperties))

            {
                _eLicenceValidity.Text = "Valid";
                _eReleaseDate.Text = licenseBlock.ReleaseDate.ToShortDateString();
                _eExpirationDate.Text =
                    licenseBlock.ExpDate.ToShortDateString() == DateTime.MaxValue.ToShortDateString()
                        ? "No expiration"
                        : licenseBlock.ExpDate.ToShortDateString();
                LicenseSuitable();
            }
            else
            {
                _eLicenceValidity.Text = "Invalid";
                _eReleaseDate.Clear();
                _eExpirationDate.Clear();
                _epgFunctionalProperties.Items.Clear();
                _epgFunctionalProperties.Refresh();
                LicenseUnsuitable();
            }

            //if (ValidateProperties(licenseBlock.LicenseBlockProperties, _requiredProperties.Keys.ToArray()))
            //    LicenseSuitable();
            //else
            //    LicenseUnsuitable();

            ShowFunctionalPropertiesAndPlugins(licenseBlock.LicenseBlockProperties);
        }

        private void LicenseSuitable()
        {
            _lLicenseSuitable.ForeColor = Color.Green;
            _lLicenseSuitable.Text = LICENSE_SUITABLE;
        }

        private void LicenseUnsuitable()
        {
            _lLicenseSuitable.ForeColor = Color.Red;
            _lLicenseSuitable.Text = LICENSE_NOT_SUITABLE;
        }
    }
}
