using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Butler.Views.Window
{
    public partial class ExceptionMessageBox
    {
        private readonly List<string> _exceptionInformationList = new List<string>();
        private readonly string _userExceptionMessage;


        public ExceptionMessageBox(Exception e, string userExceptionMessage)
        {
            InitializeComponent();

            _userExceptionMessage = userExceptionMessage;
            textBox1.Text = userExceptionMessage;

            var treeViewItem = new TreeViewItem {Header = "Exception"};
            treeViewItem.ExpandSubtree();
            BuildTreeLayer(e, treeViewItem);
            treeView1.Items.Add(treeViewItem);
        }

        private void BuildTreeLayer(Exception e, ItemsControl parent)
        {
            var exceptionInformation = "\n\r\n\r" + e.GetType() + "\n\r\n\r";
            parent.DisplayMemberPath = "Header";
            parent.Items.Add(new TreeViewStringSet {Header = "Type", Content = e.GetType().ToString()});
            var memberList = e.GetType().GetProperties();
            foreach (var info in memberList)
            {
                var value = info.GetValue(e, null);
                if (value == null) continue;
                if (info.Name == "InnerException")
                {
                    var treeViewItem = new TreeViewItem {Header = info.Name};
                    BuildTreeLayer(e.InnerException, treeViewItem);
                    parent.Items.Add(treeViewItem);
                }
                else
                {
                    var treeViewStringSet = new TreeViewStringSet {Header = info.Name, Content = value.ToString()};
                    parent.Items.Add(treeViewStringSet);
                    exceptionInformation += treeViewStringSet.Header + "\n\r\n\r" + treeViewStringSet.Content +
                                            "\n\r\n\r";
                }
            }
            _exceptionInformationList.Add(exceptionInformation);
        }


        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            textBox1.Text = e.NewValue.GetType() == typeof (TreeViewItem) ? "Exception" : e.NewValue.ToString();
        }

        private void buttonClipboard_Click(object sender, RoutedEventArgs e)
        {
            var clipboardMessage = _userExceptionMessage + "\n\r\n\r";
            clipboardMessage = _exceptionInformationList.Aggregate(clipboardMessage, (current, info) => current + info);
            Clipboard.SetText(clipboardMessage);
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private class TreeViewStringSet
        {
            public string Header { get; set; }
            public string Content { get; set; }

            public override string ToString()
            {
                return Content;
            }
        }
    }
}