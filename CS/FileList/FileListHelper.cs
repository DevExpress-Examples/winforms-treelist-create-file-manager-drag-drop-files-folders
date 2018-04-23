using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraTreeList;
using System.IO;
using DevExpress.XtraTreeList.Nodes;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace FileList
{
    class FileListHelper
    {
        string rootPath;
        TreeList Tree;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn5;


        public FileListHelper(TreeList tree)
        {
            Tree = tree;
            InitColumns();
            Tree.BeforeExpand += new DevExpress.XtraTreeList.BeforeExpandEventHandler(this.treeList1_BeforeExpand);
            Tree.AfterExpand += new DevExpress.XtraTreeList.NodeEventHandler(this.treeList1_AfterExpand);
            Tree.AfterCollapse += new DevExpress.XtraTreeList.NodeEventHandler(this.treeList1_AfterCollapse);
            Tree.CalcNodeDragImageIndex += new DevExpress.XtraTreeList.CalcNodeDragImageIndexEventHandler(this.treeList1_CalcNodeDragImageIndex);
            Tree.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeList1_DragDrop);
            Tree.DoubleClick += new System.EventHandler(this.treeList1_DoubleClick);

            InitData();
        }

        void InitColumns()
        {
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn5 = new DevExpress.XtraTreeList.Columns.TreeListColumn();

            this.treeListColumn1.Caption = "FullName";
            this.treeListColumn1.FieldName = "FullName";

            this.treeListColumn2.Caption = "Name";
            this.treeListColumn2.FieldName = "Name";
            this.treeListColumn2.VisibleIndex = 0;
            this.treeListColumn2.Visible = true;

            this.treeListColumn3.Caption = "Type";
            this.treeListColumn3.FieldName = "Type";
            this.treeListColumn3.VisibleIndex = 1;
            this.treeListColumn3.Visible = true;

            this.treeListColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn4.Caption = "Size(Bytes)";
            this.treeListColumn4.FieldName = "Size";
            this.treeListColumn4.VisibleIndex = 2;
            this.treeListColumn4.Visible = true;

            this.treeListColumn5.Caption = "treeListColumn5";
            this.treeListColumn5.FieldName = "Info";
            this.treeListColumn5.Name = "treeListColumn5";


            Tree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn2,
            this.treeListColumn3,
            this.treeListColumn4,
            this.treeListColumn5});

        }

        private void InitData()
        {
            rootPath = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            InitFolders(rootPath, null);
        }

        private void InitFolders(string path, TreeListNode pNode)
        {
            Tree.BeginUnboundLoad();
            TreeListNode node;
            DirectoryInfo di;
            try
            {
                string[] root = Directory.GetDirectories(path);
                foreach (string s in root)
                {
                    try
                    {
                        di = new DirectoryInfo(s);
                        node = Tree.AppendNode(new object[] { s, di.Name, "Folder", null, di }, pNode);
                        node.StateImageIndex = 0;
                        node.HasChildren = HasFiles(s);
                        if (node.HasChildren)
                            node.Tag = true;
                    }
                    catch { }
                }
            }
            catch { }
            InitFiles(path, pNode);
            Tree.EndUnboundLoad();
        }

        private void InitFiles(string path, TreeListNode pNode)
        {
            TreeListNode node;
            FileInfo fi;
            try
            {
                string[] root = Directory.GetFiles(path);
                foreach (string s in root)
                {
                    fi = new FileInfo(s);
                    node = Tree.AppendNode(new object[] { s, fi.Name, "File", fi.Length, fi }, pNode);
                    node.StateImageIndex = 1;
                    node.HasChildren = false;
                }
            }
            catch { }
        }

        private bool HasFiles(string path)
        {
            string[] root = Directory.GetFiles(path);
            if (root.Length > 0) return true;
            root = Directory.GetDirectories(path);
            if (root.Length > 0) return true;
            return false;
        }

        private void treeList1_BeforeExpand(object sender, DevExpress.XtraTreeList.BeforeExpandEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                Cursor currentCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                InitFolders(e.Node.GetDisplayText("FullName"), e.Node);
                e.Node.Tag = null;
                Cursor.Current = currentCursor;
            }
        }

        private void treeList1_AfterExpand(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            if (e.Node.StateImageIndex != 1) e.Node.StateImageIndex = 2;
        }

        private void treeList1_AfterCollapse(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            if (e.Node.StateImageIndex != 1) e.Node.StateImageIndex = 0;
        }

        private void treeList1_CalcNodeDragImageIndex(object sender, DevExpress.XtraTreeList.CalcNodeDragImageIndexEventArgs e)
        {
            if (e.Node[treeListColumn3].ToString() == "Folder")
            {
                e.ImageIndex = 0;
            }
            if (e.Node[treeListColumn3].ToString() == "File")
            {
                if (e.Node.ParentNode == Tree.FocusedNode.ParentNode)
                {
                    e.ImageIndex = -1;
                    return;
                }
                if (e.ImageIndex == 0)
                    if (e.Node.Id > Tree.FocusedNode.Id)
                        e.ImageIndex = 2;
                    else
                        e.ImageIndex = 1;
            }
        }

        private void treeList1_DragDrop(object sender, DragEventArgs e)
        {
            TreeListNode draggedNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
            TreeListNode tagretNode = Tree.ViewInfo.GetHitTest(Tree.PointToClient(new Point(e.X, e.Y))).Node;
            if (tagretNode == null || draggedNode == null) return;
            if (tagretNode[treeListColumn3].ToString() == "File")
            {
                if (tagretNode.ParentNode != draggedNode.ParentNode)
                    MoveInFolder(draggedNode, tagretNode.ParentNode);
            }
            else
            {
                MoveInFolder(draggedNode, tagretNode);
            }
            e.Effect = DragDropEffects.None;
        }

        void MoveInFolder(TreeListNode sourceNode, TreeListNode destNode)
        {
            Tree.MoveNode(sourceNode, destNode);
            if (sourceNode == null) return;
            FileSystemInfo sourceInfo = sourceNode[treeListColumn5] as FileSystemInfo;
            string sourcePath = sourceInfo.FullName;

            string destPath;
            if (destNode == null)
                destPath = rootPath + sourceInfo.Name;
            else
            {
                DirectoryInfo destInfo = destNode[treeListColumn5] as DirectoryInfo;
                destPath = destInfo.FullName + "\\" + sourceInfo.Name;
            }
            if (sourceInfo is DirectoryInfo)
                Directory.Move(sourcePath, destPath);
            else
                File.Move(sourcePath, destPath);
            sourceNode[treeListColumn5] = new DirectoryInfo(destPath);
        }

        private void treeList1_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as TreeList).FocusedNode[treeListColumn3].ToString() == "File")
                Process.Start(((sender as TreeList).FocusedNode[treeListColumn5] as FileSystemInfo).FullName, null);
        }

    }
}
