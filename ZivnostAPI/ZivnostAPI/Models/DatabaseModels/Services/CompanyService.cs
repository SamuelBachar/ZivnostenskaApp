﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZivnostAPI.Models.DatabaseModels.CompanyBaseData;

namespace ZivnostAPI.Models.DatabaseModels.Services;

[Table(nameof(CompanyService))]
public class CompanyService
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Company))]
    public required int Company_Id { get; set; }

    [ForeignKey(nameof(Service))]
    public required int Service_Id { get; set; }
}