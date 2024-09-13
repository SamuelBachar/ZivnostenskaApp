using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZivnostAPI.Models.DatabaseModels.Localization;

namespace ZivnostAPI.Models.DatabaseModels.CompanyBaseData;

[Table(nameof(Company))]
public class Company
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [ForeignKey(nameof(City))]
    [Required]
    public required int City_Id { get; set; }

    public string Street { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public int? ICO { get; set; }
    public int? DIC { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool Subscribed { get; set; } = false;

    public DateTime SubscriptionFrom { get; set; }

    public DateTime SubscriptionTo { get; set; }

    public DateTime RegisteredAt { get; set; }
}
