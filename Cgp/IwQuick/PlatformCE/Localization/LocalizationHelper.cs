/*
 * All localized texts have to by saved in resource files for that language
 * the name of the resource file is : Localization.LANGUAGE_NAME.resx ( e.g. ResourceLng.Slovak.resx, ResourceLng.English.resx)
 * the LocalizationHelper will search the language support only in these files.
 * 
 * values in the resource must be in this format:
 * FORM_NAME.CONTROL_NAME       (e.g. form1._bNext, form1._lText)
 * 
 * or OBJECT.VARIABLE if you want save another stuff
 * to obtain the value (in language support) call method GetStringResource(USE_FULL_NAME)
 * 
 * 
 * the form to translate must have registered event LanguageChanged, and for this event method where is call TranslateForm(this);
 * LocalizationHelper will translate only controls on the form.
 * 
 * The LocalizationHelper doesn't change CurrentUICulture and CurrentCulture 
 * 
 * How to use LocalizationHelper: create instance / set Assembly / SetLanguage
 * 
 * 
 * */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using Contal.IwQuick;


namespace Contal.IwQuick.Localization
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalizationHelper
    {
        private Dictionary<string, string> _allLanguageResources = new Dictionary<string, string>();
        private Dictionary<string, ResourceManager> _allResourceManagers = new Dictionary<string, ResourceManager>(); 
        private Assembly _actualAssembly = null;
        
        private const string LocalizationResourcePrefix = "Localization";
        private const string NoTranslation = "NO_TRANSLATION";

        /// <summary>
        /// Set default language
        /// </summary>
        public string DefaultLanguage { get; set; }

        #region Properties
        /// <summary>
        /// Assembly where LocalizationHelper reads resources
        /// </summary>
        public Assembly ActualAssembly
        {
            get { return _actualAssembly; }
            set
            {
                Validator.CheckForNull(value,"value");

                _actualAssembly = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualAssembly"></param>
        public LocalizationHelper(Assembly actualAssembly)
        {
            DefaultLanguage = "English";
            ActualAssembly = actualAssembly;
            LoadAvailableLanguages();
        }

        /// <summary>
        /// All Languages loaded by LocalizationHelper
        /// </summary>
        public string[] AllLanguages
        {
            get { return _allLanguageResources.Keys.ToArray(); }
        }

        #endregion

        /// <summary>
        /// loads available languages from resources
        /// </summary>
        public void LoadAvailableLanguages()
        {
            if (_actualAssembly == null)
                throw new DoesNotExistException("LocalizationHelper: No Assembly Source");

            string[] _xmlFiles = _actualAssembly.GetManifestResourceNames();

            foreach (string asem in _xmlFiles)
            {
                if (asem.IndexOf(LocalizationResourcePrefix, StringComparison.Ordinal) != -1)
                {
                    string[] parts = asem.Split('.');

                    string newAsem = asem.Substring(
                        0,
                        asem.Length - parts[parts.Length - 1].Length - 1);

                    _allLanguageResources.Add(parts[parts.Length - 2], newAsem);
                }
            }
        }

        public string GetString(string name)
        {
            return GetString(name, DefaultLanguage);
        }

        public string GetString(string name, string language)
        {
            string resPath;
            if (string.IsNullOrEmpty(name)
                || string.IsNullOrEmpty(language)
                || !_allLanguageResources.TryGetValue(language, out resPath))
            {
                return string.Format("{0} {1}", NoTranslation, name);
            }

            ResourceManager resMngr;
            try
            {
                if (!_allResourceManagers.TryGetValue(language, out resMngr))
                {
                    resMngr = new ResourceManager(resPath, _actualAssembly);
                    _allResourceManagers.Add(language, resMngr);
                }
            }
            catch (TypeLoadException)
            {
                return string.Format("{0} {1}", NoTranslation, name);
            }

            string resourceValue = resMngr.GetString(name);
            if (null == resourceValue)
            {
                Debug.WriteLine(" ");
                Debug.WriteLine(string.Format("Missing translation in {0} for symbol {1}", language, name));
                Debug.WriteLine(" ");
                return string.Format("{0} {1}", NoTranslation, name);
            }
            
            return resourceValue;
        }  
    }
}
