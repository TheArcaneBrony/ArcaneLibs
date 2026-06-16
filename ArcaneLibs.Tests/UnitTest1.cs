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
    public void TestFriendlyNameAttribute() {
        Assert.Equal("Class", typeof(FriendlyName).GetFriendlyName());
        Assert.Equal("Property", typeof(FriendlyName).GetProperty("Property")?.GetFriendlyName());
        Assert.Equal("Field", typeof(FriendlyName).GetField("Field")?.GetFriendlyName());
        Assert.Equal("Enum", typeof(FriendlyName.FriendlyNameEnum).GetFriendlyName());
        Assert.Equal("Enum value", typeof(FriendlyName.FriendlyNameEnum).GetField("Meow")?.GetFriendlyName());
    }

    [Fact]
    public void TestColorAttribute() {
        Assert.Equivalent(new ColorAttribute(1, 1, 1), typeof(FriendlyName).GetColorOrNull());
        Assert.Equivalent(new ColorAttribute(2, 2, 2), typeof(FriendlyName).GetProperty("Property")?.GetColorOrNull());
        Assert.Equivalent(new ColorAttribute(3, 3, 3, 255), typeof(FriendlyName).GetField("Field")?.GetColorOrNull());
        Assert.Equivalent(new ColorAttribute(4, 4, 4, 255), typeof(FriendlyName.FriendlyNameEnum).GetColorOrNull());
        Assert.Equivalent(new ColorAttribute(5, 6, 7, 8), typeof(FriendlyName.FriendlyNameEnum).GetField("Meow")?.GetColorOrNull());
    }
}

[FriendlyName(Name = "Class")]
[Color(1, 1, 1)]
internal class FriendlyName {
    [FriendlyName(Name = "Property")]
    [Color(2, 2, 2)]
    public object? Property { get; set; }

    [FriendlyName(Name = "Field")] [Color(3, 3, 3)]
    public object? Field;

    [FriendlyName(Name = "Enum")]
    [Color(4, 4, 4)]
    internal enum FriendlyNameEnum {
        [Color(5, 6, 7, 8)] [FriendlyName(Name = "Enum value")]
        Meow
    }
}