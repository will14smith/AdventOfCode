namespace AdventOfCode2023;

public partial class Day10
{
    private class PipeBuilder
    {
        private readonly Grid<Cell> _grid;
        private readonly Position _start;
        private readonly Queue<Position> _search = new();

        private readonly DirectionHandler[] _handlers;
        
        public Neighbours StartNeighbours { get; private set; } = new(false, false, false, false);
        
        public PipeBuilder(Grid<Cell> grid)
        {
            _grid = grid;
            _start = grid.Keys().First(x => grid[x] == Cell.Start);
            
            _handlers=  new [] {
                new DirectionHandler(this, new Position(0, -1), x => x.North, new[] { Cell.NorthSouth, Cell.SouthEast, Cell.SouthWest }, x => x with { North = true }),
                new DirectionHandler(this, new Position(0, 1), x => x.South, new[] { Cell.NorthSouth, Cell.NorthEast, Cell.NorthWest }, x => x with { South = true }),
                new DirectionHandler(this, new Position(1, 0), x => x.East, new[] { Cell.EastWest, Cell.NorthWest, Cell.SouthWest }, x => x with { East = true }),
                new DirectionHandler(this, new Position(-1, 0), x => x.West, new[] { Cell.EastWest, Cell.NorthEast, Cell.SouthEast }, x => x with { West = true }),
            };
        }

        public IReadOnlySet<Position> Build()
        {
            var pipe = new HashSet<Position>();
            
            _search.Enqueue(_start);
            
            while (_search.Count > 0)
            {
                var position = _search.Dequeue();
                if (!pipe.Add(position))
                {
                    continue;
                }

                var outs = GetNeighbours(_grid[position]);
                foreach (var handler in _handlers)
                {
                    handler.Handle(outs, position);
                }
            }

            return pipe;
        }

        private class DirectionHandler
        {
            private readonly PipeBuilder _builder;
            private readonly Position _heading;
            private readonly Func<Neighbours, bool> _validOutDirection;
            private readonly IReadOnlyList<Cell> _validInDirections;
            private readonly Func<Neighbours, Neighbours> _setStartType;

            public DirectionHandler(PipeBuilder builder, Position heading, Func<Neighbours, bool> validOutDirection, IReadOnlyList<Cell> validInDirections, Func<Neighbours, Neighbours> setStartType)
            {
                _builder = builder;
                _heading = heading;
                _validOutDirection = validOutDirection;
                _validInDirections = validInDirections;
                _setStartType = setStartType;
            }

            public void Handle(Neighbours neighbours, Position position)
            {
                if (!_validOutDirection(neighbours)) return;

                var next = position + _heading;

                if (!_builder._grid.IsValid(next)) return;
                if (!_validInDirections.Contains(_builder._grid[next])) return;

                if (position == _builder._start)
                {
                    _builder.StartNeighbours = _setStartType(_builder.StartNeighbours);
                }
                
                _builder._search.Enqueue(next);
            }
        }
        
        private static Neighbours GetNeighbours(Cell cell) => cell switch
        {
            Cell.Ground => new Neighbours(North: true, South: true, East: true, West: true),
            Cell.Start => new Neighbours(North: true, South: true, East: true, West: true),
            Cell.NorthSouth => new Neighbours(North: true, South: true, East: false, West: false),
            Cell.EastWest => new Neighbours(North: false, South: false, East: true, West: true),
            Cell.NorthEast => new Neighbours(North: true, South: false, East: true, West: false),
            Cell.NorthWest => new Neighbours(North: true, South: false, East: false, West: true),
            Cell.SouthWest => new Neighbours(North: false, South: true, East: false, West: true),
            Cell.SouthEast => new Neighbours(North: false, South: true, East: true, West: false),
            
            _ => throw new ArgumentOutOfRangeException(nameof(cell), cell, null)
        };
    }
}