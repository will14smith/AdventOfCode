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

        var checksum = 0L;
        var idx = 0;
        foreach (var entry in newDisk)
        {
            for (var i = 0; i < entry.Size; i++)
            {
                checksum += entry.Id * idx++;
            }
        }

        return checksum;
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
            }

            disk = Compact(disk);

            List<DiskEntry> Compact(List<DiskEntry> diskEntries)
            {
                var newDisk = new List<DiskEntry> { diskEntries[0] };

                foreach (var diskEntry in diskEntries.Skip(1))
                {
                    if (diskEntry is DiskEntry.Free currFree && newDisk[^1] is DiskEntry.Free prevFree)
                    {
                        newDisk[^1] = new DiskEntry.Free(currFree.Size + prevFree.Size);
                    }
                    else
                    {
                        newDisk.Add(diskEntry);
                    }
                }
                
                return newDisk;
            }
        }
        
        var checksum = 0L;
        var idx = 0L;
        foreach (var entry in disk)
        {
            if (entry is DiskEntry.Free free)
            {
                idx += free.Size;
            }
            else if(entry is DiskEntry.Allocated allocated)
            {
                for (var i = 0; i < allocated.Size; i++)
                {
                    checksum += allocated.Id * idx++;
                }
            }
        }

        return checksum;    }

    private abstract record DiskEntry
    {
        public record Allocated(long Id, long Size) : DiskEntry;
        public record Free(long Size) : DiskEntry;
    }
}