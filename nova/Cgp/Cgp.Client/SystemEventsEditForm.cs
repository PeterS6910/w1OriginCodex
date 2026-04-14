using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.IwQuick.Localization;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Client
{
    public partial class SystemEventsEditForm :
#if DESIGNER
        Form
#else
 ACgpFullscreenForm
#endif
    {
        //1 DATABASE_DISCONNECTED
        //2 CLIENT_CONNECTED
        //3 CLIENT_DISCONNECTED
        Cgp.Server.Beans.SystemEvent _systemEvent;
        ICollection<PresentationGroup> _presentationGroups = null;

        private string[] _eventsNames = { SystemEvent.DATABASE_DISCONNECTED, SystemEvent.CLIENT_CONNECTED, SystemEvent.CLIENT_DISCONNECTED };
        private ListBox[] _allListBox;

        public SystemEventsEditForm()
        {
            InitializeComponent();
            RegisterToMain();

            _allListBox = new ListBox[_eventsNames.Length];
            CreateComponents();
            for (int i = 0; i < _eventsNames.Length; i++)
            {
                ShowPresentationGroups(i);
            }

            FormOnEnter += new Action<Form>(OnEnter);
        }

        public override void CallEscape()
        {
            Close();
        }

        private void OnEnter(Form form)
        {
            if (!HasAccessUpdate())
                DisabledForm();
            else
                EnabledForm();
        }

        protected virtual void DisabledForm()
        {
            if (Controls != null)
            {
                foreach (Control control in Controls)
                {
                    DisabledControls(control);
                }
            }
        }

        private void DisabledControls(Control control)
        {
            if (control.Controls == null || control.Controls.Count == 0)
                DisabledControl(control);
            else
                foreach (Control actControl in control.Controls)
                    DisabledControls(actControl);
        }

        private void DisabledControl(Control control)
        {
            if (!(control is Label))
                control.Enabled = false;
        }

        protected virtual void EnabledForm()
        {
            if (Controls != null)
            {
                foreach (Control control in Controls)
                {
                    EnabledControls(control);
                }
            }
        }

        private void EnabledControls(Control control)
        {
            if (control.Controls == null || control.Controls.Count == 0)
                EnabledControl(control);
            else
                foreach (Control actControl in control.Controls)
                    EnabledControls(actControl);
        }

        private void EnabledControl(Control control)
        {
            if (!(control is Label))
                control.Enabled = true;
        }

        private static volatile SystemEventsEditForm _singleton = null;
        private static object _syncRoot = new object();

        public static SystemEventsEditForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new SystemEventsEditForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        protected override bool VerifySources()
        {
            return null != CgpClient.Singleton.MainServerProvider;
        }

        private bool SetSystemEvent(int numberOfEvent)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return false;
            Exception error;
            _systemEvent = CgpClient.Singleton.MainServerProvider.SystemEvents.GetByName(_eventsNames[numberOfEvent], out error);
            if (_systemEvent == null)
            {
                _systemEvent = new SystemEvent();
                _systemEvent.Name = _eventsNames[numberOfEvent];
                if (!CgpClient.Singleton.MainServerProvider.SystemEvents.Insert(ref _systemEvent, out error))
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInsertSystemEvent"));
                    return false;
                }
            }
            _presentationGroups = CgpClient.Singleton.MainServerProvider.SystemEvents.GetSystemEventOwnPresentationGroup(_systemEvent, out error);
            return true;
        }

        private void CreateComponents()
        {
            int tab = 0;
            int horizontalPosition = 0;
            int verticalPosition = 0;

            for (int i = 0; i < _eventsNames.Length; i++)
            {
                if (i > 2)
                {
                    verticalPosition = 200;
                    horizontalPosition = 0;
                }

                Label label = new Label();
                Button bAdd = new Button();
                Button bDelete = new Button();
                ListBox listbox = new ListBox();

                GroupBox gbEvent = new GroupBox();
                gbEvent.SuspendLayout();
                gbEvent.Controls.Add(bDelete);
                gbEvent.Controls.Add(bAdd);
                gbEvent.Controls.Add(label);
                gbEvent.Controls.Add(listbox);
                gbEvent.Location = new System.Drawing.Point(12 + (horizontalPosition * 222), 12 + verticalPosition);
                //gbEvent.Name = "_gbEvent" + i.ToString();
                gbEvent.Name = "_gbEvent"+_eventsNames[i].Replace(' ','_');
                gbEvent.Size = new System.Drawing.Size(218, 190);
                gbEvent.TabIndex = tab++;
                gbEvent.TabStop = false;
                gbEvent.Text = _eventsNames[i];

                label.AutoSize = true;
                label.Location = new System.Drawing.Point(6, 16);
                label.Name = "_lPresentationGroups" + i.ToString();
                label.Size = new System.Drawing.Size(101, 13);
                label.TabIndex = tab++;
                label.Text = GetString("PresentationGroupsFormPresentationGroupsForm");

                listbox.AccessibleName = i.ToString();
                listbox.AllowDrop = true;
                listbox.FormattingEnabled = true;
                listbox.Location = new System.Drawing.Point(9, 32);
                listbox.Name = "_lbPgEvent" + i.ToString();
                listbox.Size = new System.Drawing.Size(202, 120);
                listbox.TabIndex = tab++;
                listbox.DragOver += new System.Windows.Forms.DragEventHandler(ListboxDragOver);
                listbox.DragDrop += new System.Windows.Forms.DragEventHandler(ListboxDragDrop);
                listbox.DoubleClick += new EventHandler(listbox_DoubleClick);
                listbox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
                listbox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
                listbox.Cursor = Cursors.Hand;

                bAdd.AccessibleName = i.ToString();
                bAdd.Location = new System.Drawing.Point(55, 158);
                bAdd.Name = "_bAdd" + i.ToString();
                bAdd.Size = new System.Drawing.Size(75, 23);
                bAdd.TabIndex = tab++;
                bAdd.Text = GetString("General_bAdd");
                bAdd.UseVisualStyleBackColor = true;
                bAdd.Click += new System.EventHandler(AddClick);

                bDelete.AccessibleName = i.ToString();
                bDelete.Location = new System.Drawing.Point(136, 158);
                bDelete.Name = "_bDelete" + i.ToString();
                bDelete.Size = new System.Drawing.Size(75, 23);
                bDelete.TabIndex = tab++;
                bDelete.Text = GetString("General_bDelete");
                bDelete.UseVisualStyleBackColor = true;
                bDelete.Click += new System.EventHandler(DeleteClick);

                _allListBox[i] = listbox;
                this.Controls.Add(gbEvent);
                gbEvent.ResumeLayout(false);
                gbEvent.PerformLayout();

                horizontalPosition++;
            }
        }

        void listbox_DoubleClick(object sender, EventArgs e)
        {
            ConnectionLost();
            int numberOfEvent;
            if (!Int32.TryParse((sender as ListBox).AccessibleName, out numberOfEvent))
            {
                return;
            }

            PresentationGroup presentationGroup = _allListBox[numberOfEvent].SelectedItem as PresentationGroup;
            if (presentationGroup != null)
            {
                PresentationGroupsForm.Singleton.OpenEditForm(presentationGroup);
            }
        }


        private void AddClick(object sender, EventArgs e)
        {
            int numberOfEvent;
            if (!Int32.TryParse((sender as Button).AccessibleName, out numberOfEvent))
            {
                return;
            }

            SetSystemEvent(numberOfEvent);
            if (AddPresentationGroups())
            {
                ShowPresentationGroups(numberOfEvent);
            }
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            int numberOfEvent;
            if (!Int32.TryParse((sender as Button).AccessibleName, out numberOfEvent))
            {
                return;
            }

            SetSystemEvent(numberOfEvent);

            PresentationGroup presentationGroup = _allListBox[numberOfEvent].SelectedItem as PresentationGroup;
            if (presentationGroup != null)
            {
                if (DeletePresentationGroup(presentationGroup))
                {
                    ShowPresentationGroups(numberOfEvent);
                }
            }
        }

        private bool ConnectionLost()
        {
            bool connectionLost;
            connectionLost = CgpClient.Singleton.IsConnectionLost(true);
            return connectionLost;
        }

        private bool DeletePresentationGroup(PresentationGroup presentationGroup)
        {
            if (ConnectionLost()) return false;

            try
            {
                if (presentationGroup != null)
                {
                    if (Contal.IwQuick.UI.Dialog.Question(GetString("ConfirmRemovePresentationGroup")))
                    {
                        if (CgpClient.Singleton.MainServerProvider.SystemEvents.RemovePresentationGroup(_systemEvent, presentationGroup))
                            return true;
                        else
                        {
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorRemovePresentationGroup"));
                            return false;
                        }
                    }
                }
                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorRemovePresentationGroup"));
                return false;
            }
        }

        private bool AddPresentationGroups()
        {
            if (ConnectionLost()) return false;
            try
            {
                List<IModifyObject> listObjects = new List<IModifyObject>();

                Exception error;
                IList<IModifyObject> listPGs = CgpClient.Singleton.MainServerProvider.PresentationGroups.ListModifyObjects(out error);
                if (error != null)
                    throw error;

                listObjects.AddRange(listPGs);
                ListboxFormAdd formAdd = new ListboxFormAdd(listObjects, GetString("SystemEventsEditFormAddPgFormText"));

                ListOfObjects outPGs;
                formAdd.ShowDialogMultiSelect(out outPGs);

                if (outPGs != null)
                {
                    if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionAddPresentationGroups")))
                    {
                        for (int i = 0; i < outPGs.Count; i++)
                        {
                            var pgModifyObj = (IModifyObject)outPGs.Objects[i] ;

                            PresentationGroup presentationGroup = CgpClient.Singleton.MainServerProvider.PresentationGroups.GetObjectById(pgModifyObj.GetId);
                            if (CgpClient.Singleton.MainServerProvider.SystemEvents.AddPresentationGroup(_systemEvent, presentationGroup))
                            {
                                CgpClientMainForm.Singleton.AddToRecentList(presentationGroup);
                            }
                            else
                            {
                                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAddPresentationGroup"));
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAddPresentationGroup"));
                return false;
            }
        }

        private void ShowPresentationGroups(int numberOfEvent)
        {
            SetSystemEvent(numberOfEvent);

            if (_allListBox[numberOfEvent] == null) return;
            _allListBox[numberOfEvent].Items.Clear();
            if (_presentationGroups == null || _presentationGroups.Count == 0) return;
            foreach (PresentationGroup presentationGroup in _presentationGroups)
                _allListBox[numberOfEvent].Items.Add(presentationGroup);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);

            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.SystemEvents != null)
                CgpClient.Singleton.MainServerProvider.SystemEvents.RefreshSystemEvent();
        }

        private void SystemEventsEditForm_Enter(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                for (int i = 0; i < _eventsNames.Length; i++)
                    ShowPresentationGroups(i);
            }
            //CgpClientMainForm.Singleton.SetActOpenWindow(this);
        }

        private void AddDroppedPresentationGroup(object droppedPg, int eventNumber, int listboxIndex)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            try
            {
                if (droppedPg.GetType() == typeof(Contal.Cgp.Server.Beans.PresentationGroup))
                {
                    PresentationGroup presentationGroup = droppedPg as PresentationGroup;
                    if (CgpClient.Singleton.MainServerProvider.SystemEvents.AddPresentationGroup(_systemEvent, presentationGroup))
                    {
                        ShowPresentationGroups(eventNumber);
                    }
                    else
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAddPresentationGroup"));
                    }
                    CgpClientMainForm.Singleton.AddToRecentList(droppedPg);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _allListBox[listboxIndex],
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }


        private void ListboxDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                ListBox lb = sender as ListBox;
                int lbIndex = Int32.Parse(lb.AccessibleName);

                string[] output = e.Data.GetFormats();
                if (output == null) return;

                int numberOfEvent;
                if (!Int32.TryParse((sender as ListBox).AccessibleName, out numberOfEvent))
                {
                    return;
                }
                SetSystemEvent(numberOfEvent);
                AddDroppedPresentationGroup((object)e.Data.GetData(output[0]), numberOfEvent, lbIndex);
            }
            catch
            {
            }
        }

        private void ListboxDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        public static bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.SystemEvents.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAccessUpdate()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.SystemEvents.HasAccessUpdate();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
