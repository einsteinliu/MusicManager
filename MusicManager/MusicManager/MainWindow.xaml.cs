using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Tools;

namespace MusicManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InputFileTypes userInput = new InputFileTypes("");
        private Dictionary<CheckBox, DirectoryInfo> nodesDir = new Dictionary<CheckBox, DirectoryInfo>();
        private SteinFolders steinFolder = new SteinFolders();
        private SteinAirPlay steinAirPlay = new SteinAirPlay();
        private string localListFile = @"F:\备份\难得的软件\AIRPLAY_CONFIG\LOCAL\audition.locallist";
        private string systemConfig = @"F:\备份\难得的软件\AIRPLAY_CONFIG\SYSTEM\CONFIG";
        Process airPlayProcess = null;
        WindowState lastState = WindowState.Normal;
        WindowState currState = WindowState.Normal;
        IntPtr airPlayWndH;
        IntPtr mainwinWndH;
        Thread updatePosThread;
        Rect myRect = new Rect();
        public MainWindow()
        {
            InitializeComponent();
            //读入之前的文档,建立树。要不要建立之前的播放列表再议。
            initDefaultSettings();

            //添加事件: 音乐格式筛选TextBox事件。
            TextBox_MusicFileTypesInput.KeyDown += TextBox_MusicFileTypesInput_KeyDown;
            //添加事件: 删除文件夹
            this.KeyDown += MainWindow_KeyDown;
            this.Closed += MainWindow_Closed;
            mainwinWndH = Process.GetCurrentProcess().MainWindowHandle;
            this.StateChanged += MainWindow_StateChanged;
            updatePosThread = new Thread(updateAirplayPos);
            updatePosThread.Start();
            //鼠标右键菜单 打开fileExplorer,系统默认
        }

        void MainWindow_StateChanged(object sender, EventArgs e)
        {
            currState = this.WindowState;
        }

        void updateAirplayPos()
        {
            while (true)
            {
                Thread.Sleep(100);
                if ((airPlayProcess != null) && (!airPlayProcess.HasExited))
                {
                    if(currState!=lastState)
                    {
                        lastState = currState;
                        switch (currState)
                        {
                            case WindowState.Minimized:
                                ShowWindowAsync(airPlayWndH, SW_SHOWMINIMIZED);
                                break;
                            case WindowState.Maximized:
                                ShowWindowAsync(airPlayWndH, SW_SHOWMINIMIZED);
                                break;
                            case WindowState.Normal:
                                ShowWindowAsync(airPlayWndH, SW_SHOWNORMAL);
                                break;
                        }
                    }
                    if (WindowState.Normal == currState)
                    {
                        GetWindowRect(Process.GetCurrentProcess().MainWindowHandle, ref myRect);
                        SetWindowPos(airPlayWndH, 0, myRect.Left + 5, myRect.Bottom - 10, myRect.Right - myRect.Left - 10, 100, SWP_SHOWWINDOW);
                    }
                }
            }

        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            updatePosThread.Abort();
            if ((airPlayProcess != null)&&(!airPlayProcess.HasExited))
            {
                airPlayProcess.CloseMainWindow();
            }
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
            }
            else
            {
                File.Create(FileTypes_FileName);
            }
            initDataGrid();
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
            DirectoryInfo[] subDirs = rootDirectoryInfo.GetDirectories();
            for (int i = 0; i < subDirs.Length; i++)
            {
                treeView.Items.Add(CreateDirectoryNode(subDirs[i]));
            }
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
            nodesDir[cb] = directoryInfo;
            cb.Checked += cb_Checked;
            directoryNode.Selected += directoryNode_Selected;
            
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

        void cb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox selectedNode = sender as CheckBox;
            DirectoryInfo selectedDir = nodesDir[selectedNode];
            List<FileInfo> files = steinFolder.extractFilteredFileList(selectedDir);
            List<Track> tracks = new List<Track>();
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Extension.ToUpper() == ".CUE")
                {
                    List<Track> cueTracks = steinFolder.extractTracksFromCue(files[i].FullName);
                    if (cueTracks.Count > 0)
                    {
                        tracks.AddRange(cueTracks);
                    }
                }
                else
                {
                    tracks.Add(steinFolder.extrackTrackFromFile(files[i]));
                }
            }
            //steinAirPlay.playList.Tracks.Clear();
            steinAirPlay.playList.Tracks.AddRange(tracks);
            steinAirPlay.writeLocalListFile(localListFile);
            steinAirPlay.resetSystemConfig(systemConfig);
            addDataGridItems(steinAirPlay.playList.Tracks);
        }

        void directoryNode_Selected(object sender, RoutedEventArgs e)
        {
            //TreeViewItem selectedNode = sender as TreeViewItem;
            //DirectoryInfo selectedDir = nodesDir[selectedNode];
            //List<FileInfo> files = steinFolder.extractFilteredFileList(selectedDir);
            //List<Track> tracks = new List<Track>();
            //for (int i = 0; i < files.Count; i++)
            //{
            //    if (files[i].Extension.ToUpper() == ".CUE")
            //    {
            //        List<Track> cueTracks = steinFolder.extractTracksFromCue(files[i].FullName);
            //        if (cueTracks.Count > 0)
            //        {
            //            tracks.AddRange(cueTracks);
            //        }
            //    }
            //    else
            //    {
            //        tracks.Add(steinFolder.extrackTrackFromFile(files[i]));
            //    }
            //}
            //steinAirPlay.playList.Tracks.Clear();
            //steinAirPlay.playList.Tracks.AddRange(tracks);
            //steinAirPlay.writeLocalListFile(localListFile);

        }

        public List<string> initFoldePaths()
        {
            //List<string> fps = new List<string>();
            ////这里需要加入一个filebrowser用于添加目录到这个folderPath内,此处先偷懒。
            //fps.Add(@"F:\music");
            //fps.Add(@"E:\Google Drive\C#");

            return File.ReadAllLines("TreeDB.txt").ToList() ;
        }


        Dictionary<CheckBox, TreeViewItem> dic = new Dictionary<CheckBox, TreeViewItem>();  

        //
        private void initDataGrid()
        {
            addDataGridColumn("track",40);
            addDataGridColumn("album",250);
            addDataGridColumn("title",150);
            addDataGridColumn("duration",40);
        }

        private void addDataGridColumn(string name, int width=100)
        {
            DataGridTextColumn column = new DataGridTextColumn();
            column.Header = name;
            column.Binding = new Binding(name);
            column.Width = width;
            Playlist.Columns.Add(column);
        }

        public void addDataGridItems(List<Track> tracks)
        {
            int alreadyAddedItems = Playlist.Items.Count;
            for(int i=0;i<tracks.Count;i++)
            {
                Track currTrack = tracks[i];
                Playlist.Items.Add(new DataGridItem() {track = i+alreadyAddedItems,title = currTrack.title,album = currTrack.album,duration = TimeSpan.FromMilliseconds(currTrack.duration).ToString("mm\\:ss")});
            }
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

       
        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }
        private void buttonOpenMusic_Click(object sender, RoutedEventArgs e)
        {
            if (airPlayProcess == null)
            {
                GetWindowRect(Process.GetCurrentProcess().MainWindowHandle, ref myRect);
                string airplay = @"F:\备份\难得的软件\AIRPLAY.exe";
                airPlayProcess = Process.Start(airplay);
                System.Threading.Thread.Sleep(1000);
                airPlayWndH = airPlayProcess.MainWindowHandle;
                SetWindowPos(airPlayWndH, 0, myRect.Left, myRect.Bottom, 800, 100, SWP_SHOWWINDOW);
            }
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
                File.WriteAllLines("TreeDB.txt", selectedFolderPaths.ToArray());
                ListDirectory(FolderTreeView, path);
            }
        }

        void cb_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void TextBox_MusicFileTypesInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_MusicFileTypesInput_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetForegroundWindow(airPlayWndH);
            System.Windows.Forms.SendKeys.SendWait("^% ");
            //PostMessage(airPlayWndH, WM_KEYDOWN, VK_SPACE, 0);
            //PostMessage(airPlayWndH, WM_KEYUP, VK_SPACE, 0);
        }

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        [DllImport("user32.dll", EntryPoint = "ShowWindowAsync")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        public const Int32 WM_KEYDOWN = 0x0100;
        public const Int32 WM_KEYUP = 0x101;
        public const Int32 VK_SPACE = 0x20;
        const int SWP_SHOWWINDOW = 0x0040;
    }

    public class DataGridItem
    {
        public int track{ get; set; }
        public string title{ get; set; }
        public string album{ get; set; }
        public string duration{ get; set; }
    }


}
