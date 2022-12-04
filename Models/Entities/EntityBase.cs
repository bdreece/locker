using System.ComponentModel.DataAnnotations;
using HotChocolate.Types.Relay;

namespace Locker.Models.Entities;

[Node]
public abstract class EntityBase
{
    [ID]
    [Key]
    public string ID { get; init; } = string.Empty;

    [Required]
    public DateTime DateCreated { get; init; } = DateTime.UtcNow;

    [Required]
    public DateTime DateLastUpdated { get; set; } = DateTime.UtcNow;

    public void Update()
    {
        DateLastUpdated = DateTime.UtcNow;
    }
}