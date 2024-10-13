using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedTypesLibrary.Enums.Enums;

namespace SharedTypesLibrary.DTOs.Bidirectional.Account;

public class UpdateAccountTypeDTO
{
    public int Id { get; set; }
    public AccountType AccountType {  get; set; }
}
