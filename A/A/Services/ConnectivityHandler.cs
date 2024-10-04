using ExceptionsHandling;
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
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            throw new InvalidOperationException("No internet connection.");
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response == null || response.Content == null)
        {
            string serErrorMsg = new ExceptionHandler("UAE_403", App.UserData.CurrentCulture).CustomMessage;
            throw new Exception(serErrorMsg);
        }

        return response;
    }
}
