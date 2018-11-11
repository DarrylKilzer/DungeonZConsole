using RogueSharp;
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
            // call CreateRoom to set the room values using the rectangle placeholder for room
            foreach (Rectangle room in _map.Rooms)
            {
                CreateRoom(room);
            }

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
                    _map.SetCellProperties(x, y, true, true, true);
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
