using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonZ.Core
{
    public class Colors
    {
        //floors
        public static RLColor FloorBackground = RLColor.Black;
        public static RLColor Floor = Swatch.AlternateDarkest;
        public static RLColor FloorBackgroundFov = Swatch.DbDark;
        public static RLColor FloorFov = Swatch.Alternate;

        //walls
        public static RLColor WallBackground = Swatch.SecondaryDarkest;
        public static RLColor Wall = Swatch.Secondary;
        public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;
        public static RLColor WallFov = Swatch.SecondaryLighter;

        //doors
        public static RLColor DoorBackground = Swatch.ComplimentDarkest;
        public static RLColor Door = Swatch.ComplimentLighter;
        public static RLColor DoorBackgroundFov = Swatch.ComplimentDarker;
        public static RLColor DoorFov = Swatch.ComplimentLightest;

        //text
        public static RLColor TextHeading = RLColor.White;
        public static RLColor Text = Swatch.DbLight;

        //player
        public static RLColor Gold = Swatch.DbSun;
        public static RLColor Player = Swatch.DbSkin;

        //monsters
        public static RLColor KoboldColor = Swatch.DbBrightWood;
    }
}
