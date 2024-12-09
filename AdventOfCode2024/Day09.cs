namespace AdventOfCode2024;

[Day]
public partial class Day09 : Day<Day09.Model, long, long>
{
    public record Model(IReadOnlyList<long> Map);
    protected override Model Parse(string input) => new(input.Select(c => (long)(c - '0')).ToList());

    [Sample("2333133121414131402", 1928L)]
    protected override long Part1(Model input)
    {
        var disk = new List<DiskEntry>();
        for (var i = 0; i < input.Map.Count; i++)
        {
            if (i % 2 == 0)
            {
                disk.Add(new DiskEntry.Allocated(i / 2, input.Map[i]));
            }
            else
            {
                disk.Add(new DiskEntry.Free(input.Map[i]));
            }
        }
        
        var files = new Stack<DiskEntry.Allocated>();
        disk.OfType<DiskEntry.Allocated>().ToList().ForEach(f => files.Push(f));

        var newDisk = new List<DiskEntry.Allocated>();
        foreach (var entry in disk)
        {
            if (entry is DiskEntry.Free free)
            {
                var remaining = free.Size;

                while (remaining > 0)
                {
                    var top = files.Pop();
                    if (top.Size <= remaining)
                    {
                        newDisk.Add(top);
                        remaining -= top.Size;
                    }
                    else if (top.Size > remaining)
                    {
                        newDisk.Add(new DiskEntry.Allocated(top.Id, remaining));
                        files.Push(new DiskEntry.Allocated(top.Id, top.Size - remaining));
                        remaining = 0;
                    }
                }
            } 
            else if (entry is DiskEntry.Allocated allocated)
            {
                if (allocated.Id == files.Peek().Id)
                {
                    newDisk.Add(files.Pop());
                    break;
                }
                
                newDisk.Add(allocated);
            }
        }

        return CalculateChecksum(newDisk);
    }
    
    [Sample("2333133121414131402", 2858L)]
    protected override long Part2(Model input)
    {
        var disk = new List<DiskEntry>();
        for (var i = 0; i < input.Map.Count; i++)
        {
            if (i % 2 == 0)
            {
                disk.Add(new DiskEntry.Allocated(i / 2, input.Map[i]));
            }
            else
            {
                disk.Add(new DiskEntry.Free(input.Map[i]));
            }
        }

        var files = new Stack<DiskEntry.Allocated>();
        disk.OfType<DiskEntry.Allocated>().ToList().ForEach(f => files.Push(f));

        foreach (var file in files)
        {
            var findIndex = disk.FindIndex(x => x is DiskEntry.Free free && free.Size >= file.Size);
            var fileIndex = disk.IndexOf(file);

            if (findIndex >= 0 && findIndex < fileIndex)
            {
                disk[fileIndex] = new DiskEntry.Free(file.Size);
                
                var free = (DiskEntry.Free)disk[findIndex];
                disk.RemoveAt(findIndex);

                if (free.Size > file.Size)
                {
                    disk.Insert(findIndex, new DiskEntry.Free(free.Size - file.Size));
                }

                disk.Insert(findIndex, file);

                disk = Compact(disk);
            }
        }
        
        return CalculateChecksum(disk);
    }

    private List<DiskEntry> Compact(IReadOnlyList<DiskEntry> previousDisk)
    {
        var disk = new List<DiskEntry>(previousDisk.Count);

        DiskEntry? previous = null;
        foreach (var diskEntry in previousDisk)
        {
            switch (diskEntry)
            {
                case DiskEntry.Allocated: 
                    disk.Add(previous = diskEntry); 
                    break;
                    
                case DiskEntry.Free free:
                    if (previous is DiskEntry.Free previousFree)
                    {
                        disk[^1] = previous = new DiskEntry.Free(previousFree.Size + free.Size);
                    }
                    else
                    {
                        disk.Add(previous = free);
                    }
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(diskEntry));
            }
        }

        return disk;
    }

    private static long CalculateChecksum(IReadOnlyList<DiskEntry> disk)
    {
        var checksum = 0L;
        var idx = 0L;
        
        foreach (var entry in disk)
        {
            if (entry is DiskEntry.Allocated allocated)
            {
                for (var i = 0; i < entry.Size; i++)
                {
                    checksum += allocated.Id * idx++;
                }
            }
            else
            {
                idx += entry.Size;
            }
        }

        return checksum;
    }
    
    private abstract class DiskEntry(long size)
    {
        public long Size { get; } = size;

        public class Allocated(long Id, long size) : DiskEntry(size)
        {
            public long Id { get; } = Id;
        }

        public class Free(long size) : DiskEntry(size);
    }
}