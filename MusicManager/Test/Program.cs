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

            Tools.MusicFile musicFileTest = new MusicFile(@"F:\music\Mozart\Mozart - Violin Concertos\cd1\CDImage.ape.cue");
            musicFileTest.test();
        }
    }
}
