using System.Drawing;
using System.Windows.Forms;
using Contal.IwQuick.Localization;

namespace Contal.IwQuick.UI
{
    public partial class ImageComboBox : ComboBox
    {
        private ImageList _imageList;
        private LocalizationHelper _localizationhelper;
        private string _translationPrefix = "ObjectType_";

        public string TranslationPrefix
        {
            get { return _translationPrefix; }
            set { _translationPrefix = value; }
        }

        public LocalizationHelper Localizationhelper
        {
            get { return _localizationhelper; }
            set { _localizationhelper = value; }
        }

        public ImageList ImageList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }

        public ImageComboBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs ea)
        {
            ea.DrawBackground();
            ea.DrawFocusRectangle();

            Rectangle bounds = ea.Bounds;

            try
            {
                if (ea.Index < 0)
                {
                    ea.Graphics.DrawString(Text, ea.Font, new SolidBrush(ea.ForeColor), bounds.Left, bounds.Top);
                }
                else
                {
                    Size imageSize = _imageList.ImageSize; //if fails imagelist probably not initialized
                    var item = (ImageComboBoxItem) Items[ea.Index];

                    string text = TranslateItem(item);

                    if (!string.IsNullOrEmpty(item.ImageKey))
                    {
                        item.ImageIndex = _imageList.Images.IndexOfKey(item.ImageKey);
                    }

                    if (item.ImageIndex != -1)
                    {
                        _imageList.Draw(ea.Graphics, bounds.Left, bounds.Top,
                            item.ImageIndex);
                        ea.Graphics.DrawString(text, ea.Font, new
                            SolidBrush(ea.ForeColor), bounds.Left + imageSize.Width, bounds.Top);
                    }
                    else
                    {
                        ea.Graphics.DrawString(text, ea.Font, new
                            SolidBrush(ea.ForeColor), bounds.Left, bounds.Top);
                    }
                }
            }
            catch
            {
                ea.Graphics.DrawString(
                    ea.Index != -1 ? Items[ea.Index].ToString() : Text,
                    ea.Font, 
                    new SolidBrush(ea.ForeColor),
                    bounds.Left, 
                    bounds.Top);
            }

            base.OnDrawItem(ea);
        }

        private string TranslateItem(ImageComboBoxItem item)
        {
            string text = item.Text;
            if (!string.IsNullOrEmpty(item.ObjectType))
            {
                text = (_localizationhelper != null ? _localizationhelper.GetString(_translationPrefix + item.ObjectType) : item.Text);
            }

            return text;
        }

        public object SelectedItemObject
        {
            get
            {
                try
                {
                    var imageComboBoxItem = SelectedItem as ImageComboBoxItem;

#if PC35
                    if (imageComboBoxItem == null)
                        return null;
                    return imageComboBoxItem.MyObject;
#else
                    return imageComboBoxItem?.MyObject;
#endif
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetSelectedItemByObject(value);
            }
        }

        public string SelectedItemObjectType
        {
            get
            {
                try
                {
                    var imageComboBoxItem = SelectedItem as ImageComboBoxItem;

#if PC35
                    if (imageComboBoxItem == null)
                        return null;
                    return imageComboBoxItem.ObjectType;
#else
                    return imageComboBoxItem?.ObjectType;
#endif


                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetSelectedItemByObject(value);
            }
        }

        private void SetSelectedItemByObject(object obj)
        {
            try
            {
                foreach (object o in Items)
                {
                    var imageComboBoxItem = o as ImageComboBoxItem;
                    if (imageComboBoxItem != null)
                    {
                        if (imageComboBoxItem.MyObject.Equals(obj))
                        {
                            SelectedItem = imageComboBoxItem;
                            return;
                        }
                    }
                }
            }
            catch { }
        }
    }

    public class ImageComboBoxItem
    {
        private object _myObject;

        public string ObjectType { get; set; }

        public object MyObject
        {
            get { return _myObject; }
            set { _myObject = value; }
        }

        public int ImageIndex { get; set; }

        

#if PC35
        private string _imageKey = string.Empty;
        public string ImageKey
        {
            get { return _imageKey; }
            set { _imageKey = value; }
        }

#else
        public string ImageKey { get; set; } = string.Empty;
#endif

#if PC35
        public string Text
        {
            get { return _myObject.ToString(); }
        }
#else
        public string Text => _myObject.ToString();
#endif

        public ImageComboBoxItem()
            : this("")
        {
        }

        public ImageComboBoxItem(string text)
            : this(text, -1)
        {
        }

        public ImageComboBoxItem(object obj, int imageIndex)
            : this(obj, imageIndex, null, obj.ToString())
        {
        }

        public ImageComboBoxItem(object obj, string imageKey)
            : this(obj, -1, imageKey, obj.ToString())
        {
        }

        public ImageComboBoxItem(object obj, int imageIndex, string imageKey, string objectType)
        {
            _myObject = obj;
            ObjectType = objectType;
            ImageIndex = imageIndex;
            if (imageKey != null)
                ImageKey = imageKey;
        }

        public override string ToString()
        {
            return _myObject.ToString();
        }
    }
}

