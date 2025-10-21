using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Contal.SLA.Client.Interfaces;
using Contal.SLA.Client;
using System.Text.RegularExpressions;

namespace Contal.Cgp.Server
{
    class LicenceRequest
    {
        private static volatile LicenceRequest _singleton = null;
        private static object _syncRoot = new object();

        public static LicenceRequest Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new LicenceRequest();
                    }

                return _singleton;
            }
        }

        public bool LoadPublisherAndDescriptiveFile(out string publisherName, out string publisherKey, out string commonName,
            out Dictionary<int, string> propertyNames,
            out Dictionary<int, string> propertyTypes, out Dictionary<int, string> propertyValues, out Dictionary<int, string> propertyDescriptions)
        {
            publisherName = "";
            publisherKey = "";
            commonName = "";
            propertyNames = new Dictionary<int, string>();
            propertyTypes = new Dictionary<int, string>();
            propertyValues = new Dictionary<int, string>();
            propertyDescriptions = new Dictionary<int, string>();
            bool publisherLoaded = false;
            bool descriptiveFileLoaded = false;
            foreach (string filePath in Directory.GetFiles(Contal.IwQuick.Sys.QuickPath.AssemblyStartupPath))
            {
                if (filePath.Length >= 4 && filePath.Substring(filePath.Length - 4, 4) == ".crt")
                {
                    if (!publisherLoaded)
                        publisherLoaded = SLAClientModule.Singleton.IsValidCertificateFile(filePath, out publisherName, out publisherKey);
                }
                if (filePath.Length >= 5 && filePath.Substring(filePath.Length - 5, 5) == ".ldsc")
                {
                    if (!descriptiveFileLoaded)
                        descriptiveFileLoaded = SLAClientModule.Singleton.IsValidDescriptiveFile(filePath, out commonName, out propertyNames, out propertyTypes, out propertyValues, out propertyDescriptions);
                }
            }
            return (publisherLoaded && descriptiveFileLoaded);
        }

        public List<ISLAPropertyRecord> CreatePropertyRecords(Dictionary<int, string> propertyNames, Dictionary<int, string> propertyValues)
        {
            List<ISLAPropertyRecord> returnValue = new List<ISLAPropertyRecord>();
            foreach (KeyValuePair<int, string> propertyName in propertyNames)
            {
                returnValue.Add(SLAPropertyRecord.Create(propertyName.Key, propertyName.Value, propertyValues[propertyName.Key]));
            }
            return returnValue;

        }

        public ISLACustomerRecord CreateCustomerRecord(string commonName, string CustomerName, string customerAddress, string contactFirstName, string contactSecondName, string contactPhone, string contactEmail)
        {
            return SLACustomerRecord.Create(commonName, CustomerName, customerAddress, contactFirstName, contactSecondName, contactPhone, contactEmail);
        }

        public ISLAPublisher CreatePublisher(string publisherName, string publisherKey, string filePath)
        {
            return SLAPublisher.Create(publisherName, publisherKey, filePath);
        }

        public int CreateLicenceRequestFile(ISLACustomerRecord customerRecord, ISLAPublisher publisher, List<ISLAPropertyRecord> propList)
        {
            return SLAClientModule.Singleton.GenerateReqFile(customerRecord, publisher, propList);
        }
    }
}
