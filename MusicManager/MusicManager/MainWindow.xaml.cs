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
            //读入之前的文档,建立树。要不要建立之前的播放列表再议。
            initDefaultSettings();

            //添加事件: 音乐格式筛选TextBox事件。
            TextBox_MusicFileTypesInput.KeyDown += TextBox_MusicFileTypesInput_KeyDown;
            //添加事件: 删除文件夹
            this.KeyDown += MainWindow_KeyDown;
            //鼠标右键菜单 打开fileExplorer,系统默认

        }
        //存文件树的文件路径
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
        //存文件格式的文件路径
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
        //文件格式
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
        //文件树的数据库
        private SubfoldersClass _folderTree = new SubfoldersClass(new List<string>());
        public SubfoldersClass FolderTree
        {
            get
            {
                return _folderTree;
            }
        }



        //////////////////////////////////////////////
        //初始化 文件树函数 和 播放列表函数
        private void initDefaultSettings()
        {
            TreeDB_FileName = "TreeDB.txt";
            //如果文件树文件存在, 则根据文件树文件建立树。 如果不存在,那么新建文件树文件。
            if (File.Exists(TreeDB_FileName))
            {
                initTreeView();
            }
            else
            {
                File.Create(TreeDB_FileName);
            }
            //如果 存储文件类型文件存在, 则读取信息。 如果 不存在, 则新建文件类型文件。
            FileTypes_FileName = "FileTypes.txt";
            if (File.Exists(FileTypes_FileName))
            {
                //将FileTypes_File.txt内文件类型数据存入变量_fileTypes内,并且显示在窗口的TextBox内。
                FileTypes = File.ReadAllText(FileTypes_FileName);
                TextBox_MusicFileTypesInput.Text = FileTypes;
                //文件后缀名列表
                //List<string> fileTypes = new List<string>();
                //userInput = new InputFileTypes(FileTypes);
                //fileTypes = userInput.FileTypesList;
                List<string> fileTypesList = new InputFileTypes(FileTypes).FileTypesList;//这里可以吧InputFileTypes改成工具类
                //
                //
                initDataGrid();
            }
            else
            {
                File.Create(FileTypes_FileName);
            }           
        }
       
        private void initTreeView()
        {
            //List<string> treeDB_FolderPaths = File.ReadAllLines(TreeDB_FileName).ToList();
            //_folderTree = new SubfoldersClass(treeDB_FolderPaths);
            List<string> folderPaths = new List<string>(initFoldePaths());
            var nodes = FolderTreeView.Items;
            for (int i = 0; i < folderPaths.Count; i++)
            {
                ListDirectory(FolderTreeView, folderPaths[i]);
            }     
        }

        private void ListDirectory(TreeView treeView, string path)
        {
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Items.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            //建立一个StackPanel 来存 CheckBox和TextBlock
            StackPanel sp = new StackPanel();
            CheckBox cb = new CheckBox();
            TextBlock tb = new TextBlock();
            tb.Text = directoryInfo.Name;
            sp.Children.Add(cb);
            sp.Children.Add(tb);
            sp.Orientation = Orientation.Horizontal;
            cb.Click += cb_Click;
            TreeViewItem directoryNode = new TreeViewItem() { Header = sp };

            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Items.Add(CreateDirectoryNode(directory));
            }
            //**************
            //FileInfo fi = directoryInfo.GetFiles()[0];
            //显示下面的文件
            //foreach (var file in directoryInfo.GetFiles())
            //{
            //    directoryNode.Items.Add(new TreeViewItem() { Header = file.Name });
            //}
            return directoryNode;
        }

        public List<string> initFoldePaths()
        {
            List<string> fps = new List<string>();
            //这里需要加入一个filebrowser用于添加目录到这个folderPath内,此处先偷懒。
            fps.Add(@"F:\music");
            fps.Add(@"E:\Google Drive\C#");
            return fps;
        }


        Dictionary<CheckBox, TreeViewItem> dic = new Dictionary<CheckBox, TreeViewItem>();  

        //
        private void initDataGrid()
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
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


        private void MenuItem_AddFolder_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("menuItem Click");
             var dialog = new System.Windows.Forms.FolderBrowserDialog();
            //
            List<string> selectedFolderPaths = new List<string>();

            while (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;      
                //
                selectedFolderPaths.Add(path);
                ListDirectory(FolderTreeView, path);
            }
        }

        void cb_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(sender.ToString());
            
            //throw new NotImplementedException();
        }
    }


}
