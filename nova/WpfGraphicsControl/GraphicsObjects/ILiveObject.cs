using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public interface ILiveObject
    {
        Guid GetObjectGuid();
        void SetObjectGuid(Guid id);
        void SetCategories(List<Category> categories);
        ICollection<Category> GetCategories();
        void ChangeState(byte state);
        object GetState();
        string GetObjectTypeName();
        bool isEnable();
        void Enable(bool enable);
        ObjectType GetObjectType();
        void SetLabel(Text label);
        Text GetLabel();
    }
}
