using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Locker.Models.Inputs;
using Locker.Services;

namespace Locker.Models.Entities;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Phone), IsUnique = true)]
public sealed class User : EntityBase
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? Phone { get; set; }

    [Required]
    public string Hash { get; set; } = string.Empty;

    public IList<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public static Task<User> Get(string id, DataContext db) =>
        db.Users.SingleAsync(u => u.ID == id);

    public void Update(UpdateUserInput input)
    {
        base.Update();

        FirstName = input.FirstName ?? FirstName;
        LastName = input.LastName ?? LastName;
        Email = input.Email ?? Email;
        Phone = input.Phone ?? Phone;
    }
}