using Amazon.Runtime;
using Amazon.Runtime.Internal.Auth;
using Amazon.Util;
using System;
using System.Runtime.CompilerServices;

namespace App.SetMyPublicIp
{
    /// <summary>
    /// This app is meant to be used to calculate the AWS signature, that can then be used with an HttpClient, to invoke the Aws Api Gateway function linked to the lambda in thei solution
    /// This is basically a .net version of the SetMyPublicIp.sh file container in this solution
    /// helpful links:
    /// https://docs.aws.amazon.com/general/latest/gr/sigv4-create-canonical-request.html
    /// https://github.com/aws/aws-sdk-net/blob/6c3be79bdafd5bfff1ab0bf5fec17abc66c7b516/sdk/src/Core/Amazon.Runtime/Internal/Auth/AWS4Signer.cs
    /// https://github.com/FantasticFiasco/aws-signature-version-4/blob/master/src/Private/Extensions.cs
    /// </summary>

    class Program
    {
        static void Main(string[] args)
        {
            var uri = "https://jnhisz7ebh.execute-api.eu-west-1.amazonaws.com/api/hostedzone/Z6ZMEKJJ7H3SC/domain/marcelrienks.com";
            var region = "af-south-1";
            var service = "execute-api";
            var secretKey = "";
            var secret = "";

            var httpRequestMethod = "PATCH";
            var canonicalUri = "%2Fapi%2Fhostedzone%2FZ6ZMEKJJ7H3SC%2Fdomain%2Fmarcelrienks.com";
            var canonicalQueryString = string.Empty;
            var canonicalHeaders = string.Empty;
            var signedHeaders = string.Empty;
            var requestPayload = string.Empty;

            var canonicalRequest = $"{httpRequestMethod}\n{canonicalUri}\n{canonicalQueryString}\n{canonicalHeaders}\n{signedHeaders}\n{requestPayload}";

            var signature = AWS4Signer.ComputeSignature(
                awsAccessKey: secretKey,
                awsSecretAccessKey: secret,
                region: region,
                signedAt: DateTime.Now,
                service: service,
                signedHeaders: signedHeaders,
                canonicalRequest: canonicalRequest
                );

            Console.WriteLine(signature.Signature);
            Console.ReadLine();
        }
    }
}
