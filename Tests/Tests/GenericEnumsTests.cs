namespace Tests.Tests;

public class GenericEnumsTests
{
    [TestCase("1")]
    [TestCase("Male")]
    public void MapValueToEnum_WhenGenderIsGeneric_ReturnsGenderEnum(string value)
    {
       var result = GenericEnums.GenericEnums.MapValueToEnum<GenericEnums.GenericEnums.Gender>(value);
        
        result.ShouldBe(GenericEnums.GenericEnums.Gender.Male);
    }
    
    [TestCase("0")]
    [TestCase("Monday")]
    public void MapValueToEnum_WhenWeekdayIsGeneric_ReturnsWeekdayEnum(string value)
    { 
        var result = GenericEnums.GenericEnums.MapValueToEnum<GenericEnums.GenericEnums.Weekday>(value);
        
        result.ShouldBe(GenericEnums.GenericEnums.Weekday.Monday);
    }

    [Test]
    public void MapValueToEnum_WhenValueDoesNotExistInEnum_ThrowsException()
    {
        var value = "Monday";

        Should.Throw<Exception>(() =>
            GenericEnums.GenericEnums.MapValueToEnum<GenericEnums.GenericEnums.Gender>(value));
    }
}