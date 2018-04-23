Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.IO
Imports DevExpress.XtraTreeList.Nodes
Imports DevExpress.XtraTreeList
Imports System.Diagnostics

Namespace FileList
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
			Dim TempFileListHelper As FileListHelper = New FileListHelper(treeList1)
		End Sub

	End Class
End Namespace
