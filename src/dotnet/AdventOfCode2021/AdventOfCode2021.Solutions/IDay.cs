namespace AdventOfCode2021.Solutions;

public interface IDay
{
    int DayNumber
    {
        get
        {
            var typeName = GetType().Name;
            if (typeName.StartsWith("Day"))
            {
                var number = int.Parse(typeName.Substring(3));
                return number;
            }
            throw new InvalidOperationException("Not supported type! Implement DayNumber property");
        }
    }

    long CalculatePartOne() => 0;
    
    long CalculatePartTwo() => 0;
}
