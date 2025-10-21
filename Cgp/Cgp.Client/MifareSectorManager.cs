using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Contal.Cgp.Server.Beans;
using System.IO;
using Contal.IwQuick.Crypto;
using Ionic.Zip;

namespace Contal.Cgp.Client
{

    public class MifareSectorManager
    {
        private static volatile MifareSectorManager _singleton = null;
        private static object _syncRoot = new object();
        private const string RESERVED_APPLICATIONS_POOL = "456789abcdefghijklmnoprstuvxyz";
        private List<string> _alternativeAuthAKeysHexa = new List<string>();
        public List<string> AlternativeAuthAKeysHexa
        {
            get { return _alternativeAuthAKeysHexa; }
            set { _alternativeAuthAKeysHexa = value; }
        }

        private List<string> _alternativeAuthBKeysHexa = new List<string>();
        public List<string> AlternativeAuthBKeysHexa
        {
            get { return _alternativeAuthBKeysHexa; }
            set { _alternativeAuthBKeysHexa = value; }
        }

        public static MifareSectorManager Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new MifareSectorManager();
                        }
                    }
                return _singleton;
            }
        }

        public bool FillEncodingData(XmlDocument xmlDoc, string cardNumber, CardSystem cardSystem)
        {
            if (xmlDoc == null
                || cardNumber == null || cardNumber == string.Empty
                || cardSystem == null || cardSystem.CardData == null || !(cardSystem.CardData is MifareSectorData)
                || (cardSystem.CardData as MifareSectorData).Encoding == null)
                return false;

            MifareSectorCardStoreInfo storeInfo = GetCardStoreInfoFromCardSystem(cardSystem.CardData as MifareSectorData);
            if (storeInfo == null)
                return false;

            int lastFieldIndex = CardPrintAndEncodeManager.GetLastUsedFieldIndexFromXML(xmlDoc);

            MemoryStream dscStream = CreateEncodingMemoryStream(storeInfo, lastFieldIndex + 1);

            InsertUserEncodingVariables(xmlDoc, storeInfo, lastFieldIndex + 1);

            MemoryStream compressedStream = new MemoryStream();
            using (ZipFile zip = new ZipFile())
            {
                zip.AddEntry("mifare.dsc", dscStream.ToArray());
                zip.Save(compressedStream);
            }
            string encodedBase64 = Convert.ToBase64String(compressedStream.ToArray());

            AddEncodingNode(xmlDoc, encodedBase64);

            cardNumber = CardPrintAndEncodeManager.AddCompanyCodeToCardNumber(cardNumber, cardSystem);
            WriteCardNumberToNodes(xmlDoc, cardNumber, storeInfo, (EncodingType)((cardSystem.CardData as MifareSectorData).Encoding.Value));

            return true;
        }

        private MifareSectorCardStoreInfo GetCardStoreInfoFromCardSystem(MifareSectorData cardSystemData)
        {
            if (cardSystemData == null || cardSystemData.CypherData == null)
                return null;

            byte[] msg = cardSystemData.CypherData;
            if (msg == null) return null;

            byte part1Length = msg[3];
            byte part2Length = msg[4];
            byte part3Length = msg[5];

            if (cardSystemData.Encoding == (byte)EncodingType.Bcd)
            {
                part1Length = (byte)(part1Length * 2);
                part2Length = (byte)(part2Length * 2);
                part3Length = (byte)(part3Length * 2);
            }

            byte[] crypted = new byte[msg.Length - 6];
            Array.Copy(msg, 6, crypted, 0, msg.Length - 6);
            XTEA xtea = new XTEA();
            xtea.XTeaInit();
            byte[] decrypted = xtea.XTeaFrameDec(crypted);

            byte part1Sector = decrypted[0];
            byte part1Ofset = decrypted[2];

            byte part2Sector = decrypted[4];
            byte part2Ofset = decrypted[6];

            byte part3Sector = decrypted[8];
            byte part3Ofset = decrypted[10];

            MifareSectorSectorInfo storeInfo = null;
            return new MifareSectorCardStoreInfo()
            {
                AKey = cardSystemData.GeneralAKey,
                BKey = cardSystemData.GeneralBKey,
                Aid = cardSystemData.Aid,
                FirstPart = part1Length == 0 ? null : new MifareSectorCardNumberStorePartInfo()
                {
                    PhysicalSector = part1Sector,
                    Offset = part1Ofset,
                    Length = part1Length,
                    AKey = (storeInfo = cardSystemData.SectorsInfo.First(s => s.Bank == 1)) == null ? null : storeInfo.InheritAKey ? cardSystemData.GeneralAKey : storeInfo.AKey,
                    BKey = storeInfo == null ? null : storeInfo.InheritBKey ? cardSystemData.GeneralBKey : storeInfo.BKey
                },
                SecondPart = part2Length == 0 ? null : new MifareSectorCardNumberStorePartInfo()
                {
                    PhysicalSector = part2Sector,
                    Offset = part2Ofset,
                    Length = part2Length,
                    AKey = (storeInfo = cardSystemData.SectorsInfo.First(s => s.Bank == 2)) == null ? null : storeInfo.InheritAKey ? cardSystemData.GeneralAKey : storeInfo.AKey,
                    BKey = storeInfo == null ? null : storeInfo.InheritBKey ? cardSystemData.GeneralBKey : storeInfo.BKey
                },
                ThirdPart = part3Length == 0 ? null : new MifareSectorCardNumberStorePartInfo()
                {
                    PhysicalSector = part3Sector,
                    Offset = part3Ofset,
                    Length = part3Length,
                    AKey = (storeInfo = cardSystemData.SectorsInfo.First(s => s.Bank == 3)) == null ? null : storeInfo.InheritAKey ? cardSystemData.GeneralAKey : storeInfo.AKey,
                    BKey = storeInfo == null ? null : storeInfo.InheritBKey ? cardSystemData.GeneralBKey : storeInfo.BKey
                },
                ReservedSectors = cardSystemData.SectorsInfo.Where(si => si.Bank == null && (si.BKey != null || si.InheritBKey)).
                Select(new Func<MifareSectorSectorInfo, MifareReservedSectorInfo>(rs =>
                    {
                        return new MifareReservedSectorInfo()
                        {
                            PhysicalSector = rs.SectorNumber,
                            AKey = rs.InheritAKey ? cardSystemData.GeneralAKey : rs.AKey,
                            BKey = rs.InheritBKey ? cardSystemData.GeneralBKey : rs.BKey
                        };
                    }
                    )).ToList()
            };
        }

        private MemoryStream CreateEncodingMemoryStream(MifareSectorCardStoreInfo storeInfo, int firstAvailableIndex)
        {
            if (storeInfo == null)
                return null;

            MemoryStream result = new MemoryStream();
            StreamWriter dscWriter = new StreamWriter(result);
            dscWriter.WriteLine("[Encoding]");
            dscWriter.WriteLine("Type=2048,Mifare");
            dscWriter.WriteLine("SubType=1");
            dscWriter.WriteLine("");
            dscWriter.WriteLine("[Fields]");
            string fields = CreateFields(storeInfo, firstAvailableIndex);
            dscWriter.WriteLine(fields);
            dscWriter.WriteLine("[Description]");
            dscWriter.WriteLine("DescVer=3");
            dscWriter.WriteLine("ChipNr_00000=,UID,4,,,H,B,,,,");
            string applicationList = CreateApplicationList(storeInfo);
            dscWriter.WriteLine("Applicationlist=" + applicationList);
            dscWriter.WriteLine("");
            string applicationsDefinition = CreateApplicationsDefinition(storeInfo, applicationList);
            dscWriter.WriteLine(applicationsDefinition);
            dscWriter.Flush();

            return result;
        }

        private string CreateFields(MifareSectorCardStoreInfo storeInfo, int firstAvailableIndex)
        {
            if (storeInfo == null)
                return string.Empty;

            StringBuilder result = new StringBuilder();
            if (storeInfo.FirstPart != null)
            {
                result.AppendLine("MifareSector" + storeInfo.FirstPart.PhysicalSector + "=Field" + firstAvailableIndex++);
            }
            if (storeInfo.SecondPart != null)
            {
                result.AppendLine("MifareSector" + storeInfo.SecondPart.PhysicalSector + "=Field" + firstAvailableIndex++);
            }
            if (storeInfo.ThirdPart != null)
            {
                result.AppendLine("MifareSector" + storeInfo.ThirdPart.PhysicalSector + "=Field" + firstAvailableIndex++);
            }

            result.AppendLine("UID=Field" + firstAvailableIndex++);
            return result.ToString();
        }

        private string CreateApplicationList(MifareSectorCardStoreInfo storeInfo)
        {
            if (storeInfo == null)
                return string.Empty;

            StringBuilder result = new StringBuilder();

            if (storeInfo.FirstPart != null)
            {
                result.Append("1");
            }
            if (storeInfo.SecondPart != null)
            {
                result.Append("2");
            }
            if (storeInfo.ThirdPart != null)
            {
                result.Append("3");
            }

            int index = 0;
            foreach (MifareReservedSectorInfo reserved in storeInfo.ReservedSectors)
            {
                result.Append(RESERVED_APPLICATIONS_POOL[index++]);
            }

            return result.ToString();
        }

        private string CreateApplicationsDefinition(MifareSectorCardStoreInfo storeInfo, string applicationList)
        {
            if (storeInfo == null || applicationList == null || applicationList.Length == 0)
                return string.Empty;

            StringBuilder result = new StringBuilder();
            foreach (char application in applicationList)
            {
                result.AppendLine("[Application_" + application + "]");
                result.AppendLine("");
                result.AppendLine("Name=Sector" + application);
                result.AppendLine("DescVer=2,0,0");
                result.AppendLine("");
                int physicalSector = GetSectorForApplication(application, storeInfo);
                result.AppendLine("Sector=" + physicalSector);
                result.AppendLine("");

                //A keys for authentication
                string aKey = GetKeyForApplication(application, storeInfo, true);
                result.Append("AutKeyA=ffffffffffff,a0a1a2a3a4a5," + aKey);
                if (_alternativeAuthAKeysHexa != null)
                    _alternativeAuthAKeysHexa.ForEach(key => result.Append("," + key));
                result.AppendLine("");

                //B keys for authentication
                string bKey = GetKeyForApplication(application, storeInfo, false);
                result.Append("AutKeyB=ffffffffffff,b0b1b2b3b4b5," + bKey);
                if (_alternativeAuthBKeysHexa != null)
                    _alternativeAuthBKeysHexa.ForEach(key => result.Append("," + key));
                result.AppendLine("");

                result.AppendLine("NewKeyA=" + aKey);
                result.AppendLine("NewKeyB=" + bKey);
                result.AppendLine("NewAcCo=78778800");
                result.AppendLine("");

                if (application == '1' || application == '2' || application == '3')
                {
                    string addressString = GetApplicationAddressString(storeInfo, application);
                    if (physicalSector == 0)
                    {
                        result.AppendLine("RdArea=16,32");
                        result.AppendLine("WrArea=16,32");
                        result.AppendLine("ADR_" + addressString + "=MifareSector" + physicalSector + ",MifareSector" + physicalSector + ",X,,,H");
                    }
                    else
                    {
                        result.AppendLine("RdArea=0,48");
                        result.AppendLine("WrArea=0,48");
                        result.AppendLine("ADR_" + addressString + "=MifareSector" + physicalSector + ",MifareSector" + physicalSector + ",X,,,H");
                    }
                }

                result.AppendLine("");
            }

            return result.ToString();
        }

        private string GetApplicationAddressString(MifareSectorCardStoreInfo storeInfo, char application)
        {
            if (storeInfo == null)
                return null;

            if (application == '1')
            {
                if (storeInfo.FirstPart == null)
                    return null;

                return storeInfo.FirstPart.Offset.ToString().PadLeft(5, '0');
            }
            else if (application == '2')
            {
                if (storeInfo.SecondPart == null)
                    return null;

                return storeInfo.SecondPart.Offset.ToString().PadLeft(5, '0');
            }
            else if (application == '3')
            {
                if (storeInfo.ThirdPart == null)
                    return null;

                return storeInfo.ThirdPart.Offset.ToString().PadLeft(5, '0');
            }

            return "00000";
        }

        private int GetSectorForApplication(char application, MifareSectorCardStoreInfo storeInfo)
        {
            if (storeInfo == null)
                return 0;

            if (application == '1')
            {
                if (storeInfo.FirstPart == null)
                    return 0;
                else
                    return storeInfo.FirstPart.PhysicalSector;
            }
            else if (application == '2')
            {
                if (storeInfo.SecondPart == null)
                    return 0;
                else
                    return storeInfo.SecondPart.PhysicalSector;
            }
            else if (application == '3')
            {
                if (storeInfo.ThirdPart == null)
                    return 0;
                else
                    return storeInfo.ThirdPart.PhysicalSector;
            }
            else if (RESERVED_APPLICATIONS_POOL.Contains(application))
            {
                return storeInfo.ReservedSectors[RESERVED_APPLICATIONS_POOL.IndexOf(application)].PhysicalSector;
            }

            return 0;
        }

        private string GetKeyForApplication(char application, MifareSectorCardStoreInfo storeInfo, bool aKey)
        {
            if (storeInfo == null)
                return string.Empty;

            if (application == '1')
            {
                if (storeInfo.FirstPart == null)
                    return string.Empty;
                else
                {
                    if (aKey)
                        return CardPrintAndEncodeManager.GetHexaDecryptedStringFromKey(storeInfo.FirstPart.AKey);
                    else
                        return CardPrintAndEncodeManager.GetHexaDecryptedStringFromKey(storeInfo.FirstPart.BKey);
                }
            }
            else if (application == '2')
            {
                if (storeInfo.SecondPart == null)
                    return string.Empty;
                else
                {
                    if (aKey)
                        return CardPrintAndEncodeManager.GetHexaDecryptedStringFromKey(storeInfo.SecondPart.AKey);
                    else
                        return CardPrintAndEncodeManager.GetHexaDecryptedStringFromKey(storeInfo.SecondPart.BKey);
                }
            }
            else if (application == '3')
            {
                if (storeInfo.ThirdPart == null)
                    return string.Empty;
                else
                {
                    if (aKey)
                        return CardPrintAndEncodeManager.GetHexaDecryptedStringFromKey(storeInfo.ThirdPart.AKey);
                    else
                        return CardPrintAndEncodeManager.GetHexaDecryptedStringFromKey(storeInfo.ThirdPart.BKey);
                }
            }
            else if (RESERVED_APPLICATIONS_POOL.Contains(application))
            {
                if (aKey)
                    return CardPrintAndEncodeManager.GetHexaDecryptedStringFromKey(storeInfo.ReservedSectors[RESERVED_APPLICATIONS_POOL.IndexOf(application)].AKey);
                else
                    return CardPrintAndEncodeManager.GetHexaDecryptedStringFromKey(storeInfo.ReservedSectors[RESERVED_APPLICATIONS_POOL.IndexOf(application)].BKey);
            }

            return string.Empty;
        }

        private void InsertUserEncodingVariables(XmlDocument xmlDoc, MifareSectorCardStoreInfo storeInfo, int firstAvailableIndex)
        {
            if (xmlDoc == null || storeInfo == null)
                return;

            XmlNodeList fieldsNodes = xmlDoc.GetElementsByTagName("fields");
            if (fieldsNodes == null || fieldsNodes.Count == 0)
                return;

            XmlNode fieldNode = fieldsNodes[0];

            if (storeInfo.FirstPart != null)
            {
                XmlNode textNode = xmlDoc.CreateElement("text");
                textNode.Attributes.Append(xmlDoc.CreateAttribute("name"));
                textNode.Attributes["name"].Value = "Field" + firstAvailableIndex++.ToString("00");
                textNode.Attributes.Append(xmlDoc.CreateAttribute("text"));
                textNode.Attributes["text"].Value = "MifareSector" + storeInfo.FirstPart.PhysicalSector;
                fieldNode.AppendChild(textNode);
            }
            if (storeInfo.SecondPart != null)
            {
                XmlNode textNode = xmlDoc.CreateElement("text");
                textNode.Attributes.Append(xmlDoc.CreateAttribute("name"));
                textNode.Attributes["name"].Value = "Field" + firstAvailableIndex++.ToString("00");
                textNode.Attributes.Append(xmlDoc.CreateAttribute("text"));
                textNode.Attributes["text"].Value = "MifareSector" + storeInfo.SecondPart.PhysicalSector;
                fieldNode.AppendChild(textNode);
            }
            if (storeInfo.ThirdPart != null)
            {
                XmlNode textNode = xmlDoc.CreateElement("text");
                textNode.Attributes.Append(xmlDoc.CreateAttribute("name"));
                textNode.Attributes["name"].Value = "Field" + firstAvailableIndex++.ToString("00");
                textNode.Attributes.Append(xmlDoc.CreateAttribute("text"));
                textNode.Attributes["text"].Value = "MifareSector" + storeInfo.ThirdPart.PhysicalSector;
                fieldNode.AppendChild(textNode);
            }

            XmlNode node = xmlDoc.CreateElement("text");
            node.Attributes.Append(xmlDoc.CreateAttribute("name"));
            node.Attributes["name"].Value = "Field" + firstAvailableIndex++.ToString("00");
            node.Attributes.Append(xmlDoc.CreateAttribute("text"));
            node.Attributes["text"].Value = "UID";
            fieldNode.AppendChild(node);
        }

        private void AddEncodingNode(XmlDocument xmlDoc, string encodedBase64)
        {
            RemoveEncodingElements(xmlDoc);

            if (xmlDoc == null)
                return;

            XmlElement mifareElement = xmlDoc.CreateElement("mifare");
            mifareElement.Attributes.Append(xmlDoc.CreateAttribute("name"));
            mifareElement.Attributes["name"].Value = "Mifare standard sector reading";
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
                XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("mifare");
                if (xmlNodes != null && xmlNodes.Count > 0)
                    for (int i = xmlNodes.Count - 1; i >= 0; i--)
                    {
                        xmlDoc.GetElementsByTagName("applications")[0].RemoveChild(xmlNodes[i]);
                    }
            }
            catch { }
        }

        private void WriteCardNumberToNodes(XmlDocument xmlDoc, string cardNumber, MifareSectorCardStoreInfo storeInfo, EncodingType encoding)
        {
            if (xmlDoc == null
                || cardNumber == null || cardNumber.Length == 0
                || storeInfo == null)
                return;

            XmlNodeList textNodes = xmlDoc.GetElementsByTagName("text");
            if (textNodes == null || textNodes.Count == 0)
                return;

            if (storeInfo.FirstPart != null)
            {
                try
                {
                    string cardNumberPart = cardNumber.Substring(0, storeInfo.FirstPart.Length);
                    StoreCardNumberPart(textNodes, cardNumberPart, storeInfo.FirstPart, encoding);
                }
                catch { }
            }

            if (storeInfo.SecondPart != null)
            {
                int substringStartIndex = storeInfo.FirstPart == null ? 0 : storeInfo.FirstPart.Length;
                try
                {
                    string cardNumberPart = cardNumber.Substring(substringStartIndex, storeInfo.SecondPart.Length);
                    StoreCardNumberPart(textNodes, cardNumberPart, storeInfo.SecondPart, encoding);
                }
                catch { }
            }

            if (storeInfo.ThirdPart != null)
            {
                int substringStartIndex = storeInfo.FirstPart == null ? 0 : storeInfo.FirstPart.Length;
                substringStartIndex += storeInfo.SecondPart == null ? 0 : storeInfo.SecondPart.Length;
                try
                {
                    string cardNumberPart = cardNumber.Substring(substringStartIndex, storeInfo.ThirdPart.Length);
                    StoreCardNumberPart(textNodes, cardNumberPart, storeInfo.ThirdPart, encoding);
                }
                catch { }
            }
        }

        private void StoreCardNumberPart(XmlNodeList allTemplateNodes, string partToStore, MifareSectorCardNumberStorePartInfo cardStorePartInfo, EncodingType encoding)
        {
            if (allTemplateNodes == null || allTemplateNodes.Count == 0
                || partToStore == null || partToStore.Length == 0
                || cardStorePartInfo == null)
                return;

            string encodedPartToStore = CardPrintAndEncodeManager.EncodeString(partToStore, encoding);

            for (int i = 0; i < allTemplateNodes.Count; i++)
            {
                XmlNode node = allTemplateNodes[i];
                XmlAttribute textAttribute = node.Attributes["text"];
                if (textAttribute == null)
                    continue;

                if (textAttribute.Value == "MifareSector" + cardStorePartInfo.PhysicalSector)
                    node.InnerText = encodedPartToStore;
            }
        }

        private class MifareSectorCardStoreInfo
        {
            private string _aid = null;
            public string Aid
            {
                get { return _aid; }
                set { _aid = value; }
            }

            private byte[] _aKey = null;
            public byte[] AKey
            {
                get { return _aKey; }
                set { _aKey = value; }
            }

            private byte[] _bKey = null;
            public byte[] BKey
            {
                get { return _bKey; }
                set { _bKey = value; }
            }

            private MifareSectorCardNumberStorePartInfo _firstPart;
            public MifareSectorCardNumberStorePartInfo FirstPart
            {
                get { return _firstPart; }
                set { _firstPart = value; }
            }

            private MifareSectorCardNumberStorePartInfo _secondPart;
            public MifareSectorCardNumberStorePartInfo SecondPart
            {
                get { return _secondPart; }
                set { _secondPart = value; }
            }

            private MifareSectorCardNumberStorePartInfo _thirdPart;
            public MifareSectorCardNumberStorePartInfo ThirdPart
            {
                get { return _thirdPart; }
                set { _thirdPart = value; }
            }

            private List<MifareReservedSectorInfo> _reservedSectors;
            public List<MifareReservedSectorInfo> ReservedSectors
            {
                get { return _reservedSectors; }
                set { _reservedSectors = value; }
            }
        }

        private class MifareReservedSectorInfo
        {
            private int _physicalSector = 0;
            public int PhysicalSector
            {
                get { return _physicalSector; }
                set { _physicalSector = value; }
            }

            private byte[] _aKey = null;
            public byte[] AKey
            {
                get { return _aKey; }
                set { _aKey = value; }
            }

            private byte[] _bKey = null;
            public byte[] BKey
            {
                get { return _bKey; }
                set { _bKey = value; }
            }
        }

        private class MifareSectorCardNumberStorePartInfo
        {
            private int _physicalSector = 0;
            public int PhysicalSector
            {
                get { return _physicalSector; }
                set { _physicalSector = value; }
            }

            private int _offset = 0;
            public int Offset
            {
                get { return _offset; }
                set { _offset = value; }
            }


            private int _length = 0;
            public int Length
            {
                get { return _length; }
                set { _length = value; }
            }

            private byte[] _aKey = null;
            public byte[] AKey
            {
                get { return _aKey; }
                set { _aKey = value; }
            }

            private byte[] _bKey = null;
            public byte[] BKey
            {
                get { return _bKey; }
                set { _bKey = value; }
            }
        }
    }
}
