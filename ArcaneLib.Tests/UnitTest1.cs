using ArcaneLibs;
using Microsoft.VisualBasic.CompilerServices;
using Xunit;

namespace ArcaneLib.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Assert.Equal(2, Util.Min(2, 4, 6));
        Assert.Equal(6, Util.Max(2, 4, 6));
        Assert.Equal(2d, Util.Min(2d, 4d, 6d));
        Assert.Equal(6d, Util.Max(2d, 4d, 6d));
        
    }
}