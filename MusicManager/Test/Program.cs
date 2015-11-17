using System;
using System.Collections.Generic;
using System.IO;
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
            //测试SubfolderClass
            List<string> temp = new List<string>();
            temp.Add(@"F:\music\Mozart\Mozart - Violin Concertos");
            temp.Add(@"F:\music\Mozart-Requiem-Bernstein (APE)");
            SubfoldersClass ftc = new SubfoldersClass(temp);
            ftc.test();

            //假设这是返回的被选择的文件夹路径List
            ftc.TargetFolderPaths = temp;

            //得到一个装着需要的文件格式的List<string>
            string fileTypeInput = "mp3, cue, ape";
            InputFileTypes ift = new InputFileTypes(fileTypeInput);
            
            //用来装 被筛选过的文件信息
            List<FileTypeFilter> ftfList = new List<FileTypeFilter>();
            
            for (int i = 0; i < ftc.TargetFolderPaths.Count; i++)
            {
                FileTypeFilter ftf = new FileTypeFilter(ift.FileTypesList, ftc.TargetFolderPaths[i]);
                ftfList.Add(ftf);
            }

            //test
            for (int i = 0; i < ftc.TargetFolderPaths.Count; i++)
            {
                for (int j = 0; j < ftfList[i].Count; j++)
                {
                    Console.WriteLine(ftfList[i].FileNames[j]);
                    Console.WriteLine(ftfList[i].FilePaths[j]);
                    Console.WriteLine();
                }
            }
            string filePath = @"F:\music\Bach\Bach.-.[Goldberg.Variations(Walcha.EMI.Angle)].专辑.(Flac)\033 Aria.mp3";
            Tools.MusicFile musicFileTest = new MusicFile(filePath);
            musicFileTest.test();
            Console.WriteLine(musicFileTest.MusicDuration);
        }
    }
}
