Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DevExpress.XtraTreeList
Imports System.IO
Imports DevExpress.XtraTreeList.Nodes
Imports System.Windows.Forms
Imports System.Diagnostics
Imports System.Drawing

Namespace FileList
	Friend Class FileListHelper
		Private rootPath As String
		Private Tree As TreeList
		Private treeListColumn1 As DevExpress.XtraTreeList.Columns.TreeListColumn
		Private treeListColumn2 As DevExpress.XtraTreeList.Columns.TreeListColumn
		Private treeListColumn3 As DevExpress.XtraTreeList.Columns.TreeListColumn
		Private treeListColumn4 As DevExpress.XtraTreeList.Columns.TreeListColumn
		Private treeListColumn5 As DevExpress.XtraTreeList.Columns.TreeListColumn


		Public Sub New(ByVal tree As TreeList)
			Me.Tree = tree
			InitColumns()
			AddHandler Me.Tree.BeforeExpand, AddressOf treeList1_BeforeExpand
			AddHandler Me.Tree.AfterExpand, AddressOf treeList1_AfterExpand
			AddHandler Me.Tree.AfterCollapse, AddressOf treeList1_AfterCollapse
			AddHandler Me.Tree.CalcNodeDragImageIndex, AddressOf treeList1_CalcNodeDragImageIndex
			AddHandler Me.Tree.DragDrop, AddressOf treeList1_DragDrop
			AddHandler Me.Tree.DoubleClick, AddressOf treeList1_DoubleClick

			InitData()
		End Sub

		Private Sub InitColumns()
			Me.treeListColumn1 = New DevExpress.XtraTreeList.Columns.TreeListColumn()
			Me.treeListColumn2 = New DevExpress.XtraTreeList.Columns.TreeListColumn()
			Me.treeListColumn3 = New DevExpress.XtraTreeList.Columns.TreeListColumn()
			Me.treeListColumn4 = New DevExpress.XtraTreeList.Columns.TreeListColumn()
			Me.treeListColumn5 = New DevExpress.XtraTreeList.Columns.TreeListColumn()

			Me.treeListColumn1.Caption = "FullName"
			Me.treeListColumn1.FieldName = "FullName"

			Me.treeListColumn2.Caption = "Name"
			Me.treeListColumn2.FieldName = "Name"
			Me.treeListColumn2.VisibleIndex = 0
			Me.treeListColumn2.Visible = True

			Me.treeListColumn3.Caption = "Type"
			Me.treeListColumn3.FieldName = "Type"
			Me.treeListColumn3.VisibleIndex = 1
			Me.treeListColumn3.Visible = True

			Me.treeListColumn4.AppearanceCell.Options.UseTextOptions = True
			Me.treeListColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
			Me.treeListColumn4.Caption = "Size(Bytes)"
			Me.treeListColumn4.FieldName = "Size"
			Me.treeListColumn4.VisibleIndex = 2
			Me.treeListColumn4.Visible = True

			Me.treeListColumn5.Caption = "treeListColumn5"
			Me.treeListColumn5.FieldName = "Info"
			Me.treeListColumn5.Name = "treeListColumn5"


			Tree.Columns.AddRange(New DevExpress.XtraTreeList.Columns.TreeListColumn() { Me.treeListColumn1, Me.treeListColumn2, Me.treeListColumn3, Me.treeListColumn4, Me.treeListColumn5})

		End Sub

		Private Sub InitData()
			rootPath = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory())
			InitFolders(rootPath, Nothing)
		End Sub

		Private Sub InitFolders(ByVal path As String, ByVal pNode As TreeListNode)
			Tree.BeginUnboundLoad()
			Dim node As TreeListNode
			Dim di As DirectoryInfo
			Try
				Dim root() As String = Directory.GetDirectories(path)
				For Each s As String In root
					Try
						di = New DirectoryInfo(s)
						node = Tree.AppendNode(New Object() { s, di.Name, "Folder", Nothing, di }, pNode)
						node.StateImageIndex = 0
						node.HasChildren = HasFiles(s)
						If node.HasChildren Then
							node.Tag = True
						End If
					Catch
					End Try
				Next s
			Catch
			End Try
			InitFiles(path, pNode)
			Tree.EndUnboundLoad()
		End Sub

		Private Sub InitFiles(ByVal path As String, ByVal pNode As TreeListNode)
			Dim node As TreeListNode
			Dim fi As FileInfo
			Try
				Dim root() As String = Directory.GetFiles(path)
				For Each s As String In root
					fi = New FileInfo(s)
					node = Tree.AppendNode(New Object() { s, fi.Name, "File", fi.Length, fi }, pNode)
					node.StateImageIndex = 1
					node.HasChildren = False
				Next s
			Catch
			End Try
		End Sub

		Private Function HasFiles(ByVal path As String) As Boolean
			Dim root() As String = Directory.GetFiles(path)
			If root.Length > 0 Then
				Return True
			End If
			root = Directory.GetDirectories(path)
			If root.Length > 0 Then
				Return True
			End If
			Return False
		End Function

		Private Sub treeList1_BeforeExpand(ByVal sender As Object, ByVal e As DevExpress.XtraTreeList.BeforeExpandEventArgs)
			If e.Node.Tag IsNot Nothing Then
				Dim currentCursor As Cursor = Cursor.Current
				Cursor.Current = Cursors.WaitCursor
				InitFolders(e.Node.GetDisplayText("FullName"), e.Node)
				e.Node.Tag = Nothing
				Cursor.Current = currentCursor
			End If
		End Sub

		Private Sub treeList1_AfterExpand(ByVal sender As Object, ByVal e As DevExpress.XtraTreeList.NodeEventArgs)
			If e.Node.StateImageIndex <> 1 Then
				e.Node.StateImageIndex = 2
			End If
		End Sub

		Private Sub treeList1_AfterCollapse(ByVal sender As Object, ByVal e As DevExpress.XtraTreeList.NodeEventArgs)
			If e.Node.StateImageIndex <> 1 Then
				e.Node.StateImageIndex = 0
			End If
		End Sub

		Private Sub treeList1_CalcNodeDragImageIndex(ByVal sender As Object, ByVal e As DevExpress.XtraTreeList.CalcNodeDragImageIndexEventArgs)
			If e.Node(treeListColumn3).ToString() = "Folder" Then
				e.ImageIndex = 0
			End If
			If e.Node(treeListColumn3).ToString() = "File" Then
				If e.Node.ParentNode Is Tree.FocusedNode.ParentNode Then
					e.ImageIndex = -1
					Return
				End If
				If e.ImageIndex = 0 Then
					If e.Node.Id > Tree.FocusedNode.Id Then
						e.ImageIndex = 2
					Else
						e.ImageIndex = 1
					End If
				End If
			End If
		End Sub

		Private Sub treeList1_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
			Dim draggedNode As TreeListNode = TryCast(e.Data.GetData(GetType(TreeListNode)), TreeListNode)
			Dim tagretNode As TreeListNode = Tree.ViewInfo.GetHitTest(Tree.PointToClient(New Point(e.X, e.Y))).Node
			If tagretNode Is Nothing OrElse draggedNode Is Nothing Then
				Return
			End If
			If tagretNode(treeListColumn3).ToString() = "File" Then
                If tagretNode.ParentNode IsNot draggedNode.ParentNode Then
                    MoveInFolder(draggedNode, tagretNode.ParentNode)
                End If

			Else
				MoveInFolder(draggedNode, tagretNode)
			End If
			e.Effect = DragDropEffects.None
		End Sub

		Private Sub MoveInFolder(ByVal sourceNode As TreeListNode, ByVal destNode As TreeListNode)
			Tree.MoveNode(sourceNode, destNode)
			If sourceNode Is Nothing Then
				Return
			End If
			Dim sourceInfo As FileSystemInfo = TryCast(sourceNode(treeListColumn5), FileSystemInfo)
			Dim sourcePath As String = sourceInfo.FullName

			Dim destPath As String
			If destNode Is Nothing Then
				destPath = rootPath & sourceInfo.Name
			Else
				Dim destInfo As DirectoryInfo = TryCast(destNode(treeListColumn5), DirectoryInfo)
				destPath = destInfo.FullName & "\" & sourceInfo.Name
			End If
			If TypeOf sourceInfo Is DirectoryInfo Then
				Directory.Move(sourcePath, destPath)
			Else
				File.Move(sourcePath, destPath)
			End If
			sourceNode(treeListColumn5) = New DirectoryInfo(destPath)
		End Sub

		Private Sub treeList1_DoubleClick(ByVal sender As Object, ByVal e As EventArgs)
			If (TryCast(sender, TreeList)).FocusedNode(treeListColumn3).ToString() = "File" Then
				Process.Start((TryCast((TryCast(sender, TreeList)).FocusedNode(treeListColumn5), FileSystemInfo)).FullName, Nothing)
			End If
		End Sub

	End Class
End Namespace
