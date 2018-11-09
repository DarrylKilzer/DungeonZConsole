using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonZ.Core
{
    public class Player : Actor
    {
        public Player()
        {
            Awareness = 15;
            Name = "D$";
            Color = Colors.Player;
            Symbol = '0';
            X = 10;
            Y = 10;
        }
    }
}
