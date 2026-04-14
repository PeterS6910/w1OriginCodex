using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace Contal.IwQuick.PlatformPC.UI
{
    public class TreeViewWithFilter : TreeView
    {
        private static class CustomTreeNodesRedrawing
        {
            public static void Redraw(Graphics graphics, TreeNode node)
            {
                Redraw(graphics, node, false);
            }

            public static void Redraw(Graphics graphics, TreeNode node, bool isEditing)
            {
                var filterTreeNode = node as FilterTreeNode;
                if (filterTreeNode != null)
                {
                    filterTreeNode.RedrawFilterTextBox(graphics);
                    return;
                }

                if (graphics == null ||
                    node == null ||
                    !node.IsVisible)
                {
                    return;
                }

                var treeViewWithFilter = node.TreeView as TreeViewWithFilter;

                if (treeViewWithFilter == null)
                    return;

                bool isAdded = false;

                if (node.Parent != null)
                {
                    filterTreeNode = node.Parent.Nodes[0] as FilterTreeNode;
                    if (filterTreeNode != null)
                    {
                        isAdded = filterTreeNode.IsAddedNode(node);
                    }
                }

                if (isAdded)
                {
                    graphics.FillRectangle(
                        treeViewWithFilter.IsSelected(node)
                            ? new SolidBrush(Color.Orange)
                            : new SolidBrush(Color.OrangeRed),
                        node.Bounds.Left,
                        node.Bounds.Top,
                        treeViewWithFilter.Width - node.Bounds.Left,
                        node.Bounds.Height);
                }
                else
                {
                    graphics.FillRectangle(
                        treeViewWithFilter.IsSelected(node)
                            ? new SolidBrush(Color.LightBlue)
                            : new SolidBrush(treeViewWithFilter.BackColor),
                        node.Bounds.Left,
                        node.Bounds.Top,
                        treeViewWithFilter.Width - node.Bounds.Left,
                        node.Bounds.Height);
                }

                if (isEditing)
                    return;

                graphics.DrawString(node.Text,
                    treeViewWithFilter.Font,
                    new SolidBrush(treeViewWithFilter.ForeColor),
                    new Point(node.Bounds.Left, node.Bounds.Top + (treeViewWithFilter.ItemHeight - treeViewWithFilter.Font.Height) / 2));
            }
        }

        private class FilterTreeNode : TreeNode
        {
            private readonly LinkedList<TreeNode> _nodes = new LinkedList<TreeNode>();
            private readonly LinkedList<TreeNode> _addedNodes = new LinkedList<TreeNode>();
            private readonly Action<bool> _disableRedrawingTreeNodes;
            private readonly Func<TreeNode, string> _keySelectorForOrderBy;
            public string FilterText { get; private set; }

            public int AllNodesCount 
            {
                get { return _nodes.Count; }
            }

            public FilterTreeNode(Action<bool> disableRedrawingTreeNodes,
                Func<TreeNode, string> keySelectorForOrderBy)
                : base(string.Empty)
            {
                SetNodeIcon(this, FILTER_NODE_IMAGE_KEY);

                _nodes.AddLast(this);
                _disableRedrawingTreeNodes = disableRedrawingTreeNodes;
                _keySelectorForOrderBy = keySelectorForOrderBy;
                FilterText = string.Empty;
            }

            public void AddNode(TreeNode node)
            {
                if (node == null)
                    return;

                if (string.IsNullOrEmpty(FilterText)
                    || node.Text.ToLower().IndexOf(FilterText.ToLower()) >= 0)
                {
                    _nodes.AddLast(node);
                    RunFilter(true);
                    return;
                }

                _addedNodes.AddLast(node);

                var newNodes = new LinkedList<TreeNode>();

                foreach (var treeNode in OrderBy(_nodes))
                {
                    if (treeNode == this ||
                        treeNode.Text.ToLower().IndexOf(FilterText.ToLower()) >= 0)
                    {
                        newNodes.AddLast(treeNode);
                    }
                }

                foreach (var treeNode in OrderBy(_addedNodes))
                {
                    newNodes.AddLast(treeNode);
                }

                ShowNewNodes(newNodes, true);
            }

            private IEnumerable<TreeNode> OrderBy(IEnumerable<TreeNode> nodes)
            {
                if (_keySelectorForOrderBy != null)
                    return nodes.OrderBy(actualNode => _keySelectorForOrderBy(actualNode));
                else
                    return nodes.OrderBy(actualNode => actualNode.Text);
            }

            public void AddNodes(IEnumerable<TreeNode> nodes)
            {
                if (nodes == null)
                    return;

                if (string.IsNullOrEmpty(FilterText))
                {
                    foreach (TreeNode node in nodes)
                    {
                        _nodes.AddLast(node);
                    }

                    RunFilter(true);
                    return;
                }

                foreach (TreeNode node in nodes)
                {
                    if (node.Text.ToLower().IndexOf(FilterText.ToLower()) >= 0)
                    {
                        _nodes.AddLast(node);
                    }
                    else
                    {
                        _addedNodes.AddLast(node);
                    }
                }

                var newNodes = new LinkedList<TreeNode>();

                foreach (var treeNode in OrderBy(_nodes))
                {
                    if (treeNode == this ||
                        treeNode.Text.ToLower().IndexOf(FilterText.ToLower()) >= 0)
                    {
                        newNodes.AddLast(treeNode);
                    }
                }

                foreach (var treeNode in OrderBy(_addedNodes))
                {
                    newNodes.AddLast(treeNode);
                }

                ShowNewNodes(newNodes, true);
            }

            public bool IsAddedNode(TreeNode node)
            {
                if (node == null ||
                    !_addedNodes.Contains(node))
                {
                    return false;
                }

                return true;
            }

            public TreeNode FindNode(Func<TreeNode, bool> condition)
            {
                if (condition == null)
                    return null;

                foreach (var node in _nodes)
                {
                    if (condition(node))
                        return node;
                }

                foreach (var node in _addedNodes)
                {
                    if (condition(node))
                        return node;
                }

                return null;
            }

            public void DeleteNodes(Func<TreeNode, bool> condition)
            {
                if (condition == null)
                    return;

                var nodesToDelete = new LinkedList<TreeNode>();
                foreach (var node in _nodes)
                {
                    if (condition(node))
                        nodesToDelete.AddLast(node);
                }

                foreach (var nodeToDelete in nodesToDelete)
                {
                    _nodes.Remove(nodeToDelete);
                }

                var addedNodesToDelete = new LinkedList<TreeNode>();
                foreach (var node in _addedNodes)
                {
                    if (condition(node))
                        addedNodesToDelete.AddLast(node);
                }

                foreach (var nodeToDelete in addedNodesToDelete)
                {
                    _nodes.Remove(nodeToDelete);
                }

                var newNodes = new LinkedList<TreeNode>();

                foreach (var treeNode in OrderBy(_nodes))
                {
                    if (treeNode == this ||
                        treeNode.Text.ToLower().IndexOf(FilterText.ToLower()) >= 0)
                    {
                        newNodes.AddLast(treeNode);
                    }
                }

                foreach (var treeNode in OrderBy(_addedNodes))
                {
                    newNodes.AddLast(treeNode);
                }

                ShowNewNodes(newNodes, true);
            }

            public void RedrawFilterTextBox(Graphics graphics)
            {
                if (!IsVisible)
                {
                    return;
                }

                graphics.DrawString(
                    string.IsNullOrEmpty(FilterText)
                        ? "..."
                        : FilterText,
                    TreeView.Font,
                    new SolidBrush(TreeView.ForeColor),
                    new Point(Bounds.Left, Bounds.Top + (TreeView.ItemHeight - TreeView.Font.Height) / 2));
            }

            public void RunFilter(string filterText)
            {
                FilterText = filterText;
                RunFilter(false);
            }

            public void RunFilter(bool forcedRedraw)
            {
                if (_addedNodes.Count > 0)
                {
                    foreach (var treeNode in OrderBy(_addedNodes))
                    {
                        _nodes.AddLast(treeNode);
                    }

                    _addedNodes.Clear();
                    forcedRedraw = true;
                }

                if (string.IsNullOrEmpty(FilterText))
                {
                    ShowNewNodes(OrderBy(_nodes).ToList(), forcedRedraw);
                }
                else
                {
                    var newNodes = new LinkedList<TreeNode>();

                    foreach (var treeNode in OrderBy(_nodes))
                    {
                        if (treeNode == this ||
                            treeNode.Text.ToLower().IndexOf(FilterText.ToLower()) >= 0)
                        {
                            newNodes.AddLast(treeNode);
                        }
                    }

                    ShowNewNodes(newNodes, forcedRedraw);
                }

                var treeViewWithFilter = TreeView as TreeViewWithFilter;

                if (treeViewWithFilter != null)
                    treeViewWithFilter.RefreshSelectedNodes();
            }

            private void ShowNewNodes(ICollection<TreeNode> newNodes, bool forcedRedraw)
            {
                if (newNodes == null)
                    return;

                var actualNodes = Parent != null ? Parent.Nodes : TreeView.Nodes;

                if (!forcedRedraw)
                {
                    if (actualNodes.Count == _nodes.Count &&
                        newNodes.Count == _nodes.Count)
                    {
                        return;
                    }

                    if (actualNodes.Count == newNodes.Count &&
                        newNodes.All(actualNodes.Contains))
                    {
                        return;
                    }
                }

                if (_disableRedrawingTreeNodes != null)
                    _disableRedrawingTreeNodes(true);

                if (TreeView != null)
                    TreeView.BeginUpdate();

                actualNodes.Clear();
                actualNodes.AddRange(newNodes.ToArray());

                if (TreeView != null)
                    TreeView.EndUpdate();

                if (_disableRedrawingTreeNodes != null)
                    _disableRedrawingTreeNodes(false);
            }
        }
        public const string FILTER_NODE_IMAGE_KEY = "FilterNodeImageKey";

        private const int ITEM_HEIGHT = 20;
        private const int FILTER_TEXT_BOX_WIDTH = 200;

        private bool _disabledReadrawingTreeNodes = false;
        private HashSet<FilterTreeNode> _visibleFilterTreeNodes = new HashSet<FilterTreeNode>();
        private int _disableReadrawingTreeNodesCounter;
        private readonly TextBox _tbEditText = new TextBox();
        private readonly TextBox _tbFilter = new TextBox();
        private TreeNode _editingNode;
        private FilterTreeNode _editedFilterTreeNode;

        public event Action<TreeNode> NodeAfterEdit;
        public event Func<TreeNode, bool> DeleteParentNode;

        public TreeViewWithFilter()
        {
            ItemHeight = ITEM_HEIGHT;
            DrawMode = TreeViewDrawMode.OwnerDrawText;

            DrawNode += DoDrawNode;

            _tbEditText.KeyDown += _tbEditText_KeyDown;
            _tbEditText.Width = FILTER_TEXT_BOX_WIDTH;

            _tbFilter.Parent = this;
            _tbFilter.Visible = false;
            _tbFilter.Width = FILTER_TEXT_BOX_WIDTH;
            _tbFilter.TextChanged += _tbFilter_TextChanged;

            NodeMouseClick += TreeViewWithFilter_NodeMouseClick;
        }

        void _tbFilter_TextChanged(object sender, EventArgs e)
        {
            if (_editedFilterTreeNode != null)
            {
                _editedFilterTreeNode.RunFilter(_tbFilter.Text);
            }
        }

        private void TreeViewWithFilter_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var filterTreeNode = e.Node as FilterTreeNode;
            if (filterTreeNode != null)
            {
                _editedFilterTreeNode = filterTreeNode;
                _tbFilter.Text = filterTreeNode.FilterText;
                _tbFilter.Select(_tbFilter.Text.Length, 0);
                FilterTextBoxSetPosition(filterTreeNode);
                _tbFilter.Visible = true;
                _tbFilter.Focus();
            }
        }

        private void FilterTextBoxSetPosition(TreeNode node)
        {
            _tbFilter.Left = node.Bounds.Left;
            _tbFilter.Top = node.Bounds.Top;
        }

        public void DisableReadrawingTreeNodes(bool disabled)
        {
            if (disabled)
            {
                if (_disableReadrawingTreeNodesCounter == 0)
                    _disabledReadrawingTreeNodes = true;

                _disableReadrawingTreeNodesCounter++;
            }
            else
            {
                _disableReadrawingTreeNodesCounter--;

                if (_disableReadrawingTreeNodesCounter == 0)
                    _disabledReadrawingTreeNodes = false;
            }
        }

        private void DoDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (_disabledReadrawingTreeNodes)
                return;

            if (_editingNode != null)
            {
                if (!SelectedNode.Equals(_editingNode) || _editingNode.Bounds.Top != _tbEditText.Top)
                {
                    EndEditNode(false);
                }
            }

            var notVisibleFilterTreeNodes = new LinkedList<FilterTreeNode>();
            foreach (var visibleFilterTreeNode in _visibleFilterTreeNodes)
            {
                if (!visibleFilterTreeNode.IsVisible)
                {
                    notVisibleFilterTreeNodes.AddLast(visibleFilterTreeNode);
                }
            }

            foreach (var notVisibleFilterTreeNode in notVisibleFilterTreeNodes)
            {
                if (_editedFilterTreeNode == notVisibleFilterTreeNode)
                {
                    _tbFilter.Visible = false;
                    Focus();
                }

                _visibleFilterTreeNodes.Remove(notVisibleFilterTreeNode);
            }

            CustomTreeNodesRedrawing.Redraw(e.Graphics, e.Node);

            if (e.Node == _editedFilterTreeNode)
            {
                FilterTextBoxSetPosition(e.Node);
            }

            FilterTreeNode filterTreeNode = e.Node as FilterTreeNode;
            if (filterTreeNode != null)
            {
                if (filterTreeNode.IsVisible && !_visibleFilterTreeNodes.Contains(filterTreeNode))
                {
                    _visibleFilterTreeNodes.Add(filterTreeNode);
                }
            }
        }

        private void _tbEditText_KeyDown(object sender, KeyEventArgs e)
        {
            if (_editingNode == null || _editingNode.TreeView == null)
                return;

            if (e.KeyCode == Keys.Enter)
            {
                if (_editingNode.Text != _tbEditText.Text)
                {
                    _editingNode.Text = _tbEditText.Text;
                    EndEditNode(true);
                }
                else
                {
                    EndEditNode(false);
                }
            }
        }

        public void AddNode(TreeNode parentNode, TreeNode newNode)
        {
            AddNode(parentNode, newNode, null);
        }

        public void AddNode(TreeNode parentNode, TreeNode newNode, Func<TreeNode, string> keySelectorForOrderBy)
        {
            if (newNode == null)
                return;

            var nodes = parentNode != null ? parentNode.Nodes : Nodes;

            FilterTreeNode filterTreeNode;
            if (nodes.Count == 0 ||
                !(nodes[0] is FilterTreeNode))
            {
                filterTreeNode = new FilterTreeNode(DisableReadrawingTreeNodes, keySelectorForOrderBy);
                nodes.Add(filterTreeNode);
            }
            else
            {
                filterTreeNode = nodes[0] as FilterTreeNode;
            }

            filterTreeNode.AddNode(newNode);
        }

        public void AddNodes(TreeNode parentNode, ICollection<TreeNode> newNodes)
        {
            AddNodes(parentNode, newNodes, null);
        }

        public void AddNodes(TreeNode parentNode, ICollection<TreeNode> newNodes, Func<TreeNode, string> keySelectorForOrderBy)
        {
            if (newNodes == null || newNodes.Count == 0)
                return;

            var nodes = parentNode != null ? parentNode.Nodes : Nodes;

            FilterTreeNode filterTreeNode;
            if (nodes.Count == 0 ||
                !(nodes[0] is FilterTreeNode))
            {
                filterTreeNode = new FilterTreeNode(DisableReadrawingTreeNodes, keySelectorForOrderBy);
                nodes.Add(filterTreeNode);
            }
            else
            {
                filterTreeNode = nodes[0] as FilterTreeNode;
            }

            filterTreeNode.AddNodes(newNodes);
        }

        public TreeNode FindNode(TreeNode parentNode, Func<TreeNode, bool> condition)
        {
            var nodes = parentNode != null ? parentNode.Nodes : Nodes;

            if (nodes.Count > 0)
            {
                FilterTreeNode filterTreeNode = nodes[0] as FilterTreeNode;
                if (filterTreeNode != null)
                    return filterTreeNode.FindNode(condition);
            }

            return null;
        }

        public void DeleteNodes(TreeNode parentNode, Func<TreeNode, bool> condition)
        {
            var nodes = parentNode != null ? parentNode.Nodes : Nodes;

            if (nodes.Count > 0)
            {
                FilterTreeNode filterTreeNode = nodes[0] as FilterTreeNode;
                if (filterTreeNode != null)
                {
                    filterTreeNode.DeleteNodes(condition);
                    DoDeleteParentNode(parentNode, nodes);
                }
            }

            RefreshSelectedNodes();
        }

        private void DoDeleteParentNode(TreeNode parentNode, TreeNodeCollection nodes)
        {
            if (nodes.Count > 1)
                return;

            if (nodes.Count == 1)
            {
                var filterTreeNode = nodes[0] as FilterTreeNode;

                if (filterTreeNode == null)
                    return;

                if (filterTreeNode.AllNodesCount > 1)
                    return;
            }

            if (DeleteParentNode != null && !DeleteParentNode(parentNode))
            {
                nodes.Clear();
                return;
            }

            if (parentNode == null)
            {
                nodes.Clear();
                return;
            }

            var newParentNode = parentNode.Parent;
            var newNodes = newParentNode != null ? newParentNode.Nodes : Nodes;

            if (newNodes.Count > 0)
            {
                FilterTreeNode filterTreeNode = newNodes[0] as FilterTreeNode;
                if (filterTreeNode != null)
                    filterTreeNode.DeleteNodes(
                        node =>
                            node == parentNode);
            }

            DoDeleteParentNode(newParentNode, newNodes);
        }

        public void BeginEditNode(TreeNode node)
        {
            if (node == null || node.TreeView == null)
                return;

            Graphics graphics = node.TreeView.CreateGraphics();
            CustomTreeNodesRedrawing.Redraw(graphics, node, true);
            graphics.Save();

            _editingNode = node;
            _tbEditText.Left = node.Bounds.Left;
            _tbEditText.Top = node.Bounds.Top;
            _tbEditText.Parent = node.TreeView;

            _tbEditText.Text = node.Text;
            _tbEditText.Focus();
            _tbEditText.SelectAll();
        }

        private void EndEditNode(bool nodeWasChanged)
        {
            _tbEditText.Parent = null;
            var editingNode = _editingNode;
            _editingNode = null;

            if (nodeWasChanged)
            {
                if (NodeAfterEdit != null)
                {
                    NodeAfterEdit(editingNode);
                }

                SelectedNode = null;
                EndEditNode(editingNode);
                SelectedNode = editingNode;
            }
            else
            {
                SelectedNode = editingNode;

                var graphics = CreateGraphics();
                CustomTreeNodesRedrawing.Redraw(graphics, editingNode);
                graphics.Save();
            }
        }

        public void EndEditNode(TreeNode node)
        {
            var nodes = node.Parent != null ? node.Parent.Nodes : Nodes;

            if (nodes.Count > 0)
            {
                var filterTreeNode = nodes[0] as FilterTreeNode;

                if (filterTreeNode != null)
                {
                    filterTreeNode.RunFilter(true);
                }
            }
        }

        public static void SetNodeIcon(TreeNode node, string key)
        {
            node.ImageKey = key;
            node.SelectedImageKey = key;
            node.StateImageKey = key;
        }

        public void Clear()
        {
            Nodes.Clear();

            _tbFilter.Visible = false;

            _visibleFilterTreeNodes.Clear();
        }

        private readonly HashSet<TreeNode> _selectedNodes = new HashSet<TreeNode>();

        private TreeNode _lastSelectedNode;

        public LinkedList<TreeNode> SelectedNodes
        {
            get
            {
                lock (_selectedNodes)
                    return new LinkedList<TreeNode>(_selectedNodes);
            }
        }

        public bool IsSelected(TreeNode node)
        {
            lock (_selectedNodes)
                return _selectedNodes.Contains(node);
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
            {
                e.Cancel = true;
                return;
            }

            base.OnBeforeSelect(e);

            var selectedNode = e.Node;

            if (selectedNode == null
                || selectedNode is FilterTreeNode)
            {
                e.Cancel = true;
                return;
            }

            try
            {
                lock (_selectedNodes)
                {
                    if (_lastSelectedNode == null
                        || HaveTheSameParentNode(
                            _lastSelectedNode,
                            selectedNode))
                    {
                        if (ModifierKeys == Keys.Shift
                            && _lastSelectedNode != null)
                        {
                            var firstNode = _lastSelectedNode.Index < selectedNode.Index
                                ? _lastSelectedNode
                                : selectedNode;

                            var lastNode = selectedNode.Index > _lastSelectedNode.Index
                                ? selectedNode
                                : _lastSelectedNode;

                            var newSelectedNodes = new HashSet<TreeNode>(
                                Enumerable.Repeat(
                                    firstNode,
                                    1));

                            var acutalNode = firstNode;

                            while (!acutalNode.Equals(lastNode))
                            {
                                acutalNode = acutalNode.NextNode;

                                newSelectedNodes.Add(acutalNode);
                            }

                            var oldSelectedNodes2 = SelectedNodes;

                            _selectedNodes.Clear();

                            foreach (var node in oldSelectedNodes2)
                            {
                                if (newSelectedNodes.Contains(node))
                                    continue;

                                CustomTreeNodesRedrawing.Redraw(
                                    CreateGraphics(),
                                    node,
                                    node.IsEditing);
                            }

                            foreach (var node in newSelectedNodes)
                            {
                                if (_selectedNodes.Add(node))
                                    CustomTreeNodesRedrawing.Redraw(
                                        CreateGraphics(),
                                        node,
                                        node.IsEditing);
                            }

                            return;
                        }


                        if (ModifierKeys == Keys.Control)
                        {
                            if (!_selectedNodes.Add(selectedNode))
                                _selectedNodes.Remove(selectedNode);

                            CustomTreeNodesRedrawing.Redraw(
                                CreateGraphics(),
                                selectedNode,
                                selectedNode.IsEditing);

                            _lastSelectedNode = selectedNode;

                            return;
                        }
                    }

                    var oldSelectedNodes = SelectedNodes;

                    _selectedNodes.Clear();

                    //SuspendLayout();

                    foreach (var node in oldSelectedNodes)
                    {
                        if (node.Equals(selectedNode))
                            continue;

                        CustomTreeNodesRedrawing.Redraw(
                            CreateGraphics(),
                            node,
                            node.IsEditing);
                    }

                    if (_selectedNodes.Add(selectedNode))
                        CustomTreeNodesRedrawing.Redraw(
                            CreateGraphics(),
                            selectedNode,
                            selectedNode.IsEditing);

                    //ResumeLayout();

                    _lastSelectedNode = selectedNode;
                }
            }
            finally
            {
                e.Cancel = true;

                OnAfterSelect(
                    new TreeViewEventArgs(e.Node));
            }
        }

        private bool HaveTheSameParentNode(
            [NotNull]
            TreeNode firstNode,
            [NotNull]
            TreeNode otherNode)
        {
            if (firstNode.Parent == null)
                return otherNode.Parent == null;

            return firstNode.Parent.Equals(otherNode.Parent);
        }

        public new TreeNode SelectedNode
        {
            get
            {
                lock (_selectedNodes)
                    return _selectedNodes.FirstOrDefault();
            }
            set
            {
                lock (_selectedNodes)
                {
                    var oldSelectedNodes = SelectedNodes;

                    _selectedNodes.Clear();

                    foreach (var node in oldSelectedNodes)
                    {
                        if (node.Equals(value))
                            continue;

                        CustomTreeNodesRedrawing.Redraw(
                            CreateGraphics(),
                            node,
                            node.IsEditing);
                    }

                    if (value == null)
                        return;

                    _selectedNodes.Add(value);

                    CustomTreeNodesRedrawing.Redraw(
                        CreateGraphics(),
                        value,
                        value.IsEditing);

                    OnAfterSelect(
                        new TreeViewEventArgs(value));
                }
            }
        }

        public void RefreshSelectedNodes()
        {
            lock (_selectedNodes)
            {
                var deletedSelectedNodes = new LinkedList<TreeNode>(
                    _selectedNodes.Where(
                        node =>
                            node.TreeView == null));

                foreach (var deletedSelectedNode in deletedSelectedNodes)
                {
                    _selectedNodes.Remove(deletedSelectedNode);
                }
            }
        }

        public new void BeginUpdate()
        {
            base.BeginUpdate();

            if (Parent != null)
                Parent.SuspendLayout();
        }

        public new void EndUpdate()
        {
            base.EndUpdate();
            
            if (Parent != null)
                Parent.ResumeLayout();
        }
    }
}
