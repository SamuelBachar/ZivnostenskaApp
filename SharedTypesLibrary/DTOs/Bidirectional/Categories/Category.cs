using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Bidirectional.Categories;

public class Category
{
    public int Id { get; set; }

    public int? Parrent_Category_Id { get; set; }

    public required string Name { get; set; } = string.Empty;
}
