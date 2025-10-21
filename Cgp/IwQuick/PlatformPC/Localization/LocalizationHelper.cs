/*
 * All localized texts have to by saved in resource files for that language
 * the name of the resource file is : ResourceLng.LANGUAGE_NAME.resx ( e.g. ResourceLng.Slovak.resx, ResourceLng.English.resx)
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
 **/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Collections;
using System.Xml;
using Contal.IwQuick.Data;
using Contal.IwQuick.Localization.LocalizationAssistant;
using Contal.IwQuick.UI;
using JetBrains.Annotations;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows;
using ComboBox = System.Windows.Forms.ComboBox;
using Control = System.Windows.Forms.Control;
using GroupBox = System.Windows.Forms.GroupBox;
using ListBox = System.Windows.Forms.ListBox;
using Panel = System.Windows.Forms.Panel;
using TabControl = System.Windows.Forms.TabControl;
using UserControl = System.Windows.Controls.UserControl;
using System.Windows.Controls;

namespace Contal.IwQuick.Localization
{
    public class LocalizationHelper : IEnumerable<Localization>
    {
        private string _actualLanguage;
        private string _actualResource;
        private string _actualFormName;
        private string[] _allResources;
        private string[] _allLanguages;

        private System.Drawing.Color _kryptonColorizationColor = System.Drawing.Color.Transparent;

#if DEBUG
        private volatile LocalizationAssistentMainForm _localizationAssistantDialog;
        private readonly object _syncLocalizationAssistentDialog = new object();
#endif

        protected internal Dictionary<string, Localization> _localizations;
        private Assembly _actualAssembly;

        private volatile ResourceManager _resourceManager;
        private readonly object _syncResourceManager = new object();

        private List<string> _generateCSharpPaths;

        [NotNull]
        private readonly Dictionary<Type, Action<object>> _perTypeConversionLambdas = 
            new Dictionary<Type, Action<object>>();

        private bool _enableSystemLanguage;

        /// <summary>
        /// 
        /// </summary>
        public IList<string> CSharpPaths
        {
            get { return _generateCSharpPaths; }
            [NotNull]
            set
            {
                Validator.CheckNullAndEmpty(value,"value");
                _generateCSharpPaths = new List<string>(4);
                foreach (var path in value)
                {
                    if (Directory.Exists(path))
                        _generateCSharpPaths.Add(path);
                }
            }
        }

        private Localization _masterLocalization;
        /// <summary>
        /// 
        /// </summary>
        public Localization MasterLocalization
        {
            set { _masterLocalization = value; }
            get { return _masterLocalization; }
        }

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable 67
        public event DString2Void WordsAdded;
#pragma warning restore 67

        private const string _localizationResourcePrefix = "Localization";
        private const string _systemLanguage = "System language";

        /// <summary>
        /// raised when the language was successfuly changed
        /// </summary>
        public event DVoid2Void LanguageChanged;

        /// <summary>
        /// 
        /// </summary>
        public event Action<string, string> NoTranslationOccured;

        private void InitPerTypeConversionLambdas()
        {
            _perTypeConversionLambdas.Add(typeof (Window),TranslateWindow);

            _perTypeConversionLambdas.Add(typeof(UserControl), obj => TranslateWpfControl(obj as UserControl));

            _perTypeConversionLambdas.Add(typeof (System.Windows.Controls.Button), TranslateWpfButton);

            _perTypeConversionLambdas.Add(typeof(System.Windows.Controls.Label), TranslateWpfLabel);

            _perTypeConversionLambdas.Add(typeof(System.Windows.Controls.GroupBox), TranslateWpfGroupBox);

            _perTypeConversionLambdas.Add(typeof(System.Windows.Controls.CheckBox), TranslateWpfCheckBox);

            _perTypeConversionLambdas.Add(typeof(Canvas), TranslateWpfCanvas);

            _perTypeConversionLambdas.Add(typeof(System.Windows.Controls.ListView), TranslateWpfListView);
        }

        private void TranslateWpfListView(object obj)
        {
            var wpfListView = obj as System.Windows.Controls.ListView;

            if (wpfListView != null)
            {
                var gridView = wpfListView.View as GridView;

                if (gridView != null)
                {
                    for (var i = 0; i < gridView.Columns.Count; i++)
                    {
                        var text = GetString(string.Concat(_actualFormName, wpfListView.Name, "_Column", i));

                        if (!string.IsNullOrEmpty(text))
                        {
                            gridView.Columns[i].Header = text;
                        }
                    }
                }
            }
        }

        private void TranslateWpfCanvas(object obj)
        {
            var wpfCanvas = obj as Canvas;

            if (wpfCanvas != null && wpfCanvas.ContextMenu != null)
            {
                TranslateWpfContextMenu(wpfCanvas.ContextMenu);
            }
        }

        private void TranslateWpfCheckBox(object obj)
        {
            var wpfCheckBox = obj as System.Windows.Controls.CheckBox;

            if (wpfCheckBox != null)
            {
                var text = GetString(string.Concat(_actualFormName, wpfCheckBox.Name));

                if (!string.IsNullOrEmpty(text))
                    wpfCheckBox.Content = text;
            }
        }

        private void TranslateWpfGroupBox(object obj)
        {
            var wpfGroupBox = obj as System.Windows.Controls.GroupBox;

            if (wpfGroupBox != null)
            {
                var text = GetString(string.Concat(_actualFormName, wpfGroupBox.Name));

                if (!string.IsNullOrEmpty(text))
                    wpfGroupBox.Header = text;
                {
                    text = GetString(string.Concat( GENERAL_TAG, wpfGroupBox.Name));

                    if (!string.IsNullOrEmpty(text))
                        wpfGroupBox.Header = text;
                }
            }
        }

        private void TranslateWpfButton(object obj)
        {
            var wpfButton = obj as System.Windows.Controls.Button;

            if (wpfButton != null)
            {
                if (wpfButton.ToolTip != null)
                {
                    var toolTip = GetString(string.Concat(_actualFormName, wpfButton.Name, "_ToolTip"));

                    if (!string.IsNullOrEmpty(toolTip))
                        wpfButton.ToolTip = toolTip;
                }

                if (wpfButton.Content is string)
                {
                    var text = GetString(string.Concat(_actualFormName, wpfButton.Name));

                    if (!string.IsNullOrEmpty(text))
                        wpfButton.Content = text;

                    return;
                }

                var stackPanel = wpfButton.Content as StackPanel;
                if (stackPanel != null)
                {
                    var tb = stackPanel.Children.Cast<UIElement>().FirstOrDefault(e => e is TextBlock) as TextBlock;

                    if (tb != null)
                    {
                        var text = GetString(string.Concat(_actualFormName, wpfButton.Name));

                        if (!string.IsNullOrEmpty(text))
                            tb.Text = text;
                    }
                }
            }
        }

        private void TranslateWindow(object obj)
        {
            var window = obj as Window;

            if (window != null)
            {
                var text = GetString(string.Concat(_actualFormName, window.Name));

                if (!string.IsNullOrEmpty(text))
                    window.Title = text;
            }
        }

        private void TranslateWpfLabel(object obj)
        {
            var wpfLabel = obj as System.Windows.Controls.Label;

            if (wpfLabel != null)
            {
                var text = GetString(string.Concat(_actualFormName, wpfLabel.Name));

                if (!string.IsNullOrEmpty(text))
                    wpfLabel.Content = text;
                else
                {
                    text = GetString(string.Concat(GENERAL_TAG, wpfLabel.Name));

                    if (!string.IsNullOrEmpty(text))
                        wpfLabel.Content = text;
                }
            }
        }

        #region Properties
        /// <summary>
        /// Assembly where LocalizationHelper reads resources
        /// </summary>
        public Assembly ActualAssembly
        {
            get { return _actualAssembly; }
            [NotNull] set
            {
                Validator.CheckForNull(value,"value");

                _actualAssembly = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LocalizationHelper()
            :this(null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualAssembly"></param>
        public LocalizationHelper(Assembly actualAssembly)
        {
            InitAssembly(actualAssembly);
        }

        private void InitAssembly(Assembly actualAssembly)
        {
            if (!ReferenceEquals(null, actualAssembly))
            {
                ActualAssembly = actualAssembly;
                LoadAvailableLanguages();
            }

            _generateCSharpPaths = null;
            _masterLocalization = null;
            _localizations = new Dictionary<string, Localization>();
            InitPerTypeConversionLambdas();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualAssembly"></param>
        /// <param name="enableSystemLanguage"></param>
        public LocalizationHelper(Assembly actualAssembly, bool enableSystemLanguage)
        {
            _enableSystemLanguage = enableSystemLanguage;
            InitAssembly(actualAssembly);
        }

        /// <summary>
        /// All Languages loaded by LocalizationHelper
        /// </summary>
        public string[] AllLanguages
        {
            get { return _allLanguages; }
        }
        /// <summary>
        /// Current language in LocalizationHelper
        /// </summary>
        public string ActualLanguage
        {
            get { return _actualLanguage; }
        }
        #endregion

        /// <summary>
        /// Set language 
        /// </summary>
        /// <param name="selectedLanguage">selected language</param>
        public void SetLanguage(string selectedLanguage)
        {
            if (selectedLanguage == _actualLanguage) 
                return;
            
            var ok = false;
            LoadAvailableLanguages();

            for (var i = 0; i < _allLanguages.Length; i++)
            {
                if (selectedLanguage == _allLanguages[i])
                {
                    ok = true;
                    _actualLanguage = _allLanguages[i];

                    _actualResource = selectedLanguage.Contains(_systemLanguage) 
                        ? _allResources[0] 
                        : _allResources[i];

                    // Use this, not ensure, due overwrite nature
                    lock(_syncResourceManager)
                        _resourceManager = new ResourceManager(_actualResource, _actualAssembly);

                    break;
                }
            }

            if (ok)
            {
                if (LanguageChanged != null)
                {
                    try
                    {
                        LanguageChanged();
                    }
                    catch {}
                }
            }
            else
            {
                if (selectedLanguage == _systemLanguage
                    && _allLanguages.Length > 0)
                {
                    SetLanguage(_allLanguages[0]);
                    return;
                }

                throw new DoesNotExistException(selectedLanguage);
            }
        }

        /// <summary>
        /// loads available languages from resources
        /// </summary>
        public void LoadAvailableLanguages()
        {
            if (_actualAssembly == null)
                throw new DoesNotExistException("LocalizationHelper: No Assembly Source");

            var _xmlFiles = _actualAssembly.GetManifestResourceNames();

            var resources = new List<string>();
            var languages = new List<string>();
            foreach (var asem in _xmlFiles)
            {
                if (asem.Contains(_localizationResourcePrefix))
                {
                    var parts = asem.Split('.');
                    var newLenfgt = asem.Length - 1;
                    newLenfgt -= parts[parts.Length - 1].Length;
                    var newAsem = asem.Substring(0, newLenfgt);
                    resources.Add(newAsem);
                    languages.Add(parts[parts.Length - 2]);
                }
            }

            if (_enableSystemLanguage)
                languages.Add(_systemLanguage);
            
            _allResources = new string[resources.Count];
            resources.CopyTo(_allResources, 0);
            _allLanguages = new string[languages.Count];
            languages.CopyTo(_allLanguages, 0);
        }

        public const string NO_TRANSLATION = "(!T)";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if name is null, or if language is null or empty</exception>
        public string GetString(
            [NotNull] string name, 
            [NotNull] string language)
        {
            Validator.CheckForNull(name,"name");
            Validator.CheckNullString(language , "language");

            language = language.ToLower();
            int langIndex = -1;


            for (var i = 0; i < _allLanguages.Length; i++)
            {
                if (_allLanguages[i].ToLower().Equals(language))
                {
                    langIndex = i;
                    break;
                }
            }

            if (langIndex == -1)
            {
                Debug.WriteLine("\t Unknown language: " + language);
                return GetStringNoTranslation(name, true);
            }

            var manager = new ResourceManager(_allResources[langIndex], _actualAssembly);
            var result = manager.GetString(name);
            if (result != null)
                return result;

            return GetStringNoTranslation(name, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="useAssistenIfNoTranslation"></param>
        /// <returns></returns>
        public string GetString(
            [NotNull] string name, 
            bool useAssistenIfNoTranslation)
        {
            string translation;

            GetString(name, useAssistenIfNoTranslation, out translation);

            return translation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="useAssistenIfNoTranslation"></param>
        /// <param name="translationFound"></param>
        /// <returns></returns>
        public string GetString(
            [NotNull] string name,
            bool useAssistenIfNoTranslation,
            out bool translationFound)
        {
            string translation;

            translationFound = GetString(name, useAssistenIfNoTranslation, out translation);

            return translation;
        }

        private const string NullSymbol = "NULL_SYMBOL";
        private const string TransError = "TRANSLATION_ERROR";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="useAssistenIfNoTranslation"></param>
        /// <param name="translation"></param>
        /// <returns>false returns only if translation was not found , but there was no problem neither with resources,
        /// neither with symbol</returns>
        private bool GetString(
            string name, 
            bool useAssistenIfNoTranslation, 
            out string translation)
        {
            translation = String.Empty;

            if (null == name)
            {
                translation = NullSymbol;
                return true;
            }
            try
            {
                if (_actualLanguage.ToLower().Contains(_systemLanguage.ToLower()))
                {
                    if (string.IsNullOrEmpty(_resourceManager.GetString(name)))
                        return false;

                    translation = name;
                    return true;
                }

                EnsureResourceManager();
                //System.Resources.ResourceManager resMngr = new System.Resources.ResourceManager(_actualResource, _actualAssembly);
                var resourceValue = _resourceManager.GetString(name);
                
                if (null == resourceValue)
                {
                    translation = GetStringNoTranslation(name, useAssistenIfNoTranslation);
                    return false;
                }

                translation = resourceValue;
                return true;
            }
            catch(Exception e)
            {
                DebugHelper.TryBreak(e,name,useAssistenIfNoTranslation,translation);

                translation = TransError;

                return true;
            }
        }


        /// <summary>
        /// Return value from actual resource
        /// </summary>
        /// <param name="name">resource name</param>
        /// <returns>resource values</returns>
        public string GetString([NotNull] string name)
        {
            return GetString(name, true);
        }

        /// <summary>
        /// Return value from actual resource
        /// </summary>
        /// <param name="name">resource name</param>
        /// <param name="args">string format args</param>
        /// <returns>resource values</returns>
        public string GetString([NotNull] string name, params object[] args)
        {
            if (args.Any())
                return string.Format(GetString(name, true), args);

            return GetString(name, true);
        }

#if DEBUG
        private string _debugSolutionPath;

        private volatile ProcessingQueue<string> _wordsAddedProcessingQueue;
        private readonly object _syncWordsAddedProcessingQueue = new object();

        private void EnqueueWord(string word)
        {
            if (null == _wordsAddedProcessingQueue)
                lock(_syncWordsAddedProcessingQueue)
                    if (null == _wordsAddedProcessingQueue)
                    {
                        var pq = new ProcessingQueue<string>();
                        pq.ItemProcessing += OnWordsAddedItemProcessing;
                        _wordsAddedProcessingQueue = pq;
                    }

            bool toShow = false;

            lock (_syncLocalizationAssistentDialog)
            {
                if (_localizationAssistantDialog != null && _localizationAssistantDialog.IsDisposed)
                    _localizationAssistantDialog = null;                

                if (_localizationAssistantDialog == null)
                {
                    if (_debugSolutionPath == null)
                        _debugSolutionPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();

                    _localizationAssistantDialog = new LocalizationAssistentMainForm(_actualLanguage, _debugSolutionPath, this);
                    toShow = true;
                }
            }

            if (toShow)
            {
                Screen screen = GetSecondaryScreen();
                if (screen != null)
                {
                    _localizationAssistantDialog.StartPosition = FormStartPosition.Manual;
                    _localizationAssistantDialog.Location = screen.WorkingArea.Location;
                }

                _localizationAssistantDialog.Show();
            }

            _wordsAddedProcessingQueue.Enqueue(word);
        }

        private void OnWordsAddedItemProcessing(string parameter)
        {
            WordsAdded(parameter);
        }
#endif


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="useAssistentIfNotFound"></param>
        /// <returns></returns>
// ReSharper disable once UnusedParameter.Local
        private string GetStringNoTranslation(string name, bool useAssistentIfNotFound)
        {
            if (NoTranslationOccured != null)
            {
                NoTranslationOccured(_actualLanguage, name);
            }
#if DEBUG
            #region LocalizationAssistentHook
            if (useAssistentIfNotFound)
            {
                EnqueueWord(name);
            }
            #endregion

            Debug.WriteLine("\t" + NO_TRANSLATION + " Missing translation in " + (_actualLanguage ?? "UNKNOWN") + " for symbol " + name);
#endif
            return string.Concat(NO_TRANSLATION, StringConstants.SPACE, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Screen GetSecondaryScreen()
        {
            if (Screen.AllScreens.Length == 1)
            {
                return null;
            }

            return Screen.AllScreens.FirstOrDefault(screen => screen.Primary == false);
        }

        private const string GENERAL_TAG = "General";

        private string GetFromResource(string name)
        {
            if (string.IsNullOrEmpty(name)
                || _resourceManager == null)
            {
                return null;
            }

            if (_actualLanguage.ToLower().Contains(_systemLanguage.ToLower()))
            {
                if (string.IsNullOrEmpty(_resourceManager.GetString(name)))
                    return null;

                return name;
            }

            return _resourceManager.GetString(name);
        }

        /// <summary>
        /// Return localized text from the resX file for variable, support multisearch
        /// </summary>
        /// <param name="controlToSet">localized variable</param>
        /// <returns>localized text</returns>Z
        //private void GetTranslate(string variableName, ref string toSetText)
        private void GetTranslate(Control controlToSet)
        {
            if (controlToSet == null)
                return;

            string localizedText;
            
            //If tag is ComboBox it translate its items
            var values = controlToSet.Tag as string[];
            if (values != null && (controlToSet is ComboBox || controlToSet is ListBox))
            {
                var cb = controlToSet as ComboBox;
                var lb = controlToSet as ListBox;

                var count = cb != null ? cb.Items.Count : lb.Items.Count;

                if (values.Length < count)
                    count = values.Length;

                for (var i = 0; i < count; i++)
                {
                    if (!string.IsNullOrEmpty(values[i]))
                    {
                        localizedText = GetFromResource(_actualFormName + controlToSet.Name + "_" + values[i]);
                        if (!string.IsNullOrEmpty(localizedText))
                        {
                            if (cb != null)
                                cb.Items[i] = localizedText;
                            else
                                lb.Items[i] = localizedText;
                            continue;
                        }

                        localizedText = GetFromResource(GENERAL_TAG + controlToSet.Name + "_" + values[i]);
                        if (!string.IsNullOrEmpty(localizedText))
                        {
                            if (cb != null)
                                cb.Items[i] = localizedText;
                            else
                                lb.Items[i] = localizedText;
                            continue;
                        }

                        localizedText = GetFromResource(values[i]);
                        if (!string.IsNullOrEmpty(localizedText))
                        {
                            if (cb != null)
                                cb.Items[i] = localizedText;
                            else
                                lb.Items[i] = localizedText;
                        }
                    }
                }
            }

            //If tag is not null and string type
            var s = controlToSet.Tag as string;
            if (s != null)
            {
                localizedText = GetFromResource(s);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    controlToSet.Text = localizedText;
                    return;
                }
            }

            localizedText = GetFromResource(_actualFormName + controlToSet.Name);
            if (!string.IsNullOrEmpty(localizedText))
            {
                controlToSet.Text = localizedText;
                return;
            }

            localizedText = GetFromResource(GENERAL_TAG + controlToSet.Name);
            if (!string.IsNullOrEmpty(localizedText))
            {
                controlToSet.Text = localizedText;
                return;
            }

            localizedText = GetFromResource(controlToSet.Name + controlToSet.Name);
            if (!string.IsNullOrEmpty(localizedText))
            {
                controlToSet.Text = localizedText;
                return;
            }

            localizedText = GetFromResource(controlToSet.Name);
            if (!string.IsNullOrEmpty(localizedText))
            {
                controlToSet.Text = localizedText;
                return;
            }

            if ((!string.IsNullOrEmpty(controlToSet.Name)) && (Char.IsDigit(controlToSet.Name[controlToSet.Name.Length - 1])))
            {
                var tmpName = new StringBuilder(controlToSet.Name);
                while (Char.IsDigit(tmpName[tmpName.Length - 1]))
                {
                    tmpName.Length = tmpName.Length - 1;
                }

                localizedText = GetFromResource(_actualFormName + tmpName);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    controlToSet.Text = localizedText;
                    return;
                }

                localizedText = GetFromResource(GENERAL_TAG + tmpName);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    controlToSet.Text = localizedText;
                    return;
                }
            }

            var directSymbol = string.Concat(
                        StringConstants.UNDERSCORE,
                        controlToSet.Text.Replace(StringConstants.SPACE, StringConstants.UNDERSCORE)
                        );
            localizedText = GetFromResource(directSymbol);
            if (!string.IsNullOrEmpty(localizedText))
            {
                controlToSet.Text = localizedText;
            }
            //if is here no translate for value was founded.
        }

        private void GetTranslate(ToolStripItem itemToSet)
        //where T : Control 
        {
            var localizedText = GetFromResource(_actualFormName + itemToSet.Name);
            if (!string.IsNullOrEmpty(localizedText))
                itemToSet.Text = localizedText;

            localizedText = GetFromResource(itemToSet.Name + itemToSet.Name);
            if (!string.IsNullOrEmpty(localizedText))
                itemToSet.Text = localizedText;

            localizedText = GetFromResource(itemToSet.Name);
            if (!string.IsNullOrEmpty(localizedText))
                itemToSet.Text = localizedText;

            if ((!string.IsNullOrEmpty(itemToSet.Name)) && (Char.IsDigit(itemToSet.Name[itemToSet.Name.Length - 1])))
            {
                var tmpName = new StringBuilder(itemToSet.Name);
                while (Char.IsDigit(tmpName[tmpName.Length - 1]))
                {
                    tmpName.Length = tmpName.Length - 1;
                }

                localizedText = GetFromResource(tmpName.ToString());
                if (!string.IsNullOrEmpty(localizedText))
                    itemToSet.Text = localizedText;
            }

            Debug.WriteLine(" ");
            Debug.WriteLine("Missing translation in " + (_actualLanguage ?? "UNKNOWN") + " for symbol " + itemToSet.Name);
            Debug.WriteLine(" ");
            //if is here no translate for value was founded.

            if (NoTranslationOccured != null)
            {
                NoTranslationOccured(_actualLanguage, _actualFormName + itemToSet.Name);
            }
        }

        /// <summary>
        /// Change available text on the form controls.
        /// </summary>
        /// <param name="formToChange">the form to change</param>
        public delegate void DChangeFormLng(Form formToChange);
        public void TranslateForm(Form formToChange)
        {
            if (formToChange.InvokeRequired)
            {
                formToChange.BeginInvoke(new DChangeFormLng(TranslateForm), formToChange);
            }
            else
            {
                //init form name and Resource manager
                _actualFormName = formToChange.Name;
                EnsureResourceManager();
                GetTranslate(formToChange);

                var contColl = formToChange.Controls;

                foreach (Control cnt in contColl)
                {
                    GetTranslate(cnt);

                    if (cnt.ContextMenuStrip != null)
                    {
                        foreach (ToolStripItem mi in cnt.ContextMenuStrip.Items)
                        {
                            GetTranslate(mi);
                            TranslateToolStripItemsRecursive(mi);
                        }
                    }
                    
                    var ms = cnt as MenuStrip;
                    if (ms != null)
                    {
                        //System.Windows.Forms.ToolStrip ms = (System.Windows.Forms.ToolStrip)cnt;
                        foreach (ToolStripItem mi in ms.Items)
                        {
                            GetTranslate(cnt);
                            TranslateToolStripItemsRecursive(mi);
                        }
                        continue;
                    }

                    var ts = cnt as ToolStrip;
                    if (ts != null)
                    {
                        //System.Windows.Forms.ToolStrip ms = (System.Windows.Forms.ToolStrip)cnt;
                        foreach (ToolStripItem ti in ts.Items)
                        {
                            GetTranslate(cnt);
                            TranslateToolStripItemsRecursive(ti);
                        }
                        continue;
                    }

                    if (cnt is TabControl ||
                        cnt is TabPage ||
                        cnt is Panel ||
                        cnt is GroupBox ||
                        cnt is SplitContainer ||
                        //cnt is SplitterPanel || // always false
                        cnt is TextBoxMenu ||
                        cnt is ExtendedPropertyGrid ||
                        cnt is System.Windows.Forms.UserControl)
                    {
                        TranslateControl(cnt);
                    }
                }
            }
        }

        private void EnsureResourceManager()
        {
            if (_resourceManager == null)
                lock (_syncResourceManager)
                    if (_resourceManager == null)
                        _resourceManager = new ResourceManager(_actualResource, _actualAssembly);
        }

        /// <summary>
        /// change texts on Tool Strip
        /// </summary>
        /// <param name="tsmi">ToolStripItem</param>
        private void TranslateToolStripItemsRecursive(ToolStripItem tsmi)
        {
            GetTranslate(tsmi);
            var tsddi = tsmi as ToolStripDropDownItem;
            if (tsddi != null)
            {
                try
                {
                    foreach (ToolStripItem childTsmi in tsddi.DropDownItems)
                    {
                        GetTranslate(childTsmi);
                        TranslateToolStripItemsRecursive(childTsmi);
                    }
                }
                catch
                {
                    Console.WriteLine();
                }
            }
        }

        public void TranslateControl(Control controlHe)
        {
            GetTranslate(controlHe);

            var contColl = controlHe.Controls;
            foreach (Control control in contColl)
            {
                if (control != null) // && cnt.Name != string.Empty)
                {
                    if (control.ContextMenuStrip != null)
                    {
                        foreach (ToolStripItem mi in control.ContextMenuStrip.Items)
                        {
                            GetTranslate(mi);
                            TranslateToolStripItemsRecursive(mi);
                        }
                    }

                    GetTranslate(control);

                    var dgw = control as DataGridView;
                    if (dgw != null)
                    {
                        TranslateDataGridViewColumnsHeaders(dgw);
                        continue;
                    }

                    var ms = control as ToolStrip;
                    if (ms != null)
                    {
                        foreach (ToolStripItem mi in ms.Items)
                        {
                            GetTranslate(control);
                            TranslateToolStripItemsRecursive(mi);
                        }
                        continue;
                    }
                    if (control is ExtendedPropertyGrid)
                    {
                        TranslateExPropertyGrid(control as ExtendedPropertyGrid);
                    }


                    //if (cnt.Controls != null)
                    //{
                    //    foreach (Control lwCnt in cnt.Controls)
                    //    {
                    //        TranslateControl((System.Windows.Forms.Control)cnt);
                    //    }
                    //    //continue;
                    //}


                    if (control is TabControl ||
                        control is TabPage ||
                        control is TableLayoutPanel ||
                        control is Panel ||
                        control is GroupBox ||
                        control is SplitContainer ||
                        //cnt is SplitterPanel || // always false
                        control is TextBoxMenu ||
                        control is ExtendedPropertyGrid ||
                        control is System.Windows.Forms.UserControl)
                    {
                        TranslateControl(control);
                        //continue;
                    }

                    if (control is ElementHost)
                    {
                        TranslateWpfControl((control as ElementHost).Child as UserControl);
                    }
                }
            }
        }

        public void TranslateWpfControl(string actualFormName, DependencyObject wpfControl)
        {
            _actualFormName = actualFormName;
            TranslateWpfControl(wpfControl);
        }

        public void TranslateWpfControl(DependencyObject wpfControl)
        {
            Action<object> func;

            if (wpfControl is Window && _perTypeConversionLambdas.TryGetValue(typeof(Window), out func))
            {
                func(wpfControl);
            }

            foreach (var element in FindVisualChildren(wpfControl))
            {
                if (_perTypeConversionLambdas.TryGetValue(element.GetType(), out func))
                {
                    func(element);
                }
            }
        }

        private IEnumerable<DependencyObject> FindVisualChildren(DependencyObject depObj)
        {
            if (depObj != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null)
                    {
                        yield return child;
                    }

                    foreach (var childOfChild in FindVisualChildren(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void TranslateWpfContextMenu(System.Windows.Controls.ContextMenu contextMenu)
        {
            if (contextMenu != null)
            {
                foreach (System.Windows.Controls.MenuItem mi in contextMenu.Items)
                {
                    var text = GetString(string.Concat(_actualFormName, mi.Name));

                    if (!string.IsNullOrEmpty(text))
                        mi.Header = text;
                    else
                    {
                        text = GetString(string.Concat( GENERAL_TAG, mi.Name));

                        if (!string.IsNullOrEmpty(text))
                            mi.Header = text;
                    }

                    TranslateWpfMenuItem(mi);
                }
            }
        }

        private void TranslateWpfMenuItem(System.Windows.Controls.MenuItem menuItem)
        {
            if (menuItem != null)
            {
                foreach (System.Windows.Controls.MenuItem mi in menuItem.Items)
                {
                    if (mi.Items.Count > 0)
                        TranslateWpfMenuItem(mi);

                    var text = GetString(string.Concat( _actualFormName, mi.Name));

                    if (!string.IsNullOrEmpty(text))
                        mi.Header = text;
                }
            }
        }

        private const string COLUMN_TAG = "Column";

        /// <summary>
        /// Translate HeaderText in DataGridView
        /// </summary>
        /// <param name="dgv">dataGridView to translate</param>
        public void TranslateDataGridViewColumnsHeaders(DataGridView dgv)
        {
            if (_resourceManager == null) return;

            for (var i = 0; i < dgv.ColumnCount; i++)
            {
                var addString = string.Empty;
                var columnName = dgv.Columns[i].Name;

                var resourceValue = _resourceManager.GetString(columnName);

                if (string.IsNullOrEmpty(resourceValue) && !columnName.StartsWith(COLUMN_TAG))
                {
                    addString = COLUMN_TAG;
                    resourceValue = _resourceManager.GetString(addString + columnName);
                }

                if (string.IsNullOrEmpty(resourceValue))
                {
                    var columnNameWithoutNumber = Regex.Replace(columnName, @"[\d-]", string.Empty);
                    resourceValue = _resourceManager.GetString(addString + columnNameWithoutNumber);
                }

                if (string.IsNullOrEmpty(resourceValue))
                {
                    var directSymbol = string.Concat(
                        StringConstants.UNDERSCORE,
                        dgv.Columns[i].HeaderText.Replace(StringConstants.SPACE, StringConstants.UNDERSCORE)
                        );
                    resourceValue = _resourceManager.GetString(directSymbol);
                }

                if (!string.IsNullOrEmpty(resourceValue))
                    dgv.Columns[i].HeaderText = resourceValue;
                else
                {
                    if (NoTranslationOccured != null)
                    {
                        NoTranslationOccured(_actualLanguage, addString + columnName);
                    }
                }
            }
        }

        /// <summary>
        /// Translate single header column in DGV
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="localeId"></param>
        public void TranslateDgvHeaderColumn(DataGridView dgv, string columnName, string newColumnLocaleId)
        {
            string resourceValue = _resourceManager.GetString(newColumnLocaleId);

            if (string.IsNullOrEmpty(resourceValue))
            {
                var columnNameWithoutNumber = Regex.Replace(columnName, @"[\d-]", string.Empty);
                resourceValue = _resourceManager.GetString(columnNameWithoutNumber);
            }

            if (string.IsNullOrEmpty(resourceValue))
            {
                var directSymbol = string.Concat(StringConstants.UNDERSCORE,
                    dgv.Columns[columnName].HeaderText.Replace(StringConstants.SPACE, StringConstants.UNDERSCORE));
                resourceValue = _resourceManager.GetString(directSymbol);
            }

            if (!string.IsNullOrEmpty(resourceValue))
                dgv.Columns[columnName].HeaderText = resourceValue;
            else
            {
                if (NoTranslationOccured != null)
                {
                    NoTranslationOccured(_actualLanguage, columnName);
                }
            }
        }

        /// <summary>
        /// Translate ToolStripItem
        /// </summary>
        /// <param name="formName">form that own toolStripItem</param>
        /// <param name="tsmi">ToolStripItem to translate</param>
        public void TranslateToolStripItems(string formName, ToolStripItem tsmi)
        {
            if (_resourceManager == null)
                return;

            TranslateToolStripItemsRecursive(tsmi);
        }

        private void TranslateExPropertyGrid(ExtendedPropertyGrid exPropertyGrid)
        {
            const string description = "Description";
            foreach (CustomProperty property in exPropertyGrid.Items)
            {
                var name = _resourceManager.GetString(_actualFormName + property.Tag);
                if (!string.IsNullOrEmpty(name))
                    property.Name = name;
                var desc = _resourceManager.GetString(description + property.Tag);
                if (!string.IsNullOrEmpty(desc))
                    property.Description = desc;
            }
        }

        #region IEnumerable<Localization> Members

        public IEnumerator<Localization> GetEnumerator()
        {
            return _localizations.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _localizations.Values.GetEnumerator();
        }

        #endregion

        public void SynchronizeSymbolsFromMaster()
        {
            if (null == _masterLocalization)
                throw new InvalidOperationException("There is no master localization present");

            foreach (var aLocalization in _localizations.Values)
            {
                if (!aLocalization.IsMasterResource)
                {
                    aLocalization.CopySymbolsFrom(_masterLocalization, false);
                }
            }
        }

        /// <summary>
        /// Create or replace resource file with localization items
        /// </summary>
        /// <param name="localizationItems">localization items to resource file</param>
        /// <param name="filePath">the path of file</param>
        /// <param name="language">language</param>
        public void SaveResxLocalization(Dictionary<string, TranslationItem> localizationItems, string filePath, string language)
        {
            var writer = new ResXResourceWriter(filePath + @"\" + "Localization." + language + ".resx");
            foreach (var locItem in localizationItems)
            {
                var node = new ResXDataNode(locItem.Value.Name, locItem.Value.Value);
                writer.AddResource(node);
            }
            writer.Generate();
            writer.Close();
        }

        /// <summary>
        /// Load localization items from resx file
        /// </summary>
        /// <param name="filePath">path of resx file</param>
        /// <param name="language">language of localization</param>
        /// <param name="isMasterResource">master resource information - true / false</param>
        /// <returns>localization</returns>
        public Localization LoadResxLocalization(string filePath, string language, bool isMasterResource)
        {
            var localization = new Localization(this, language, false);
            localization.LoadResxFile(filePath);
            if (isMasterResource && null == _masterLocalization)
            {
                _masterLocalization = localization;
                localization.IsMasterResource = true;
            }
            _localizations[language] = localization;
            return localization;
        }

        /// <summary>
        /// Save package, which contains resx files and localization info file
        /// </summary>
        /// <param name="locaInfoFilePath">localization file path</param>
        public void SaveResxPackage(string locaInfoFilePath)
        {
            Validator.CheckNullString(locaInfoFilePath);
            var outputDirectory = Path.GetDirectoryName(locaInfoFilePath);
            if (outputDirectory == null || !Directory.Exists(outputDirectory))
            {
                throw new DoesNotExistException(outputDirectory, "Directory \"" + outputDirectory + "\" for current package does not exist");
            }
            var doc = new XmlDocument();
            XmlNode root = doc.CreateElement("root");
            doc.AppendChild(root);
            foreach (var localization in _localizations.Values)
            {
                SaveResxLocalization(localization.Data, outputDirectory, localization.Language);
                XmlNode languageNode = doc.CreateElement("language");
                languageNode.InnerText = localization.Language;
                if (localization.IsMasterResource)
                {
                    var langAttr = doc.CreateAttribute("IsMaster");
                    langAttr.Value = "true";

                    if (languageNode.Attributes != null)
                        languageNode.Attributes.Append(langAttr);
                }
                root.AppendChild(languageNode);
            }
            doc.Save(new FileStream(locaInfoFilePath, FileMode.Create, FileAccess.Write));
        }

        /// <summary>
        /// Load package, which contains localization data
        /// </summary>
        /// <param name="packageFile">files path</param>
        public void LoadResxPackage(string packageFile)
        {
            if (!File.Exists(packageFile))
            {
                throw new DoesNotExistException(packageFile, "Package specified by \"" + packageFile + "\" does not exist");
            }
            var dir = Path.GetDirectoryName(packageFile);
            var xmlTextRd = new XmlTextReader(packageFile);
            var failed = new LinkedList<string>();
            var wasIn = false;
            while (xmlTextRd.Read())
            {
                if (xmlTextRd.NodeType == XmlNodeType.Element && xmlTextRd.Name == "language")
                {
                    wasIn = true;
                    var isMasterResource = xmlTextRd.AttributeCount > 0;

                    var language = xmlTextRd.ReadElementString();
                    var absPath = dir + "\\" + "Localization." + language + ".resx";
                    if (!File.Exists(absPath))
                    {
                        failed.AddLast("Referenced localization file \"" + absPath + "\" does not exist");
                        continue;
                    }
                    
                    try
                    {
                        LoadResxLocalization(absPath, language, isMasterResource);
                    }
                    catch (Exception aError)
                    {
                        failed.AddLast(aError.Message);
                    }
                }
                if (failed.Count > 0)
                {
                    var report = string.Empty;
                    var j = 0;
                    foreach (var reportPart in failed)
                    {
                        j++;
                        report += j + ". " + reportPart + "\n\n";
                    }
                    throw new InvalidDataException(report);
                }
            }
            xmlTextRd.Close();
            if (!wasIn)
            {
                throw new DoesNotExistException(packageFile, "Package specified by \"" + packageFile + "\" is not correct");
            }
        }

        public Localization GetFirstLocalization()
        {
            if (null != _masterLocalization)
            {
                return _masterLocalization;
            }

            return _localizations.Values.FirstOrDefault();
        }

        public Localization CreateLocalization(string language, bool isMaster)
        {
            Validator.CheckNullString(language);
            if (_localizations.ContainsKey(language))
            {
                throw new AlreadyExistsException("Language \"" + language + "\" already exists");
            }
            var aLocalization = new Localization(this, language, isMaster);
            if (isMaster && null == _masterLocalization)
            {
                _masterLocalization = aLocalization;
            }
            _localizations.Add(language, aLocalization);
            return aLocalization;
        }

        public int Count
        {
            get { return _localizations.Count; }
        }

        public System.Drawing.Color KryptonColorizationColor
        {
            get
            {
                return _kryptonColorizationColor;
            }

            set
            {
                _kryptonColorizationColor = value;
            }
        }

        public void Clear()
        {
            foreach (var aLocalization in _localizations.Values)
                aLocalization.Clear();

            _localizations.Clear();
            _masterLocalization = null;
        }
    }
}
