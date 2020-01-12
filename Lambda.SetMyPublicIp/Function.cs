using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lambda.SetMyPublicIp
{
    public class Function
    {
        private static string _prefix = " - SetMyPublicIp:";

        /// <summary>
        /// The main entry point for the custom runtime.
        /// </summary>
        /// <param name="args"></param>
        private static async Task Main(string[] args)
        {
            Func<APIGatewayProxyRequest, APIGatewayProxyResponse> func = SetMyPublicIp;
            using(var handlerWrapper = HandlerWrapper.GetHandlerWrapper(func, new JsonSerializer()))
            using(var bootstrap = new LambdaBootstrap(handlerWrapper)) { await bootstrap.RunAsync(); }
        }

        /// <summary>
        /// This function takes in the name of the Route 53 domain to be updated, inspects the request to find the public IP, and assigns this IP to the domain name in Route 53.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns>an <c>APIGatewayProxyResponse</c> indicating if the function call was successfull</returns>
        public static APIGatewayProxyResponse SetMyPublicIp(APIGatewayProxyRequest apiGatewayProxyRequest)
        {
            Console.WriteLine($"{_prefix} Entrypoint");

            if (apiGatewayProxyRequest != null)
            {
                if (apiGatewayProxyRequest.Headers != null && apiGatewayProxyRequest.Headers.Any())
                {
                    if (apiGatewayProxyRequest.Headers.TryGetValue("X-Forwarded-For", out string publicIpAddress))
                    {
                        Console.WriteLine($"{_prefix} X-Forwarded-For:{publicIpAddress}");

                        Console.WriteLine($"{_prefix} Return");
                        return new APIGatewayProxyResponse { StatusCode = 200, Body = "" };
                    }
                }
                else
                    throw new ArgumentNullException(nameof(apiGatewayProxyRequest.Headers), "No request headers present.");
            }
            else
                throw new ArgumentNullException(nameof(apiGatewayProxyRequest), "The argument cannot be null.");
        }
    }
}
