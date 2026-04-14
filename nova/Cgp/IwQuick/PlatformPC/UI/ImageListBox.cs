using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Contal.IwQuick.UI
{
    public partial class ImageListBox : ListBox
    {
        private ImageList _imageList;
        public ImageList ImageList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }

        public ImageListBox()
        {
            InitializeComponent();

            // Set owner draw mode  
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            ImageListBoxItem item;
            Rectangle bounds = e.Bounds;

            try
            {
                Size imageSize = _imageList.ImageSize; //if fails imagelist probably not initialized
                item = (ImageListBoxItem)Items[e.Index];

                if (item.ImageKey != null && item.ImageKey != string.Empty)
                {
                    item.ImageIndex = _imageList.Images.IndexOfKey(item.ImageKey);
                }

                if (item.ImageIndex != -1)
                {
                    ImageList.Draw(e.Graphics, bounds.Left, bounds.Top, item.ImageIndex);
                    e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(e.ForeColor),
                        bounds.Left + imageSize.Width, bounds.Top);
                }
                else
                {
                    e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(e.ForeColor),
                        bounds.Left, bounds.Top);
                }
            }
            catch
            {
                if (e.Index != -1 && Items.Count > 0)
                {
                    e.Graphics.DrawString(Items[e.Index].ToString(), e.Font,
                        new SolidBrush(e.ForeColor), bounds.Left, bounds.Top);
                }
                else
                {
                    e.Graphics.DrawString(Text, e.Font, new SolidBrush(e.ForeColor),
                        bounds.Left, bounds.Top);
                }
            }

            base.OnDrawItem(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public object SelectedItemObject
        {
            get
            {
                try
                {
                    if (this.SelectedItem == null)
                    {
                        return null;
                    }
                    return (this.SelectedItem as ImageListBoxItem).MyObject;
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


        public List<object> SelectedItemsObjects
        {
            get
            {
                try
                {
                    if (this.SelectedItems == null || !(this.SelectedItems.Count > 0))
                    {
                        return null;
                    }

                    List<object> selectedObjects = new List<object>();
                    foreach (ImageListBoxItem item in this.SelectedItems)
                    {
                        if (item != null)
                        {
                            selectedObjects.Add(item.MyObject);
                        }
                    }
                    return selectedObjects;
                }
                catch
                {
                    return null;
                }
            }
        }

        private void SetSelectedItemByObject(object obj)
        {
            try
            {
                foreach (ImageListBoxItem item in this.Items)
                {
                    if (item.MyObject.Equals(obj))
                    {
                        this.SelectedItem = item;
                        return;
                    }
                }
            }
            catch { }
        }
    }

    public class ImageListBoxItem
    {
        private object _myObject;
        private int _myImageIndex;
        private string _imageKey = string.Empty;

        // properties   
        public object MyObject
        {
            get { return _myObject; }
            set { _myObject = value; }
        }

        public string Text
        {
            get { return ToString(); }
        }

        public int ImageIndex
        {
            get { return _myImageIndex; }
            set { _myImageIndex = value; }
        }

        public string ImageKey
        {
            get { return _imageKey; }
            set { _imageKey = value; }
        }

        //constructor
        public ImageListBoxItem(string text, int index)
        {
            _myObject = text;
            _myImageIndex = index;
        }

        public ImageListBoxItem(object newObject, int index)
        {
            _myObject = newObject;
            _myImageIndex = index;
        }

        public ImageListBoxItem(object newObject, string imageKey)
        {
            _myObject = newObject;
            _imageKey = imageKey;
        }

        public ImageListBoxItem(string text) : this(text, -1) { }
        public ImageListBoxItem() : this("") { }

        public override string ToString()
        {
            return _myObject == null ? "(null)" : _myObject.ToString();
        }
    }
}
