using System.Collections.Generic;
using SQLite4Unity3d;

public class Car
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    
    public Quality Quality { get; set; }

    
    public List<string> Tags { get; set; }

    public override string ToString()
    {
        return string.Format("      [Car: Id={0}, Name={1}, Max Speed ={2}, Tags={3}]",
            Id,
            Name,
            Quality != null ? Quality.Speed : -1,
            Tags != null ? string.Join(" ; ", Tags.ToArray()) : "no defined");
    }
}

public class Quality
{
    public int Speed { get; set; }
    public int Hardness { get; set; }
}

public class BrandContainer<T>
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    
    public List<T> Types { get; set; }

    public override string ToString()
    {
        return string.Format("[Brand: Id={0}, Name={1}, Type Count ={2}]", Id, Name, Types.Count);
    }
}