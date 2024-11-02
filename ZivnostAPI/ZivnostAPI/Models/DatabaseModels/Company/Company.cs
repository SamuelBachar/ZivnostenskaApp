using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZivnostAPI.Models.DatabaseModels.Localization;

namespace ZivnostAPI.Models.DatabaseModels.Company;

[Table(nameof(Company))]
public class Company
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [ForeignKey(nameof(Country))]
    public required int Country_Id { get; set; }

    [ForeignKey(nameof(Region))]
    public required int Region_Id { get; set; }

    [ForeignKey(nameof(District))]
    public required int District_Id { get; set; }

    [ForeignKey(nameof(City))]
    public int? City_Id { get; set; }

    public string Address { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string CIN { get; set; } = string.Empty;
    public int? DIC { get; set; } // Remove ???

    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool Subscribed { get; set; } = false;

    public DateTime SubscriptionFrom { get; set; }

    public DateTime SubscriptionTo { get; set; }

    public DateTime RegisteredAt { get; set; }
}
