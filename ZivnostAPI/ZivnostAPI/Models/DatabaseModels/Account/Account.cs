using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZivnostAPI.Models.DatabaseModels.Account;

[Table(nameof(Account))]
public class Account
{
    [Key]
    public int Id { get; set; }

    public string CommonId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string MiddleName { get; set; } = string.Empty;

    public string SureName { get; set; } = string.Empty;

    public string PictureURL { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public required bool IsCompanyAccount { get; set; } = false;

    [Required]
    public required bool IsCustomerAccount { get; set; } = false;

    public bool IsHybridAccount { get; set; } = false;

    public DateTime? RegisteredAsCompanyAt { get; set; }

    public DateTime? RegisteredAsCustomerAt { get; set; }

    [ForeignKey(nameof(Company))]
    public int? Company_Id { get; set; }

    public string PasswordHashWithSalt { get; set; } = string.Empty;

    public string VerificationToken { get; set; } = string.Empty;

    public DateTime? VerifiedAt { get; set; }

    public string PasswordResetToken { get; set; } = string.Empty;

    public DateTime? ResetTokenExpires { get; set; }
}
