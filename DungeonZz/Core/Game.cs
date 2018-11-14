using DungeonZ.Systems;
using RLNET;
using RogueSharp;
using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonZ.Core
{
    public class Game
    {
        // The screen height and width are in number of tiles this will change based on scale passed in root!
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;
        private static RLRootConsole _rootConsole;

        private static readonly int _mapWidth = 80;
        private static readonly int _mapHeight = 48;
        private static RLConsole _mapConsole;

        //for attacks and stuff, might split?
        private static readonly int _messageWidth = 80;
        private static readonly int _messageHeight = 11;
        private static RLConsole _messageConsole;

        // player and monster stats, again might split?
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static RLConsole _statConsole;

        // players stuff, like equipment, abilities, and items, sub consoles again?
        private static readonly int _inventoryWidth = 80;
        private static readonly int _inventoryHeight = 11;
        private static RLConsole _inventoryConsole;

        private static bool _renderRequired = true;

        //systems
        public static CommandSystem CommandSystem { get; private set; }
        public static DungeonMap DungeonMap { get; private set; }
        public static MessageLog MessageLog { get; private set; }
        public static SchedulingSystem SchedulingSystem { get; private set; }

        public static IRandom Random { get; private set; }
        public static Player Player { get; set; }
        private static int _mapLevel = 1;
        public static int seed = (int)DateTime.UtcNow.Ticks;

        public static void Play()
        {
            string fontFileName = "terminal16x16_gs_ro.png";
            // for testing use 1138043851
            Random = new DotNetRandom(seed);
            //TODO: Take seed out after debugging
            string consoleTitle = $"D$ DungeonZ Level {_mapLevel} - Seed {seed}";

            //setup systems
            SchedulingSystem = new SchedulingSystem();
            CommandSystem = new CommandSystem();
            MessageLog = new MessageLog();
            Player = new Player();

            // Dont change map width or height
            //Attempting to make 20 rooms that are between 5 and 13 cells for room size
            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 50, 5, 13, _mapLevel);

            DungeonMap = mapGenerator.CreateMap();
            DungeonMap.UpdatePlayerFieldOfView();

            // first numbers effect tile size
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 16, 16, 1.5f, consoleTitle);

            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);

            //these console methods have to be run before the update and render below
            _mapConsole.SetBackColor(0, 0, _mapWidth, _mapHeight, Colors.FloorBackground);
            _mapConsole.Print(1, 1, "Map", Colors.TextHeading);

            // Create a new MessageLog and print the random seed used to generate the level
            MessageLog.Add($"{Player.Name} arrives on level 1");
            MessageLog.Add($"Level created with seed '{seed}'");

            // Remove these lines:
            _messageConsole.SetBackColor(0, 0, _messageWidth, _messageHeight, Swatch.DbDeepWater);
            _messageConsole.Print(1, 1, "Messages", Colors.TextHeading);

            _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Swatch.DbWood);
            _inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);

            // have to register events for RLNet for update and render
            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;
            _rootConsole.Run();

        }

        // Event handler for Update event
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            //might move this junk out of consoleupdate to clean up, but this moves char for now
            bool didPlayerAct = false;
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            if (CommandSystem.IsPlayerTurn)
            {
                if (keyPress != null)
                {
                    if (keyPress.Key == RLKey.Up)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                    }
                    else if (keyPress.Key == RLKey.Down)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                    }
                    else if (keyPress.Key == RLKey.Left)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                    }
                    else if (keyPress.Key == RLKey.Right)
                    {
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                    }
                    else if (keyPress.Key == RLKey.Escape)
                    {
                        _rootConsole.Close();
                    }
                    else if (keyPress.Key == RLKey.Period)
                    {
                        if (DungeonMap.CanMoveDownToNextLevel())
                        {
                            _mapLevel += 1;
                            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 50, 5, 13, _mapLevel);
                            DungeonMap = mapGenerator.CreateMap();
                            MessageLog = new MessageLog();
                            CommandSystem = new CommandSystem();
                            string consoleTitle = $"D$ DungeonZ Level {_mapLevel} - Seed {seed}";
                            didPlayerAct = true;
                        }
                    }
                }

                if (didPlayerAct)
                {
                    _renderRequired = true;
                    CommandSystem.EndPlayerTurn();
                }
            }
            else
            {
                CommandSystem.ActivateMonsters();
                _renderRequired = true;
            }

        }

        // Event handler for Render event
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            //dont want to redraw all the time for no reason
            if (_renderRequired)
            {
                //clear all
                _mapConsole.Clear();
                _statConsole.Clear();
                _messageConsole.Clear();

                // draw the map, this has to be first i think
                DungeonMap.Draw(_mapConsole, _statConsole);
                Player.Draw(_mapConsole, DungeonMap);
                Player.DrawStats(_statConsole);
                MessageLog.Draw(_messageConsole);

                //combine all the smaller consoles to the main one
                RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight,
                  _rootConsole, 0, _inventoryHeight);
                RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight,
                  _rootConsole, _mapWidth, 0);
                RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight,
                  _rootConsole, 0, _screenHeight - _messageHeight);
                RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight,
                  _rootConsole, 0, 0);
                _rootConsole.Draw();
            }
        }
    }
}
