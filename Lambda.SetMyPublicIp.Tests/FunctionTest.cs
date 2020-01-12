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
        [Fact]
        public void ArgumentNullExceptionForRequest()
        {
            // Arrange
            APIGatewayProxyRequest apiGatewayProxyRequest = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>("apiGatewayProxyRequest", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Equal("The argument cannot be null. (Parameter 'apiGatewayProxyRequest')", exception.Message);
        }

        [Fact]
        public void ArgumentExceptionForHttpMethod()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "POST";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>("apiGatewayProxyRequest.HttpMethod", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Equal("Invalid HttpMethod POST. (Parameter 'apiGatewayProxyRequest.HttpMethod')", exception.Message);
        }

        [Fact]
        public void ArgumentNullExceptionForDomain()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>("apiGatewayProxyRequest.QueryStringParameters", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Equal("No 'domain' query string present. (Parameter 'apiGatewayProxyRequest.QueryStringParameters')", exception.Message);
        }

        [Fact]
        public void ArgumentNullExceptionForHeaders()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.QueryStringParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.QueryStringParameters.Add("domain", "test.com");

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>("apiGatewayProxyRequest.Headers", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Equal("No request headers present. (Parameter 'apiGatewayProxyRequest.Headers')", exception.Message);
        }

        [Fact]
        public void ArgumentNullExceptionForXForward()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.QueryStringParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.QueryStringParameters.Add("domain", "test.com");
            apiGatewayProxyRequest.Headers = new Dictionary<string, string>();
            apiGatewayProxyRequest.Headers.Add("key", "value");

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>("apiGatewayProxyRequest.Headers", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
            Assert.Equal("No X-Forwarded-For header present. (Parameter 'apiGatewayProxyRequest.Headers')", exception.Message);
        }

        [Fact]
        public void Success()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.HttpMethod = "PATCH";
            apiGatewayProxyRequest.QueryStringParameters = new Dictionary<string, string>();
            apiGatewayProxyRequest.QueryStringParameters.Add("domain", "test.com");
            apiGatewayProxyRequest.Headers = new Dictionary<string, string>();
            apiGatewayProxyRequest.Headers.Add("X-Forwarded-For", "127.0.0.1, 127.0.0.2");

            // Act
            var apiGatewayProxyResponse = Function.SetMyPublicIp(apiGatewayProxyRequest);

            // Assert
            Assert.Equal(200, apiGatewayProxyResponse.StatusCode);
            Assert.Equal("{\"domain\":\"test.com\",\"publicIp\":\"127.0.0.1, 127.0.0.2\"}", apiGatewayProxyResponse.Body);
        }
    }
}
