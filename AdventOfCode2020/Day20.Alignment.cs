using System.Collections.Concurrent;

namespace AdventOfCode2020;

public partial class Day20
{
    private class Aligner
    {
        // input
        private readonly IReadOnlyDictionary<long, Tile> _tiles;
        
        // helpers
        private readonly IReadOnlyDictionary<ushort, IReadOnlySet<long>> _matchingEdges;
        private readonly IReadOnlyDictionary<long, int> _neighbourCounts;
        
        // output
        private readonly TransformedTile[,] _result;

        public Aligner(IReadOnlyDictionary<long, Tile> tiles)
        {
            _tiles = tiles;
            _matchingEdges = FindMatchingEdges(tiles);
            _neighbourCounts = CountNeighbours(_tiles, _matchingEdges);
            
            // assume the grid is square
            var size = (int) Math.Sqrt(tiles.Count);
            _result = new TransformedTile[size,size];
        }
        
        public TransformedTile[,] Align()
        {
            for (var y = 0; y < _result.GetLength(0); y++)
            {
                for (var x = 0; x < _result.GetLength(1); x++)
                {
                    var tile = Pick(y, x);
                    _result[y, x] = Align(y, x, tile);
                }
            }
            
            return _result;
        }
        
        private Tile Pick(int y, int x)
        {
            if (x == 0 && y == 0)
            {
                // pick a random corner
                var cornerId = _neighbourCounts.First(t => t.Value == 2).Key;
                return _tiles[cornerId];
            }
            
            TransformedTile otherTile;
            int otherTileEdge;
            
            if (x == 0)
            {
                // look at the tile above
                otherTile = _result[y - 1, x];
                otherTileEdge = 2;
            }
            else
            {
                // look at the tile to the left
                otherTile = _result[y, x - 1];
                otherTileEdge = 1;
            }
            
            var edgeToMatch = EdgeToNum(otherTile.GetEdge(otherTileEdge));
            var tileId = _matchingEdges[edgeToMatch].First(t => t != otherTile.Tile.Id);
            
            return _tiles[tileId];
        }

        private TransformedTile Align(int y, int x, Tile tile) => EnumerateTransformations(tile).First(t => IsAligned(y, x, t));

        private bool IsAligned(int y, int x, TransformedTile tile)
        {
            var edges = tile.GetEdges().Select(EdgeToNum).ToArray();

            if (y == 0)
            {
                if (_matchingEdges[edges[0]].Count != 1) return false;
            }
            else
            {
                var otherTile = _matchingEdges[edges[0]].SingleOrDefault(t => t != tile.Tile.Id);
                if (_result[y - 1, x].Tile.Id != otherTile || EdgeToNum(_result[y - 1, x].GetEdge(2)) != edges[0]) return false;
            }

            if (x == 0)
            {
                if (_matchingEdges[edges[3]].Count != 1) return false;
            }
            else
            {
                var otherTile = _matchingEdges[edges[3]].SingleOrDefault(t => t != tile.Tile.Id);
                if (_result[y, x - 1].Tile.Id != otherTile || EdgeToNum(_result[y, x - 1].GetEdge(1)) != edges[3]) return false;
            }

            if (y + 1 == _result.GetLength(0))
            {
                if (_matchingEdges[edges[2]].Count != 1) return false;
            }

            if (x + 1 == _result.GetLength(1))
            {
                if (_matchingEdges[edges[1]].Count != 1) return false;
            }

            return true;
        }
        
        private static IReadOnlyDictionary<ushort, IReadOnlySet<long>> FindMatchingEdges(IReadOnlyDictionary<long,Tile> tiles)
        {
            var potentialMatches = new ConcurrentDictionary<ushort, HashSet<long>>();

            foreach (var (_, tile) in tiles)
            {
                var transformed = new TransformedTile(tile, Flip.None, Rotate.CW0);
                foreach (var edge in transformed.GetEdges())
                {
                    potentialMatches.GetOrAdd(EdgeToNum(edge), _ => new HashSet<long>()).Add(tile.Id);
                    potentialMatches.GetOrAdd(EdgeToNum(edge.Reverse().ToList()), _ => new HashSet<long>()).Add(tile.Id);
                }
            }

            return potentialMatches.ToDictionary(x => x.Key, x => (IReadOnlySet<long>) x.Value);
        }
        
        private static IReadOnlyDictionary<long, int> CountNeighbours(IReadOnlyDictionary<long,Tile> tiles, IReadOnlyDictionary<ushort,IReadOnlySet<long>> matchingEdges)
        {
            var pairs = matchingEdges.Where(x => x.Value.Count > 1).Select(x => x.Value).Select(x =>
            {
                if (x.Count != 2) throw new InvalidOperationException();
                
                return (x.First(), x.Last());
            }).ToList();
        
            // divide the result by 2 since there will be a forward & backward match for all edges
            return tiles.ToDictionary(x => x.Key, x => pairs.Count(p => p.Item1 == x.Key || p.Item2 == x.Key) / 2);
        }
        
        private static ushort EdgeToNum(IReadOnlyList<bool> edge)
        {
            if (edge.Count > 16) throw new ArgumentOutOfRangeException(nameof(edge));

            ushort num = 0;

            for (var i = 0; i < edge.Count; i++)
            {
                num = (ushort) (num | (edge[i] ? 1 << i : 0));
            }
        
            return num;
        }
    }
}