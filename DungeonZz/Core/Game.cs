﻿using DungeonZ.Systems;
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
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;
        private static readonly int _mapWidth = 80;
        private static readonly int _mapHeight = 48;
        private static readonly int _messageWidth = 80;
        private static readonly int _messageHeight = 11;
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static readonly int _inventoryWidth = 80;
        private static readonly int _inventoryHeight = 11;

        private static RLConsole _mapConsole;
        private static RLRootConsole _rootConsole;
        private static RLConsole _messageConsole;
        private static RLConsole _statConsole;
        private static RLConsole _inventoryConsole;


        private static int _mapLevel = 1;
        private static bool _renderRequired = true;

        public static Player Player { get; set; }
        public static DungeonMap DungeonMap { get; private set; }
        public static MessageLog MessageLog { get; private set; }
        public static CommandSystem CommandSystem { get; private set; }
        public static SchedulingSystem SchedulingSystem { get; private set; }
        public static IRandom Random { get; private set; }

        public static void Play()
        {
            // fixed test seed 1138043851
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            string fontFileName = "terminal16x16_gs_ro.png";
            string consoleTitle = $"DungeonZ Level {_mapLevel}";


            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 16, 16, 1.5f, consoleTitle);
            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);

            CommandSystem = new CommandSystem();
            MessageLog = new MessageLog();
            MessageLog.Add($"{Player.Name} arrives on level 1");
            SchedulingSystem = new SchedulingSystem();
            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 50, 5, 13, _mapLevel);
            DungeonMap = mapGenerator.CreateMap();
            DungeonMap.UpdatePlayerFieldOfView();
            //messages have to be added after createmap to have access to player



            //register to delegate
            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;

            //these console methods have to be run before the update and render below
            _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Swatch.DbWood);
            _inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);

            // Run RLNet game loop
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
                            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 50, 5, 13, ++_mapLevel);
                            DungeonMap = mapGenerator.CreateMap();
                            MessageLog = new MessageLog();
                            CommandSystem = new CommandSystem();
                            _rootConsole.Title = $"DungeonZ Level {_mapLevel}";
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
                //draw main console
                _rootConsole.Draw();

                _renderRequired = false;
            }
        }
    }
}
