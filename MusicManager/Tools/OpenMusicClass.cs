using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools
{
    public class OpenMusicClass
    {
        //输入: 文件夹列表 
        //功能: 用airPlay打开文件夹列表 播放音乐 获取播放器.exe位置
        //输出: 下层音乐文件列表。文件夹名、文件名、格式、时长(待定)

        public OpenMusicClass(List<string> selectedFolderPath)
        {

        }

        //文件夹目录列表
        private List<string> _folderPaths = new List<string>();
        public List<string> FolderPath 
        {
            get
            {
                return _folderPaths;
            } 
            set
            {

            }
        }

        //音乐文件列表
        private List<MusicFile> _musicFiles = new List<MusicFile>();
        public List<MusicFile> MusicFiles { get; set; }


        //类测试函数
        public void test()
        {
            string folder;
            string playHistoryPath = @"AIRPLAY_CONFIG\LOCAL\1588462501.locallist";
            folder = getFolder();
            clearPlayHistory(playHistoryPath);
            openMusic(folder);
        }


        public string getFolder()
        {
            string folder = @"E:\SyncFromLaptop\Mozart.-.[Complete.Edition.Vol.4.-.Piano.Concertos.].专辑.(APE)\CD1";
            Console.WriteLine("Open the folder: " + folder);
            return folder;
        }

        public string[] getFiles(string folder)
        {
            string[] files = Directory.GetFiles(folder);
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine(files[i]);
            }

            return files;
        }

        public void openMusic(string folder)
        {
            string filepath = "\"" + folder + "\"";
            Process pro = Process.Start(@"AIRPLAY.exe", folder);
            Thread.Sleep(800);
            pro.Close();
        }

        ////判断是否为音乐格式
        //public bool isType(List<string> spcMusicTypes)
        //{
        //    bool b = false;
        //    for (int i = 0; i < spcMusicTypes.Count; i++)
        //    {
        //        b = (FileType == spcMusicTypes[i] ? true : false);
        //    }
        //    return b;
        //}

        public void clearPlayHistory(string filename)
        {

            File.Delete(filename);
        }

    }

    //DataGrid内需要的信息
    //输入:单个文件地址 
    //输出: 音乐文件名, 文件夹名、文件名、格式、时长(待定)
    public class MusicFile
    {

        public MusicFile(string fp)
        {
            FilePath = fp;
            FileName = Path.GetFileNameWithoutExtension(FilePath);
            FileType = Path.GetExtension(FilePath);
            _musicDuration = setMusicDuraion();
            //MusicDuration = 
        }

        public void test()
        {
            TagLib.File tl = TagLib.File.Create(FilePath);
            Console.WriteLine(tl.Properties.Duration.ToString());

            Console.WriteLine(FilePath);
            Console.WriteLine(FileName);
            Console.WriteLine(FileType);
           //Console.WriteLine(FileInformation);
            Console.WriteLine(MusicDuration);

        }

        private string _filePath;
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set 
            {
                _filePath = value;
            }
        }

        //private FileInfo _fileInformation;
        //public FileInfo FileInformation
        //{
        //    get
        //    {
        //        return _fileInformation;
        //    }
        //    set
        //    {
        //        _fileInformation = value;
        //    }
        //}

        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        private string _fileType;
        public string FileType
        {
            get
            {
                return _fileType;
            }
            set
            {
                _fileType = value;
            }
        }

        private string _musicDuration;
        public string MusicDuration
        {
            get
            {
                return _musicDuration;
            }
        }

        public string setMusicDuraion()
        {
            string d = null;
            TagLib.File tl = TagLib.File.Create(FilePath);
            d = tl.Properties.Duration.ToString();
            return d;
        }
    }
}
