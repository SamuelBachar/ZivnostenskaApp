using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Services;

public class ConnectivityHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Skontroluj, či je dostupné internetové pripojenie
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            throw new InvalidOperationException("No internet connection.");
        }

        // Ak je pripojenie v poriadku, pokračuj s odoslaním požiadavky
        return await base.SendAsync(request, cancellationToken);
    }
}
