using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Locker.Models.Inputs;
using Locker.Services;

namespace Locker.Models.Entities;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Phone), IsUnique = true)]
public sealed class User : AuthEntityBase
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [NotMapped]
    public override string Name
    {
        get => $"{FirstName} {LastName}";
        set
        {
            var tokens = value.Split(' ');
            if (tokens.Length != 2)
                throw new ArgumentException("Invalid name, please enter space-delimited");

            FirstName = tokens[0];
            LastName = tokens[1];
        }
    }

    public string? Email { get; set; }
    public string? Phone { get; set; }

    [Required]
    [GraphQLIgnore]
    public string Hash { get; set; } = string.Empty;

    public IList<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public static Task<User> Get([ID] string id, DataContext db) =>
        db.Users.SingleAsync(u => u.ID == id);

    public void UpdateHash(string hash)
    {
        Hash = hash;
        UpdateSecurityStamp();
    }

    public void Update(UpdateUserInput input)
    {
        base.Update();

        FirstName = input.FirstName ?? FirstName;
        LastName = input.LastName ?? LastName;
        Email = input.Email ?? Email;
        Phone = input.Phone ?? Phone;

        if (input.Email is not null || input.Phone is not null)
            UpdateSecurityStamp();
    }

}