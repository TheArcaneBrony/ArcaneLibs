using System.Reflection;

namespace ArcaneLibs.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
public class TableHideAttribute : Attribute;