using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Bidirectional.Categories;

public class CategoryDTO
{
    public int Id { get; set; }

    public required int Service_Id { get; set; }

    public required string Name { get; set; } = string.Empty;
}
