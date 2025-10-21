using System;
using System.Reflection;
using System.Windows.Forms;
using Contal.IwQuick.UI;
using ComponentFactory.Krypton.Toolkit;
using System.Drawing;
using Contal.IwQuick;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.Client
{
    /// <summary>
    /// Basic form with support to translate
    /// </summary>
    public class CgpTranslateForm : KryptonForm
    {
        private const byte BUTTONSLANGUAGEWIDTH = 60;
        private const byte BUTTONSLANGUAGEHEIGHT = 20;
        private const byte BUTTONSLANGUAGESPACE = 5;
        private const byte BUTTONSLANGUAGETOP = 10;
        private const byte BUTTONSLANGUAGERIGHT = 10;

        // Default language
        private const string DEFAULTLANGUAGE = "English";

        // Delegate to change language
        private readonly DVoid2Void _delegateChangeLanguage;

        /// <summary>
        /// Property to get _localizationHelper
        /// </summary>
        protected LocalizationHelper LocalizationHelper { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CgpTranslateForm(LocalizationHelper localizationHelper)
        {
            LocalizationHelper = localizationHelper;

            // If form is loaded then call NovaTranslateForm
            Load += TanslateForm;
            // If form is closed then unregister delegate to change language
            FormClosed += UnregisterDelegateChangeLanguage;

            _delegateChangeLanguage = ChangeLanguage;
            LocalizationHelper.LanguageChanged += _delegateChangeLanguage;
        }

        /// <summary>
        /// Constructor only for designer
        /// </summary>
        public CgpTranslateForm()
        {
            if (LocalizationHelper != null)
                ApplyKryptonFormStyle(LocalizationHelper.KryptonColorizationColor);
        }

        /// <summary>
        /// Function is called when language is change
        /// </summary>
        private void ChangeLanguage()
        {
            if (LocalizationHelper == null)
                return;

            LocalizationHelper.TranslateForm(this);
            RefreshNonOverridenComboboxes();
            AfterTranslateForm();
            
        }

        private MethodInfo _cbRefresItemsMethodInfo = null;

        private void RefreshNonOverridenComboboxes()
        {
            if (_cbRefresItemsMethodInfo == null)
                _cbRefresItemsMethodInfo = typeof (ComboBox).GetMethod("RefreshItems",
                    BindingFlags.NonPublic | BindingFlags.Instance);

            WinFormsHelper.TraverseSubcontrols(
                this,
                control =>
                {
                    var cb = control as ComboBox;
                    if (cb == null)
                        return;

                    try
                    {
                        _cbRefresItemsMethodInfo.Invoke(cb, null);
                    }
                    catch
                    {
                        
                    }
                });
            
        }

        /// <summary>
        /// Function is running after translate form
        /// </summary>
        protected virtual void AfterTranslateForm()
        {
 
        }

        private void ApplyKryptonFormStyle(Color colorizationColor)
        {
            bool useColor = colorizationColor == Color.Transparent ? false : true;

            // StateActive
            if (useColor)
                StateActive.Border.Color1 = colorizationColor;
            StateActive.Border.Rounding = 0;
            if (useColor)
                StateActive.Border.Width = 2;
            else
                StateActive.Border.Width = 1;

            if (useColor)
            {
                StateActive.Header.Back.Color1 = colorizationColor;
                StateActive.Header.Back.Color2 = colorizationColor;
            }
            else
            {
                StateActive.Header.Back.Color1 = Color.White;
                StateActive.Header.Back.Color2 = Color.White;
            }

            StateActive.Header.Border.Rounding = 0;
            StateActive.Header.Border.Width = 0;

            if (useColor)
            {
                StateActive.Header.Content.LongText.Color1 = Color.White;
                StateActive.Header.Content.ShortText.Color1 = Color.White;
            }

            // StateCommon
            StateCommon.Border.Rounding = 0;

            // StateInactive
            StateInactive.Border.Rounding = 0;
        }

        /// <summary>
        /// Function for translate form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TanslateForm(object sender, EventArgs e)
        {
            if (LocalizationHelper == null)
                return;

            if (LocalizationHelper.ActualLanguage == null)
            {
                LocalizationHelper.SetLanguage(DEFAULTLANGUAGE);
            }
            else
            {
                ApplyKryptonFormStyle(LocalizationHelper.KryptonColorizationColor);

                LocalizationHelper.TranslateForm(this);
                AfterTranslateForm();
            }
        }

        /// <summary>
        /// Function to unregister delegate to change language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnregisterDelegateChangeLanguage(object sender, FormClosedEventArgs e)
        {
            if (LocalizationHelper == null)
                return;

            LocalizationHelper.LanguageChanged -= _delegateChangeLanguage;
        }

        /// <summary>
        /// Function for create buttons to change language
        /// </summary>
        public void ShowLanguages()
        {
            if (LocalizationHelper == null)
                return;

            string[] languages = LocalizationHelper.AllLanguages;

            if (languages.Length <= 0)
                return;

            int left = Width - (languages.Length * BUTTONSLANGUAGEWIDTH) - ((languages.Length - 1) * BUTTONSLANGUAGESPACE) - BUTTONSLANGUAGERIGHT;

            for (int actual = 0; actual < languages.Length; actual++)
            {
                var bLanguage = new Button
                {
                    Name = "_bLanguage" + languages[actual],
                    Text = languages[actual],
                    Left =
                        left +
                        ((BUTTONSLANGUAGEWIDTH + BUTTONSLANGUAGESPACE)*
                         actual),
                    Top = BUTTONSLANGUAGETOP,
                    Width = BUTTONSLANGUAGEWIDTH,
                    Height = BUTTONSLANGUAGEHEIGHT
                };

                bLanguage.Click += buttonLanguageClick;
                Controls.Add(bLanguage);
            }
        }

        /// <summary>
        /// Function is called when click to button to change language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLanguageClick(object sender, EventArgs e)
        {
            if (LocalizationHelper == null)
                return;

            LocalizationHelper.SetLanguage((sender as Button).Text);
        }

        /// <summary>
        /// Function to GetString from LocalizationHelper
        /// </summary>
        /// <param name="name">Name in resource file</param>
        /// <param name="args">Arguments for string format</param>
        /// <returns>Return string</returns>
        public string GetString(string name, params object[] args)
        {
            return 
                LocalizationHelper == null
                    ? LocalizationHelper.NO_TRANSLATION + StringConstants.SPACE +
                      (name ?? string.Empty)
                    : LocalizationHelper.GetString(name, args);
        }

        public new void Show()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(Show));
                return;
            }
            base.Show();
            BringToFront();
        }

        public new void Hide()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(Hide));
                return;
            }
            base.Hide();
        }
    }
}
