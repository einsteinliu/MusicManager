using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            FolderTreeClass ftc = new FolderTreeClass();
            List<string> temp = new List<string>();
            temp.Add(@"F:\music\Mozart\Mozart - Violin Concertos\cd1");
            temp.Add(@"F:\music\Mozart-Requiem-Bernstein (APE)");

            //假设这是返回的被选择的文件夹路径List
            ftc.targetFolderPaths = temp;

            //得到一个装着需要的文件格式的List<string>
            string fileTypeInput = "mp3, cue, ape";
            InputFileTypes ift = new InputFileTypes(fileTypeInput);
            
            //用来装 被筛选过的文件信息
            List<FileTypeFilter> ftfList = new List<FileTypeFilter>();
            
            for (int i = 0; i < ftc.targetFolderPaths.Count; i++)
            {
                FileTypeFilter ftf = new FileTypeFilter(ift.FileTypesList, ftc.targetFolderPaths[i]);
                ftfList.Add(ftf);
            }

            //test
            for (int i = 0; i < ftc.targetFolderPaths.Count; i++)
            {
                for (int j = 0; j < ftfList[i].Count; j++)
                {
                    Console.WriteLine(ftfList[i].FileNames[j]);
                    Console.WriteLine(ftfList[i].FilePaths[j]);
                    Console.WriteLine();
                }
            }
            string filePath = @"F:\music\Mozart\Mozart - Violin Concertos\cd1\img538.jpg";
            Tools.MusicFile musicFileTest = new MusicFile(filePath);
            musicFileTest.test();
        }
    }
}
