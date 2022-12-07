using System.Collections.Immutable;

namespace AdventOfCode2022;

[Day]
public partial class Day07 : ParseDay<Day07.Model, Day07.TokenType, int, int>
{
    private const string Sample = "$ cd /\n$ ls\ndir a\n14848514 b.txt\n8504156 c.dat\ndir d\n$ cd a\n$ ls\ndir e\n29116 f\n2557 g\n62596 h.lst\n$ cd e\n$ ls\n584 i\n$ cd ..\n$ cd ..\n$ cd d\n$ ls\n4060174 j\n8033020 d.log\n5626152 d.ext\n7214296 k";
    
    [Sample(Sample, 95437)]
    protected override int Part1(Model input) => SmallerThan100K(BuildTree(input)).Sum(x => x.Size);

    [Sample(Sample, 24933642)]
    protected override int Part2(Model input)
    {
        const int diskSize = 70000000;
        const int requiredSpace = 30000000;

        var tree = BuildTree(input);

        var currentFree = diskSize - tree.Size;
        var deletionTarget = requiredSpace - currentFree;
        
        return ClosestTo(tree, deletionTarget)?.Size ?? throw new Exception("no solution!");
    }
    
    private static IReadOnlyList<Tree> SmallerThan100K(Tree tree)
    {
        var candidates = new List<Tree>();
        
        if (tree.Size <= 100_000)
        {
            candidates.Add(tree);
        }
        
        foreach (var (_, child) in tree.Directories)
        {
            candidates.AddRange(SmallerThan100K(child));
        }

        return candidates;
    }
    
    private static Tree? ClosestTo(Tree tree, int target)
    {
        Tree? candidate = null;
        
        if (tree.Size >= target)
        {
            candidate = tree;
        }
        
        foreach (var (_, child) in tree.Directories)
        {
            var childCandidate = ClosestTo(child, target);
            if (childCandidate is not null && (candidate is null || childCandidate.Size < candidate.Size))
            {
                candidate = childCandidate;
            }
        }

        return candidate;
    }
    
    private static Tree BuildTree(Model model)
    {
        var trees = new Stack<Tree>();

        foreach (var command in model.Commands)
        {
            switch (command)
            {
                case Command.ChangeDirectory cd:
                    switch (cd.RelativePath)
                    {
                        case "/": trees.Push(Tree.New("/")); break;
                        case "..": CdParent(); break;
                        default: trees.Push(trees.Peek().Directories[cd.RelativePath]); break;
                    }
                    break;

                case Command.ListDirectory ls:
                    trees.UpdateTop(tree => ls.Files.Aggregate(tree, (current, fileInfo) => AddFileInfoToTree(fileInfo, current)));
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(command));
            }
        }

        while (trees.Count > 1)
        {
            CdParent();
        }
        
        return trees.Peek();

        void CdParent()
        {
            var tree = trees.Pop();
            trees.UpdateTop(parentTree => parentTree with { Directories = parentTree.Directories.SetItem(tree.Name, tree) });
        }

        Tree AddFileInfoToTree(FileInfo fileInfo, Tree tree) =>
            fileInfo switch
            {
                FileInfo.Directory directory => tree with
                {
                    Directories = tree.Directories.Add(directory.Name, Tree.New(directory.Name))
                },
                FileInfo.File file => tree with { Files = tree.Files.Add(file.Name, file.Size) },
                _ => throw new ArgumentOutOfRangeException(nameof(fileInfo))
            };
    }

    private record Tree(string Name, ImmutableDictionary<string, int> Files, ImmutableDictionary<string, Tree> Directories)
    {
        public static Tree New(string name) => new Tree(name, ImmutableDictionary<string, int>.Empty, ImmutableDictionary<string, Tree>.Empty);
        
        public int Size => FileSize + Directories.Sum(x => x.Value.Size);
        public int FileSize => Files.Sum(x => x.Value);
    }
    
    public record Model(IReadOnlyList<Command> Commands);
    public abstract record Command
    {
        public record ChangeDirectory(string RelativePath) : Command;
        public record ListDirectory(IReadOnlyList<FileInfo> Files) : Command;
    }

    public abstract record FileInfo
    {
        public record File(string Name, int Size) : FileInfo;
        public record Directory(string Name) : FileInfo;
    }

}
