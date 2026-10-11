namespace FitnessProjecttt.Domain.Entities;

public class EmailVerification
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Otp { get; set; } = string.Empty;

    public DateTime ExpirationTime { get; set; }

    public bool IsUsed { get; set; }
}