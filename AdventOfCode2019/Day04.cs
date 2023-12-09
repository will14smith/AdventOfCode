namespace AdventOfCode2019;

[Day]
public partial class Day04 : Day<Day04.Model, int, int>
{
    protected override Model Parse(string input)
    {
        var parts = input.Split('-');
        return new Model(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    [Sample("111111-111111", 1)]
    [Sample("223450-223450", 0)]
    [Sample("123789-123789", 0)]
    protected override int Part1(Model input)
    {
        var valid = 0;
        for (var password = input.Start; password <= input.End; password++)
        {
            var hasDouble = false;
            var neverDecreases = true;

            var str = password.ToString();
            for (var i = 0; i < str.Length - 1; i++)
            {
                if (str[i] == str[i + 1])
                {
                    hasDouble = true;
                }
                
                if (str[i] > str[i + 1])
                {
                    neverDecreases = false;
                } 
            }
            
            if (hasDouble && neverDecreases)
            {
                valid++;
            }
        }

        return valid;
    }
    
    [Sample("112233-112233", 1)]
    [Sample("123444-123444", 0)]
    [Sample("111122-111122", 1)]
    protected override int Part2(Model input)
    {
        var valid = 0;
        
        for (var password = input.Start; password <= input.End; password++)
        {
            var groups = new List<(char Digit, int Size)>();

            var str = password.ToString();
            
            var index = 0;
            while (index < str.Length)
            {
                var groupLength = 0;
                for (var i = index; i < str.Length; i++)
                {
                    if (str[index] == str[i]) groupLength++;
                    else break;
                }
                
                groups.Add((str[index], groupLength));
                index += groupLength;
            }

            var hasDouble = false;
            var neverDecreases = true;

            for (var i = 0; i < groups.Count; i++)
            {
                if (groups[i].Size == 2)
                {
                    hasDouble = true;
                }
                
                if (i + 1 < groups.Count && groups[i].Digit > groups[i + 1].Digit)
                {
                    neverDecreases = false;
                } 
            }
                        
            if (hasDouble && neverDecreases)
            {
                valid++;
            }
        }
        
        return valid;
    }

    public record Model(int Start, int End);
}
