using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Globals;
using Contal.IwQuick.Localization;

namespace Cgp.Components
{
    public partial class MultiRenameDialog : TranslateForm
    {
        private class ShortObject : IShortObject
        {
            public ObjectType ObjectType { get; private set; }
            public string GetSubTypeImageString(object value)
            {
                return null;
            }

            public string Name { get; private set; }
            public object Id { get; private set; }
            public IModifyObject ModifyObject { get; set; }

            public ShortObject(
                ObjectType objectType,
                string name,
                object id,
                IModifyObject modifyObject)
            {
                ObjectType = objectType;
                Name = name;
                Id = id;
                ModifyObject = modifyObject;
            }
        }

        public bool RenameAll { get; private set; }

        public List<IModifyObject> SelectedItems
        {
            get
            {
                return _wpfObjectListView.SelectedItems.Cast<ShortObject>().
                    Select(item => item.ModifyObject).ToList();
            }
        }

        public MultiRenameDialog(
            ImageList imageList,
            List<IModifyObject> items,
            string oldString,
            string newString,
            LocalizationHelper localizationHelper)
            : base(localizationHelper)
        {
            InitializeComponent();

            if (items == null
                || items.Count == 0)
                return;

            _wpfObjectListView.EnableCheckboxes = true;
            _wpfObjectListView.DefaultCheckBoxesState = true;
            _wpfObjectListView.Icons = imageList;

            _wpfObjectListView.Items = items.Select
                (item => new ShortObject
                    (item.GetOrmObjectType,
                        string.Format("{0} -> {1}", item.FullName, item.FullName.Replace(oldString, newString)),
                        item.GetId,
                        item)).
                Cast<IShortObject>().ToList();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bRename_Click(object sender, EventArgs e)
        {
            RenameAll = false;
        }

        private void _bRenameAll_Click(object sender, EventArgs e)
        {
            RenameAll = true;
        }
    }
}
