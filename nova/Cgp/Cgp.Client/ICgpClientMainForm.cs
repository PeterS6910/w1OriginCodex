using System.Drawing;
using System.Windows.Forms;

using Contal.IwQuick.Localization;

namespace Contal.Cgp.Client
{
    public interface ICgpClientMainForm
    {
        void StartProgress(Form form);
        void StopProgress(Form form);
        void AddToRecentList(object obj, Form form, bool editEnable);
        void DeleteFromRecentList(object obj);

        void AddToOpenWindows(Form form);
        void AddToOpenWindows(Form form, string objectName);
        void AddToOpenWindows(Form form, string objectName, bool localisedObjectName);
        bool SetActOpenWindow(Form form);
        void RemoveFromOpenWindows(Form form);
        bool MainIsConnectionLost(bool showDialog);
        LocalizationHelper GetLocalizationHelper();
        bool HidingWindows { get; }

        void AddToUndockedForms(Form form);
        void RemoveFormUndockedForms(Form form);

        Color GetDragDropTextColor { get; }
        Color GetDragDropBackgroundColor { get; }
        Color GetReferenceTextColor { get; }
        Color GetReferenceBackgroundColor { get; }

        void RegisterColorChanged(Contal.IwQuick.DVoid2Void eventColorChanged);
        void UnregisterColorChanged(Contal.IwQuick.DVoid2Void eventColorChanged);
    }
}
