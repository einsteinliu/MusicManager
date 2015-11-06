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
            string filePath = @"F:\music\Mozart\Mozart - Violin Concertos\cd1\img538.jpg";
            Tools.MusicFile musicFileTest = new MusicFile(filePath);
            musicFileTest.test();
        }
    }
}
