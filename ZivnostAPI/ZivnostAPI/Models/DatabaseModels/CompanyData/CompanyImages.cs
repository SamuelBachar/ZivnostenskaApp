﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZivnostAPI.Models.DatabaseModels.CompanyData;

[Table(nameof(CompanyImages))]
public class CompanyImages
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Company))]
    [Required]
    public required int Company_Id { get; set; }

    [Required]
    public string ImageName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
