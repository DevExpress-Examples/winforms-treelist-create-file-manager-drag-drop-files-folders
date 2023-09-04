Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Diagnostics

Namespace FileList

    Public Partial Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
            Dim tmp_FileListHelper = New FileListHelper(treeList1)
        End Sub
    End Class
End Namespace
