using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.Json;
using Amazon.Route53;
using Lambda.SetMyPublicIp.Handlers;
using Lambda.SetMyPublicIp.Helpers;
using Lambda.SetMyPublicIp.Interfaces;
using Lambda.SetMyPublicIp.Models;
using System;
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
        public static async Task Main(string[] args)
        {
            try
            {
                _routeHandler = new RouteHandler(new AmazonRoute53Client());

                Func<Request, ILambdaContext, Task<Response>> func = SetMyPublicIp;
                using (var handlerWrapper = HandlerWrapper.GetHandlerWrapper(func, new JsonSerializer()))
                using (var bootstrap = new LambdaBootstrap(handlerWrapper)) { await bootstrap.RunAsync(); }
            }
            catch (Exception ex)
            {
                Logging.Log($"{typeof(Exception)}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// This function takes in the name of the Route 53 domain to be updated, inspects the request to find the public IP, and assigns this IP to the domain name in Route 53.
        /// </summary>
        /// <param name="request">the request model containing HostedZonneId, DomainName, PublicIps</param>
        /// <param name="context">the Lambda context</param>
        /// <returns>an <c>APIGatewayProxyResponse</c> indicating if the function call was successfull</returns>
        public static async Task<Response> SetMyPublicIp(Request request, ILambdaContext context)
        {
            Logging.Log("Entrypoint...");

            // Validate and get the arguments from request
            ValidateRequest(request);

            // Update the recorset with the public IP
            var (requestId, requestStatus) = await _routeHandler.UpsertRecordset(request.HostedZoneId, request.DomainName, GeneralHelpers.GetFirstIp(request.PublicIps));

            // Buid response
            var response = new Response()
            {
                ChangeRequestId = requestId,
                ChangeRequestStatus = requestStatus,
                PublicIp = GeneralHelpers.GetFirstIp(request.PublicIps)
            };

            // Return
            Logging.Log($"Response: ChangeRequestId:{response.ChangeRequestId}, ChangeRequestStatus:{response.ChangeRequestStatus}, PublicIp:{response.PublicIp}");
            return response;
        }

        /// <summary>
        /// Validates that all required arguments are present. HttpMethod, QueryStringParamater, and Headers
        /// </summary>
        /// <param name="request">the request from the API Gateway</param>
        /// <returns></returns>
        private static void ValidateRequest(Request request)
        {
            Logging.Log("Validate Request...");

            // Validate that request is not null
            if (request == null)
                throw new ArgumentNullException(nameof(request), "The argument cannot be null.");

            // Validate and get query param 'hostedZoneId' is present
            if (string.IsNullOrEmpty(request.HostedZoneId))
                throw new ArgumentException("No HostedZoneId present.", "request.HostedZoneId");

            Logging.Log($"HostedZoneId: {request.HostedZoneId}");

            // Validate and get query param 'hostedZoneId' is present
            if (string.IsNullOrEmpty(request.DomainName))
                throw new ArgumentException("No DomainName present.", "request.DomainName");

            Logging.Log($"DomainName: {request.DomainName}");

            // Validate and get query param 'hostedZoneId' is present
            if (string.IsNullOrEmpty(request.PublicIps))
                throw new ArgumentException("No PublicIps present.", "request.PublicIps");

            Logging.Log($"PublicIps: {request.PublicIps}");
        }
    }
}
