using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
using System.Diagnostics;

namespace FileList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            new FileListHelper(treeList1);
        }

    }
}
