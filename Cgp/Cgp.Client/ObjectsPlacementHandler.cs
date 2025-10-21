using System.Collections.Generic;
using System.Windows.Forms;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.RemotingCommon;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public static class ObjectsPlacementHandler
    {
        private const string STRUCTURE_SITE_IMAGE_KEY = "structureSiteImageKey";
        private const string SUB_TREE_REFERENCING_IMAGE_KEY = "subTreeReferencingImageKey";
        private const string USER_FOLDER_IMAGE_KEY = "userFolderImageKey";

        private static ImageList _imageList;

        static ObjectsPlacementHandler()
        {
            _imageList = new ImageList();
            _imageList.Images.Add(STRUCTURE_SITE_IMAGE_KEY, ResourceGlobal.FolderStructure16);
            _imageList.Images.Add(SUB_TREE_REFERENCING_IMAGE_KEY, ResourceGlobal.SubTreeReferencing);
            _imageList.Images.Add(USER_FOLDER_IMAGE_KEY, ResourceGlobal.Folder_expand);
        }

        public static void AddImageList(ImageListBox ilbUsersFolders)
        {
            ilbUsersFolders.ImageList = _imageList;
        }

        public static void UserFolders_Enter(ImageListBox ilbUsersFolders, AOrmObject editingObject)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            ilbUsersFolders.Items.Clear();

            var objectPlacments =
                CgpClient.Singleton.MainServerProvider.StructuredSubSites.GetObjectPlacements(
                    editingObject.GetObjectType(),
                    editingObject.GetIdString(),
                    @"\",
                    CgpClient.Singleton.LocalizationHelper.GetString("SelectStructuredSubSiteForm_RootNode"),
                    false);

            if (objectPlacments != null)
            {
                foreach (var objectPlacement in objectPlacments)
                {
                    ilbUsersFolders.Items.Add(new ImageListBoxItem(
                        objectPlacement,
                        objectPlacement.Parent != null
                            ? USER_FOLDER_IMAGE_KEY
                            : objectPlacement.IsReference
                                ? SUB_TREE_REFERENCING_IMAGE_KEY
                                : STRUCTURE_SITE_IMAGE_KEY));
                }
            }
        }

        public static void UserFolders_MouseDoubleClick(ImageListBox ilbUsersFolders, AOrmObject editingObject)
        {
            if (ilbUsersFolders.Items != null && ilbUsersFolders.Items.Count > 0)
            {
                var imageListBoxItem = ilbUsersFolders.SelectedItem as ImageListBoxItem;
                if (imageListBoxItem == null)
                    return;

                var objectPlacement = imageListBoxItem.MyObject as ObjectPlacement;
                if (objectPlacement == null)
                    return;

                StructuredSiteForm.Singleton.OpenFormAndSelectObject(
                    objectPlacement,
                    new IdAndObjectType(editingObject.GetId(), editingObject.GetObjectType()));
            }
            else
            {
                StructuredSiteForm.Singleton.Show();
            }
        }

        public static bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
