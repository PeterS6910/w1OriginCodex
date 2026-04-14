using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;

namespace Contal.Cgp.Client.PluginSupport
{
    public interface ICgpVisualPlugin : ICgpClientPlugin
    {
        void InitializeUI(Form invokingParent);
        IPluginMainForm MainForm { get; }
        IEnumerable<IPluginMainForm> SubForms { get; }
        Form InvokingParent { get; }
        bool LoadPluginControlToForm(Object obj, Control control, IExtendedCgpEditForm cgpEditForm, bool allowEdit);
        string GetTranslateString(string name, params object[] args);
        string GetTranslateNameForm(Form form);
        void AddToOnClose(Action<ICgpClientPlugin, Form> close);
        void AddToOnShow(Action<ICgpClientPlugin, Form> show);
        void AddToOnEnter(Action<ICgpClientPlugin, Form> enter);

        /// <summary>
        /// Loads and show opened windows for specific user
        /// </summary>
        /// <param name="selectedForm">Selected form</param>
        void LoadAndShowOpenedForms(out Form selectedForm);

        ImageList GetPluginObjectsImages();
        System.Drawing.Icon GetIconForObjectType(ObjectType objectType);
        Dictionary<string, bool> GetDefaultCloneFilterForObjectType(ObjectType objectType);
        bool LoadPluginControlToForm(Object obj, Dictionary<string, bool> filter, Control control, IExtendedCgpEditForm cgpEditForm, bool allowEdit);
        IPluginMainForm GetEditPluginForm();
        string GetTranslateTableObjectTypeName(string typeName);
        string GetLocalizedObjectName(AOrmObject aOrmObject);
    }
}