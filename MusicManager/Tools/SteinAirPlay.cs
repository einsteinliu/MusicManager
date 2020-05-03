using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tools
{
    public struct Track
    {
        public string album;
        public string artist;
        public int begin;
        public int duration;
        public int end;
        public string filename;
        public string title;
        public int track_index;
        public int track_total;
    }
    public struct PlayList
    {
        public int focus;
        public List<Track> Tracks;
    }

    public class SteinAirPlay
    {
        public PlayList playList = new PlayList();
        string localListFile = "1588462501.locallist";
        
        public SteinAirPlay()
        {
            initAPlayList();
        }

        public void resetSystemConfig(string systemConfig)
        {
            string config = File.ReadAllText(systemConfig);
            config = setSystemConfigKeyValue(config, "\"track\":", ",\"type\":");
            config = setSystemConfigKeyValue(config, "\"offset\":", ",\"owner\":", "100");
            File.WriteAllText(systemConfig, config);
            //string config = File.ReadAllText(systemConfig);
            //int posTrack = config.IndexOf("\"track\":")+8;
            //int posType = config.IndexOf(",\"type\":");
            //config = config.Remove(posTrack, posType - posTrack);
            //config = config.Insert(posTrack, "0");
            //File.WriteAllText(systemConfig, config);
        }

        public string setSystemConfigKeyValue(string config,string keyFront, string keyAfter, string value = "0")
        {
            int posTrack = config.IndexOf(keyFront) + keyFront.Length;
            int posType = config.IndexOf(keyAfter);
            string sub = config.Substring(posTrack, posType - posTrack);
            config = config.Remove(posTrack, posType - posTrack);
            config = config.Insert(posTrack, value);
            return config;
        }

        public void writeLocalListFile(string localFile = "")
        {
            if (localFile.Length > 3)
                localListFile = localFile;

            string localListFileContent = "{\"focus\":" + playList.focus.ToString() + ",";
            localListFileContent += "\"tracks\":[";
            for (int i = 0; i < playList.Tracks.Count;i++ )
            {
                localListFileContent += formTrackInfo(playList.Tracks[i]);
                if(i!=(playList.Tracks.Count-1))
                    localListFileContent += ",";
            }
            localListFileContent += "]}";
            File.WriteAllText(localListFile, localListFileContent);
        }

        public void loadLocalListFile()
        {

        }

        string formTrackInfo(Track track)
        {
            string trackInfo = "{";
            trackInfo += "\"album\":\"" + track.album + "\",";
            trackInfo += "\"album_pic\":\"\",";
            trackInfo += "\"artist\":\"" + track.artist + "\",";
            trackInfo += "\"artist_pic\":\"\",";
            trackInfo += "\"begin\":" + track.begin.ToString() + ",";
            trackInfo += "\"duration\":" + track.duration.ToString() + ",";
            trackInfo += "\"end\":" + track.end.ToString() + ",";
            trackInfo += "\"filename\":\"" + track.filename + "\",";
            trackInfo += "\"flat\":0,";
            trackInfo += "\"lyric\":\"\",";
            trackInfo += "\"lyric_off\":0,";
            trackInfo += "\"package\":\"\",";
            trackInfo += "\"title\":\"" + track.title + "\",";
            trackInfo += "\"track_index\":" + track.track_index + ",";
            trackInfo += "\"track_total\":" + track.track_total;
            trackInfo += "}";
            return trackInfo;
        }

        void initAPlayList()
        {
            playList.focus = 0;
            playList.Tracks = new List<Track>();
            //Track track1 = new Track();
            //track1.album = "test album1";
            //track1.artist = "test artist1";
            //track1.begin = -1;
            //track1.duration = 30000;
            //track1.end = -1;
            //track1.filename = "xxx.mp3";
            //track1.title = "Arial";
            //track1.track_index = 0;
            //track1.track_total = 2;
            //playList.Tracks.Add(track1);
        }
    }
}
