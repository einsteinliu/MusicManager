using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CueSharp;
using TagLib;

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

        public List<Track> extractTracksFromCue(string cueFile)
        {
            List<Track> tracks = new List<Track>();
            CueSheet cue = new CueSheet(cueFile);
            //获得cue文件所在的文件夹
            string filename = new FileInfo(cueFile).Directory.FullName;
            filename += "\\" + cue.Tracks[0].DataFile.Filename;
            TagLib.File tagFile = TagLib.File.Create(filename);
            int totalDuration = (int)tagFile.Properties.Duration.TotalMilliseconds;
            filename = filename.Replace("\\", "\\\\");
            List<CueSharp.Index> indexList = new List<Index>();
            for (int i = 0; i < cue.Tracks.Length; i++)
            {
                if (cue.Tracks[i].Indices.Length > 0)
                    indexList.Add(getMainIndex(cue.Tracks[i].Indices));
                else
                    return tracks;
            }

            for (int i = 0; i < cue.Tracks.Length; i++)
            {
                CueSharp.Track cueTrack = cue.Tracks[i];
                Track currTrack = new Track();
                currTrack.album = cue.Title;
                currTrack.artist = cue.Performer;
                currTrack.filename = filename;
                currTrack.title = cueTrack.Title;
                currTrack.track_index = cueTrack.TrackNumber;
                currTrack.track_total = cue.Tracks.Length;

                if (i < (cue.Tracks.Length - 1))
                {
                    currTrack.begin = indexList[i].Minutes * 60000 + indexList[i].Seconds * 1000 + (int)(indexList[i].Frames*13.3333333);
                    currTrack.end = indexList[i+1].Minutes * 60000 + indexList[i+1].Seconds * 1000 + (int)(indexList[i+1].Frames * 13.3333333);
                }
                else
                {
                    currTrack.begin = indexList[i].Minutes * 60000 + indexList[i].Seconds * 1000 + (int)(indexList[i].Frames * 13.3333333);
                    currTrack.end = totalDuration;
                }
                currTrack.duration = currTrack.end - currTrack.begin;
                tracks.Add(currTrack);
            }
            return tracks;
        }

        CueSharp.Index getMainIndex(CueSharp.Index[] indexs)
        {//主要的index是序号为1的index,序号为0的index代表音轨的空白部分
            for(int i=0;i<indexs.Length;i++)
            {
                if (indexs[i].Number == 1)
                    return indexs[i];
            }
            return indexs[0];
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
