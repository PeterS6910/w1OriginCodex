using System;

namespace Contal.Cgp.Client
{
    public interface ICgpEditForm
    {
        object GetEditingObject();
        ShowOptionsEditForm ShowOption { get; }
        bool? AllowEdit { get; set; }
        bool ValueChanged { get;}
        void DockWindow();
        void UndockWindow();
        void EditUnregisterEvents();
        void ShowAndRunSetValues();
    }

    public interface IExtendedCgpEditForm
    {
        event Action<object> EditingObjectChanged;
        void ExtendedEditTextChanger(object sender, EventArgs e);
        void ExtendedEditTextChangerOnlyInDatabase(object sender, EventArgs e);
    }
}
