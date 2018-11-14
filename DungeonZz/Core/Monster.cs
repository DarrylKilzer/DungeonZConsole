using DungeonZ.Behaviors;
using DungeonZ.Systems;
using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonZ.Core
{
    public class Monster : Actor
    {
        public int? TurnsAlerted { get; set; }

        public void DrawStats(RLConsole statConsole, int position)
        {
            // Start at Y=13 which is below the player stats.
            // Multiply the position by 2 to leave a space between each stat
            int yPosition = 13 + (position * 2);

            statConsole.Print(1, yPosition, Symbol.ToString(), Color);
            int width = Convert.ToInt32(((double)Health / (double)MaxHealth) * 16.0);
            int remainingWidth = 16 - width;

            // Set the background colors of the health bar and show health, dmg, and name
            statConsole.SetBackColor(3, yPosition, width, 1, Swatch.Primary);
            statConsole.SetBackColor(3 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest);
            statConsole.Print(2, yPosition, $": {Name}", Swatch.DbLight);
        }

        public virtual void PerformAction(CommandSystem commandSystem)
        {
            var behavior = new StandardMoveAndAttack();
            behavior.Act(this, commandSystem);
        }
    }
}
