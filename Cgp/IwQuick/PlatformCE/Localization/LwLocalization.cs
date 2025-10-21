using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

using System.Text.RegularExpressions;

using System.Xml.Serialization;
using System.Diagnostics;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick.Localization
{
    [XmlRoot("TranslationItem")]
    public struct LwTranslationItem//: IXmlSerializable
    {
        [XmlAttribute("Id")]
        public int _id;
        [XmlElement("StringId")]
        public string m_strId;
        [XmlElement("Text")]
        public string _text;
        [XmlElement("Comment")]
        public string _comment;

        public override int GetHashCode()
        {
            if (null == m_strId)
                return base.GetHashCode();
            else
                return m_strId.GetHashCode();
        }

        public override string ToString()
        {
            return _text ?? String.Empty;
        }

    }

    public class LwLocalization
    {
        private string _language;
        public string Language
        {
            get
            {
                return _language;
            }
        }

        private bool _isMasterResource;
        public bool IsMasterResource
        {
            get { return _isMasterResource; }
        }

        private XmlSerializableDictionary<int, LwTranslationItem> _data;
        public XmlSerializableDictionary<int, LwTranslationItem> Data
        {
            get
            {
                return _data;
            }
        }

        private bool _updateSymbolMap;

        private Dictionary<string,LwTranslationItem> _symbols = null;
        public Dictionary<string,LwTranslationItem> Symbols {
            get {
                if (null == _symbols || _updateSymbolMap)
                {
                    LoadSymbolMap();
                    _updateSymbolMap = false;
                }

                return _symbols;
            }
        }

        private readonly LwLocalizationHelper _localizationAssistent = null;
        


        
        public LwLocalization(
            [NotNull] LwLocalizationHelper localizationAssistent,
            string language,
            bool isMasterResource)
        {
            Validator.CheckForNull(localizationAssistent,"localizationAssistent");

            _localizationAssistent = localizationAssistent;

            if (Validator.IsNotNullString(language))
            {
                _language = language;

                _isMasterResource = isMasterResource;
            }

            _data = new XmlSerializableDictionary<int, LwTranslationItem>(64);
        }

        public LwLocalization([NotNull] LwLocalizationHelper localizationAssistent,string resourceFile)
        {
            Validator.CheckForNull(localizationAssistent,"localizationAssistent");

            _localizationAssistent = localizationAssistent;

            Load(resourceFile);
        }


        private void LoadSymbolMap()
        {
            if (null == _data)
                return;

            if (null == _symbols)
                _symbols = new Dictionary<string, LwTranslationItem>(200);

            foreach (KeyValuePair<int, LwTranslationItem> aPair in _data)
            {
                if (Validator.IsNullString(aPair.Value.m_strId))
                    continue;

                if (_symbols.ContainsKey(aPair.Value.m_strId))
                    continue;

                _symbols[aPair.Value.m_strId] = aPair.Value;
            }
        }

        private int FindFreeId()
        {
            for (int i = 0; i < int.MaxValue; i++)
                if (!_data.ContainsKey(i))
                    return i;

            Debug.Assert(false);
            return -1;
        }

        public LwTranslationItem InsertTranslationItem(string i_strStringId,string i_strText,string comment)
        {
            if (null == _symbols)
                throw new InvalidOperationException("Insert operation is not permitted outside \"Localization Assistent\" application");

            Validator.CheckNullString(i_strStringId);

            string strPattern = @"^[a-z_A-Z]?[0-9a-z_A-Z]*$";
            Regex aRegex = new Regex(strPattern);
            if (!aRegex.IsMatch(i_strStringId))
                throw new ArgumentException("Specified String ID does not follow the pattern \"" + strPattern + "\"");

            if (_symbols.ContainsKey(i_strStringId))
                throw new AlreadyExistsException("Specified StringID \""+i_strStringId+"\"was already used");

            

            LwTranslationItem aItem = new LwTranslationItem();
            aItem.m_strId = i_strStringId;
            aItem._text = (null != i_strText ? i_strText : "") ;
            aItem._comment = (null != comment ? comment : "");

            aItem._id = FindFreeId();
            if (-1 == aItem._id)
                throw new InvalidOperationException("The Database is full");

            _data.Add(aItem._id, aItem);
            Symbols.Add(i_strStringId, aItem);

            return aItem;
        }

        public LwTranslationItem UpdateTranslationItem(string i_strStringId, string i_strText, string comment)
        {
            if (null == _symbols)
                throw new InvalidOperationException("Update operation is not permitted outside \"Localization Assistent\" application");

            Validator.CheckNullString(i_strStringId);

            LwTranslationItem aItem;
            if (!_symbols.TryGetValue(i_strStringId,out aItem))
                throw new DoesNotExistException(i_strStringId,"Specified StringID \"" + i_strStringId + "\" does not exist in the database");

            aItem._text = (null != i_strText ? i_strText : "");
            aItem._comment = (null != comment ? comment : "");

            Symbols[i_strStringId] = aItem;
            _data[aItem._id] = aItem;

            return aItem;
        }

        public void DeleteTranslationItem(string i_strStringId)
        {
            if (null == _symbols)
                throw new InvalidOperationException("Delete operation is not permitted outside \"Localization Assistent\" application");

            Validator.CheckNullString(i_strStringId);

            LwTranslationItem aItem;
            if (!_symbols.TryGetValue(i_strStringId, out aItem))
                throw new DoesNotExistException(i_strStringId,"Specified StringID \"" + i_strStringId + "\" does not exist in the database");

            Symbols.Remove(i_strStringId);
            _data.Remove(aItem._id);
        }

        public override string ToString()
        {
            string strReport = _language;
            if (_isMasterResource)
                strReport += " (master)";

            return strReport;
        }

        public void Load(string i_strResourceFile)
        {
            if (!File.Exists(i_strResourceFile))
                throw new DoesNotExistException(i_strResourceFile,"Resource file \"" + i_strResourceFile + "\" does not exist");


            FileStream aReaderStream = null;

            try
            {
                aReaderStream = new FileStream(i_strResourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception aError)
            {
                throw new ArgumentException("Failed to load resource file \"" + i_strResourceFile + "\"", aError);
            }

            LwLocalizationInfo aLocalization;
            try
            {
                XmlSerializer aSerializer = new XmlSerializer(typeof(LwLocalizationInfo));

                aLocalization = (LwLocalizationInfo)aSerializer.Deserialize(aReaderStream);

                // means already loaded
                if (Validator.IsNotNullString(aLocalization._language) &&
                    _localizationAssistent._localizations.ContainsKey(aLocalization._language))
                    throw new InvalidOperationException("Localization \"" + aLocalization._language + "\" already loaded");



            }
            catch (InvalidOperationException)
            {
                aReaderStream.Close();
                throw;
            }
            catch (Exception aError)
            {
                aReaderStream.Close();
                throw new ArgumentException("Unable to load data from resource file \"" + i_strResourceFile + "\"", aError);
            }

            if (Validator.IsNull(aLocalization._language))
                throw new ArgumentException("Invalid language specification in resource file \"" + i_strResourceFile + "\"");

            _updateSymbolMap = true;

            _isMasterResource = aLocalization._masterResource;
            _language = aLocalization._language;
            if (null != _data)
                try { _data.Clear(); }
                catch { }

            _data = aLocalization._data;

        }

        [XmlRoot("Localization")]
        public struct LwLocalizationInfo
        {
            [XmlAttribute("Lang")]
            public string _language;

            [XmlAttribute("IsMaster")]
            public bool _masterResource;

            [XmlElement("Data")]
            public Contal.IwQuick.Data.XmlSerializableDictionary<int, LwTranslationItem> _data;
        }

        public void Save(string i_strResourceFile)
        {
            Validator.CheckNullString(i_strResourceFile);

            FileStream aWriterStream = new FileStream(i_strResourceFile, FileMode.Create, FileAccess.Write, FileShare.Read);

            XmlSerializer aSerializer = new XmlSerializer(typeof(LwLocalizationInfo));

            LwLocalizationInfo aL = new LwLocalizationInfo();
            aL._language = _language;
            aL._masterResource = _isMasterResource;
            aL._data = _data;

            try
            {
                aSerializer.Serialize(aWriterStream, aL);
            }
            catch (Exception aError)
            {
                throw new InvalidOperationException("Failed to serialize localization \"" + this.ToString() + "\"", aError);
            }
            finally
            {
                try { aWriterStream.Close(); }
                catch { }
            }

        }

        public void GenerateCSharpEnumeration(string outFile)
        {
            FileStream aOutputFile = new FileStream(outFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter aWriter = new StreamWriter(aOutputFile, Encoding.UTF8);

            aWriter.WriteLine("using System;");
            aWriter.WriteLine("using System.Text;");
            aWriter.WriteLine();
            aWriter.WriteLine("namespace _Localizations");
            aWriter.WriteLine("{");
            aWriter.WriteLine("\tpublic enum TL");
            aWriter.WriteLine("\t{");
            


            bool bFirst = true;
            foreach (KeyValuePair<int, LwTranslationItem> aPair in _data)
            {
                aWriter.Write("\t\t");
                if (bFirst)
                    bFirst = false;
                else
                    aWriter.Write(",");
                
                aWriter.WriteLine(aPair.Value.m_strId + " = " + aPair.Key);
            }

            aWriter.WriteLine("\t}");
            aWriter.WriteLine("}");

            aWriter.Flush();
            aOutputFile.Close();

        }

        protected internal void GenerateJSONSymbols([NotNull] StreamWriter streamWriter)
        {
            Validator.CheckForNull(streamWriter,"streamWriter");

            streamWriter.WriteLine("function tTL() {");
            foreach (KeyValuePair<int, LwTranslationItem> aPair in _data)
            {
                streamWriter.WriteLine("\tthis." + aPair.Value.m_strId + " = " + aPair.Key + ";");
            }
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("var TL = new tTL();");
            streamWriter.WriteLine();
            

            /*
            if (Info._masterResource)
                aWriter.WriteLine("LA.ApplyMasterLocalization(\""+Info._language+"\");");
            else
                aWriter.WriteLine("LA.ApplyLocalization(\"" + Info._language + "\");");
            */
        }

        public void GenerateJSONData(string i_strOutputFile)
        {
            
            FileStream aOutputFile = new FileStream(i_strOutputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter aWriter = new StreamWriter(aOutputFile, Encoding.UTF8);

            aWriter.WriteLine("var localization_"+_language+" = ");
            aWriter.WriteLine("{");
            bool bFirst = true;
            foreach (KeyValuePair<int, LwTranslationItem> aPair in _data)
            {
                aWriter.Write("\t");

                if (bFirst)
                    bFirst = false;
                else
                    aWriter.Write(",");


                aWriter.WriteLine(aPair.Key+" : \""+ 
                    Contal.IwQuick.Parsing.EscapeString.Do(aPair.Value._text)+"\"");
            }

            aWriter.WriteLine("}");

            
            aWriter.WriteLine();


            if (_isMasterResource)
                aWriter.WriteLine("LA.OnMasterLocalizationLoad(\""+_language+"\");");
            else
                aWriter.WriteLine("LA.OnLocalizationLoad(\"" + _language + "\");");

            aWriter.Flush();
            aOutputFile.Close();
        }

        protected internal void Clear()
        {
            if (null != _data)
                _data.Clear();

            if (null != _symbols)
            {
                _symbols.Clear();
                _symbols = null;
            }
        }

        public void CopySymbolsFrom(
            [NotNull] LwLocalization localization,
            bool isStrict)
        {
            Validator.CheckForNull(localization,"localization");

            if (isStrict)
                _data.Clear();

            LwTranslationItem aItem;
            foreach (KeyValuePair<int, LwTranslationItem> aPair in localization._data)
            {
                if (_data.ContainsKey(aPair.Key))
                    continue;

                aItem = new LwTranslationItem();
                aItem._id = aPair.Key;
                aItem._comment = String.Empty;
                aItem.m_strId = aPair.Value.m_strId;
                aItem._text = String.Empty;
                _data.Add(aPair.Key, aItem);
            }

            _updateSymbolMap = true;
        }

        
    }
}
