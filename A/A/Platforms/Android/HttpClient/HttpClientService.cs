using A.Interfaces;
using Javax.Net.Ssl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Android.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace A.Services;


/* Ak by som mal pre android chybu hostname 10.0.2.2 not verified: certificate: sha1/4wh/...  
 * (tato chyba by mala nastavat len pri .NET 6 verzii)
*/
using Object = Java.Lang.Object;

public partial class HttpClientService : IPlatformHttpMessageHandler
{
    public partial HttpMessageHandler GetPlatformSpecificHttpMessageHandler()
    {
        var androidHttpHandler = new AndroidMessageHandler // Ak by bol problem hostname 10.0.2.2 tak potom = new LocalHostAndoirMessageHandler
        {
            ServerCertificateCustomValidationCallback = (httpRequestMessage, certificate, chain, sslPolicyErrors) =>
            {
                if (certificate?.Issuer == "CN=localhost" || sslPolicyErrors == SslPolicyErrors.None)
                {
                    return true;
                }

                if (certificate != null)
                {
                    if (certificate.Issuer.Contains("azure"))
                    {
                        return true;
                    }
                }

                return false;
            }
        };

        return androidHttpHandler;
    }

    /* Ak by som mal pre android chybu hostname 10.0.2.2 not verified: certificate: sha1/4wh/...  
     * (tato chyba by mala nastavat len pri .NET 6 verzii)
    */

    class LocalHostAndoirMessageHandler : AndroidMessageHandler
    {
        protected override IHostnameVerifier GetSSLHostnameVerifier(HttpsURLConnection connection) => new LocalhostNameVerifier();
    }

    class LocalhostNameVerifier : Object, IHostnameVerifier
    {
        public bool Verify(string hostname, ISSLSession session)
        {

            if ((HttpsURLConnection.DefaultHostnameVerifier.Verify(hostname, session)) || (hostname == "10.0.2.2"))
            {
                return true;
            }

            return false;
        }
    }
}
