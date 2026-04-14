using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

using System.Diagnostics;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.IwQuick.Localization
{
    public class LwLocalizationHelper : IEnumerable<LwLocalization>
    {
        private List<string> _generateCSharpPaths = null;
        public IList<string> CSharpPaths
        {
            get { return _generateCSharpPaths; }
            set
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

        private List<string> _generateJSONPaths = null;
        public IList<string> JSONPaths
        {
            get { return _generateJSONPaths; }
            set
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

        private LwLocalization _masterLocalization = null;
        public LwLocalization MasterLocalization
        {
            get { return _masterLocalization; }
        }

        private LwLocalization _activeLocalization = null;
        protected internal Dictionary<string, LwLocalization> _localizations = new Dictionary<string, LwLocalization>(4);

        public LwLocalizationHelper(string i_strMasterResourceFile)
        {
            if (Validator.IsNotNullString(i_strMasterResourceFile)) {
                // let the exceptions flow up
                LoadLocalization(i_strMasterResourceFile);

                if (null == _masterLocalization)
                    throw new ArgumentException("Invalid master resource file \"" + i_strMasterResourceFile + "\"");
                else
                    _activeLocalization = _masterLocalization;
            }
        }

        public LwLocalization LoadLocalization(string i_strResourceFile)
        {
            LwLocalization aLocalization = new LwLocalization(this,null,false);
            aLocalization.Load(i_strResourceFile);

            if (aLocalization.IsMasterResource && null == _masterLocalization)
                _masterLocalization = aLocalization;

            _localizations.Add(aLocalization.Language, aLocalization);

            return aLocalization;
            
        }

        public LwLocalization CreateLocalization(string language,bool i_bMaster)
        {
            Validator.CheckNullString(language);

            if (_localizations.ContainsKey(language))
                throw new AlreadyExistsException("Language \""+language+"\" already exists");

            LwLocalization aLocalization = new LwLocalization(this,language,i_bMaster);

            if (i_bMaster && null == _masterLocalization)
                _masterLocalization = aLocalization;

            _localizations.Add(language, aLocalization);

            return aLocalization;
        }

        public void ApplyLocalization(string language)
        {
            Validator.CheckNullString(language);

            LwLocalization aLocalization;
            if (!_localizations.TryGetValue(language, out aLocalization))
                throw new DoesNotExistException(language,"Language \"" + language + "\" is not supported");
            else
                _activeLocalization = aLocalization;
        }

        public void RevertMasterLocalization()
        {
            _activeLocalization = _masterLocalization;
        }

        private const string _generalStringError = "ERROR_NO_TRANSLATION";
        public string GetMasterString(int i_iId)
        {
            if (null == _masterLocalization)
                return "ERROR_NO_LOCALIZATION";

            if (null == _activeLocalization)
                _activeLocalization = _masterLocalization;

            LwTranslationItem aItem;
            if (_masterLocalization.Data.TryGetValue(i_iId, out aItem))
            {
                if (Validator.IsNotNullString(aItem._text))
                    return aItem._text;
                else
                    return _generalStringError;
            }
            else
                return _generalStringError;
        }

        
        public string GetString(int i_iId)
        {
            if (null == _activeLocalization)
                return GetMasterString(i_iId);            
            

            LwTranslationItem aItem;
            if (_activeLocalization.Data.TryGetValue(i_iId, out aItem))
            {
                if (Validator.IsNotNullString(aItem._text))
                    return aItem._text;
                else
                    return GetMasterString(i_iId);
            }
            else {
                return GetMasterString(i_iId);                   
            }
                
        }

        public string this[int i_iId]
        {
            get
            {
                return GetString(i_iId);
            }
        }

        public void SaveLocalization(string language, string i_strResourceFile)
        {
            Validator.CheckNullString(language);
            Validator.CheckNullString(i_strResourceFile);

            LwLocalization aLocalization;
            if (!_localizations.TryGetValue(language,out aLocalization))
                throw new DoesNotExistException(language,"Language \"" + language + "\" is not supported");


            aLocalization.Save(i_strResourceFile);

        }

        public LwLocalization GetLocalization(string language)
        {
            Validator.CheckNullString(language);

            LwLocalization aLocalization;
            if (!_localizations.TryGetValue(language,out aLocalization))
                throw new DoesNotExistException(language,"Language \"" + language + "\" is not supported");

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

        public void LoadPackage(string i_strPackageFile)
        {
            if (!File.Exists(i_strPackageFile))
            {
                throw new DoesNotExistException(i_strPackageFile,"Package specified by \""+i_strPackageFile+"\" does not exist");
            }

            string strDirectory = Path.GetDirectoryName(i_strPackageFile);
            if (!Directory.Exists(strDirectory))
            {
                throw new DoesNotExistException(i_strPackageFile,"Directory \"" + i_strPackageFile + "\" for package \""+i_strPackageFile+"\" does not exist");
            }

            Xml.QuickXml aXml = new Xml.QuickXml();
            if (!aXml.Load(i_strPackageFile, true))
                throw new ArgumentException("XML localization info file \""+i_strPackageFile+"\" is invalid");

            string[] arPaths = aXml.GetTexts("//lang");
            if (null == arPaths ||
                0 == arPaths.Length)
                throw new ArgumentException("XML localization info file \"" + i_strPackageFile + "\" does not contain localization information");

            LinkedList<string> aFailed = new LinkedList<string>();
            string strAbsPath = null;
            foreach (string strPath in arPaths)
            {
                strAbsPath = strDirectory + "\\" + "localization_"+strPath + ".ldx";
                if (!File.Exists(strAbsPath))
                {
                    aFailed.AddLast("Referenced localization file \""+strAbsPath+"\" does not exist");
                    continue;
                }
                else
                {
                    try
                    {
                        LoadLocalization(strAbsPath);
                    }
                    catch (Exception aError)
                    {
                        aFailed.AddLast(aError.Message);
                    }                    
                }
            }

            IList<string> aPaths = new List<string>(4);
            aXml.GetTexts("//default-paths/json",ref aPaths);
            JSONPaths = aPaths;


            aPaths.Clear();
            aXml.GetTexts("//default-paths/csharp",ref aPaths);
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

                throw new InvalidProgramException(strReport);
            }


            
        }

        public void GenerateJSONSymbols(string i_strOutputFile)
        {
            FileStream aOutputFile = new FileStream(i_strOutputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter aWriter = new StreamWriter(aOutputFile, Encoding.UTF8);

            LwLocalization aFirstLocalization = GetFirstLocalization();
            if (null == aFirstLocalization)
                throw new DoesNotExistException(null,"No localization loaded");

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

        public void GenerateAllJSONData([NotNull] IList<string> outputDirectories)
        {
            Validator.CheckForNull(outputDirectories,"outputDirectories");

            string strFilePath;

            if (null == _masterLocalization)
            {
                throw new InvalidProgramException("No master localization present");
            }

            foreach (string strDirPath in outputDirectories)
            {

                if (!Directory.Exists(strDirPath))
                {
                    throw new DoesNotExistException(strDirPath,"Output directory \"" + strDirPath + " does not exist");
                }
               
                LinkedList<string> aFailed = new LinkedList<string>();

                strFilePath = strDirPath + @"\localization_symbols.js";
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
                        strReport += j + ". " + strReport + "\n\n";
                    }

                    throw new InvalidProgramException(strReport);
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
                throw new DoesNotExistException(strOutputDirectory,"Directory \"" + strOutputDirectory + "\" for current package does not exist");
            }

            Xml.QuickXml aXml = new Contal.IwQuick.Xml.QuickXml();

            if (!aXml.Load(localizationInfoPath, false))
                throw new ArgumentException("XML localization info file \"" + localizationInfoPath + "\" is invalid");

            string strXmlFile = String.Empty;

            List<string> aLanguages = new List<string>();
            foreach (LwLocalization aLocalization in _localizations.Values)
            {
                strXmlFile = strOutputDirectory + "\\localization_" + aLocalization.Language + ".ldx";
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

            else
            {
                foreach (LwLocalization aLocalization in _localizations.Values)
                    return aLocalization;
            }

            return null;
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
