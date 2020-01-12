using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.Json;
using System;
using System.Collections.Generic;
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
            using (var handlerWrapper = HandlerWrapper.GetHandlerWrapper(func, new JsonSerializer()))
            using (var bootstrap = new LambdaBootstrap(handlerWrapper)) { await bootstrap.RunAsync(); }
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

            if (ValidateRequest(apiGatewayProxyRequest))
            {
                var domain = apiGatewayProxyRequest.QueryStringParameters.Single(item => item.Key == "domain").Value;
                Console.WriteLine($"{_prefix} domain: {domain}");

                var publicIp = apiGatewayProxyRequest.Headers.Single(item => item.Key == "X-Forwarded-For").Value;
                Console.WriteLine($"{_prefix} publicIp: {publicIp}");

                var body = new Dictionary<string, string>();
                body.Add("domain", domain);
                body.Add("publicIp", publicIp);

                //TODO: Complete the logic here that will update the Public IP of the Route 53 domain

                Console.WriteLine($"{_prefix} Return 200");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = System.Text.Json.JsonSerializer.Serialize(body)
                };
            }

            // Technically this should bever run, if there is a validation error, it will be thrown before this
            // try clean this up though, it's not nice code
            return null;
        }

        /// <summary>
        /// Validates that all required arguments are present. HttpMethod, QueryStringParamater, and Headers
        /// </summary>
        /// <param name="apiGatewayProxyRequest">the request from the API Gateway proxy</param>
        /// <returns></returns>
        public static bool ValidateRequest(APIGatewayProxyRequest apiGatewayProxyRequest)
        {
            Console.WriteLine($"{_prefix} ValidateRequest");

            if (apiGatewayProxyRequest != null)
            {
                if (apiGatewayProxyRequest.HttpMethod == "PATCH")
                {
                    if (apiGatewayProxyRequest.QueryStringParameters != null && apiGatewayProxyRequest.QueryStringParameters.TryGetValue("domain", out string domain))
                    {
                        if (apiGatewayProxyRequest.Headers != null && apiGatewayProxyRequest.Headers.Any())
                        {
                            if (apiGatewayProxyRequest.Headers.TryGetValue("X-Forwarded-For", out string publicIpAddress))
                                return true;

                            else
                                throw new ArgumentNullException("apiGatewayProxyRequest.Headers", "No X-Forwarded-For header present.");
                        }
                        else
                            throw new ArgumentNullException("apiGatewayProxyRequest.Headers", "No request headers present.");
                    }
                    else
                        throw new ArgumentNullException("apiGatewayProxyRequest.QueryStringParameters", "No 'domain' query string present.");
                }
                else
                    throw new ArgumentException($"Invalid HttpMethod {apiGatewayProxyRequest.HttpMethod}.", "apiGatewayProxyRequest.HttpMethod");
            }
            else
                throw new ArgumentNullException(nameof(apiGatewayProxyRequest), "The argument cannot be null.");
        }
    }
}
