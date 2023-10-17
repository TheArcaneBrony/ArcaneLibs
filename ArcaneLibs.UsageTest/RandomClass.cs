using System;
namespace ArcaneLibs.UsageTest;

public class RandomClass
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateCreated { get; set; }
    public SubClass SubClassInst { get; set; }

    public class SubClass
    {
        public string Description { get; set; }
    }
}
