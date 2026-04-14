using System;
using System.Globalization;
using System.Linq;
using System.Xml;

using Microsoft.Win32;
using System.IO;

namespace Contal.IwQuick.Sys
{
    public class HeaderErrorException : Exception
    {
        public HeaderErrorException(string msg)
            : base(msg)
        {
        }

        public HeaderErrorException()
        {
        }
    }

    public class RegistrySerializer
    {
        //private const string TAG_CONFIG_FILE = "config_file";
        private const string TAG_HEADER = "header";
        //private const string TAG_REGISTRY_SETTINGS = "registry_settings";
        private const string TAG_SECTION = "section";
        private const string TAG_VALUE = "value";
        private const string ATR_NAME = "name";
        private const string ATR_TYPE = "type";
        private const int OPTIMAL_FILESTREAM_BUFFERSIZE = 32768;

        private static readonly Log _log = new Log("RegistrySerializer", true, true, null, 0);
        //private XmlDocument _xmlDoc = null;
        //private XmlNode _currentSection = null;

        /// <summary>
        /// Deserialize the values from xml files to registry
        /// </summary>
        /// <param name="filename">Filename containing the serialized registry</param>
        public bool Deserialize(string filename)
        {
            var retValue = true;
            var registrySection = string.Empty;
            XmlReader xR = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, OPTIMAL_FILESTREAM_BUFFERSIZE);
                xR = XmlReader.Create(fs);
                while (xR.Read())
                {
                    switch (xR.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (xR.Name)
                            {
                                case TAG_SECTION:
                                    registrySection = xR.GetAttribute(ATR_NAME);
                                    break;
                                case TAG_VALUE:
                                    string valueType = xR.GetAttribute(ATR_TYPE); 
                                    string valueName = xR.GetAttribute(ATR_NAME);
                                    var val = xR.ReadString();
                                    SetRegistry(registrySection,valueName,valueType,val);
                                    break;
                            }
                            break;
                    }
                }
                
            }
            catch (Exception ex)
            {
                _log.Error("Registry deserializer error: " + ex.Message);
                retValue = false;
            }
            finally
            {
                xR.TryClose();
                fs.TryClose();
            }
            return retValue;
        }

        public bool IsHeaderCorrect(string filename, string header)
        {
            var retValue = false;
            XmlReader xR = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, OPTIMAL_FILESTREAM_BUFFERSIZE);
                xR = XmlReader.Create(fs);
                while (xR.Read())
                {
                    if (xR.NodeType == XmlNodeType.Element)
                    {
                        if (xR.Name == TAG_HEADER)
                        {
                            var val = xR.ReadString();
                            if (val == header)
                            {
                                retValue = true;
                                break;
                            }
                        }
                    }
                }
                xR.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                _log.Error("Registry header check error: " + ex.Message);
                retValue = false;
            }
            finally
            {
                if (xR != null)
                    try { xR.Close(); }
                    catch { }
                if (fs != null)
                    try { fs.Close(); }
                    catch { }
            }
            return retValue;
        }

        private void SetRegistry(string registrySection,string valueName, string valueType, string value)
        {
            if (valueName.Equals("@"))
                // Registry.SetValue accepts empty as definition for (Default)
                valueName = String.Empty;

            try
            {
                switch (valueType)
                {
                    case "binary":
                        var arrayStr = value.Split('#');
                        var arrayByte = new byte[arrayStr.Length];
                        for (var i = 0; i < arrayStr.Length; i++)
                            arrayByte[i] = Convert.ToByte(arrayStr[i]);
                        Registry.SetValue(registrySection, valueName, arrayByte);

                        break;
                    case "dword":
                        int intValue32;
                        if (value.IndexOf("0x",StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            value = value.Substring(2);
                            intValue32 = Int32.Parse(value, NumberStyles.HexNumber);
                        }
                        else
                            intValue32 = Int32.Parse(value);

                        Registry.SetValue(registrySection, valueName, intValue32);
                        break;
                    case "expandstring":
                        Console.WriteLine("Deserialize expandstring");
                        System.Diagnostics.Debug.WriteLine("Not implemented");
                        //throw new NotImplementedException();
                        break;
                    case "multistring":
                        var arrayString = value.Split('#');
                        Registry.SetValue(registrySection, valueName, arrayString);
                        break;
                    case "qword":
                        var intValue64 = Int64.Parse(value);
                        Registry.SetValue(registrySection, valueName, intValue64);
                        break;
                    case "string":
                        var stringValue = value;
                        Registry.SetValue(registrySection, valueName, stringValue);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Unknown value type.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Registry section deserializer error: " + ex.Message);
            }
        }

        public void Serialize(RegistryKey section, string filename, string header)
        {
            XmlWriter xW = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read, OPTIMAL_FILESTREAM_BUFFERSIZE);
                xW = XmlWriter.Create(fs);
                xW.WriteStartDocument();
                xW.WriteStartElement("config_file");
                xW.WriteElementString(TAG_HEADER, header);
                xW.WriteStartElement("registry_settings");
                SerializeSection(xW,section);
                xW.WriteEndElement();
                xW.WriteEndDocument();
                xW.Close();
                fs.Close();
                xW = null;
                fs = null;
                _log.Info("Serialize registry is succeeded. Create backup file " + filename);
            }
            catch (Exception e)
            {
                _log.Error("Registry section serializer error: " + e.Message);
                System.Diagnostics.Debug.WriteLine("Registry section serializer error: " + e.Message);
                throw new Exception("Registry section serializer error: " + e.Message);
            }
            finally
            {
                if (xW != null)
                    try { xW.Close(); }
                    catch { }
                if (fs != null)
                    try { fs.Close(); }
                    catch { }
            }
        }
        private void SerializeSection(XmlWriter xW,RegistryKey section)
        {
            xW.WriteStartElement(TAG_SECTION);
            xW.WriteAttributeString(ATR_NAME, section.Name);

            // read all values for this section
            var valueNames = section.GetValueNames();
            foreach (var valueName in valueNames)
            {
                var valueKind = section.GetValueKind(valueName);
                var value = section.GetValue(valueName);

                string attrType = null;
                var valueToString = string.Empty;
                switch (valueKind)
                {
                    case RegistryValueKind.Binary:
                        var binaryValue = (byte[])value;
                        for (var i = 0; i < binaryValue.Length; i++)
                        {
                            if (i != (binaryValue.Length - 1))
                                valueToString += binaryValue[i] + "#";
                            else
                                valueToString += binaryValue[i].ToString(CultureInfo.InvariantCulture);
                        }
                        attrType = "binary";
                        break;
                    case RegistryValueKind.DWord:
                        var intValue32 = (Int32)value;
                        attrType = "dword";
                        valueToString = intValue32.ToString(CultureInfo.InvariantCulture);
                        break;
                    case RegistryValueKind.ExpandString:
                        Console.WriteLine("Serialize ExpandString - Not implemented");
                        System.Diagnostics.Debug.WriteLine("ExpandString - Not implemented");
                        // will do nothing
                        //throw new NotImplementedException();
                        break;
                    /*attrType = "expandstring";
                    break;*/
                    case RegistryValueKind.MultiString:
                        var mStrValue = (string[])value;
                        attrType = "multistring";

                        for (var i = 0; i < mStrValue.Length; i++)
                        {
                            if (i != (mStrValue.Length - 1))
                                valueToString += mStrValue[i] + "#";// \t alebo #
                            else
                                valueToString += mStrValue[i];
                        }
                        break;
                    case RegistryValueKind.QWord:
                        var intValue64 = (Int64)value;
                        attrType = "qword";
                        valueToString = intValue64.ToString(CultureInfo.InvariantCulture);
                        break;
                    case RegistryValueKind.String:
                        var stringValue = (string)value;
                        attrType = "string";
                        valueToString = stringValue;
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Unknown value type.");
                        attrType = "unknown";
                        break;
                }

                xW.WriteStartElement(TAG_VALUE);
                xW.WriteAttributeString(ATR_NAME, valueName);
                xW.WriteAttributeString(ATR_TYPE, attrType);
                xW.WriteString(valueToString);
                xW.WriteEndElement();
            }
            xW.WriteEndElement();

            var subKeyNames = section.GetSubKeyNames();
            foreach (var name in subKeyNames)
            {
                var subSection = section.OpenSubKey(name);
                SerializeSection(xW,subSection);
            }

        }
        /// <summary>
        /// Serialize section of the registry to xml file
        /// </summary>
        /// <param name="section">Section of the registry to be serialized</param>
        /// <param name="filename">Xml file with serialized registry section</param>
        public void Serialize(RegistryKey section, string filename)
        {
            Serialize(section, filename, null);
        }

        /*
        /// <summary>
        /// Recursively sweep through all subsection and store all values in xml object
        /// </summary>
        /// <param name="section">Section root to be serialized</param>
        private void SerializeSection(RegistryKey section)
        {
            XmlNode sectionNode = _xmlDoc.CreateNode("element", "section", "");
            XmlAttribute attr = _xmlDoc.CreateAttribute("name");
            attr.Value = section.Name;
            sectionNode.Attributes.Append(attr);

            _currentSection.AppendChild(sectionNode);
            _currentSection = sectionNode;

            // read all values for this section
            string[] valueNames = section.GetValueNames();
            foreach (string valueName in valueNames)
            {
                RegistryValueKind valueKind = section.GetValueKind(valueName);
                object value = section.GetValue(valueName);

                string attrType = string.Empty;
                string valueToString = string.Empty;

                switch (valueKind)
                {
                    case RegistryValueKind.Binary:
                        byte[] binaryValue = (byte[])value;
                        for (int i = 0; i < binaryValue.Length; i++)
                        {
                            if (i != (binaryValue.Length - 1))
                                valueToString += binaryValue[i].ToString() + "#";
                            else
                                valueToString += binaryValue[i].ToString();
                        }
                        attrType = "binary";

                        break;
                    case RegistryValueKind.DWord:
                        Int32 intValue32 = (Int32)value;
                        attrType = "dword";
                        valueToString = intValue32.ToString();
                        break;
                    case RegistryValueKind.ExpandString:
                        Console.WriteLine("Serialize ExpandString");
                        System.Diagnostics.Debug.WriteLine("ExpandString - Not implemented");
                        throw new NotImplementedException();
                        //attrType = "expandstring";
                        //break;
                    case RegistryValueKind.MultiString:
                        string[] mStrValue = (string[])value;
                        attrType = "multistring";

                        for (int i = 0; i < mStrValue.Length; i++)
                        {
                            if(i != (mStrValue.Length -1))
                                valueToString += mStrValue[i] + "#";// \t alebo #
                            else
                                valueToString += mStrValue[i];
                        }
                        break;
                    case RegistryValueKind.QWord:
                        Int64 intValue64 = (Int64)value;
                        attrType = "qword";
                        valueToString = intValue64.ToString();
                        break;
                    case RegistryValueKind.String:
                        string stringValue = (string)value;
                        attrType = "string";
                        valueToString = stringValue;
                        break;
                    case RegistryValueKind.Unknown:
                    default:
                        System.Diagnostics.Debug.WriteLine("Unknown value type.");
                        attrType = "unknown";
                        break;
                }

                XmlNode valueNode = _xmlDoc.CreateNode("element", "value", "");
                
                XmlAttribute typeAttr = _xmlDoc.CreateAttribute("type");
                typeAttr.Value = attrType;
                XmlAttribute nameAttr = _xmlDoc.CreateAttribute("name");
                nameAttr.Value = valueName;

                valueNode.Attributes.Append(typeAttr);
                valueNode.Attributes.Append(nameAttr);

                valueNode.InnerText = valueToString;

                sectionNode.AppendChild(valueNode);
            }

            string[] subKeyNames = section.GetSubKeyNames();
            foreach (string name in subKeyNames)
            {
                RegistryKey subSection = section.OpenSubKey(name);
                SerializeSection(subSection);
            }

            if (_currentSection.ParentNode != null)
                _currentSection = _currentSection.ParentNode;
        }
        */
    }
}
