using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Contal.IwQuick.Parsing;
using JetBrains.Annotations;

namespace Contal.IwQuick.Localization
{
    public struct TranslationItem
    {
        public string Name;
        public string Value;
        public string Comment;

        public override string ToString()
        {
            return EscapeString.Do(Value) ;
        }
    }

    public class Localization
    {
        private readonly string _language;
        public string Language
        { get { return _language; } }

        private bool _isMasterResource;
        public bool IsMasterResource
        { set { _isMasterResource = value; } get { return _isMasterResource; } }

        [NotNull]
        private readonly Dictionary<string, TranslationItem> _data = new Dictionary<string, TranslationItem>();
        public Dictionary<string, TranslationItem> Data
        { get { return _data; } }

// ReSharper disable once NotAccessedField.Local
        private LocalizationHelper _localizationAssistent;
        private bool _updateSymbolMap;

        private Dictionary<string, TranslationItem> _symbols;
        public Dictionary<string, TranslationItem> Symbols
        {
            get
            {
                if (null == _symbols || _updateSymbolMap)
                {
                    LoadSymbolMap();
                    _updateSymbolMap = false;
                }

                return _symbols;
            }
        }

        public Localization(
            [NotNull] LocalizationHelper localizationAssistent, 
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

            _updateSymbolMap = false;
        }

        /// <summary>
        /// Load localization items from resx file
        /// </summary>
        /// <param name="filePath">path of the file</param>
        public void LoadResxFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new DoesNotExistException(filePath, "Resource file \"" + filePath + "\" does not exist");
            }

            try
            {
                _data.Clear();


                ResXResourceReader resxReader = new ResXResourceReader(filePath);
                foreach (DictionaryEntry entry in resxReader)
                {
                    TranslationItem transItem = new TranslationItem
                    {
                        Name = entry.Key.ToString(),
                        Value = entry.Value.ToString()
                    };
                    _data.Add(transItem.Name, transItem);
                }
                resxReader.Close();

            }
            catch(Exception exception)
            {
                Sys.HandledExceptionAdapter.Examine(exception);
            }


        }

        /// <summary>
        /// Generate c# enumeration file
        /// </summary>
        /// <param name="outputFile">name and path of file</param>
        public void GenCSharpEnum(string outputFile)
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
            foreach (KeyValuePair<string, TranslationItem> item in _data)
            {
                aWriter.Write("\t\t");
                if (bFirst)
                {
                    bFirst = false;
                }
                else
                {
                    aWriter.Write(",");
                }
                aWriter.WriteLine(item.Value.Name + " = " + "\"" + item.Value.Name + "\"");
            }
            aWriter.WriteLine("\t}");
            aWriter.WriteLine("}");
            aWriter.Flush();
            aOutputFile.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localization"></param>
        /// <param name="isStrict"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void CopySymbolsFrom([NotNull] Localization localization, bool isStrict)
        {
            Validator.CheckForNull(localization,"localization");

            if (isStrict)
            {
                _data.Clear();
            }

            foreach (KeyValuePair<string, TranslationItem> pair in localization.Data)
            {
                if (_data.ContainsKey(pair.Key))
                {
                    continue;
                }
                TranslationItem item = new TranslationItem
                {
                    Comment = String.Empty,
                    Name = pair.Value.Name,
                    Value = String.Empty
                };
                _data.Add(pair.Key, item);
            }

            _updateSymbolMap = true;
        }

        private void LoadSymbolMap()
        {
            if (null == _symbols)
                _symbols = new Dictionary<string, TranslationItem>();

            foreach (KeyValuePair<string, TranslationItem> pair in _data)
            {
                if (Validator.IsNullString(pair.Value.Name))
                {
                    continue;
                }
                if (_symbols.ContainsKey(pair.Value.Name))
                {
                    continue;
                }
                _symbols[pair.Value.Name] = pair.Value;
            }
        }

        protected internal void Clear()
        {
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
        /// <param name="stringId"></param>
        /// <param name="text"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public TranslationItem InsertTranslationItem(string stringId, string text, string comment)
        {
            if (null == _symbols)
            {
                throw new InvalidOperationException("Insert operation is not permitted outside \"Localization Assistent\" application");
            }
            Validator.CheckNullString(stringId);
            const string strPattern = @"^[a-z_A-Z]?[0-9a-z_A-Z]*$";
            Regex aRegex = new Regex(strPattern);
            if (!aRegex.IsMatch(stringId))
            {
                throw new ArgumentException("Specified String ID does not follow the pattern \"" + strPattern + "\"");
            }
            if (_symbols.ContainsKey(stringId))
            {
                throw new AlreadyExistsException("Specified StringID \"" + stringId + "\"was already used");
            }
            TranslationItem item = new TranslationItem();
            item.Name = stringId;
            item.Value = (text ?? string.Empty);
            item.Comment = (comment ?? string.Empty);
            _data.Add(item.Name, item);
            Symbols.Add(item.Name, item);
            return item;
        }

        public TranslationItem UpdateTranslationItem(string stringId, string text, string comment)
        {
            if (null == _symbols)
            {
                throw new InvalidOperationException("Update operation is not permitted outside \"Localization Assistent\" application");
            }
            Validator.CheckNullString(stringId);
            TranslationItem item;
            if (!_symbols.TryGetValue(stringId, out item))
            {
                throw new DoesNotExistException(stringId, "Specified StringID \"" + stringId + "\" does not exist in the database");
            }
            item.Value = (text ?? string.Empty);
            item.Comment = (comment ?? string.Empty);
            Symbols[item.Name] = item;
            _data[item.Name] = item;
            return item;
        }

        public void DeleteTranslationItem(string stringId)
        {
            if (null == _symbols)
            {
                throw new InvalidOperationException("Delete operation is not permitted outside \"Localization Assistent\" application");
            }
            Validator.CheckNullString(stringId);
            TranslationItem item;
            if (!_symbols.TryGetValue(stringId, out item))
            {
                throw new DoesNotExistException(stringId, "Specified StringID \"" + stringId + "\" does not exist in the database");
            }
            Symbols.Remove(stringId);
            _data.Remove(item.Name);
        }

        public override string ToString()
        {
            string report = _language;
            if (_isMasterResource)
            {
                report += " (master)";
            }
            return report;
        }
    }
}
