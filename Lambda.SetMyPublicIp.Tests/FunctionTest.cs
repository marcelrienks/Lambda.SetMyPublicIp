using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using Lambda.SetMyPublicIp;
using Amazon.Lambda.APIGatewayEvents;
using System.Net.Http;

namespace Lambda.SetMyPublicIp.Tests
{
    public class FunctionTest
    {
        public FunctionTest()
        {
            // Initialise the Function class by passing in a Mock RouteHandler
            Function.Initialise(new MockRouteHandler());
        }

        [Fact]
        public void ArgumentNullExceptionForRequest()
        {
            // Arrange
            APIGatewayProxyRequest apiGatewayProxyRequest = null;

            // Act
            var apiGatewayProxyResponse = Function.SetMyPublicIp(apiGatewayProxyRequest).Result;

            // Assert
            Assert.Equal(400, apiGatewayProxyResponse.StatusCode);
            Assert.Contains("The argument cannot be null.", apiGatewayProxyResponse.Body);
        }

        [Fact]
        public void ArgumentExceptionForHttpMethod()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "POST";

            // Act
            var apiGatewayProxyResponse = Function.SetMyPublicIp(apiGatewayProxyRequest).Result;

            // Assert
            Assert.Equal(400, apiGatewayProxyResponse.StatusCode);
            Assert.Contains("Invalid HttpMethod POST.", apiGatewayProxyResponse.Body);
        }

        [Fact]
        public void ArgumentNullExceptionForDomain()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";

            // Act
            var apiGatewayProxyResponse = Function.SetMyPublicIp(apiGatewayProxyRequest).Result;

            // Assert
            Assert.Equal(400, apiGatewayProxyResponse.StatusCode);
            Assert.Contains("No hostedZoneId query string present.", apiGatewayProxyResponse.Body);
        }

        [Fact]
        public void ArgumentNullExceptionForDomainHostedZoneId()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.QueryStringParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.QueryStringParameters.Add("hostedZoneId", "123456");

            // Act
            var apiGatewayProxyResponse = Function.SetMyPublicIp(apiGatewayProxyRequest).Result;

            // Assert
            Assert.Equal(400, apiGatewayProxyResponse.StatusCode);
            Assert.Contains("No domain query string present.", apiGatewayProxyResponse.Body);
        }

        [Fact]
        public void ArgumentNullExceptionForHeaders()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.QueryStringParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.QueryStringParameters.Add("hostedZoneId", "123456");
            apiGatewayProxyRequest.QueryStringParameters.Add("domain", "test.com");

            // Act
            var apiGatewayProxyResponse = Function.SetMyPublicIp(apiGatewayProxyRequest).Result;

            // Assert
            Assert.Equal(400, apiGatewayProxyResponse.StatusCode);
            Assert.Contains("No request headers present.", apiGatewayProxyResponse.Body);
        }

        [Fact]
        public void ArgumentNullExceptionForXForward()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.QueryStringParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.QueryStringParameters.Add("hostedZoneId", "123456");
            apiGatewayProxyRequest.QueryStringParameters.Add("domain", "test.com");
            apiGatewayProxyRequest.Headers = new Dictionary<string, string>();
            apiGatewayProxyRequest.Headers.Add("key", "value");

            // Act
            var apiGatewayProxyResponse = Function.SetMyPublicIp(apiGatewayProxyRequest).Result;

            // Assert
            Assert.Equal(400, apiGatewayProxyResponse.StatusCode);
            Assert.Contains("No X-Forwarded-For header present.", apiGatewayProxyResponse.Body);
        }

        [Fact]
        public void Success()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.QueryStringParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.QueryStringParameters.Add("hostedZoneId", "123456");
            apiGatewayProxyRequest.QueryStringParameters.Add("domain", "test.com");
            apiGatewayProxyRequest.Headers = new Dictionary<string, string>();
            apiGatewayProxyRequest.Headers.Add("X-Forwarded-For", "127.0.0.1, 127.0.0.2");

            // Act
            var apiGatewayProxyResponse = Function.SetMyPublicIp(apiGatewayProxyRequest).Result;

            // Assert
            Assert.Equal(200, apiGatewayProxyResponse.StatusCode);
            Assert.Equal("{\"status\":\"OK\",\"domain\":\"test.com\",\"publicIp\":\"127.0.0.1, 127.0.0.2\"}", apiGatewayProxyResponse.Body);
        }
    }
}
