using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Interfaces;

public interface ISettingsService
{
    public Task<T> GetAsync<T>(string key, T defaultValue);

    public Task SaveAsync<T>(string key, T value);

    public static abstract Task<T> GetStaticAsync<T>(string key, T defaultValue);

    public static abstract Task<bool> ContainsStaticAsync(string key);
}
