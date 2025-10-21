using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

using System.Text.RegularExpressions;

using System.Xml.Serialization;
using System.Diagnostics;

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
        public string _stringId;
        [XmlElement("Text")]
        public string _text;
        [XmlElement("Comment")]
        public string _comment;

        public override int GetHashCode()
        {
// ReSharper disable NonReadonlyFieldInGetHashCode
            if (null == _stringId)
                return base.GetHashCode();

            return _stringId.GetHashCode();
// ReSharper restore NonReadonlyFieldInGetHashCode
        }

        public override string ToString()
        {
            return _text ?? String.Empty;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class LwLocalization
    {
        private string _language;
        /// <summary>
        /// 
        /// </summary>
        public string Language
        {
            get
            {
                return _language;
            }
        }

        private bool _isMasterResource;
        /// <summary>
        /// 
        /// </summary>
        public bool IsMasterResource
        {
            set { _isMasterResource = value; }
            get { return _isMasterResource; }
        }

        private XmlSerializableDictionary<int, LwTranslationItem> _data;
        /// <summary>
        /// 
        /// </summary>
        public XmlSerializableDictionary<int, LwTranslationItem> Data
        {
            get
            {
                return _data;
            }
        }

        private bool _updateSymbolMap;

        private Dictionary<string,LwTranslationItem> _symbols;
        private readonly object _syncSymbols = new object();
        
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string,LwTranslationItem> Symbols 
        {
            get
            {
                CheckSymbolMapForReload();
                return _symbols;
            }
        }

        private readonly LwLocalizationHelper _localizationAssistent;
        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="localizationAssistent"></param>
        /// <param name="language"></param>
        /// <param name="isMasterResource"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localizationAssistent"></param>
        /// <param name="resourceFile"></param>
        public LwLocalization(
            [NotNull] LwLocalizationHelper localizationAssistent,
            [NotNull] string resourceFile)
        {
            Validator.CheckForNull(localizationAssistent,"localizationAssistent");

            _localizationAssistent = localizationAssistent;

            Load(resourceFile);
        }


        private void CheckSymbolMapForReload()
        {
            lock(_syncSymbols)
                if (null == _symbols || _updateSymbolMap)
                {
                    if (null == _symbols)
                        _symbols = new Dictionary<string, LwTranslationItem>(200);

                    if (null == _data)
                        return;

                    foreach (KeyValuePair<int, LwTranslationItem> aPair in _data)
                    {
                        if (Validator.IsNullString(aPair.Value._stringId))
                            continue;

                        if (_symbols.ContainsKey(aPair.Value._stringId))
                            continue;

                        _symbols[aPair.Value._stringId] = aPair.Value;
                    }

                    _updateSymbolMap = false;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringId"></param>
        /// <param name="text"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public LwTranslationItem InsertTranslationItem(
            string stringId,
            string text,
            string comment)
        {
            if (null == _symbols)
                throw new InvalidOperationException("Insert operation is not permitted outside \"Localization Assistent\" application");

            Validator.CheckNullString(stringId);

            const string strPattern = @"^[a-z_A-Z]?[0-9a-z_A-Z]*$";
            Regex aRegex = new Regex(strPattern);
            if (!aRegex.IsMatch(stringId))
                throw new ArgumentException("Specified String ID does not follow the pattern \"" + strPattern + "\"");

            if (_symbols.ContainsKey(stringId))
                throw new AlreadyExistsException("Specified StringID \""+stringId+"\"was already used");

            

            LwTranslationItem aItem = new LwTranslationItem
            {
                _stringId = stringId,
                _text = (text ?? ""),
                _comment = (comment ?? ""),
                _id = FindFreeId()
            };

            if (-1 == aItem._id)
                throw new InvalidOperationException("The Database is full");

            _data.Add(aItem._id, aItem);
            Symbols.Add(stringId, aItem);

            return aItem;
        }

        public LwTranslationItem UpdateTranslationItem(string stringId, string text, string comment)
        {
            if (null == _symbols)
                throw new InvalidOperationException("Update operation is not permitted outside \"Localization Assistent\" application");

            Validator.CheckNullString(stringId);

            LwTranslationItem aItem;
            if (!_symbols.TryGetValue(stringId,out aItem))
                throw new DoesNotExistException(stringId,"Specified StringID \"" + stringId + "\" does not exist in the database");

            aItem._text = (text ?? "");
            aItem._comment = (comment ?? "");

            Symbols[stringId] = aItem;
            _data[aItem._id] = aItem;

            return aItem;
        }

        public void DeleteTranslationItem(string stringId)
        {
            if (null == _symbols)
                throw new InvalidOperationException("Delete operation is not permitted outside \"Localization Assistent\" application");

            Validator.CheckNullString(stringId);

            LwTranslationItem aItem;
            if (!_symbols.TryGetValue(stringId, out aItem))
                throw new DoesNotExistException(stringId,"Specified StringID \"" + stringId + "\" does not exist in the database");

            Symbols.Remove(stringId);
            _data.Remove(aItem._id);
        }

        public override string ToString()
        {
            string strReport = _language;
            if (_isMasterResource)
                strReport += " (master)";

            return strReport;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceFile"></param>
        public void Load([NotNull] string resourceFile)
        {
            if (!File.Exists(resourceFile))
                throw new DoesNotExistException(resourceFile,"Resource file \"" + resourceFile + "\" does not exist");


            FileStream aReaderStream;

            try
            {
                aReaderStream = new FileStream(resourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception aError)
            {
                throw new ArgumentException("Failed to load resource file \"" + resourceFile + "\"", aError);
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
                throw new ArgumentException("Unable to load data from resource file \"" + resourceFile + "\"", aError);
            }

            if (ReferenceEquals(aLocalization._language,null))
                throw new ArgumentException("Invalid language specification in resource file \"" + resourceFile + "\"");

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
            public XmlSerializableDictionary<int, LwTranslationItem> _data;
        }

        public void Save(string resourceFile)
        {
            Validator.CheckNullString(resourceFile);

            FileStream aWriterStream = new FileStream(resourceFile, FileMode.Create, FileAccess.Write, FileShare.Read);

            XmlSerializer aSerializer = new XmlSerializer(typeof(LwLocalizationInfo));

            LwLocalizationInfo localizationInfo = new LwLocalizationInfo
            {
                _language = _language,
                _masterResource = _isMasterResource,
                _data = _data
            };

            try
            {
                aSerializer.Serialize(aWriterStream, localizationInfo);
            }
            catch (Exception aError)
            {
                throw new InvalidOperationException("Failed to serialize localization \"" + ToString() + "\"", aError);
            }
            finally
            {
                try { aWriterStream.Close(); }
                catch { }
            }

        }

        public void GenerateCSharpEnumeration(string outputFile)
        {
            FileStream aOutputFile = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
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
                
                aWriter.WriteLine(aPair.Value._stringId + " = " + aPair.Key);
            }

            aWriter.WriteLine("\t}");
            aWriter.WriteLine("}");

            aWriter.Flush();
            aOutputFile.Close();

        }

        protected internal void GenerateJSONSymbols(StreamWriter writter)
        {
            Validator.CheckForNull(writter,"writter");

            writter.WriteLine("function tTL() {");
            foreach (KeyValuePair<int, LwTranslationItem> aPair in _data)
            {
                writter.WriteLine("\tthis." + aPair.Value._stringId + " = " + aPair.Key + ";");
            }
            writter.WriteLine("}");
            writter.WriteLine();
            writter.WriteLine("var TL = new tTL();");
            writter.WriteLine();
            

            /*
            if (Info._masterResource)
                aWriter.WriteLine("LA.ApplyMasterLocalization(\""+Info._language+"\");");
            else
                aWriter.WriteLine("LA.ApplyLocalization(\"" + Info._language + "\");");
            */
        }

        public void GenerateJSONData(string outputFile)
        {
            
            FileStream aOutputFile = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
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
                    Parsing.EscapeString.Do(aPair.Value._text)+"\"");
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localization"></param>
        /// <param name="isStrict">if true, the data is cleared before copied, therefore non-equal records would not remain</param>
        public void CopySymbolsFrom([NotNull] LwLocalization localization,bool isStrict)
        {
            Validator.CheckForNull(localization,"localization");

            if (isStrict)
                _data.Clear();

            foreach (KeyValuePair<int, LwTranslationItem> aPair in localization._data)
            {
                if (_data.ContainsKey(aPair.Key))
                    continue;

                LwTranslationItem item = new LwTranslationItem
                {
                    _id = aPair.Key,
                    _comment = String.Empty,
                    _stringId = aPair.Value._stringId,
                    _text = String.Empty
                };
                _data.Add(aPair.Key, item);
            }

            _updateSymbolMap = true;
        }

        
    }
}
