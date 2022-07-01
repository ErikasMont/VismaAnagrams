namespace Tests.Tests;

public class GenericEnumsTests
{
    [TestCase("1")]
    [TestCase("Male")]
    public void MapValueToEnum_WhenGenderIsGenericAndParameterString_ReturnsGenderEnum(string value)
    {
       var result = GenericEnums.GenericEnums.MapValueToEnum<GenericEnums.GenericEnums.Gender, string>(value);
        
        result.ShouldBe(GenericEnums.GenericEnums.Gender.Male);
    }
    
    [Test]
    public void MapValueToEnum_WhenGenderIsGenericAndParameterInt_ReturnsGenderEnum()
    {
        var value = 1;
        
        var result = GenericEnums.GenericEnums.MapValueToEnum<GenericEnums.GenericEnums.Gender, int>(value);
        
        result.ShouldBe(GenericEnums.GenericEnums.Gender.Male);
    }
    
    [TestCase("0")]
    [TestCase("Monday")]
    public void MapValueToEnum_WhenWeekdayIsGenericAndParameterString_ReturnsWeekdayEnum(string value)
    { 
        var result = GenericEnums.GenericEnums.MapValueToEnum<GenericEnums.GenericEnums.Weekday, string>(value);
        
        result.ShouldBe(GenericEnums.GenericEnums.Weekday.Monday);
    }
    
    [Test]
    public void MapValueToEnum_WhenWeekdayIsGenericAndParameterInt_ReturnsGenderEnum()
    {
        var value = 0;
        
        var result = GenericEnums.GenericEnums.MapValueToEnum<GenericEnums.GenericEnums.Weekday, int>(value);
        
        result.ShouldBe(GenericEnums.GenericEnums.Weekday.Monday);
    }

    [Test]
    public void MapValueToEnum_WhenValueDoesNotExistInEnum_ThrowsException()
    {
        var value = "Monday";

        Should.Throw<Exception>(() =>
            GenericEnums.GenericEnums.MapValueToEnum<GenericEnums.GenericEnums.Gender, string>(value));
    }
}