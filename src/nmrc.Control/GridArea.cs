using Nmrc.Control.Constants;
using Nmrc.Control.Extensions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nmrc.Control
{
    public class GridArea
    {
        private readonly int _width;
        private readonly int _height;
        private (int x, int y, Orientation o) _position;

        public GridArea(Area area)
        {
            _width = area.Width;
            _height = area.Height;
            _position = (0, 0, Orientation.North);
        }

        public (int x, int y, Orientation o) Execute(string commands)
        {
            var reg = new Regex("^([MRL]+)$");

            if (!reg.IsMatch(commands))
                throw new ArgumentOutOfRangeException(nameof(commands), "Invalid command.");

            var tokens = commands
                .Select(c => c == 'M' ? Movement.Forward : c == 'R' ? Movement.Right : Movement.Left);

            return Execute(tokens.ToArray());
        }

        private (int x, int y, Orientation o) Execute(Movement[] tokens)
        {
            if (!tokens.Any())
                return _position;

            var first = tokens.First();

            switch (first)
            {
                case Movement.Forward:
                    Forward();
                    break;
                case Movement.Left:
                    Left();
                    break;
                case Movement.Right:
                    Right();
                    break;
                default:
                    return _position;
            }
            
            return Execute(tokens[1..]);
        }

        private void Forward()
        {
            var x = _position.x;
            var y = _position.y;
            switch (_position.o)
            {
                case Orientation.North:
                    y++;
                    break;
                case Orientation.West:
                    x--;
                    break;
                case Orientation.South:
                    y--;
                    break;
                case Orientation.East:
                    x++;
                    break;
                default:
                    break;
            }

            if (!x.In(0, _width) || !y.In(0, _height))
            {
                throw new IndexOutOfRangeException("Position is out of area.");
            }

            _position = (x, y, _position.o);
        }

        /// <summary>
        /// Turn -90º (right)
        /// </summary>
        private void Right()
        {
            var o = _position.o;
            switch (o)
            {
                case Orientation.North:
                    o = Orientation.East;
                    break;
                case Orientation.West:
                    o = Orientation.North;
                    break;
                case Orientation.South:
                    o = Orientation.West;
                    break;
                case Orientation.East:
                    o = Orientation.South;
                    break;
                default:
                    break;
            }

            _position = (_position.x, _position.y, o);
        }

        /// <summary>
        /// Turn +90º (left)
        /// </summary>
        private void Left()
        {
            var o = _position.o;
            switch (o)
            {
                case Orientation.North:
                    o = Orientation.West;
                    break;
                case Orientation.West:
                    o = Orientation.South;
                    break;
                case Orientation.South:
                    o = Orientation.East;
                    break;
                case Orientation.East:
                    o = Orientation.North;
                    break;
                default:
                    break;
            }

            _position = (_position.x, _position.y, o);
        }
    }
}
