using System.Diagnostics.CodeAnalysis;

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
        var files = new Stack<DiskEntry>();
        for (var i = 0; i < input.Map.Count; i++)
        {
            if (i % 2 == 0)
            {
                var file = DiskEntry.Allocated(i / 2, input.Map[i]);
                disk.Add(file);
                files.Push(file);
            }
            else
            {
                disk.Add(DiskEntry.Free(input.Map[i]));
            }
        }
        
        var newDisk = new List<DiskEntry>();
        foreach (var entry in disk)
        {
            if (entry.IsFree)
            {
                var remaining = entry.Size;

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
                        newDisk.Add(DiskEntry.Allocated(top.Id!.Value, remaining));
                        files.Push(DiskEntry.Allocated(top.Id.Value, top.Size - remaining));
                        remaining = 0;
                    }
                }
            } 
            else
            {
                if (entry.Id == files.Peek().Id)
                {
                    newDisk.Add(files.Pop());
                    break;
                }
                
                newDisk.Add(entry);
            }
        }

        return CalculateChecksum(newDisk);
    }
    
    [Sample("2333133121414131402", 2858L)]
    protected override long Part2(Model input)
    {
        var disk = new LinkedList<DiskEntry>();
        var files = new Stack<LinkedListNode<DiskEntry>>();

        for (var i = 0; i < input.Map.Count; i++)
        {
            var size = input.Map[i];
            if (i % 2 == 0)
            {
                files.Push(disk.AddLast(DiskEntry.Allocated(i / 2, size)));
            }
            else if(size > 0)
            {
                disk.AddLast(DiskEntry.Free(size));
            }
        }
        
        foreach (var fileEntry in files)
        {
            var file = fileEntry.Value;

            var freeEntry = disk.First;
            while (freeEntry != null)
            {
                if (freeEntry == fileEntry)
                {
                    break;
                }
                
                if (freeEntry.Value.IsFree && freeEntry.Value.Size >= file.Size)
                {
                    break;
                }
                
                freeEntry = freeEntry.Next;
            }

            // TODO check 
            if (freeEntry != null && freeEntry != fileEntry)
            {
                if (fileEntry.Previous?.Value.IsFree ?? false)
                {
                    if (fileEntry.Next?.Value.IsFree ?? false)
                    {
                        fileEntry.Previous.Value = DiskEntry.Free(fileEntry.Previous.Value.Size + file.Size + fileEntry.Next.Value.Size);
                        disk.Remove(fileEntry.Next);
                        disk.Remove(fileEntry);
                    }
                    else
                    {
                        fileEntry.Previous.Value = DiskEntry.Free(fileEntry.Previous.Value.Size + file.Size);
                        disk.Remove(fileEntry);
                    }
                }
                else
                {
                    if (fileEntry.Next?.Value.IsFree ?? false)
                    {
                        fileEntry.Next.Value = DiskEntry.Free(file.Size + fileEntry.Next.Value.Size);
                        disk.Remove(fileEntry);
                    }
                    else
                    {
                        fileEntry.Value = DiskEntry.Free(file.Size);
                    }
                }

                var free = freeEntry.Value;
                if (free.Size > file.Size)
                {
                    disk.AddBefore(freeEntry, file);
                    freeEntry.Value = DiskEntry.Free(free.Size - file.Size);
                }
                else
                {
                    freeEntry.Value = file;
                }
            }
        }
        
        return CalculateChecksum(disk);
    }
    
    private static long CalculateChecksum(IEnumerable<DiskEntry> disk)
    {
        var checksum = 0L;
        var idx = 0L;
        
        foreach (var entry in disk)
        {
            if (!entry.IsFree)
            {
                for (var i = 0; i < entry.Size; i++)
                {
                    checksum += entry.Id.Value * idx++;
                }
            }
            else
            {
                idx += entry.Size;
            }
        }

        return checksum;
    }
    
    private class DiskEntry(long? id, long size)
    {
        public long? Id { get; } = id;
        public long Size { get; } = size;
        [MemberNotNullWhen(false, nameof(Id))]
        public bool IsFree { get; } = id is null;
        
        public static DiskEntry Allocated(long id, long size) => new(id, size);
        public static DiskEntry Free(long size) => new(null, size);
    }
}