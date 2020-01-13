using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.Json;
using Amazon.Route53;
using Lambda.SetMyPublicIp.Handlers;
using Lambda.SetMyPublicIp.Helpers;
using Lambda.SetMyPublicIp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lambda.SetMyPublicIp
{
    public class Function
    {
        private static IRouteHandler _routeHandler;

        public static void Initialise(IRouteHandler routeHandler)
        {
            _routeHandler = routeHandler;
        }

        /// <summary>
        /// The main entry point for the custom runtime.
        /// </summary>
        /// <param name="args"></param>
        private static async Task Main(string[] args)
        {
            _routeHandler = new RouteHandler(new AmazonRoute53Client());

            Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>> func = SetMyPublicIp;
            using (var handlerWrapper = HandlerWrapper.GetHandlerWrapper(func, new JsonSerializer()))
            using (var bootstrap = new LambdaBootstrap(handlerWrapper)) { await bootstrap.RunAsync(); }
        }

        /// <summary>
        /// This function takes in the name of the Route 53 domain to be updated, inspects the request to find the public IP, and assigns this IP to the domain name in Route 53.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns>an <c>APIGatewayProxyResponse</c> indicating if the function call was successfull</returns>
        public async static Task<APIGatewayProxyResponse> SetMyPublicIp(APIGatewayProxyRequest apiGatewayProxyRequest)
        {
            try
            {
                Logging.Log("Entrypoint");

                // Validate and get the arguments from request
                var (hostedZoneId, domain, publicIp) = ValidateRequest(apiGatewayProxyRequest);

                // Update the recorset with the public IP
                var status = await _routeHandler.UpsertRecordset(hostedZoneId, domain, publicIp);

                // Create return body
                var body = new Dictionary<string, string>();
                body.Add("status", status);
                body.Add("domain", domain);
                body.Add("publicIp", publicIp);

                var serializedBody = System.Text.Json.JsonSerializer.Serialize(body);

                // Return
                Logging.Log($"Return 200, with body: {serializedBody}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = serializedBody
                };
            }
            catch (Exception ex)
            {
                Logging.Log($"{typeof(Exception)}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Validates that all required arguments are present. HttpMethod, QueryStringParamater, and Headers
        /// </summary>
        /// <param name="apiGatewayProxyRequest">the request from the API Gateway proxy</param>
        /// <returns></returns>
        private static (string hostedZoneId, string domain, string publicIp) ValidateRequest(APIGatewayProxyRequest apiGatewayProxyRequest)
        {
            Logging.Log("ValidateRequest");

            // Validate that request is not null
            if (apiGatewayProxyRequest == null)
                throw new ArgumentNullException(nameof(apiGatewayProxyRequest), "The argument cannot be null.");

            // Validate that the correct Http Method is used
            if (apiGatewayProxyRequest.HttpMethod != "PATCH")
                throw new ArgumentException($"Invalid HttpMethod {apiGatewayProxyRequest.HttpMethod}.", "apiGatewayProxyRequest.HttpMethod");

            // Validate and get query param 'hostedZoneId' is present
            if (apiGatewayProxyRequest.QueryStringParameters == null || !apiGatewayProxyRequest.QueryStringParameters.TryGetValue("hostedZoneId", out string hostedZoneId))
                throw new ArgumentNullException("apiGatewayProxyRequest.QueryStringParameters", "No hostedZoneId query string present.");

            Logging.Log($"hostedZoneId: {hostedZoneId}");

            // Validate and get query param 'domain' is present
            if (apiGatewayProxyRequest.QueryStringParameters == null || !apiGatewayProxyRequest.QueryStringParameters.TryGetValue("domain", out string domain))
                throw new ArgumentNullException("apiGatewayProxyRequest.QueryStringParameters", "No domain query string present.");

            Logging.Log($"domain: {domain}");

            // Validate that headders are present
            if (apiGatewayProxyRequest.Headers == null || !apiGatewayProxyRequest.Headers.Any())
                throw new ArgumentNullException("apiGatewayProxyRequest.Headers", "No request headers present.");

            // Validate and get header 'X-Forwarded-For' is present
            if (!apiGatewayProxyRequest.Headers.TryGetValue("X-Forwarded-For", out string publicIp))
                throw new ArgumentNullException("apiGatewayProxyRequest.Headers", "No X-Forwarded-For header present.");

            // Ensure only first IP is taken, incase there are multiple
            publicIp = publicIp.Split(',').FirstOrDefault();

            Logging.Log($"publicIp: {publicIp}");

            return (hostedZoneId, domain, publicIp);
        }
    }
}
