using Xunit;
using ArcaneLibs.Attributes;

namespace ArcaneLibs.Tests;

public class UnitTest1 {
    [Fact]
    public void Test1() {
        Assert.Equal(2, Util.Min(2, 4, 6));
        Assert.Equal(6, Util.Max(2, 4, 6));
        Assert.Equal(2d, Util.Min(2d, 4d, 6d));
        Assert.Equal(6d, Util.Max(2d, 4d, 6d));
    }

    [Fact]
    public void TestFriendlyName() {
        Assert.Equal("Class", typeof(FriendlyName).GetFriendlyName());
        Assert.Equal("Property", typeof(FriendlyName).GetProperty("Property")?.GetFriendlyName());
        Assert.Equal("Field", typeof(FriendlyName).GetField("Field")?.GetFriendlyName());
        Assert.Equal("Enum", typeof(FriendlyName.FriendlyNameEnum).GetFriendlyName());
        Assert.Equal("Enum value", typeof(FriendlyName.FriendlyNameEnum).GetField("Meow")?.GetFriendlyName());
    }
}

[FriendlyName(Name = "Class")]
internal class FriendlyName {
    [FriendlyName(Name = "Property")]
    public object? Property { get; set; }

    [FriendlyName(Name = "Field")] public object? Field;

    [FriendlyName(Name = "Enum")]
    internal enum FriendlyNameEnum {
        [FriendlyName(Name = "Enum value")]
        Meow
    }
}