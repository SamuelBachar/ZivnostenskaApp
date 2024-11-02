using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SharedTypesLibrary.DbResponse;

namespace ExtensionsLibrary.DbExtensions;

public static class DbContextExtensions
{
    public static async Task<DbActionResponse> ExtSaveChangesAsync(this DbContext dbContext)
    {
        var result = new DbActionResponse();

        try
        {
            int affectedRows = await dbContext.SaveChangesAsync();

            if (affectedRows > 0)
            {
                result.IsSucces = true;
            }
            else
            {
                result.IsSucces = false;
                result.ApiErrorCode = "UAE_900";
            }
        }
        catch (Exception ex)
        {
            result.IsSucces = false;
            result.Exception = ex.Message;
        }

        return result;
    }
}

