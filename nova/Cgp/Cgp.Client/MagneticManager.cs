using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Server.Beans;
using System.IO;
using System.Xml;
using Ionic.Zip;

namespace Contal.Cgp.Client
{
    public class MagneticManager
    {
        private static volatile MagneticManager _singleton = null;
        private static object _syncRoot = new object();

        public static MagneticManager Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new MagneticManager();
                        }
                    }
                return _singleton;
            }
        }

        public bool FillEncodingData(System.Xml.XmlDocument xmlDoc, string cardNumber, CardSystem cardSystem)
        {
            if (xmlDoc == null || cardNumber == null || cardNumber == string.Empty || cardSystem == null)
                return false;

            int lastFieldIndex = CardPrintAndEncodeManager.GetLastUsedFieldIndexFromXML(xmlDoc);
            MemoryStream dscStream = CreateEncodingMemoryStream(lastFieldIndex + 1);

            InsertUserEncodingVariables(xmlDoc, lastFieldIndex + 1);

            MemoryStream compressedStream = new MemoryStream();
            using (ZipFile zip = new ZipFile())
            {
                zip.AddEntry("magnetic.dsc", dscStream.ToArray());
                zip.Save(compressedStream);
            }

            string encodedBase64 = Convert.ToBase64String(compressedStream.ToArray());
            AddEncodingNode(xmlDoc, encodedBase64);
            cardNumber = CardPrintAndEncodeManager.AddCompanyCodeToCardNumber(cardNumber, cardSystem);
            WriteCardNumberToNodes(xmlDoc, cardNumber, EncodingType.Bcd);

            return true;
        }

        private void WriteCardNumberToNodes(XmlDocument xmlDoc, string cardNumber, EncodingType encodingType)
        {
            if (xmlDoc == null
                || cardNumber == null || cardNumber.Length == 0)
                return;

            XmlNodeList textNodes = xmlDoc.GetElementsByTagName("text");
            if (textNodes == null || textNodes.Count == 0)
                return;

            int index = 0;
            StringBuilder hexaString = new StringBuilder();
            
            foreach (char letter in cardNumber)
            {
                if (index % 2 == 0)
                {
                    if (index + 2 <= cardNumber.Length)
                        hexaString.Append(cardNumber.Substring(index, 2));
                    else
                        hexaString.Append(cardNumber.Substring(index, 1));
                }

                index++;
            }

            for (int i = 0; i < textNodes.Count; i++)
            {
                XmlNode node = textNodes[i];
                XmlAttribute textAttribute = node.Attributes["text"];
                if (textAttribute == null)
                    continue;

                if (textAttribute.Value.ToLower() == "track 2")
                    node.InnerText = hexaString.ToString();
            }

        }

        private void AddEncodingNode(XmlDocument xmlDoc, string encodedBase64)
        {
            RemoveEncodingElements(xmlDoc);

            if (xmlDoc == null)
                return;

            XmlElement mifareElement = xmlDoc.CreateElement("magstripe");
            mifareElement.Attributes.Append(xmlDoc.CreateAttribute("name"));
            mifareElement.Attributes["name"].Value = "Magstripe Track2 Standard ISO 7811 HiCo";
            mifareElement.InnerText = encodedBase64;

            XmlNodeList appNodes = xmlDoc.GetElementsByTagName("applications");
            if (appNodes == null || appNodes.Count == 0)
                return;

            appNodes[0].AppendChild(mifareElement);
        }

        public static void RemoveEncodingElements(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
                return;

            try
            {
                XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("magstripe");
                if (xmlNodes != null && xmlNodes.Count > 0)
                    for (int i = xmlNodes.Count - 1; i >= 0; i--)
                    {
                        xmlDoc.GetElementsByTagName("applications")[0].RemoveChild(xmlNodes[i]);
                    }
            }
            catch { }
        }

        private void InsertUserEncodingVariables(XmlDocument xmlDoc, int firstAvailableFieldIndex)
        {
            if (xmlDoc == null)
                return;

            XmlNodeList fieldsNodes = xmlDoc.GetElementsByTagName("fields");
            if (fieldsNodes == null || fieldsNodes.Count == 0)
                return;

            XmlNode fieldNode = fieldsNodes[0];

            XmlNode textNode = xmlDoc.CreateElement("text");
            textNode.Attributes.Append(xmlDoc.CreateAttribute("name"));
            textNode.Attributes["name"].Value = "Field" + firstAvailableFieldIndex++.ToString("00");
            textNode.Attributes.Append(xmlDoc.CreateAttribute("text"));
            textNode.Attributes["text"].Value = "Track 2";
            fieldNode.AppendChild(textNode);
        }

        private MemoryStream CreateEncodingMemoryStream(int firstAvailableFieldIndex)
        {
            MemoryStream result = new MemoryStream();
            StreamWriter dscWriter = new StreamWriter(result);
            dscWriter.WriteLine("[Encoding]");
            dscWriter.WriteLine("Type=2,Magstripe");
            dscWriter.WriteLine("SubType=1");
            dscWriter.WriteLine("");
            dscWriter.WriteLine("[Fields]");
            dscWriter.WriteLine("Track 2=Field" + firstAvailableFieldIndex);
            dscWriter.WriteLine("");
            dscWriter.WriteLine("[Description]");
            dscWriter.WriteLine("Applicationlist=1");
            dscWriter.WriteLine("");
            dscWriter.WriteLine("[Application_1]");
            dscWriter.WriteLine("");
            dscWriter.WriteLine("Name=ISO");
            dscWriter.WriteLine("DescVer=2,0,0");
            dscWriter.WriteLine("");
            dscWriter.WriteLine("Format=ISO");
            dscWriter.WriteLine("Coerc=HiCo");
            dscWriter.WriteLine("Track=2");
            dscWriter.WriteLine("");
            dscWriter.WriteLine("ADR_00000=Track 2,Track 2,X,,,S");
            dscWriter.Flush();

            return result;
        }
    }
}
