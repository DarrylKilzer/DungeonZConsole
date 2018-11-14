using DungeonZ.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonZ
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("What is your Name?");
            Game.Player = new Player(new string(Console.ReadLine().Where(Char.IsLetterOrDigit).ToArray()));
            Game.Play();
        }
    }
}
