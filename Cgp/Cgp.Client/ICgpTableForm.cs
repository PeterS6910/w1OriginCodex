namespace Contal.Cgp.Client
{
    public interface ICgpTableForm
    {
        bool HasAccessView();

        bool IsMyEditForm(ICgpEditForm editForm);
    }
}

