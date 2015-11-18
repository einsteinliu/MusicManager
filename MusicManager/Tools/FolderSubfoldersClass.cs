using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tools
{
    //按照选择的文件夹路径生成文件夹树, 
    //输入: 默认数据库内容 , 鼠标选择内容
    public class SubfoldersClass
    {
        public void test()
        {
            //输出文件夹和子文件夹的dictionary
            for (int i = 0; i < SubFolderDic.Count; i++)
            {
                Console.WriteLine(SubFolderDic.Keys.ElementAt<string>(i));
                Console.WriteLine();
                for (int j = 0; j < SubFolderDic.Values.ElementAt<List<string>>(i).Count; j++)
                {
                    Console.WriteLine(SubFolderDic.Values.ElementAt<List<string>>(i)[j]);
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            //输出文件夹和子文件们的dictionary
            for (int i = 0; i < SubFilePathsDic.Count; i++)
            {
                Console.WriteLine(SubFilePathsDic.Keys.ElementAt<string>(i));
                Console.WriteLine();
                for (int j = 0; j < SubFilePathsDic.Values.ElementAt<List<string>>(i).Count; j++)
                {
                    Console.WriteLine(SubFilePathsDic.Values.ElementAt<List<string>>(i)[j]);
                }
                Console.WriteLine();
                Console.WriteLine();
            }



        }

        public SubfoldersClass(List<string> folderPathList)
        {

            for (int i = 0; i < folderPathList.Count; i++)
            {
                string key = folderPathList[i];
                DirectoryInfo rootDirectoryInfo = new DirectoryInfo(folderPathList[i]);
                List<string> subFolderPathsList = new List<string>();
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPathList[i]);
                List<string> subdirectoryEntries = new List<string>();
                List<string> sl = new List<string>();
                //递归得到下级的所有文件 用于传递给FileTypeFilter
                subFolderPathsList = subFolders(directoryInfo, /*subdirectoryEntries,*/sl);
                _subFolderDic[key] = subFolderPathsList;
                //得到这些根目录下的文件们。初始化_subFilePathsDic
                SetSubfilesDic();
            }
        }

        private Dictionary<string, List<string>> _subFolderDic = new Dictionary<string,List<string>>();
        public Dictionary<string, List<string>> SubFolderDic
        {
            get
            {
                return _subFolderDic;
            }
        }

        private Dictionary<string, List<string>> _subFilePathsDic = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> SubFilePathsDic
        {
            get
            {
                return _subFilePathsDic;
            }
        }

        //得到一些根目录下所有子目录内的文件集合,Dictionary string是根目录, List<string> 内为其内部的所有文件路径。
        public Dictionary<string, List<string>> SetSubfilesDic()
        {
            //Dictionary<string, List<string>> _subFilePathsDic = new Dictionary<string, List<string>>();
            //处理单个根目录
            for (int i = 0; i < _subFolderDic.Count; i++)
            {
                List<string> filenames = new List<string>();

                //添加根目录下的文件们
                string[] targetFolderFiles = Directory.GetFiles(_subFolderDic.Keys.ElementAt<string>(i));   
                filenames.AddRange(targetFolderFiles.ToList());
                //添加子目录下的所有文件们
                for (int j = 0; j < _subFolderDic.Values.ElementAt<List<string>>(i).Count; j++)
                {
                    string folderPath = _subFolderDic.Values.ElementAt<List<string>>(i)[j];
                    string[] files = Directory.GetFiles(folderPath);
                    filenames.AddRange(files.ToList());            
                }

                _subFilePathsDic[_subFolderDic.Keys.ElementAt<string>(i)]=filenames;
            }
            return _subFilePathsDic;
        }


        public List<string> subFolders(DirectoryInfo targetDirectory, List<string> SubFolderList)
        {
            //
            DirectoryInfo[] subdirs = targetDirectory.GetDirectories();
            for (int i = 0; i < subdirs.Length; i++)
            {
                SubFolderList.Add(subdirs[i].FullName); ;
            }
            
            foreach (var di in subdirs)
            {
                subFolders(di, SubFolderList);
            }
            return SubFolderList;
        }
        
        private bool hasSubfolder(string path)
        {
            IEnumerable<string> subfolders = Directory.EnumerateDirectories(path);
            return subfolders != null && subfolders.Any();
        }

        //
        //

        //输出的targetFolderPath



        private string _initIndexFile;
        public string InitIndexFile
        {
            set
            {
                _initIndexFile = value;
            }
            get
            {
                return _initIndexFile;
            }
        }


        private List<string> _targetFolderPaths;
        public List<string> TargetFolderPaths
        {
            get
            {
                return _targetFolderPaths;
            }
            //
            set
            {
                _targetFolderPaths = value;
            }
        }

    }


    //按照输入的文件后缀名 筛选 指定文件夹内 拥有相应后缀名的文件
    //输入:string fileTypes, targetFolder
    //输出:List<string> FilePaths, string Key
    public class FileTypeFilter
    {
        public void test()
        {
            Console.WriteLine(FilePaths);
        }

        public FileTypeFilter(List<string> fileTypesList, string targetDirectory)
        {
            setFileNames(fileTypesList, targetDirectory);
        }

        private List<string> _fileNames;
        public List<string> FileNames
        {
            get
            {
                return _fileNames;
            }
        }

        private int _count = 0;
        public int Count
        {
            get
            {
                return _count;
            }
        }

        private string _key;
        public string Key
        {
            set
            {
                _key = value;
            }
            get
            {
                return _key;
            }
        }

        private List<string> _filePaths;
        public List<string> FilePaths
        {
            get
            {
                return _filePaths;
            }
        }

        public void setFileNames(List<string> fileTypeInput, string targetDirectory)
        {
            DirectoryInfo di = new DirectoryInfo(targetDirectory);
            List<string> fn = new List<string>();
            List<string> fp = new List<string>();
            for (int i = 0; i < fileTypeInput.Count; i++)
            {
                //
                string key = fileTypeInput[i];
                //
                foreach (var fi in di.GetFiles(key))
                {
                    Console.WriteLine(fi.DirectoryName);
                    fn.Add(fi.Name);
                    fp.Add(fi.FullName);
                    _count++;
                }
            }
            _fileNames = fn;
            _filePaths = fp;
        }
        //
    }

    //这部分需要存储
    public class InputFileTypes
    {
        public InputFileTypes(string input)
        {

            setFileTypes(input);
        }

        private List<string> _fileTypesList;
        public List<string> FileTypesList
        {
            get
            {
                return _fileTypesList;
            }
        }
        public void setFileTypes(string line)
        {
            List<string> fileTypesList = new List<string>();
            string[] linesplit = line.Split(',', ' ');
            for (int i = 0; i < linesplit.Length; i++)
            {
                if (linesplit[i] != "")
                {
                    fileTypesList.Add("*." + linesplit[i]);
                }
            }
            _fileTypesList = fileTypesList;
        }
    }
}
