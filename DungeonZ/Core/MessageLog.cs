using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonZ.Core
{
    public class MessageLog
    {
        //max number of lines to keep
        private static readonly int _maxLines = 9;

        private readonly Queue<string> _lines;

        public MessageLog()
        {
            _lines = new Queue<string>();
        }

        public void Add(string message)
        {
            _lines.Enqueue(message);

            // When exceeding the maximum number of lines remove the oldest one.
            if (_lines.Count > _maxLines)
            {
                _lines.Dequeue();
            }
        }

        // Draw each line of the MessageLog queue to the console
        public void Draw(RLConsole console)
        {
            console.Clear();
            string[] lines = _lines.ToArray();
            for (int i = 0; i < lines.Length; i++)
            {
                console.Print(1, i + 1, lines[i], RLColor.White);
            }
        }
    }
}
