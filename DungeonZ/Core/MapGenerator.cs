using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonZ.Core
{
    public class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;

        private readonly DungeonMap _map;

        public MapGenerator(int width, int height,
        int maxRooms, int roomMinSize, int roomMaxSize)
        {
            _width = width;
            _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _map = new DungeonMap();
        }

        private void PlacePlayer()
        {
            Player player = Game.Player;
            if (player == null)
            {
                player = new Player();
            }

            player.X = _map.Rooms[0].Center.X;
            player.Y = _map.Rooms[0].Center.Y;

            _map.AddPlayer(player);
        }

        
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                _map.SetCellProperties(x, yPosition, true, true);
            }
        }

        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                _map.SetCellProperties(xPosition, y, true, true);
            }
        }

        // this is going to create a new map it places rooms randomly
        //currently not very good at placing rooms, probably a lot of open space unused
        // need to take a look at retrying intersecting room placement more often
        public DungeonMap CreateMap()
        {
            // set the properties of all cells to false
            _map.Initialize(_width, _height);

            for (int r = _maxRooms; r > 0; r--)
            {
                //all the randoms for room values
                int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
                int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

                var newRoom = new Rectangle(roomXPosition, roomYPosition,
                  roomWidth, roomHeight);

                //make sure no intersections, clean space to place
                bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));

                // as long as it doesn't intersect add it to the list of rooms
                //intersecting rooms are lost for now
                if (!newRoomIntersects)
                {
                    _map.Rooms.Add(newRoom);
                }
            }

            // Iterate through each room that was generated
            // Don't do anything with the first room, so start at r = 1 instead of r = 0
            for (int r = 1; r < _map.Rooms.Count; r++)
            {
                // For all remaing rooms get the center of the room and the previous room
                int previousRoomCenterX = _map.Rooms[r - 1].Center.X;
                int previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
                int currentRoomCenterX = _map.Rooms[r].Center.X;
                int currentRoomCenterY = _map.Rooms[r].Center.Y;

                // Give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
                if (Game.Random.Next(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                }
            }

            // call CreateRoom to set the room values using the rectangle placeholder for room
            foreach (Rectangle room in _map.Rooms)
            {
                CreateRoom(room);
            }

            //after map is made, add players and monsters and things
            PlacePlayer();
            PlaceMonsters();

            return _map;
        }

        // set the cell properties for that area to true
        //need to play around with the over values later
        private void CreateRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    //set last to true to see rooms without exploring for dev
                    _map.SetCellProperties(x, y, true, true, false);
                }
            }
        }

        private void PlaceMonsters()
        {
            foreach (var room in _map.Rooms)
            {
                // Each room has a 60% chance of having monsters
                if (Dice.Roll("1D10") < 7)
                {
                    // Generate between 1 and 4 monsters
                    var numberOfMonsters = Dice.Roll("1D4");
                    for (int i = 0; i < numberOfMonsters; i++)
                    {
                        Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom(room);
                        // if room doesnt have space, skip creating the monster
                        if (randomRoomLocation != null)
                        {
                            // hard coded to be created at level 1
                            //map generator will need to know the level of map its creating.
                            var monster = Kobold.Create(1);
                            monster.X = randomRoomLocation.X;
                            monster.Y = randomRoomLocation.Y;
                            _map.AddMonster(monster);
                        }
                    }
                }
            }
        }
    }
}





//single room with walls around outside
//public class MapGenerator
//{
//    private readonly int _width;
//    private readonly int _height;

//    private readonly DungeonMap _map;

//    public MapGenerator(int width, int height)
//    {
//        _width = width;
//        _height = height;
//        _map = new DungeonMap();
//    }

//    public DungeonMap CreateMap()
//    {
//        // Initialize every cell in the map by
//        // setting walkable, transparency, and explored to true
//        _map.Initialize(_width, _height);
//        foreach (Cell cell in _map.GetAllCells())
//        {
//            _map.SetCellProperties(cell.X, cell.Y, true, true, true);
//        }

//        // Set the first and last rows in the map to not be transparent or walkable
//        foreach (Cell cell in _map.GetCellsInRows(0, _height - 1))
//        {
//            _map.SetCellProperties(cell.X, cell.Y, false, false, true);
//        }

//        // Set the first and last columns in the map to not be transparent or walkable
//        foreach (Cell cell in _map.GetCellsInColumns(0, _width - 1))
//        {
//            _map.SetCellProperties(cell.X, cell.Y, false, false, true);
//        }

//        return _map;
//    }
//}
