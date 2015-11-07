using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tools;

namespace MusicManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InputFileTypes userInput = new InputFileTypes("");

        public MainWindow()
        {
            InitializeComponent();
            initDefaultSettings();
            TextBox_MusicFileTypesInput.KeyDown += TextBox_MusicFileTypesInput_KeyDown;
            //删除文件夹
            this.KeyDown += MainWindow_KeyDown;
            //鼠标右键菜单 打开fileExplorer


        }



        private string _treeDB_FileName;
        public string TreeDB_FileName
        {
            get
            {
                return _treeDB_FileName;
            }
            set
            {
                _treeDB_FileName = value;
            }
        }

        private string _fileTypes_FileName;
        public string FileTypes_FileName
        {
            get
            {
                return _fileTypes_FileName;
            }
            set
            {
                _fileTypes_FileName = value;
            }
        }

        //
        private string _fileTypes;
        public string FileTypes
        {
            set
            {
                _fileTypes = value;
            }
            get
            {
                return _fileTypes;
            }
        }

        private void initDefaultSettings()
        {
            TreeDB_FileName = "TreeDB.txt";

            if (File.Exists(TreeDB_FileName))
            {
                //MessageBox.Show("Tree exists");
                initTree();
            }
            else
            {
                File.Create(TreeDB_FileName);
            }

            FileTypes_FileName = "FileTypes.txt";
            if (File.Exists(FileTypes_FileName))
            {
                FileTypes = File.ReadAllText(FileTypes_FileName);
                //TextBox内文字显示FileTypes_File.txt内文件内容
                TextBox_MusicFileTypesInput.Text = FileTypes;
                //文件后缀名列表
                List<string> fileTypes = new List<string>();
                userInput = new InputFileTypes(FileTypes);
                fileTypes = userInput.FileTypesList;


                initDataGrid();
            }
            else
            {
                File.Create(FileTypes_FileName);
            }           
        }


        //
        private void initTree()
        {
            
            
            
        }


        //
        private void initDataGrid()
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBox_MusicFileTypesInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = TextBox_MusicFileTypesInput.Text;
                userInput = new InputFileTypes(TextBox_MusicFileTypesInput.Text);
                File.WriteAllText(_fileTypes_FileName, text);
            }
            //throw new NotImplementedException();
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                removeFolder();
            }
            //throw new NotImplementedException();
        }

        private void removeFolder()
        {

        }


        private void buttonOpenMusic_Click(object sender, RoutedEventArgs e)
        {
            
        }

        //
        private void buttonModifyIndex_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            //
            List<string> selectedFolderPaths = new List<string>();

            while (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                
                //
                selectedFolderPaths.Add(path);
                //ListDirectory(FolderTreeView, path);
            }
        }





    }

}
