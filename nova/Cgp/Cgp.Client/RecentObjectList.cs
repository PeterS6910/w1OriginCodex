using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;


namespace Contal.Cgp.Client
{
    internal class RecentObjectList
    {
        /// <summary>
        /// Helper class to collect information about elements for Recent Object List
        /// </summary>
        public class RecentObject
        {
            private object _object;
            private Form _form;
            private Icon _icon;
            private bool _editEnable = true;

            public bool CompareRecentObjectGuid(object idObj)
            {
                if (_object is AOrmObject)
                {
                    AOrmObject ormObj = (AOrmObject)_object;
                    if (ormObj != null)
                    {
                        return (ormObj.GetId().ToString() == idObj.ToString());
                    }
                }
                return false;
            }

            public Icon GetIcon
            {
                get { return _icon; }
            }

            public object GetObject
            {
                get
                {
                    return _object;
                }
            }

            public bool OpenEditForm()
            {
                try
                {
                    if (_form != null)
                    {
                        var types = new Type[2];
                        types[0] = _object.GetType();
                        types[1] = typeof(bool);
                        MethodInfo methodInfo = _form.GetType().GetMethod("OpenEditForm", types);
                        if (methodInfo != null)
                        {
                            var parameters = new object[2]
                            {
                                _object, 
                                _editEnable
                            };

                            methodInfo.Invoke(_form, parameters);
                            return true;
                        }
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
            }

            public RecentObject(object refObj, Form form, Icon icon, bool editEnable)
            {
                _editEnable = editEnable;
                _form = form;
                _object = refObj;
                _icon = icon;
            }

            /// <summary>
            /// return created ListViewItem for RecentObject
            /// </summary>
            /// <param name="imageIndex"></param>
            /// <returns></returns>
            public ListViewItem Get(int imageIndex)
            {
                return new ListViewItem(ToString(), imageIndex);
            }

            public bool CompareTo(object obj)
            {
                AOrmObject aOrmObject = _object as AOrmObject;
                return aOrmObject != null ? aOrmObject.Compare(obj) : _object == obj;
            }

            public override string ToString()
            {
                if (_object is AOrmObject)
                {
                    switch ((_object as AOrmObject).GetObjectType())
                    {
                        case ObjectType.Calendar:
                            if ((_object as AOrmObject).ToString() == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                            {
                                return CgpClient.Singleton.LocalizationHelper.GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                            }
                            break;
                        case ObjectType.DayType:
                            switch ((_object as AOrmObject).ToString())
                            {
                                case DayType.IMPLICIT_DAY_TYPE_HOLIDAY:
                                    return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
                                case DayType.IMPLICIT_DAY_TYPE_VACATION:
                                    return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
                            }
                            break;
                        case ObjectType.DailyPlan:
                            switch ((_object as AOrmObject).ToString())
                            {
                                case DailyPlan.IMPLICIT_DP_ALWAYS_ON:
                                    return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_ON);
                                case DailyPlan.IMPLICIT_DP_ALWAYS_OFF:
                                    return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_OFF);
                                case DailyPlan.IMPLICIT_DP_DAYLIGHT_ON:
                                    return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_DAYLIGHT_ON);
                                case DailyPlan.IMPLICIT_DP_NIGHT_ON:
                                    return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_NIGHT_ON);
                            }
                            break;
                    }
                }
                return _object.ToString();
            }

            public bool OpenEditSpecial()
            {
                if (_object is AOrmObject)
                {
                    switch ((_object as AOrmObject).GetObjectType())
                    {
                        case ObjectType.CardTemplate:
                            CardTemplatesForm.Singleton.EditSpecific(_object as CardTemplate);
                            return true;
                        default:
                            return false;
                    }
                }
                return false;
            }
        }

        List<RecentObject> _recObjList = new List<RecentObject>();
        ListView _listView;
        int _maxObjectsCount = 20;
        internal ImageList _imagesSmall;

        public Image CloseImage;
        public Image CloseImageOn;

        public int MaxObjectsCount
        {
            get { return _maxObjectsCount; }
            set { _maxObjectsCount = value; }
        }

        public RecentObjectList(ListView listView)
        {
            _listView = listView;
            CloseImage = ResourceGlobal.CloseImage16;
            CloseImageOn = ResourceGlobal.CloseImage16_on;

            SetListView();
        }

        #region ListViewSettings

        /// <summary>
        /// change setting for listView
        /// </summary>
        private void SetListView()
        {
            if (_listView.InvokeRequired)
            {
                _listView.BeginInvoke(new DVoid2Void(SetListView));
            }
            else
            {
                _listView.Columns.Add("info", _listView.Width - 28);
                _listView.HeaderStyle = ColumnHeaderStyle.None;
                _listView.View = View.Details;
                _listView.MultiSelect = false;
                //_listView.AllowDrop = true;
                _listView.MouseDown += ListViewMouseDown;
                _listView.DragOver += ListViewDragOver;
                _listView.Resize += ListViewResize;
                _listView.MouseUp += _listView_MouseUp;
            }
        }

        private bool _isDoubleClick;

        void _listView_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDoubleClick)
            {
                _isDoubleClick = false;
                RecentObject ro = _recObjList[_listView.FocusedItem.Index];

                if (!ro.OpenEditSpecial() && !ro.OpenEditForm())
                {
                    _recObjList.RemoveAt(_listView.FocusedItem.Index);
                    RefreshListView();
                    Dialog.Error("Connection Lost.");
                }
            }
        }

        void ListViewResize(object sender, EventArgs e)
        {
            if (_listView.Columns.Count > 0)
            {
                _listView.Columns[0].Width = _listView.Width - 28;
                //_listView.Scrollable = false;
                //_listView.Scrollable = true;
            }
        }

        void ListViewDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        //void ListViewDragDrop(object sender, DragEventArgs e)
        //{
        //    //if Object.ReferenceEquals(droppedThing, thingWhereItWasDropped)
        //    //{
        //    //    return;
        //    //}
        //    //else
        //    {
        //        string[] output = e.Data.GetFormats();
        //        if (output == null) return;
        //        //Add((object)e.Data.GetData(typeof(System.MarshalByRefObject)));
        //        Add((object)e.Data.GetData(output[0]));
        //        //_listView.AllowDrop = true;
        //    }
        //}

        void ListViewMouseDown(object sender, MouseEventArgs e)
        {
            if (_listView == null) 
                return;

            if (_recObjList == null || _recObjList.Count == 0) 
                return;

            if (e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                _isDoubleClick = true;
            }
            else
            {
                if (_listView.Items.Count == 0) return;
                if (_recObjList == null) return;
                if (_recObjList.Count == 0) return;
                if (_listView.HitTest(e.X, e.Y).Item == null) return;
                int index = _listView.HitTest(e.X, e.Y).Item.Index;
                //_listView.AllowDrop = false;
                object send = _recObjList[index].GetObject;
                if (send == null) return;
                _listView.Parent.DoDragDrop(send, DragDropEffects.All);
            }
        }
        #endregion

        /// <summary>
        /// add recent object to _recObj, doesn't call the refresh on listView
        /// </summary>
        /// <param name="recentObj"></param>
        /// <returns></returns>
        private void AddRecentObject(RecentObject recentObj)
        {
            try
            {
                int position = IsInList(recentObj.GetObject);

                if (position >= 0)
                    _recObjList.RemoveAt(position);

                _recObjList.Insert(0, recentObj);

                //remove the last object if the count of recent objects is more then allowed max
                if (_maxObjectsCount < _recObjList.Count)
                {
                    _recObjList.RemoveAt(_recObjList.Count - 1);
                }

                RefreshListView();
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private int IsInList(object obj)
        {
            if (obj == null || _recObjList == null)
                return -1;

            for (int i = 0; i < _recObjList.Count; i++)
            {
                if (_recObjList[i].CompareTo(obj))
                {
                    return i;
                }
            }

            return -1;
        }

        private RecentObject CreateCgpRecentObject(object inObject, bool editEnabled)
        {
            Form form = null;
            Icon icon = null;
            bool add = false;

            if (inObject.GetType() == typeof(Person))
            {
                form = PersonsForm.Singleton;
                icon = ResourceGlobal.IconPersonsNew16;
                add = true;
            }
            else if (inObject.GetType() == typeof(LoginGroup))
            {
                form = LoginGroupsForm.Singleton;
                icon = ResourceGlobal.IconLoginGroup16;
                add = true;
            }
            else if (inObject.GetType() == typeof(Login))
            {
                form = LoginsForm.Singleton;
                icon = ResourceGlobal.IconLogins16;
                add = true;
            }
            else if (inObject.GetType() == typeof(PresentationGroup))
            {
                form = PresentationGroupsForm.Singleton;
                icon = ResourceGlobal.IconNewPresentationGroup16;
                add = true;
            }
            else if (inObject.GetType() == typeof(PresentationFormatter))
            {
                form = PresentationFormattersForm.Singleton;
                icon = ResourceGlobal.IconFormater16;
                add = true;
            }
            else if (inObject.GetType() == typeof(CisNG))
            {
                form = CisNGsForm.Singleton;
                icon = ResourceGlobal.IconCisNG128;
                add = true;
            }
            else if (inObject.GetType() == typeof(CisNGGroup))
            {
                form = CisNGGroupsForm.Singleton;
                icon = ResourceGlobal.IconCisNGGrpup128;
                add = true;
            }
            else if (inObject.GetType() == typeof(CardSystem))
            {
                form = CardSystemsForm.Singleton;
                icon = ResourceGlobal.IconCardSystemNew16;
                add = true;
            }
            else if (inObject.GetType() == typeof(Card))
            {
                form = CardsForm.Singleton;
                icon = ResourceGlobal.IconCardsNew16;
                add = true;
            }
            else if (inObject.GetType() == typeof(Car))
            {
                form = CarsForm.Singleton;
                icon = ResourceGlobal.Car16;
                add = true;
            }
            else if (inObject.GetType() == typeof(DailyPlan))
            {
                form = DailyPlansForm.Singleton;
                icon = ResourceGlobal.IconDailyPLan16;
                add = true;
            }
            else if (inObject.GetType() == typeof(TimeZone))
            {
                form = TimeZonesForm.Singleton;
                icon = ResourceGlobal.IconTimeZone16;
                add = true;
            }
            else if (inObject.GetType() == typeof(DayType))
            {
                form = DayTypesForm.Singleton;
                icon = ResourceGlobal.IconDayType16;
                add = true;
            }
            else if (inObject.GetType() == typeof(Calendar))
            {
                form = CalendarsForm.Singleton;
                icon = ResourceGlobal.IconCalendar16;
                add = true;
            }
            else if (inObject.GetType() == typeof(CardTemplate))
            {
                form = CardTemplatesForm.Singleton;
                icon = ResourceGlobal.IconCardTemplate16;
                add = true;
            }
            else if (inObject.GetType() == typeof(GlobalAlarmInstruction))
            {
                form = GlobalAlarmInstructionsForm.Singleton;
                icon = ResourceGlobal.IconAlarmInstructions16;
                add = true;
            }

            if (add)
                return new RecentObject(inObject, form, icon, editEnabled);

            return null;
        }

        /// <summary>
        /// Add or update object in RecentObjectList 
        /// </summary>
        /// <param name="inObject">object</param>
        /// <param name="editEnabled">is edit enabled</param>
        public void Add(object inObject, bool editEnabled)
        {
            try
            {
                if (inObject == null)
                    return;

                var recentObject = CreateCgpRecentObject(inObject, editEnabled);
                if (recentObject == null)
                    return;

                AddRecentObject(recentObject);
            }
            catch(Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        public void Add(object inObject, Form tableForm, bool editEnable)
        {
            try
            {
                if (inObject == null)
                    return;

                var ro = new RecentObject(inObject, tableForm, tableForm.Icon, editEnable);
                AddRecentObject(ro);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        /// <summary>
        /// Delete object form RecentList
        /// </summary>
        /// <param name="inObject">objcet to delete</param>
        public void Delete(object inObject)
        {
            if (inObject == null) return;

            int position = IsInList(inObject);
            if (position != -1)
                _recObjList.RemoveAt(position);
            
            RefreshListView();
        }

        public void ClearRecentList()
        {
            _recObjList.Clear();
            RefreshListView();
        }

        /// <summary>
        /// Show recent objects in the listView
        /// </summary>
        public void RefreshListView()
        {
            if (_listView.InvokeRequired)
            {
                _listView.BeginInvoke(new DVoid2Void(RefreshListView));
            }
            else
            {
                _listView.Items.Clear();
                if (_recObjList == null) return;
                if (_recObjList.Count == 0) return;
                _imagesSmall = new ImageList();
                _imagesSmall.ColorDepth = ColorDepth.Depth32Bit;
                //for (int i = _recObj.Length - 1; i >= 0 ; i--)
                for (int i = 0; i < _recObjList.Count; i++)
                {
                    _imagesSmall.Images.Add(_recObjList[i].GetIcon);
                    _listView.Items.Add(_recObjList[i].Get(i));
                }
                _listView.SmallImageList = _imagesSmall;
            }
        }

        public void RemoveObjectFromRecentListById(object idObj)
        {
            try
            {
                if (_recObjList == null || _recObjList.Count == 0)
                    return;
                foreach (RecentObject recentObj in _recObjList)
                {
                    if (recentObj.CompareRecentObjectGuid(idObj))
                    {
                        _recObjList.Remove(recentObj);
                        RefreshListView();
                        return;
                    }
                }
            }
            catch { }
        }
    }
}
