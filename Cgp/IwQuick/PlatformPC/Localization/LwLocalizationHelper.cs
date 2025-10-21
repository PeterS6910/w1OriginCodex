using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Contal.IwQuick.Localization
{
    public class LwLocalizationHelper : IEnumerable<LwLocalization>
    {
        private List<string> _generateCSharpPaths;
        /// <summary>
        /// 
        /// </summary>
        public IList<string> CSharpPaths
        {
            get { return _generateCSharpPaths; }
            [NotNull] set
            {
                Validator.CheckForNull(value,"value");

                _generateCSharpPaths = new List<string>(4);

                foreach (string strPath in value)
                {
                    if (Directory.Exists(strPath))
                        _generateCSharpPaths.Add(strPath);
                }
            }
        }

        private List<string> _generateJSONPaths;
        /// <summary>
        /// 
        /// </summary>
        public IList<string> JSONPaths
        {
            get { return _generateJSONPaths; }
            [NotNull] set
            {
                Validator.CheckForNull(value,"value");

                _generateJSONPaths = new List<string>(4);

                foreach (string strPath in value)
                {
                    if (Directory.Exists(strPath))
                        _generateJSONPaths.Add(strPath);
                }
            }
        }

        private LwLocalization _masterLocalization;
        /// <summary>
        /// 
        /// </summary>
        public LwLocalization MasterLocalization
        {
            set { _masterLocalization = value; }
            get { return _masterLocalization; }
        }

        private LwLocalization _activeLocalization;
        protected internal Dictionary<string, LwLocalization> _localizations = new Dictionary<string, LwLocalization>(4);

        public LwLocalizationHelper(string masterResourceFile)
        {
            if (Validator.IsNotNullString(masterResourceFile))
            {
                // let the exceptions flow up
                LoadLocalization(masterResourceFile);

                if (null == _masterLocalization)
                    throw new ArgumentException("Invalid master resource file \"" + masterResourceFile + "\"");
                
                _activeLocalization = _masterLocalization;
            }
        }

        public LwLocalization LoadLocalization(string resourceFile)
        {
            LwLocalization aLocalization = new LwLocalization(this, null, false);
            aLocalization.Load(resourceFile);
            if (aLocalization.IsMasterResource && null == _masterLocalization)
            {
                _masterLocalization = aLocalization;
            }
            _localizations.Add(aLocalization.Language, aLocalization);
            return aLocalization;
        }

        public LwLocalization CreateLocalization(string language, bool isMaster)
        {
            Validator.CheckNullString(language);

            if (_localizations.ContainsKey(language))
                throw new AlreadyExistsException("Language \"" + language + "\" already exists");

            LwLocalization aLocalization = new LwLocalization(this, language, isMaster);

            if (isMaster && null == _masterLocalization)
                _masterLocalization = aLocalization;

            _localizations.Add(language, aLocalization);

            return aLocalization;
        }

        public void ApplyLocalization(string language)
        {
            Validator.CheckNullString(language);

            LwLocalization aLocalization;
            if (!_localizations.TryGetValue(language, out aLocalization))
                throw new DoesNotExistException(language, "Language \"" + language + "\" is not supported");
            _activeLocalization = aLocalization;
        }

        public void RevertMasterLocalization()
        {
            _activeLocalization = _masterLocalization;
        }

        private const string _generalStringError = "ErrNoTrans";
        public string GetMasterString(int id)
        {
            if (null == _masterLocalization)
                return "ErrNoLoc";

            if (null == _activeLocalization)
                _activeLocalization = _masterLocalization;

            LwTranslationItem aItem;
            if (_masterLocalization.Data.TryGetValue(id, out aItem))
            {
                if (Validator.IsNotNullString(aItem._text))
                    return aItem._text;
                return _generalStringError;
            }
            return _generalStringError;
        }


        public string GetString(int id)
        {
            if (null == _activeLocalization)
                return GetMasterString(id);


            LwTranslationItem aItem;
            if (_activeLocalization.Data.TryGetValue(id, out aItem))
            {
                if (Validator.IsNotNullString(aItem._text))
                    return aItem._text;
                return GetMasterString(id);
            }
            return GetMasterString(id);
        }

        public string this[int id]
        {
            get
            {
                return GetString(id);
            }
        }

        public void SaveLocalization(string language, string resourceFile)
        {
            Validator.CheckNullString(language);
            Validator.CheckNullString(resourceFile);

            LwLocalization aLocalization;
            if (!_localizations.TryGetValue(language, out aLocalization))
                throw new DoesNotExistException(language, "Language \"" + language + "\" is not supported");
            aLocalization.Save(resourceFile);

        }

        public LwLocalization GetLocalization(string language)
        {
            Validator.CheckNullString(language);

            LwLocalization aLocalization;
            if (!_localizations.TryGetValue(language, out aLocalization))
                throw new DoesNotExistException(language, "Language \"" + language + "\" is not supported");

            return aLocalization;
        }

        #region IEnumerable<TLocalization> Members

        public IEnumerator<LwLocalization> GetEnumerator()
        {
            return _localizations.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _localizations.Values.GetEnumerator();
        }

        #endregion

        public void LoadPackage(string packageFile)
        {
            if (!File.Exists(packageFile))
            {
                throw new DoesNotExistException(packageFile, "Package specified by \"" + packageFile + "\" does not exist");
            }
            // if packageFile exists, its directory exists too
            string dir = Path.GetDirectoryName(packageFile);
            Xml.QuickXml aXml = new Xml.QuickXml();
            if (!aXml.Load(packageFile, true))
                throw new ArgumentException("XML localization info file \"" + packageFile + "\" is invalid");
            string[] arPaths = aXml.GetTexts("//lang");
            if (null == arPaths ||
                0 == arPaths.Length)
                throw new ArgumentException("XML localization info file \"" + packageFile + "\" does not contain localization information");

            LinkedList<string> aFailed = new LinkedList<string>();
            foreach (string strPath in arPaths)
            {
                string strAbsPath = dir + "\\" + "localization_" + strPath + ".ldx";
                if (!File.Exists(strAbsPath))
                {
                    aFailed.AddLast("Referenced localization file \"" + strAbsPath + "\" does not exist");
                    continue;
                }
                try
                {
                    LoadLocalization(strAbsPath);
                }
                catch (Exception aError)
                {
                    aFailed.AddLast(aError.Message);
                }
            }
            IList<string> aPaths = new List<string>(4);
            aXml.GetTexts("//default-paths/json", ref aPaths);
            JSONPaths = aPaths;
            aPaths.Clear();
            aXml.GetTexts("//default-paths/csharp", ref aPaths);
            CSharpPaths = aPaths;
            if (aFailed.Count > 0)
            {
                string strReport = String.Empty;


                int j = 0;
                foreach (string strReportPart in aFailed)
                {
                    j++;
                    strReport += j + ". " + strReportPart + "\n\n";
                }

                throw new InvalidDataException(strReport);
            }
        }

        public void GenerateJSONSymbols(string outputFile)
        {
            FileStream aOutputFile = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter aWriter = new StreamWriter(aOutputFile, Encoding.UTF8);

            LwLocalization aFirstLocalization = GetFirstLocalization();
            if (null == aFirstLocalization)
                throw new DoesNotExistException(null, "No localization loaded");

            aWriter.WriteLine("var localization_symbols = true;");
            aWriter.WriteLine();

            aFirstLocalization.GenerateJSONSymbols(aWriter);

            aWriter.WriteLine();
            aWriter.WriteLine("if (undefined != window['LA']) {");

            foreach (LwLocalization aLocalization in _localizations.Values)
            {
                aWriter.WriteLine("\tLA.LoadLocalization(\"" + aLocalization.Language + "\");");
            }

            aWriter.WriteLine("\tLA.SymbolsLoaded();");
            aWriter.WriteLine("}");

            aWriter.Flush();
            aOutputFile.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputDirectories"></param>
        public void GenerateAllJSONData(IList<string> outputDirectories)
        {
            Validator.CheckForNull(outputDirectories,"outputDirectories");

            if (null == _masterLocalization)
            {
                throw new InvalidDataException("No master localization present");
            }

            foreach (string strDirPath in outputDirectories)
            {

                if (!Directory.Exists(strDirPath))
                {
                    throw new DoesNotExistException(strDirPath, "Output directory \"" + strDirPath + " does not exist");
                }

                LinkedList<string> aFailed = new LinkedList<string>();

                string strFilePath = strDirPath + @"\localization_symbols.js";
                try
                {
                    GenerateJSONSymbols(strFilePath);
                }
                catch (Exception aError)
                {
                    aFailed.AddLast(aError.Message);
                }

                foreach (LwLocalization aLocalization in _localizations.Values)
                {
                    strFilePath = strDirPath + @"\localization_" + aLocalization.Language + ".js";
                    try
                    {
                        aLocalization.GenerateJSONData(strFilePath);
                    }
                    catch (Exception aError)
                    {
                        aFailed.AddLast(aError.Message);
                    }
                }

                if (aFailed.Count > 0)
                {
                    string strReport = String.Empty;


                    int j = 0;
                    foreach (string strReportPart in aFailed)
                    {
                        j++;
                        strReport += j + ". " + strReportPart + "\n\n";
                    }

                    throw new InvalidDataException(strReport);
                }
            }

        }

        public void Clear()
        {
            foreach (LwLocalization aLocalization in _localizations.Values)
                aLocalization.Clear();

            _localizations.Clear();
            _activeLocalization = null;
            _masterLocalization = null;
        }

        public int Count
        {
            get { return _localizations.Count; }
        }

        public void SavePackage(string localizationInfoPath)
        {
            Validator.CheckNullString(localizationInfoPath);

            string strOutputDirectory = Path.GetDirectoryName(localizationInfoPath);
            if (!Directory.Exists(strOutputDirectory))
            {
                throw new DoesNotExistException(strOutputDirectory, "Directory \"" + strOutputDirectory + "\" for current package does not exist");
            }

            Xml.QuickXml aXml = new Xml.QuickXml();

            if (!aXml.Load(localizationInfoPath, false))
                throw new ArgumentException("XML localization info file \"" + localizationInfoPath + "\" is invalid");

            List<string> aLanguages = new List<string>();
            foreach (LwLocalization aLocalization in _localizations.Values)
            {
                string strXmlFile = strOutputDirectory + "\\localization_" + aLocalization.Language + ".ldx";
                try
                {
                    aLocalization.Save(strXmlFile);

                    aLanguages.Add(aLocalization.Language);
                }
                catch (Exception aError)
                {
                    Debug.Assert(false, aError.Message);
                }
            }

            aXml.SetOrAddTexts(aLanguages, "/root/lang");

            try
            {
                if (null != JSONPaths)
                    aXml.SetOrAddTexts(_generateJSONPaths, "/root/default-paths/json");

                if (null != _generateCSharpPaths)
                    aXml.SetOrAddTexts(_generateCSharpPaths, "/root/default-paths/csharp");
            }
            catch (Exception aError)
            {
                Debug.Assert(false, aError.Message);
            }

            aXml.Save();

        }

        public LwLocalization GetFirstLocalization()
        {
            if (null != _masterLocalization)
                return _masterLocalization;

            return _localizations.Values.FirstOrDefault();
        }

        public void SynchronizeSymbolsFromMaster()
        {
            if (null == _masterLocalization)
                throw new InvalidOperationException("There is no master localization present");

            foreach (LwLocalization aLocalization in _localizations.Values)
            {
                if (!aLocalization.IsMasterResource)
                    aLocalization.CopySymbolsFrom(_masterLocalization, false);
            }
        }
    }
}
