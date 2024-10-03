using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZivnostAPI.Models.DatabaseModels.CompanyBaseData;

namespace ZivnostAPI.Models.DatabaseModels.Account;

[Table(nameof(Account))]
public class Account
{
    [Key]
    public int Id { get; set; }

    public string CommonId { get; set; } = string.Empty; // Common field used for login and register check in case of OAuth authentication

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public required bool IsCompany { get; set; } = false;

    [ForeignKey(nameof(Company))]
    public int? Company_Id { get; set; }

    public string PasswordHashWithSalt { get; set; } = string.Empty;

    public string VerificationToken { get; set; } = string.Empty;

    public DateTime? VerifiedAt { get; set; }

    public string PasswordResetToken { get; set; } = string.Empty;

    public DateTime? ResetTokenExpires { get; set; }
}
