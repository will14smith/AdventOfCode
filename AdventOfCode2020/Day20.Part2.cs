namespace AdventOfCode2020;

public partial class Day20
{
    [Sample(Sample, 273L)]
    protected override long Part2(Model input)
    {
        var tiles = new Aligner(input.Tiles).Align();
        var image = BuildImage(tiles);
            
        MatchMonsters(image);

        return CountSea(image);
    }

    private static byte[,] BuildImage(TransformedTile[,] tiles)
    {
        var firstTile = tiles[0, 0].Tile.Image;
        var (tileSizeY, tileSizeX) = (Y: firstTile.GetLength(0) - 2, X: firstTile.GetLength(1) - 2);

        var image = new byte[tileSizeY * tiles.GetLength(0), tileSizeX * tiles.GetLength(1)];

        for (var ty = 0; ty < tiles.GetLength(0); ty++)
        {
            var yBase = ty * tileSizeY;

            for (var tx = 0; tx < tiles.GetLength(1); tx++)
            {
                var xBase = tx * tileSizeX;
                var tile = tiles[ty, tx];

                for (var y = 0; y < tileSizeY; y++)
                {
                    for (var x = 0; x < tileSizeX; x++)
                    {
                        image[yBase + y, xBase + x] = (byte)(tile.Get(y + 1, x + 1) ? 1 : 0);
                    }
                }
            }
        }

        return image;
    }

    private void MatchMonsters(byte[,] image)
    {
        for (var y = 0; y < image.GetLength(0); y++)
        {
            for (var x = 0; x < image.GetLength(1); x++)
            {
                MatchMonsters(image, y, x);
            }
        }
    }

    private void MatchMonsters(byte[,] image, in int baseY, in int baseX)
    {
        foreach (var monster in _transformedMonsters)
        {
            if (!HasMonster(image, monster, baseY, baseX))
            {
                continue;
            }

            for (var y = 0; y < monster.YSize; y++)
            {
                for (var x = 0; x < monster.XSize; x++)
                {
                    if (monster.Get(y, x))
                    {
                        image[baseY + y, baseX + x] = 2;
                    }
                }
            }
        }
    }

    private static bool HasMonster(byte[,] image, TransformedTile monster, in int baseY, in int baseX)
    {
        if (baseY + monster.YSize >= image.GetLength(0))
        {
            return false;
        }

        if (baseX + monster.XSize >= image.GetLength(1))
        {
            return false;
        }

        for (var y = 0; y < monster.YSize; y++)
        {
            for (var x = 0; x < monster.XSize; x++)
            {
                if (monster.Get(y, x))
                {
                    if (image[baseY + y, baseX + x] == 0)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private static long CountSea(byte[,] image)
    {
        var count = 0;

        for (var y = 0; y < image.GetLength(0); y++)
        {
            for (var x = 0; x < image.GetLength(1); x++)
            {
                if (image[y, x] == 1)
                {
                    count++;
                }
            }
        }

        return count;
    }
}