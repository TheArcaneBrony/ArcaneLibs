namespace ArcaneLibs.UsageTest;

public class RandomClass {
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime DateCreated { get; set; }
    public required SubClass SubClassInst { get; set; }

    public class SubClass {
        public required string Description { get; set; }
    }
}