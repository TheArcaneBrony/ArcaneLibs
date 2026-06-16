using System.Reflection;

namespace ArcaneLibs.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ColorAttribute(byte r, byte g, byte b, byte a = 255) : Attribute {
    public byte R = r;
    public byte G = g;
    public byte B = b;
    public byte A = a;
}

public static class ColorAttributeExtensions {
    public static ColorAttribute? GetColorOrNull(this MemberInfo type) {
        var attrs = type.GetCustomAttributes(typeof(ColorAttribute), false);
        return attrs.Length == 0 ? null : (ColorAttribute)attrs[0];
    }
}