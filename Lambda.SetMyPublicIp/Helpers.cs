using Amazon.Lambda.APIGatewayEvents;
using System;
using System.Collections.Generic;

namespace Lambda.SetMyPublicIp
{
    public static class Helpers
    {
        /// <summary>
        /// Serializes message and stack trace information of he exception into json
        /// </summary>
        /// <param name="exception">the Exception to be handled</param>
        /// <returns>json representation of the exception</returns>
        public static string SerializeException(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var body = new Dictionary<string, string>();
            body.Add("Exception", exception.Message);
            body.Add("Stacktrace", exception.StackTrace);
            if (exception.InnerException != null)
                body.Add("InnerException", exception.InnerException.Message);

            return System.Text.Json.JsonSerializer.Serialize(body); 
        }

        /// <summary>
        /// Validates that all required arguments are present. HttpMethod, QueryStringParamater, and Headers
        /// </summary>
        /// <param name="apiGatewayProxyRequest">the request from the API Gateway proxy</param>
        /// <returns></returns>
        public static (string domain, string publicIp) ValidateRequest(APIGatewayProxyRequest apiGatewayProxyRequest)
        {
            Console.WriteLine($"{_prefix} ValidateRequest");

            if (apiGatewayProxyRequest == null)
                throw new ArgumentNullException(nameof(apiGatewayProxyRequest), "The argument cannot be null.");

            if (apiGatewayProxyRequest.HttpMethod != "PATCH")
                throw new ArgumentException($"Invalid HttpMethod {apiGatewayProxyRequest.HttpMethod}.", "apiGatewayProxyRequest.HttpMethod");

            if (apiGatewayProxyRequest.QueryStringParameters == null || !apiGatewayProxyRequest.QueryStringParameters.TryGetValue("domain", out string domain))
                throw new ArgumentNullException("apiGatewayProxyRequest.QueryStringParameters", "No domain query string present.");

            Console.WriteLine($"{_prefix} domain: {domain}");

            if (apiGatewayProxyRequest.Headers == null || !apiGatewayProxyRequest.Headers.Any())
                throw new ArgumentNullException("apiGatewayProxyRequest.Headers", "No request headers present.");

            if (!apiGatewayProxyRequest.Headers.TryGetValue("X-Forwarded-For", out string publicIp))
                throw new ArgumentNullException("apiGatewayProxyRequest.Headers", "No X-Forwarded-For header present.");

            Console.WriteLine($"{_prefix} publicIp: {publicIp}");

            return (domain, publicIp);
        }
    }
}
