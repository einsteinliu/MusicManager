using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Tools
{
    public class SteinFolders
    {
        Dictionary<string, string> filteredFileList = null;
        string[] exts = {"mp3","ape","cue","wv","wav","flac","ogg","wma" };
        public List<FileInfo> extractFilteredFileList(DirectoryInfo Folder)
        {
            List<FileInfo> currFileList = new List<FileInfo>();

            List<FileInfo> files = Folder.GetFiles().ToList();
            files = filterFiles(files, exts);
            currFileList.AddRange(files);

            List<DirectoryInfo> folders = Folder.GetDirectories().ToList();
            foreach(DirectoryInfo folder in folders)
            {
                List<FileInfo> currFiles = extractFilteredFileList(folder);
                currFileList.AddRange(currFiles);
            }

            return currFileList;
        }

        List<FileInfo> filterFiles(List<FileInfo> files,string[] allowedExts)
        {
            List<FileInfo> filteredFiles = files.ToArray().Where(file => allowedExts.Any(file.Name.ToLower().EndsWith)).ToList();

            List<FileInfo> cueFiles = new List<FileInfo>();
            List<FileInfo> otherFiles = new List<FileInfo>();
            foreach (FileInfo file in filteredFiles)
            {
                if (file.Extension.ToLower() == ".cue")
                    cueFiles.Add(file);
                else
                    otherFiles.Add(file);
            }

            if (cueFiles.Count > 0)
                return cueFiles;
            else
                return otherFiles;

        }
    }
}
