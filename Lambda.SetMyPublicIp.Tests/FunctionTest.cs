using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using Lambda.SetMyPublicIp;
using Amazon.Lambda.APIGatewayEvents;

namespace Lambda.SetMyPublicIp.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void ArgumentNullExceptionForRequeest()
        {
            // Arrange
            APIGatewayProxyRequest apiGatewayProxyRequest = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>("apiGatewayProxyRequest", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
        }

        [Fact]
        public void ArgumentNullExceptionForHeaders()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();

            // Act & Assert
            Assert.Throws<ArgumentNullException>("Headers", () => Function.SetMyPublicIp(apiGatewayProxyRequest));
        }

        [Fact]
        public void Success()
        {
            // Arrange
            var apiGatewayProxyRequest = new APIGatewayProxyRequest();
            apiGatewayProxyRequest.Headers = new Dictionary<string, string>();
            apiGatewayProxyRequest.Headers.Add("X-Forwarded-For", "127.0.0.1, 127.0.0.2");

            // Act
            var apiGatewayProxyResponse = Function.SetMyPublicIp(apiGatewayProxyRequest);

            // Assert
            Assert.Equal(200, apiGatewayProxyResponse.StatusCode);
        }
    }
}
