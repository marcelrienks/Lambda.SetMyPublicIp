using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.Json;
using System;
using System.Collections.Generic;
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
            try
            {
                Helpers.Log("Entrypoint");

                // Validate and get the arguments from request
                var (domain, publicIp) = Helpers.ValidateRequest(apiGatewayProxyRequest);

                //TODO: Complete the logic here that will update the Public IP of the Route 53 domain

                // Create return body
                var body = new Dictionary<string, string>();
                body.Add("domain", domain);
                body.Add("publicIp", publicIp);

                var serializedBody = System.Text.Json.JsonSerializer.Serialize(body);

                // Return
                Helpers.Log($"Return 200, with body: {serializedBody}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = serializedBody
                };
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Log($"ArgumentNullException: {ex.Message}");
                return new APIGatewayProxyResponse { StatusCode = 400, Body = Helpers.SerializeException(ex) };
            }
            catch (ArgumentException ex)
            {
                Helpers.Log($"ArgumentException: {ex.Message}");
                return new APIGatewayProxyResponse { StatusCode = 400, Body = Helpers.SerializeException(ex) };
            }
            catch (Exception ex)
            {
                Helpers.Log($"Exception: {ex.Message}");
                return new APIGatewayProxyResponse { StatusCode = 500, Body = Helpers.SerializeException(ex) };
            }
        }
    }
}
