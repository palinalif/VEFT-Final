namespace ShroomCity.Models.Entities;

public class Attribute
{
    public int Id { get; set; }
    public string Value { get; set; }
    public AttributeType AttributeType { get; set; }
    public User RegisteredBy { get; set; }
    public ICollection<Mushroom> mushrooms;
}