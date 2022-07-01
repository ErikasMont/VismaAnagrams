namespace GenericEnums;

public class GenericEnums
{
    public enum Gender: int
    {
        Male = 1,
        Female = 2,
        Other = 3
    }
    public enum Weekday
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public static TEnum MapValueToEnum<TEnum, TValue>(TValue value) 
        where TEnum : struct
    {
        if (!Enum.TryParse<TEnum>(value.ToString(), out var result))
        {
            throw new Exception($"Value '{value}' is not part of enum");
        }

        return result;
    }
}