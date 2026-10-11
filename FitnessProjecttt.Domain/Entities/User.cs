using Microsoft.AspNetCore.Identity;

namespace FitnessProjecttt.Domain.Entities;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
