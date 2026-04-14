using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Threads;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Contal.Cgp.Client
{
    public class AccessPanelControl
    {
        private class BaseAccessTreeNode : TreeNode
        {
            private bool _selected;
            private readonly bool _readOnly;
            private string _nodeName;

            public string AccessName
            {
                get { return _nodeName; }
            }

            public BaseAccessTreeNode(string nodeName, string text)
                : base(text)
            {
                _nodeName = nodeName;
            }

            public BaseAccessTreeNode(string nodeName, string text, bool readOnly)
                : this(nodeName, text)
            {
                _readOnly = readOnly;
            }

            private void Redraw()
            {
                if (IsVisible)
                {
                    var graphics = TreeView.CreateGraphics();
                    Redraw(graphics, _selected);
                    graphics.Save();
                }
            }

            public void Redraw(Graphics graphics)
            {
                if (IsVisible)
                {
                    Redraw(graphics, _selected);
                }
            }

            protected virtual void Redraw(Graphics graphics, bool selected)
            {
                if (selected)
                {
                    graphics.FillRectangle(new SolidBrush(Color.LightBlue),
                        Bounds.Left,
                        Bounds.Top,
                        TreeView.Width - Bounds.Left,
                        Bounds.Height);
                }
                else
                {
                    graphics.FillRectangle(new SolidBrush(TreeView.BackColor),
                        Bounds.Left,
                        Bounds.Top,
                        TreeView.Width - Bounds.Left,
                        Bounds.Height);
                }

                graphics.DrawString(Text,
                    TreeView.Font,
                    _readOnly ? new SolidBrush(Color.Gray) : new SolidBrush(TreeView.ForeColor),
                    new Point(Bounds.Left, Bounds.Top));

                RedrawCheckBoxes(graphics, _readOnly);
            }

            protected virtual void RedrawCheckBoxes(Graphics graphics, bool readOnly)
            {
            }

            private void Select()
            {
                _selected = true;
                Redraw();
            }

            private void Unselect()
            {
                _selected = false;
                Redraw();
            }

            public void MouseClickToNode(Point point)
            {
                if (!_readOnly)
                    MouseClick(point);
            }

            public void MouseHover(ref BaseAccessTreeNode actualSelectedAccessTreeNode)
            {
                if (this != actualSelectedAccessTreeNode)
                {
                    Select();

                    if (actualSelectedAccessTreeNode != null)
                        actualSelectedAccessTreeNode.Unselect();

                    actualSelectedAccessTreeNode = this;
                }
            }

            protected virtual void MouseClick(Point point)
            {
            }

            public void MouseDoubleClick(Point point)
            {
                if (point.X > Bounds.Left + Bounds.Width)
                {
                    if (!DoCollapseExpand(point))
                        return;

                    if (IsExpanded)
                        Collapse();
                    else
                        Expand();
                }
            }

            protected virtual bool DoCollapseExpand(Point point)
            {
                return true;
            }
        }

        private class SeparatorTreeNode : BaseAccessTreeNode
        {
            public SeparatorTreeNode()
                : base(string.Empty, string.Empty)
            {
            }

            protected override void Redraw(Graphics graphics, bool selected)
            {
                graphics.FillRectangle(new SolidBrush(TreeView.BackColor),
                    Bounds.Left,
                    Bounds.Top,
                    TreeView.Width - Bounds.Left,
                    Bounds.Height);

                graphics.DrawLine(new Pen(TreeView.ForeColor), new Point(0, Bounds.Top + Bounds.Height / 2),
                    new Point(TreeView.Width, Bounds.Top + Bounds.Height / 2));
            }
        }

        private class AccessGroupTreeNode : BaseAccessTreeNode
        {
            public AccessGroupTreeNode(string nodeName, string text, bool readOnly)
                : base(nodeName, text, readOnly)
            {
            }

            private bool? GetAccessGroupView()
            {
                bool? view = null;
                foreach (var node in Nodes)
                {
                    var childAccessTreeNode = node as AccessTreeNode;
                    if (childAccessTreeNode == null ||
                        childAccessTreeNode.View == null)
                    {
                        continue;
                    }

                    if (view == null)
                    {
                        view = childAccessTreeNode.View;
                    }
                    else
                    {
                        if (view != childAccessTreeNode.View)
                        {
                            view = null;
                            break;
                        }
                    }
                }

                return view;
            }

            private bool? GetAccessGroupAdmin()
            {
                bool? admin = null;
                foreach (var node in Nodes)
                {
                    var childAccessTreeNode = node as AccessTreeNode;
                    if (childAccessTreeNode == null)
                        continue;

                    if (admin == null)
                    {
                        admin = childAccessTreeNode.Admin;
                    }
                    else
                    {
                        if (admin != childAccessTreeNode.Admin)
                        {
                            admin = null;
                            break;
                        }
                    }
                }

                return admin;
            }

            protected override void RedrawCheckBoxes(Graphics graphics, bool readOnly)
            {
                bool? view = GetAccessGroupView();
                bool? admin = GetAccessGroupAdmin();

                CheckBoxRenderer.DrawCheckBox(graphics,
                    new Point(TreeView.Width - ACCESS_VIEW_OFFSET - Bounds.Height / 2, Bounds.Top),
                    view == null
                        ? CheckBoxState.MixedNormal
                        : view.Value ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);

                CheckBoxRenderer.DrawCheckBox(graphics,
                    new Point(TreeView.Width - ACCESS_ADMIN_OFFSET - Bounds.Height / 2, Bounds.Top),
                    admin == null
                        ? CheckBoxState.MixedNormal
                        : admin.Value ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
            }

            protected override void MouseClick(Point point)
            {
                if (point.X >= TreeView.Width - ACCESS_VIEW_OFFSET - Bounds.Height / 2 &&
                    point.X <= TreeView.Width - ACCESS_VIEW_OFFSET + Bounds.Height / 2)
                {
                    bool? view = GetAccessGroupView();

                    if (view == null || !view.Value)
                    {
                        SetView(true);
                    }
                    else
                    {
                        SetView(false);
                    }

                    return;
                }

                if (point.X >= TreeView.Width - ACCESS_ADMIN_OFFSET - Bounds.Height / 2 &&
                    point.X <= TreeView.Width - ACCESS_ADMIN_OFFSET + Bounds.Height / 2)
                {
                    bool? admin = GetAccessGroupAdmin();

                    if (admin == null || !admin.Value)
                    {
                        SetAdmin(true);
                    }
                    else
                    {
                        SetAdmin(false);
                    }
                }
            }

            private void SetView(bool view)
            {
                SetChildNodesView(view);
                RedrawAccessGroup();
            }

            private void SetChildNodesView(bool view)
            {
                if (Nodes.Count > 0)
                {
                    foreach (var node in Nodes)
                    {
                        var childAccessTreeNode = node as AccessTreeNode;
                        if (childAccessTreeNode != null)
                        {
                            childAccessTreeNode.SetView(view, false);
                        }
                    }
                }
            }

            private void SetAdmin(bool admin)
            {
                SetChildNodesAdmin(admin);
                RedrawAccessGroup();
            }

            private void SetChildNodesAdmin(bool admin)
            {
                if (Nodes.Count > 0)
                {
                    foreach (var node in Nodes)
                    {
                        var childAccessTreeNode = node as AccessTreeNode;
                        if (childAccessTreeNode != null)
                        {
                            childAccessTreeNode.SetAdmin(admin, false);
                        }
                    }
                }
            }

            private void RedrawAccessGroup()
            {
                Graphics graphics = TreeView.CreateGraphics();

                Redraw(graphics);

                foreach (var node in Nodes)
                {
                    var childAccessTreeNode = node as BaseAccessTreeNode;
                    if (childAccessTreeNode != null)
                    {
                        childAccessTreeNode.Redraw(graphics);
                    }
                }

                graphics.Save();
            }

            protected override bool DoCollapseExpand(Point point)
            {
                if (point.X >= TreeView.Width - ACCESS_VIEW_OFFSET - Bounds.Height / 2 &&
                    point.X <= TreeView.Width - ACCESS_VIEW_OFFSET + Bounds.Height / 2)
                {
                    return false;
                }

                if (point.X >= TreeView.Width - ACCESS_ADMIN_OFFSET - Bounds.Height / 2 &&
                    point.X <= TreeView.Width - ACCESS_ADMIN_OFFSET + Bounds.Height / 2)
                {
                    return false;
                }

                return true;
            }
        }

        private class AccessTreeNode : BaseAccessTreeNode
        {
            private readonly AccessPresentation _accessPresentation;
            private readonly Access _viewAccess;
            private readonly AccessSourceAccessValue[] _forcedSetAccessesView;
            private readonly AccessSourceAccessValue[] _forcedUnsetAccessesView;
            private readonly Access _adminAccess;
            private readonly AccessSourceAccessValue[] _forcedSetAccessesAdmin;
            private readonly AccessSourceAccessValue[] _forcedUnsetAccessesAdmin;
            private readonly EventHandler EditTextChanger;
            private readonly Action<AccessSourceAccessValue[]> ForcedSetAccess;
            private readonly Action<AccessSourceAccessValue[]> ForcedUnsetAccess;

            private bool _view;
            private bool _admin;

            public bool? View
            {
                get
                {
                    if (_viewAccess != null)
                        return _view;

                    return null;
                }
            }

            public bool Admin
            {
                get
                {
                    if (_adminAccess != null)
                        return _admin;

                    return _view;
                }
            }

            public AccessTreeNode(string nodeName, string text, bool readOnly, EventHandler editTextChanger,
                Action<AccessSourceAccessValue[]> forcedSetAccess,
                Action<AccessSourceAccessValue[]> forcedUnsetAccess, AccessPresentation accessPresentation,
                bool viewRights, bool adminRights)
                : base(nodeName, text, readOnly)
            {
                _accessPresentation = accessPresentation;

                if (_accessPresentation != null)
                {
                    _viewAccess = _accessPresentation.ViewAccess;
                    _forcedSetAccessesView = _accessPresentation.ForcedSetAccessesView;
                    _forcedUnsetAccessesView = _accessPresentation.ForcedUnsetAccessesView;

                    _adminAccess = _accessPresentation.AdminAccess;
                    _forcedSetAccessesAdmin = _accessPresentation.ForcedSetAccessesAdmin;
                    _forcedUnsetAccessesAdmin = _accessPresentation.ForcedUnsetAccessesAdmin;
                }

                SetView(viewRights, false);
                SetAdmin(adminRights, false);

                EditTextChanger = editTextChanger;
                ForcedSetAccess = forcedSetAccess;
                ForcedUnsetAccess = forcedUnsetAccess;
            }

            protected override void RedrawCheckBoxes(Graphics graphics, bool readOnly)
            {
                if (_accessPresentation == null || _accessPresentation.ViewAccess != null)
                {
                    CheckBoxRenderer.DrawCheckBox(graphics,
                        new Point(TreeView.Width - ACCESS_VIEW_OFFSET - Bounds.Height / 2, Bounds.Top),
                        _view ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);

                    if (_accessPresentation != null &&
                        !string.IsNullOrEmpty(_accessPresentation.CustomAccessViewDescription))
                    {
                        graphics.DrawString(
                            CgpClient.Singleton.LocalizationHelper.GetString(string.Format("AccessDescription_{0}",
                                _accessPresentation.CustomAccessViewDescription)),
                            TreeView.Font,
                            readOnly ? new SolidBrush(Color.Gray) : new SolidBrush(TreeView.ForeColor),
                            new Point(TreeView.Width - ACCESS_VIEW_OFFSET + Bounds.Height / 2, Bounds.Top));
                    }
                }

                if (_accessPresentation == null || _accessPresentation.AdminAccess != null)
                {
                    CheckBoxRenderer.DrawCheckBox(graphics,
                        new Point(TreeView.Width - ACCESS_ADMIN_OFFSET - Bounds.Height / 2, Bounds.Top),
                        _admin ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);

                    if (_accessPresentation != null &&
                        !string.IsNullOrEmpty(_accessPresentation.CustomAccessAdminDescription))
                    {
                        graphics.DrawString(
                            CgpClient.Singleton.LocalizationHelper.GetString(string.Format("AccessDescription_{0}",
                                _accessPresentation.CustomAccessAdminDescription)),
                            TreeView.Font,
                            readOnly ? new SolidBrush(Color.Gray) : new SolidBrush(TreeView.ForeColor),
                            new Point(TreeView.Width - ACCESS_ADMIN_OFFSET + Bounds.Height / 2, Bounds.Top));
                    }
                }
            }

            protected override void MouseClick(Point point)
            {
                if (point.X >= TreeView.Width - ACCESS_VIEW_OFFSET - Bounds.Height / 2 &&
                    point.X <= TreeView.Width - ACCESS_VIEW_OFFSET + Bounds.Height / 2)
                {
                    if (_viewAccess == null)
                        return;

                    if (!_view)
                    {
                        SetView(true, true);
                    }
                    else
                    {
                        SetView(false, true);
                    }

                    return;
                }

                if (point.X >= TreeView.Width - ACCESS_ADMIN_OFFSET - Bounds.Height / 2 &&
                    point.X <= TreeView.Width - ACCESS_ADMIN_OFFSET + Bounds.Height / 2)
                {
                    if (_adminAccess == null)
                        return;

                    if (!_admin)
                    {
                        SetAdmin(true, true);
                    }
                    else
                    {
                        SetAdmin(false, true);
                    }
                }
            }

            private void RedrawAccess()
            {
                Graphics graphics = TreeView.CreateGraphics();

                var parentCustomTreeNode = Parent as BaseAccessTreeNode;
                if (parentCustomTreeNode != null)
                {
                    parentCustomTreeNode.Redraw(graphics);
                }

                Redraw(graphics);

                graphics.Save();
            }

            public void SetAccessView()
            {
                if (!_view)
                    SetView(true, true);
            }

            public void UnsetAccessView()
            {
                if (_view)
                    SetView(false, true);
            }

            public void SetView(bool view, bool redraw)
            {
                if (view)
                {
                    _view = true;

                    if (_forcedSetAccessesView != null &&
                        ForcedSetAccess != null)
                    {
                        ForcedSetAccess(_forcedSetAccessesView);
                    }
                }
                else
                {
                    _view = false;

                    if (_forcedUnsetAccessesView != null &&
                        ForcedUnsetAccess != null)
                    {
                        ForcedUnsetAccess(_forcedUnsetAccessesView);
                    }

                    _admin = false;

                    if (_forcedUnsetAccessesAdmin != null &&
                        ForcedUnsetAccess != null)
                    {
                        ForcedUnsetAccess(_forcedUnsetAccessesAdmin);
                    }
                }

                if (redraw)
                    RedrawAccess();

                if (EditTextChanger != null)
                    EditTextChanger(null, null);
            }

            public void SetAccessAdmin()
            {
                if (!_admin)
                    SetAdmin(true, true);
            }

            public void UnsetAccessAdmin()
            {
                if (_admin)
                    SetAdmin(false, true);
            }

            public void SetAdmin(bool admin, bool redraw)
            {
                if (admin)
                {
                    _view = true;

                    if (_forcedSetAccessesView != null &&
                        ForcedSetAccess != null)
                    {
                        ForcedSetAccess(_forcedSetAccessesView);
                    }

                    _admin = true;

                    if (_forcedSetAccessesAdmin != null &&
                        ForcedSetAccess != null)
                    {
                        ForcedSetAccess(_forcedSetAccessesAdmin);
                    }
                }
                else
                {
                    _admin = false;

                    if (_forcedUnsetAccessesAdmin != null &&
                        ForcedUnsetAccess != null)
                    {
                        ForcedUnsetAccess(_forcedUnsetAccessesAdmin);
                    }
                }

                if (redraw)
                    RedrawAccess();

                if (EditTextChanger != null)
                    EditTextChanger(null, null);
            }

            protected override bool DoCollapseExpand(Point point)
            {
                if (point.X >= TreeView.Width - ACCESS_VIEW_OFFSET - Bounds.Height / 2 &&
                    point.X <= TreeView.Width - ACCESS_VIEW_OFFSET + Bounds.Height / 2)
                {
                    return _viewAccess == null;
                }

                if (point.X >= TreeView.Width - ACCESS_ADMIN_OFFSET - Bounds.Height / 2 &&
                    point.X <= TreeView.Width - ACCESS_ADMIN_OFFSET + Bounds.Height / 2)
                {
                    return _adminAccess == null;
                }

                return true;
            }

            public void AddAccessRights(IList<Access> accesses)
            {
                if (_accessPresentation == null)
                    return;

                if (_adminAccess != null && _admin)
                {
                    accesses.Add(_adminAccess);
                    return;
                }

                if (_viewAccess != null && _view)
                {
                    accesses.Add(_viewAccess);
                }
            }
        }

        private const int ACCESS_VIEW_OFFSET = 300;
        private const int ACCESS_ADMIN_OFFSET = 150;
        private const int TREE_VIEW_CCESS_START_Y = 30;

        private const int LABEL_ACCESS_START_X = 7;
        private const int LABELS_START_Y = 10;
        private const int LABEL_VIEW_ADMIN_HEIGHT = 20;
        private const int LABEL_VIEW_ADMIN_LENGTH = 100;

        private readonly EventHandler EditTextChanger;
        private readonly Func<Access, bool> CheckAccess;
        private readonly Panel _panelClbAcces;
        private readonly TreeView _accessTreeView;
        private BaseAccessTreeNode _actualSelectedAccessTreeNode;
        private readonly Dictionary<string, Dictionary<int, DVoid2Void>> _setAccessDelegates = new Dictionary<string, Dictionary<int, DVoid2Void>>();
        private readonly Dictionary<string, Dictionary<int, DVoid2Void>> _unsetAccessDelegates = new Dictionary<string, Dictionary<int, DVoid2Void>>();

        public AccessPanelControl(Func<Access, bool> checkAccess,
            EventHandler editTextChanger,
            Panel panelClbAccess)
        {
            EditTextChanger = editTextChanger;
            CheckAccess = checkAccess;
            _panelClbAcces = panelClbAccess;

            _accessTreeView = new TreeView
            {
                Top = TREE_VIEW_CCESS_START_Y,
                Height = panelClbAccess.Height - TREE_VIEW_CCESS_START_Y,
                Left = 0,
                Width = panelClbAccess.Width,
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                DrawMode = TreeViewDrawMode.OwnerDrawText
            };

            _accessTreeView.DrawNode += DrawNode;
            _accessTreeView.NodeMouseClick += NodeMouseClick;
            _accessTreeView.MouseMove += MouseMove;
            _accessTreeView.NodeMouseDoubleClick += NodeMouseDoubleClick;

            _panelClbAcces.Controls.Add(_accessTreeView);
        }

        void MouseMove(object sender, MouseEventArgs e)
        {
            var accessTreeNode = _accessTreeView.GetNodeAt(e.X, e.Y) as BaseAccessTreeNode;
            if (accessTreeNode != null)
            {
                accessTreeNode.MouseHover(ref _actualSelectedAccessTreeNode);
            }
        }

        public void LoadAccessRights(bool readOnly)
        {
            _accessTreeView.Nodes.Clear();
            _panelClbAcces.Controls.Clear();
            _setAccessDelegates.Clear();
            _unsetAccessDelegates.Clear();

            if (CheckAccess != null && CheckAccess(BaseAccess.GetAccess(LoginAccess.SuperAdmin)))
            {
                var lIsSuperAdmin = new Label
                {
                    Name = "_lIsSuperAdmin",
                    Text = CgpClient.Singleton.LocalizationHelper.GetString("LoginEditForm_lIsSuperAdmin"),
                    AutoSize = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                    Top = LABELS_START_Y,
                    Left = LABEL_ACCESS_START_X,
                    TextAlign = ContentAlignment.TopLeft
                };

                _panelClbAcces.Controls.Add(lIsSuperAdmin);

                return;
            }

            var labelView = new Label
            {
                Text = CgpClient.Singleton.LocalizationHelper.GetString("AccessDescription_View"),
                AutoSize = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Top = LABELS_START_Y,
                Height = LABEL_VIEW_ADMIN_HEIGHT,
                Left = _panelClbAcces.Width - ACCESS_VIEW_OFFSET - LABEL_VIEW_ADMIN_LENGTH / 2,
                Width = LABEL_VIEW_ADMIN_LENGTH,
                TextAlign = ContentAlignment.TopCenter
            };

            var labelAdmin = new Label
            {
                Text = CgpClient.Singleton.LocalizationHelper.GetString("AccessDescription_Admin"),
                AutoSize = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Top = LABELS_START_Y,
                Height = LABEL_VIEW_ADMIN_HEIGHT,
                Left = _panelClbAcces.Width - ACCESS_ADMIN_OFFSET - LABEL_VIEW_ADMIN_LENGTH / 2,
                Width = LABEL_VIEW_ADMIN_LENGTH,
                TextAlign = ContentAlignment.TopCenter
            };

            _panelClbAcces.Controls.Add(labelView);
            _panelClbAcces.Controls.Add(labelAdmin);
            _panelClbAcces.Controls.Add(_accessTreeView);

            LoadAccessTreeView(readOnly);
        }

        private void NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var accessTreeNode = e.Node as BaseAccessTreeNode;
            if (accessTreeNode != null)
            {
                accessTreeNode.MouseClickToNode(e.Location);
            }
        }

        private void NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var accessTreeNode = e.Node as BaseAccessTreeNode;
            if (accessTreeNode != null)
            {
                accessTreeNode.MouseDoubleClick(e.Location);
            }
        }

        private void DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            var customTreeNode = e.Node as BaseAccessTreeNode;
            if (customTreeNode != null)
            {
                customTreeNode.Redraw(e.Graphics);
            }
        }

        private void LoadAccessTreeView(bool readOnly)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;


            var accesses = BaseAccess.GetAccessList(CgpClient.Singleton.GetPluginsListAccess());

            if (accesses != null)
            {
                var nodes = new LinkedList<TreeNode>();
                nodes.AddLast(new SeparatorTreeNode());

                foreach (KeyValuePair<string, List<AccessPresentation>> accessGroup in accesses)
                {
                    if (accessGroup.Value.Count > 1)
                    {
                        var accessGroupTreeNode =
                            new AccessGroupTreeNode(
                                accessGroup.Key,
                                CgpClient.Singleton.LocalizationHelper.GetString(string.Format("AccessGroup_{0}",
                                    accessGroup.Key)),
                                readOnly);

                        nodes.AddLast(accessGroupTreeNode);

                        foreach (var accessPresentation in accessGroup.Value)
                        {
                            AccessTreeNode accessTreeNode = CreateAccessTreeNode(accessPresentation, readOnly);

                            if (accessTreeNode != null)
                                accessGroupTreeNode.Nodes.Add(accessTreeNode);
                        }
                    }
                    else
                    {
                        AccessTreeNode accessTreeNode = CreateAccessTreeNode(accessGroup.Value.FirstOrDefault(),
                            readOnly);

                        if (accessTreeNode != null)
                            nodes.AddLast(accessTreeNode);
                    }

                    nodes.AddLast(new SeparatorTreeNode());
                }

                ShowAccessTreeView(nodes.ToArray());
            }
        }

        private void ShowAccessTreeView(ICollection<TreeNode> nodes)
        {
            if (_accessTreeView.InvokeRequired)
            {
                _accessTreeView.BeginInvoke(new Action<ICollection<TreeNode>>(ShowAccessTreeView), nodes);
            }
            else
            {
                _accessTreeView.BeginUpdate();
                _accessTreeView.Nodes.AddRange(nodes.ToArray());
                _accessTreeView.EndUpdate();
            }
        }

        private AccessTreeNode CreateAccessTreeNode(AccessPresentation accessPresentation, bool readOnly)
        {
            if (accessPresentation == null)
                return null;

            bool viewRights = false;
            bool adminRights = false;

            Access viewAccess = accessPresentation.ViewAccess;
            Access adminAccess = accessPresentation.AdminAccess;

            if (CheckAccess != null)
            {
                if (viewAccess != null)
                    viewRights = CheckAccess(viewAccess);

                if (adminAccess != null)
                    adminRights = CheckAccess(adminAccess);
            }

            var accessTreeNode =
                new AccessTreeNode(
                    accessPresentation.Name,
                    CgpClient.Singleton.LocalizationHelper.GetString(string.Format("Access_{0}", accessPresentation.Name)),
                    readOnly,
                    EditTextChanger,
                    ForcedSetAccess,
                    ForcedUnsetAccess,
                    accessPresentation,
                    viewRights,
                    adminRights);

            if (viewAccess != null)
            {
                AddSetAccessDelegate(viewAccess.Source, viewAccess.AccessValue, accessTreeNode.SetAccessView);
                AddUnsetAccessDelegate(viewAccess.Source, viewAccess.AccessValue, accessTreeNode.UnsetAccessView);
            }

            if (adminAccess != null)
            {
                AddSetAccessDelegate(adminAccess.Source, adminAccess.AccessValue, accessTreeNode.SetAccessAdmin);
                AddUnsetAccessDelegate(adminAccess.Source, adminAccess.AccessValue, accessTreeNode.UnsetAccessAdmin);
            }

            return accessTreeNode;
        }

        private string GetNotNullAccessSource(string accessSource)
        {
            return accessSource != null ? accessSource : string.Empty;
        }

        private void AddSetAccessDelegate(string accessSource, int accessValue, DVoid2Void setAccessDelegate)
        {
            Dictionary<int, DVoid2Void> accessSourceSetAccessDelegates;
            if (_setAccessDelegates.TryGetValue(GetNotNullAccessSource(accessSource), out accessSourceSetAccessDelegates))
            {
                accessSourceSetAccessDelegates[accessValue] = setAccessDelegate;
                return;
            }

            accessSourceSetAccessDelegates = new Dictionary<int, DVoid2Void>();
            accessSourceSetAccessDelegates.Add(accessValue, setAccessDelegate);
            _setAccessDelegates.Add(GetNotNullAccessSource(accessSource), accessSourceSetAccessDelegates);
        }

        public void ForcedSetAccess(AccessSourceAccessValue[] forcedSetAccesses)
        {
            if (forcedSetAccesses == null)
                return;

            foreach (var accessSourceAccessValue in forcedSetAccesses)
            {
                Dictionary<int, DVoid2Void> accessSourceSetAccessDelegates;
                if (
                    _setAccessDelegates.TryGetValue(
                        GetNotNullAccessSource(accessSourceAccessValue.AccessSource),
                        out accessSourceSetAccessDelegates))
                {
                    DVoid2Void setAccessDelegate;
                    if (accessSourceSetAccessDelegates.TryGetValue(accessSourceAccessValue.AccessValue,
                        out setAccessDelegate))
                    {
                        if (setAccessDelegate != null)
                            setAccessDelegate();
                    }
                }
            }
        }

        private void AddUnsetAccessDelegate(string accessSource, int accessValue, DVoid2Void unsetAccessDelegate)
        {
            Dictionary<int, DVoid2Void> accessSourceUnsetAccessDelegates;
            if (_unsetAccessDelegates.TryGetValue(GetNotNullAccessSource(accessSource), out accessSourceUnsetAccessDelegates))
            {
                accessSourceUnsetAccessDelegates[accessValue] = unsetAccessDelegate;
                return;
            }

            accessSourceUnsetAccessDelegates = new Dictionary<int, DVoid2Void>();
            accessSourceUnsetAccessDelegates.Add(accessValue, unsetAccessDelegate);
            _unsetAccessDelegates.Add(GetNotNullAccessSource(accessSource), accessSourceUnsetAccessDelegates);
        }

        public void ForcedUnsetAccess(AccessSourceAccessValue[] forcedUnsetAccesses)
        {
            if (forcedUnsetAccesses == null)
                return;

            foreach (var accessSourceAccessValue in forcedUnsetAccesses)
            {
                Dictionary<int, DVoid2Void> accessSourceUnsetAccessDelegates;
                if (
                    _unsetAccessDelegates.TryGetValue(
                        GetNotNullAccessSource(accessSourceAccessValue.AccessSource),
                        out accessSourceUnsetAccessDelegates))
                {
                    DVoid2Void unsetAccessDelegate;
                    if (accessSourceUnsetAccessDelegates.TryGetValue(accessSourceAccessValue.AccessValue,
                        out unsetAccessDelegate))
                    {
                        if (unsetAccessDelegate != null)
                            unsetAccessDelegate();
                    }
                }
            }
        }

        public void Translate()
        {
            if (_accessTreeView.InvokeRequired)
            {
                _accessTreeView.BeginInvoke(new DVoid2Void(Translate));
            }
            else
            {
                _accessTreeView.BeginUpdate();

                foreach (TreeNode node in _accessTreeView.Nodes)
                {
                    TranslateNode(node);
                }

                _accessTreeView.EndUpdate();
            }
        }

        private void TranslateNode(TreeNode node)
        {
            if (node == null)
                return;

            foreach (TreeNode childNode in node.Nodes)
            {
                TranslateNode(childNode);
            }

            var baseAccessTreeNode = node as BaseAccessTreeNode;

            if (baseAccessTreeNode == null)
                return;

            if (baseAccessTreeNode is AccessGroupTreeNode)
            {
                baseAccessTreeNode.Text =
                    CgpClient.Singleton.LocalizationHelper.GetString(string.Format("AccessGroup_{0}",
                        baseAccessTreeNode.AccessName));

                return;
            }

            if (baseAccessTreeNode is AccessTreeNode)
            {
                baseAccessTreeNode.Text =
                    CgpClient.Singleton.LocalizationHelper.GetString(string.Format("Access_{0}",
                        baseAccessTreeNode.AccessName));
            }
        }

        public IList<Access> GetListOfAccesses()
        {
            IList<Access> listAccesses = new List<Access>();
            AddAccessRights(listAccesses, _accessTreeView.Nodes);

            return listAccesses;
        }

        private void AddAccessRights(IList<Access> accesses, TreeNodeCollection nodes)
        {
            if (accesses == null || nodes == null || nodes.Count == 0)
                return;

            foreach (TreeNode node in nodes)
            {
                var accessTreeNode = node as AccessTreeNode;
                if (accessTreeNode != null)
                {
                    accessTreeNode.AddAccessRights(accesses);
                }

                if (node.Nodes.Count > 0)
                {
                    AddAccessRights(accesses, node.Nodes);
                }
            }
        }
    }
}
