using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2020;

[Day]
public partial class Day04 : ParseDay<Day04.Model[], int, int>
{
    private const string Sample = "ecl:gry pid:860033327 eyr:2020 hcl:#fffffd\nbyr:1937 iyr:2017 cid:147 hgt:183cm\n\niyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884\nhcl:#cfa07d byr:1929\n\nhcl:#ae17e1 iyr:2013\neyr:2024\necl:brn pid:760753108 byr:1931\nhgt:179cm\n\nhcl:#cfa07d eyr:2025 pid:166559648\niyr:2011 ecl:brn hgt:59in";

    private static readonly TextParser<string> FieldName = Character.Lower.AtLeastOnce().Select(x => new string(x));
    private static readonly TextParser<string> FieldValue = Character.Except(char.IsWhiteSpace, "Non-whitespace char").AtLeastOnce().Select(x => new string(x));
    private static readonly TextParser<(string Name, string Value)> Field =
        from name in FieldName
        from _ in Character.EqualTo(':')
        from value in FieldValue
        select (name, value);
    private static readonly TextParser<string> FieldSep = Character.EqualTo(' ').Select(_ => " ").Or(SuperpowerExtensions.NewLine);
    private static readonly TextParser<Model> PassportParser = Field.Try().AtLeastOnceDelimitedBy(FieldSep)
        .Select(xs => xs.ToDictionary(x => x.Name, x => x.Value))
        .Select(x => new Model(x));
    private static readonly TextParser<string> PassportSep = SuperpowerExtensions.NewLine.Then(_ => SuperpowerExtensions.NewLine);
    private static readonly TextParser<Model[]> PassportsParser = PassportParser.ManyDelimitedBy(PassportSep);

    protected override TextParser<Model[]> Parser => PassportsParser;

    [Sample(Sample, 2)]
    protected override int Part1(Model[] input) => input.Count(x => x.HasRequiredFields());
   
    [Sample(Sample, 2)]
    protected override int Part2(Model[] input) => input.Count(x => x.IsValid());

    public record Model(IReadOnlyDictionary<string, string> Fields)
    {
        private static readonly ISet<string> ValidEyeColours = new HashSet<string> { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
        private static readonly IReadOnlyDictionary<string, Func<string, bool>> RequiredFieldValidators = new Dictionary<string, Func<string, bool>>
        {
            { "byr", s => int.TryParse(s, out var x) && x >= 1920 && x <= 2002 },
            { "iyr", s => int.TryParse(s, out var x) && x >= 2010 && x <= 2020 },
            { "eyr", s => int.TryParse(s, out var x) && x >= 2020 && x <= 2030 },
            { "hgt", ValidateHeight },
            { "hcl", s => s.Length == 7 && s[0] == '#' && s.Skip(1).All(x => x.IsHexChar()) },
            { "ecl", s => ValidEyeColours.Contains(s) },
            { "pid", s => s.Length == 9 && s.All(x => x.IsDigit()) },
            // "cid",
        };
        
        public bool HasRequiredFields()
        {
            return RequiredFieldValidators.Keys.All(fieldName => Fields.ContainsKey(fieldName));
        }
        public bool IsValid()
        {
            foreach (var (fieldName, validator) in RequiredFieldValidators)
            {
                if (!Fields.TryGetValue(fieldName, out var fieldValue))
                {
                    return false;
                }

                if (!validator(fieldValue))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateHeight(string s)
        {
            if(!int.TryParse(s[..^2], out var num))
            {
                return false;
            }

            var units = s[^2..];
            return units switch
            {
                "cm" => num is >= 150 and <= 193,
                "in" => num is >= 59 and <= 76,
                _ => false,
            };
        }
    }
}
