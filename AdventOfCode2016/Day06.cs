using System.Text;

namespace AdventOfCode2016;

[Day]
public partial class Day06 : LineDay<string, string, string>
{
    private const string Sample = "eedadn\ndrvtee\neandsr\nraavrd\natevrs\ntsrnev\nsdttsa\nrasrtv\nnssdts\nntnada\nsvetve\ntesnvt\nvntsnd\nvrdear\ndvrsen\nenarar";
    
    protected override string ParseLine(string input) => input;
    
    [Sample(Sample, "easter")]
    protected override string Part1(IEnumerable<string> input)
    {
        var inputList = input.ToList();
        var output = new StringBuilder();

        for (var i = 0; i < inputList[0].Length; i++)
        {
            var freq = inputList.Select(x => x[i]).ToFrequency();
            output.Append(freq.MaxBy(x => x.Value).Key);
        } 
        
        return output.ToString();
    }

    [Sample(Sample, "advent")]
    protected override string Part2(IEnumerable<string> input)
    {
        var inputList = input.ToList();
        var output = new StringBuilder();

        for (var i = 0; i < inputList[0].Length; i++)
        {
            var freq = inputList.Select(x => x[i]).ToFrequency();
            output.Append(freq.MinBy(x => x.Value).Key);
        } 
        
        return output.ToString();
    }
}
