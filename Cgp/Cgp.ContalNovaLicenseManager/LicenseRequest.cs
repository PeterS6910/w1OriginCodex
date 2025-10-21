using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Contal.IwQuick.UI;
using Contal.IwQuick.Net;
using Contal.SLA.Client;
using Contal.SLA.Client.Interfaces;
using JetBrains.Annotations;

namespace Contal.Cgp.ContalNovaLicenseManager
{
    public partial class LicenseRequest : Form
    {
        private readonly IDictionary<string, int> _requiredProperties = null;
        private readonly string _commonName = string.Empty;
        private readonly string _publisherName = string.Empty;
        private readonly string _publisherkey = string.Empty;
        private int _editionSelectedIndex = -1;

        public LicenseRequest(
            [NotNull] IDictionary<int, string> validatedPropertiesFromLdsc,
            [NotNull] string publisherName,
            [NotNull] string publisherKey,
            [NotNull] string commonName)
        {
            
            _requiredProperties = new Dictionary<string, int>(validatedPropertiesFromLdsc.Count);
            foreach (var property in validatedPropertiesFromLdsc)
            {
                _requiredProperties[property.Value] = property.Key;
            }


            _commonName = commonName;
            _publisherName = publisherName;
            _publisherkey = publisherKey;
            InitializeComponent();
            _cbCountries.DataSource = GetCountriesList();
            _cbCountries.SelectedIndex = -1;
            _nudMajorVersion.Text = typeof(Program).Assembly.GetName().Version.Major.ToString(CultureInfo.InvariantCulture);
        }

        private struct EditionProperties
        {
            internal string Name;

// ReSharper disable once NotAccessedField.Local
            internal int ConnectionCountMin;
            internal int ConnectionCountMax;

// ReSharper disable once NotAccessedField.Local
            internal int DsmCountMin;
            internal int DsmCountMax;

            internal bool TimetecMasterIncluded;
            internal bool TimetecMasterOptional;

            internal bool GraphicsIncluded;
            internal bool GraphicsOptional;

            internal bool CisIncluded;
            internal bool CisOptional;

            internal bool StructuredSiteIncluded;
            internal bool StructuredSiteOptional;

            internal bool IdManagementIncluded ;
            internal bool IdManagementOptional;

            internal bool OfflineImportIncluded ;
            internal bool OfflineImportOptional ;

            internal int Cat12ComboCountMin;
            internal int Cat12ComboCountMax;
        }

        private readonly EditionProperties[] _propertiesPerEdition =
        {
            new EditionProperties()
            {
                Name = "Express",

                ConnectionCountMin = 2,
                ConnectionCountMax = 2,

                DsmCountMin = 1,
                DsmCountMax = 10,

                TimetecMasterIncluded = false,
                TimetecMasterOptional = true,

                GraphicsIncluded = false,
                GraphicsOptional = true,

                CisIncluded = false,
                CisOptional = false,

                IdManagementIncluded = false,
                IdManagementOptional = false,

                OfflineImportIncluded = false,
                OfflineImportOptional = false,

                StructuredSiteIncluded = false,
                StructuredSiteOptional = false,

                Cat12ComboCountMin =  0,
                Cat12ComboCountMax = 1

            }
            ,
            new EditionProperties()
            {
                Name = "Standard",

                ConnectionCountMin = 2,
                ConnectionCountMax = 5,

                DsmCountMin = 11,
                DsmCountMax = 256,

                TimetecMasterIncluded = false,
                TimetecMasterOptional = true,

                GraphicsIncluded = false,
                GraphicsOptional = true,

                CisIncluded = false,
                CisOptional = true,

                IdManagementIncluded = false,
                IdManagementOptional = false,

                OfflineImportIncluded = true,
                OfflineImportOptional = false,

                StructuredSiteIncluded = true,
                StructuredSiteOptional = false,

                Cat12ComboCountMin =  0,
                Cat12ComboCountMax = 10
            }
            ,
            new EditionProperties()
            {
                Name = "Enterprise",

                ConnectionCountMin = 2,
                ConnectionCountMax = 15,

                DsmCountMin = 257,
                DsmCountMax = 512,

                TimetecMasterIncluded = true,
                TimetecMasterOptional = false,

                GraphicsIncluded = false,
                GraphicsOptional = true,

                CisIncluded = false,
                CisOptional = true,

                IdManagementIncluded = false,
                IdManagementOptional = true,

                OfflineImportIncluded = true,
                OfflineImportOptional = false,

                StructuredSiteIncluded = true,
                StructuredSiteOptional = false,

                Cat12ComboCountMin =  0,
                Cat12ComboCountMax = 100,

            }
            ,
            new EditionProperties()
            {
                Name = "Premium",

                ConnectionCountMin = 2,
                ConnectionCountMax = 100,

                DsmCountMin = 513,
                DsmCountMax = 9999,

                TimetecMasterIncluded = false,
                TimetecMasterOptional = true,

                GraphicsIncluded = false,
                GraphicsOptional = true,

                CisIncluded = false,
                CisOptional = true,

                IdManagementIncluded = false,
                IdManagementOptional = true,

                OfflineImportIncluded = true,
                OfflineImportOptional = false,

                StructuredSiteIncluded = true,
                StructuredSiteOptional = false,

                Cat12ComboCountMin =  0,
                Cat12ComboCountMax = 9999

            }
        };
       
        private void OnEditionSelected(object sender, EventArgs e)
        {
            if (_cbEdition.SelectedIndex < 0)
                return;

            if (_cbEdition.SelectedIndex == _editionSelectedIndex)
                return;


            if (_editionSelectedIndex == -1 &&
                _cbEdition.SelectedIndex >= 0)
            {
                _lEdition.ForeColor = SystemColors.ControlText;
                _eConnectionCount.Visible = true;
                _eDSMCount.Visible = true;
                _eCat12ComboCount.Visible = true;

                _lConnectionCount.Visible = true;
                _lDSMCount.Visible = true;
                _lCat12ComboCount.Visible = true;
                
            }

            _editionSelectedIndex = _cbEdition.SelectedIndex;

            EditionProperties editionProperties;


            try
            {
                editionProperties = _propertiesPerEdition[_editionSelectedIndex];
            }
            catch
            {
                return;
            }

            _eConnectionCount.Maximum = editionProperties.ConnectionCountMax;
            _eConnectionCount.Minimum = editionProperties.ConnectionCountMin;
            _eConnectionCount.Value = editionProperties.ConnectionCountMin;
           
            _eDSMCount.Minimum = editionProperties.DsmCountMin;
            _eDSMCount.Maximum = editionProperties.DsmCountMax;
            _eDSMCount.Value = editionProperties.DsmCountMax;

            SetTimetecMasterCheckBoxSettings();

            _chbGraphics.Checked = editionProperties.GraphicsIncluded;
            _chbGraphics.AutoCheck = !editionProperties.GraphicsIncluded;
            _chbGraphics.Visible = editionProperties.GraphicsIncluded || editionProperties.GraphicsOptional;

            _chbCis.Checked = editionProperties.CisIncluded;
            _chbCis.AutoCheck = !editionProperties.CisIncluded;
            _chbCis.Visible = editionProperties.CisIncluded || editionProperties.CisOptional;

            _chbMaxSubsiteCount.Checked = editionProperties.StructuredSiteIncluded;
            _chbMaxSubsiteCount.AutoCheck = !editionProperties.StructuredSiteIncluded;
            _chbMaxSubsiteCount.Visible = editionProperties.StructuredSiteIncluded || editionProperties.StructuredSiteOptional;

            _chbIdManagement.Checked = editionProperties.IdManagementIncluded;
            _chbIdManagement.AutoCheck = !editionProperties.IdManagementIncluded;
            _chbIdManagement.Visible = editionProperties.IdManagementIncluded || editionProperties.IdManagementOptional;

            _chbOfflineImport.Checked = editionProperties.OfflineImportIncluded;
            _chbOfflineImport.AutoCheck = !editionProperties.OfflineImportIncluded;
            _chbOfflineImport.Visible = editionProperties.OfflineImportIncluded || editionProperties.OfflineImportOptional;

            _eCat12ComboCount.Minimum = editionProperties.Cat12ComboCountMin;
            _eCat12ComboCount.Maximum = editionProperties.Cat12ComboCountMax;
            _eCat12ComboCount.Value = editionProperties.Cat12ComboCountMin;
        }





        private void _bCreateRequest_Click(object sender, EventArgs e)
        {
            if (!AreInfosFilledAndValid())
                return;

            List<ISLAPropertyRecord> propertyRecords = CreatePropertyRecords();
            if (propertyRecords == null)
            {
                Dialog.Error("Failed to create license request");
                return;
            }

            ISLACustomerRecord customer = SLACustomerRecord.Create(
                _commonName, 
                _eCustomerName.Text, 
                string.Format("{0}, {1}",
                _eAddress.Text, 
                _cbCountries.SelectedItem.ToString()),
                _eFirstName.Text, 
                _eLastName.Text, 
                _ePhone.Text, 
                _eEmail.Text);

            if (customer == null)
            {
                Dialog.Error("Failed to create license request");
                return;
            }

            SaveFileDialog sf = new SaveFileDialog {Filter = "(Licence request) *.lreq | *.lreq"};

            if (sf.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(sf.FileName))
            {
                Dialog.Error("Failed to create license request");
                return;
            }

            ISLAPublisher publisher = SLAPublisher.Create(_publisherName, _publisherkey, sf.FileName);
            if (publisher == null)
            {
                Dialog.Error("Failed to create license request");
                return;
            }

            if (SLAClientModule.Singleton.GenerateReqFile(customer, publisher, propertyRecords) <= 0)
                Dialog.Error("Failed to create license request");
            else
                Dialog.Info("Request successfully created");
        }

        private readonly string[] _specificCountries =
            {
                "Sweden",
                "Norway",
                "Denmark",
                "Finland"                
            };

        private bool IsSpecificCountry()
        {
            if (_cbCountries.SelectedItem == null)
                return false;

            

            return _specificCountries.Contains(_cbCountries.SelectedItem.ToString());
        }

        private int PropertyName2PropertyId(string propertyName)
        {
            int result;
            _requiredProperties.TryGetValue(propertyName, out result);
            return result;
        }

        private List<ISLAPropertyRecord> CreatePropertyRecords()
        {
            try
            {
                List<ISLAPropertyRecord> result = new List<ISLAPropertyRecord>();

                var propertyValues = 
                    new Dictionary<string, string>(_requiredProperties.Count);


                propertyValues[ContalNovaLicenseManager.PropertyEdition] = _cbEdition.Text;

                propertyValues[ContalNovaLicenseManager.PropertyConnectionCount] = _eConnectionCount.Value.ToString(CultureInfo.InvariantCulture);

                propertyValues[ContalNovaLicenseManager.PropertyDoorEnvironmentCount] = _eDSMCount.Value.ToString(CultureInfo.InvariantCulture);

                propertyValues[ContalNovaLicenseManager.PropertyCISIntegration] = _chbCis.Checked.ToString();


                propertyValues[ContalNovaLicenseManager.PropertyOfflineImport] = _chbOfflineImport.Checked.ToString();

                propertyValues[ContalNovaLicenseManager.PropertyCgpNCASServerplugin] = "True";


                propertyValues[ContalNovaLicenseManager.PropertyMajorVersion] = _nudMajorVersion.Value.ToString(CultureInfo.InvariantCulture);


                propertyValues[ContalNovaLicenseManager.PropertyIdManagement] = _chbIdManagement.Checked.ToString();


                propertyValues[ContalNovaLicenseManager.PropertyGraphics] = _chbGraphics.Checked.ToString();

                propertyValues[ContalNovaLicenseManager.PropertyTimetecMaster] = _chbTimetecMaster.Checked.ToString();

                propertyValues[ContalNovaLicenseManager.PropertyMaxSubsiteCount] = _chbMaxSubsiteCount.Checked ? "999" : "0";

                if (IsSpecificCountry())
                    propertyValues[ContalNovaLicenseManager.PropertyCCU40MaxDsm] = "8";
                else
                    propertyValues[ContalNovaLicenseManager.PropertyCCU40MaxDsm] = "-1";


                if (IsSpecificCountry())
                    propertyValues[ContalNovaLicenseManager.PropertyCAT12CEMaxDsm] = "4";
                else
                    propertyValues[ContalNovaLicenseManager.PropertyCAT12CEMaxDsm] = "-1";

                if (IsSpecificCountry())
                    propertyValues[ContalNovaLicenseManager.PropertyCCU05MaxDsm] = "2";
                else
                    propertyValues[ContalNovaLicenseManager.PropertyCCU05MaxDsm] = "-1";

                propertyValues[ContalNovaLicenseManager.PropertyCat12ComboCount] = 
                    _eCat12ComboCount.Value.ToString(CultureInfo.InvariantCulture);


                foreach (var keyPair in propertyValues)
                {
                    int propertyId = PropertyName2PropertyId(keyPair.Key);

                    if (propertyId != -1)
                        result.Add(
                            SLAPropertyRecord.Create(
                                propertyId,
                                keyPair.Key, 
                                keyPair.Value
                                ));
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        private List<string> GetCountriesList()
        {
            List<string> countries = new List<string>();
            CultureInfo[] cultureInfos = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo cultureInfo in cultureInfos)
            {
                RegionInfo regionInfo = new RegionInfo(cultureInfo.LCID);

                if (!countries.Contains(regionInfo.EnglishName))
                    countries.Add(regionInfo.EnglishName);
            }

            countries.Sort();
            return countries;
        }

        private bool AreInfosFilledAndValid()
        {
            if (_eCustomerName.Text == string.Empty)
            {
                ControlNotification.Singleton.Warning(
                    NotificationPriority.Last, 
                    _eCustomerName, 
                    "Customer name must be set", 
                    ControlNotificationSettings.Default);

                _eCustomerName.Focus();
                return false;
            }

            if (_eAddress.Text == string.Empty)
            {
                ControlNotification.Singleton.Warning(
                    NotificationPriority.Last, 
                    _eAddress, 
                    "Customer address must be set", 
                    ControlNotificationSettings.Default);
                _eAddress.Focus();
                return false;
            }

            if (_cbCountries.SelectedItem == null
                || string.IsNullOrEmpty(_cbCountries.SelectedItem.ToString()))
            {
                ControlNotification.Singleton.Warning(
                    NotificationPriority.Last, 
                    _cbCountries, 
                    "Customer contry must be set", 
                    ControlNotificationSettings.Default);
                _cbCountries.Focus();
                return false;
            }

            if (_eFirstName.Text == string.Empty)
            {
                ControlNotification.Singleton.Warning(
                    NotificationPriority.Last, 
                    _eFirstName, 
                    "Customer first name must be set",
                    ControlNotificationSettings.Default);
                _eFirstName.Focus();
                return false;
            }

            if (_eLastName.Text == string.Empty)
            {
                ControlNotification.Singleton.Warning(
                    NotificationPriority.Last, 
                    _eLastName, 
                    "Customer last name must be set", 
                    ControlNotificationSettings.Default);
                _eLastName.Focus();
                return false;
            }

            if (_ePhone.Text == string.Empty)
            {
                ControlNotification.Singleton.Warning(
                    NotificationPriority.Last, 
                    _ePhone, 
                    "Customer phone must be set", 
                    ControlNotificationSettings.Default);

                _ePhone.Focus();
                return false;
            }

            if (_eEmail.Text == string.Empty)
            {
                ControlNotification.Singleton.Warning(
                    NotificationPriority.Last, 
                    _eEmail, 
                    "Customer email must be set", 
                    ControlNotificationSettings.Default);

                _eEmail.Focus();
                return false;
            }
            
            if (!EmailAddress.IsValid(_eEmail.Text))
            {
                ControlNotification.Singleton.Warning(NotificationPriority.Last, _eEmail, "Customer email must be valid", ControlNotificationSettings.Default);
                _eEmail.Focus();
                return false;
            }

            return true;
        }

        private void _chbMaxSubsiteCount_CheckedChanged(object sender, EventArgs e)
        {
            if (_chbMaxSubsiteCount.Checked && _cbEdition.SelectedIndex < 1)
                _cbEdition.SelectedIndex = 1;
        }

        private void LicenseRequest_Load(object sender, EventArgs e)
        {
            foreach (var editionProperties in _propertiesPerEdition)
            {
                _cbEdition.Items.Add(editionProperties.Name);
            }
        }

        private void _cbCountries_SelectedValueChanged(object sender, EventArgs e)
        {
            SetTimetecMasterCheckBoxSettings();
        }

        private void SetTimetecMasterCheckBoxSettings()
        {
            if (_cbCountries.SelectedValue == null
                || _editionSelectedIndex == -1)
            {
                return;
            }

            var editionProperties = _propertiesPerEdition[_editionSelectedIndex];

            _chbTimetecMaster.Checked = editionProperties.TimetecMasterIncluded
                                        && _cbCountries.SelectedValue.ToString() == "Slovakia";

            _chbTimetecMaster.AutoCheck = !editionProperties.TimetecMasterIncluded
                                          && _cbCountries.SelectedValue.ToString() == "Slovakia";

            _chbTimetecMaster.Visible = editionProperties.TimetecMasterIncluded ||
                                        editionProperties.GraphicsOptional
                                        && _cbCountries.SelectedValue.ToString() == "Slovakia";
        }
    }
}
