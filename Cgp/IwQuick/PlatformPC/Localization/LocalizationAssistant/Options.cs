using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using Contal.IwQuick.UI;
using JetBrains.Annotations;

namespace Contal.IwQuick.Localization.LocalizationAssistant
{
    public partial class Options : Form
    {
        private readonly LocalizationHelper _localizationAssistent;

        public Options([NotNull] LocalizationHelper localizationAssistent)
        {
            Validator.CheckForNull(localizationAssistent,"localizationAssistent");
            _localizationAssistent = localizationAssistent;
            InitializeComponent();
        }

        private void SetCSharpPaths([NotNull] IEnumerable<string> directories)
        {
// ReSharper disable PossibleMultipleEnumeration
            Validator.CheckForNull(directories,"directories");

            _lbGenerateCSharpPaths.Items.Clear();
            foreach (string path in directories)
            {
                _lbGenerateCSharpPaths.Items.Add(path);
            }
// ReSharper restore PossibleMultipleEnumeration
        }

        private void frmOptions_Shown(object sender, EventArgs e)
        {
            if (null != _localizationAssistent.CSharpPaths)
                SetCSharpPaths(_localizationAssistent.CSharpPaths);
        }

        private void BrowseCSharp(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (_lbGenerateCSharpPaths.SelectedIndex >= 0)
            {
                try
                {
                    string suggestedPath = _lbGenerateCSharpPaths.Items[_lbGenerateCSharpPaths.SelectedIndex].ToString();
                    if (Directory.Exists(suggestedPath))
                    {
                        dialog.SelectedPath = suggestedPath;
                    }
                }
                catch { }
            }
            if (DialogResult.OK == dialog.ShowDialog())
            {
                foreach (string path in _lbGenerateCSharpPaths.Items)
                {
                    if (path == dialog.SelectedPath)
                    {
                        Dialog.Warning("Specified path \"" + path + "\"is already in the list");
                        return;
                    }
                }
                _lbGenerateCSharpPaths.Items.Add(dialog.SelectedPath);
            }
                
        }

        private IList<string> GetCSharpPaths()
        {
            List<string> paths = new List<string>();
            foreach (string strPath in _lbGenerateCSharpPaths.Items)
            {
                paths.Add(strPath);

            }
            return paths;
        }

        private void m_btMinus_Click(object sender, EventArgs e)
        {
            if (_lbGenerateCSharpPaths.SelectedIndex < 0)
            {
                return;
            }
            _lbGenerateCSharpPaths.Items.RemoveAt(_lbGenerateCSharpPaths.SelectedIndex);
        }

        private void _btOkPaths_Click(object sender, EventArgs e)
        {
            try
            {
                _localizationAssistent.CSharpPaths = GetCSharpPaths();
            }
            catch (Exception aError)
            {
                Dialog.Error(aError.Message);
            }
            DialogResult = DialogResult.OK;
        }
    }
}
