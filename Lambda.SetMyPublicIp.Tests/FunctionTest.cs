using System.Collections.Generic;
using Xunit;
using Amazon.Lambda.APIGatewayEvents;
using Lambda.SetMyPublicIp.Tests.Mocks;
using System;
using System.Threading.Tasks;

namespace Lambda.SetMyPublicIp.Tests
{
    public class FunctionTest
    {
        public FunctionTest()
        {
            // Initialise the Function class by passing in a Mock RouteHandler
            Function.Initialise(new MockedRouteHandler());
        }

        [Fact]
        public async Task ArgumentNullExceptionForRequest()
        {
            // Arrange
            APIGatewayProxyRequest apiGatewayProxyRequest = null;

            // Act & assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>("apiGatewayProxyRequest", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Contains("The argument cannot be null. (Parameter 'apiGatewayProxyRequest')", exception.Message);
        }

        [Fact]
        public async Task ArgumentExceptionForHttpMethod()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "POST";

            // Act & assert
            var exception = await Assert.ThrowsAsync<ArgumentException>("apiGatewayProxyRequest.HttpMethod", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Contains("Invalid HttpMethod POST. (Parameter 'apiGatewayProxyRequest.HttpMethod')", exception.Message);
        }

        [Fact]
        public async Task ArgumentNullExceptionForDomain()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";

            // Act & assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>("apiGatewayProxyRequest.QueryStringParameters", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Contains("No hostedZone path paramater present. (Parameter 'apiGatewayProxyRequest.QueryStringParameters')", exception.Message);
        }

        [Fact]
        public async Task ArgumentNullExceptionForHostedZoneId()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.PathParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.PathParameters.Add("hostedZoneId", "123456");

            // Act & assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>("apiGatewayProxyRequest.QueryStringParameters", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Contains("No domain path paramater present. (Parameter 'apiGatewayProxyRequest.QueryStringParameters')", exception.Message);
        }

        [Fact]
        public async Task ArgumentNullExceptionForHeaders()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.PathParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.PathParameters.Add("hostedZoneId", "123456");
            apiGatewayProxyRequest.PathParameters.Add("domainname", "test.com");

            // Act & assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>("apiGatewayProxyRequest.Headers", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Contains("No request headers present. (Parameter 'apiGatewayProxyRequest.Headers')", exception.Message);
        }

        [Fact]
        public async Task ArgumentNullExceptionForXForward()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.PathParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.PathParameters.Add("hostedZoneId", "123456");
            apiGatewayProxyRequest.PathParameters.Add("domainname", "test.com");
            apiGatewayProxyRequest.Headers = new Dictionary<string, string>();
            apiGatewayProxyRequest.Headers.Add("key", "value");

            // Act & assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>("apiGatewayProxyRequest.Headers", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Contains("No X-Forwarded-For header present. (Parameter 'apiGatewayProxyRequest.Headers')", exception.Message);
        }

        [Fact]
        public void Success()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.PathParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.PathParameters.Add("hostedZoneId", "123456");
            apiGatewayProxyRequest.PathParameters.Add("domainname", "test.com");
            apiGatewayProxyRequest.Headers = new Dictionary<string, string>();
            apiGatewayProxyRequest.Headers.Add("X-Forwarded-For", "127.0.0.1, 127.0.0.2");

            // Act
            var apiGatewayProxyResponse = Function.SetMyPublicIp(apiGatewayProxyRequest).Result;

            // Assert
            Assert.Equal(200, apiGatewayProxyResponse.StatusCode);
            Assert.Equal("{\"status\":\"OK\",\"domain\":\"test.com\",\"publicIp\":\"127.0.0.1\"}", apiGatewayProxyResponse.Body);
        }
    }
}
