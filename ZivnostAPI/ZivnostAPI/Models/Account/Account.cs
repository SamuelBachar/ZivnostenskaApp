using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZivnostAPI.Models.CompanyBaseData;
using ZivnostAPI.Models.Localization;

namespace ZivnostAPI.Models.Account;

[Table(nameof(Account))]
public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required bool IsCompany { get; set; } = false;

    [ForeignKey(nameof(Company))]
    public int? Company_Id { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; } = string.Empty;

    public string PasswordHashWithSalt { get; set; } = string.Empty;

    public string VerificationToken { get; set; } = string.Empty;

    public DateTime? VerifiedAt { get; set; }

    public string PasswordResetToken { get; set; } = string.Empty;

    public DateTime? ResetTokenExpires { get; set; }
}
