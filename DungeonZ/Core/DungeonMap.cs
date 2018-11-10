using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonZ.Core
{
    //Altering the base class from RogueSharp
    public class DungeonMap : Map
    {
        public List<Rectangle> Rooms { get; set; }

        public DungeonMap()
        {
            Rooms = new List<Rectangle>();
        }

        public void Draw(RLConsole mapConsole)
        {
            mapConsole.Clear();
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }
        }

        private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
        {
            //hidden
            //dont draw cell if it is not explored
            if (!cell.IsExplored)
            {
                return;
            }
            //no fog
            // if cell is currently in the field-of-view it should be drawn with light colors
            if (IsInFov(cell.X, cell.Y))
            {
                // draw the floor or a wall
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            // fog
            // if a cell is outside of the field of view draw it with dark colors
            else
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }
        }

        //
        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;
            // awareness changes the distance a player sees
            ComputeFov(player.X, player.Y, player.Awareness, true);
            // update cells to be explored
            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            // Only allow actor placement if the cell is walkable
            if (GetCell(x, y).IsWalkable)
            {
                // The cell the actor was previously on is now walkable
                SetIsWalkable(actor.X, actor.Y, true);
                // Update the actor's position
                actor.X = x;
                actor.Y = y;
                // The new cell the actor is on is now not walkable
                SetIsWalkable(actor.X, actor.Y, false);
                // Don't forget to update the field of view if we just repositioned the player
                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }

        // A helper method for setting the IsWalkable property on a Cell
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = (Cell)GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }
    }
}
