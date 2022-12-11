using System.ComponentModel.DataAnnotations;

namespace Locker.Models.Entities;

[Node]
public abstract class EntityBase
{
    [ID]
    [GraphQLName("id")]
    [Key]
    public string ID { get; init; } = Guid.NewGuid().ToString();

    [Required]
    public DateTime DateCreated { get; init; } = DateTime.UtcNow;

    [Required]
    public DateTime DateLastUpdated { get; set; } = DateTime.UtcNow;

    public void Update()
    {
        DateLastUpdated = DateTime.UtcNow;
    }
}