using System.Collections.Generic;
using Xunit;
using Amazon.Lambda.APIGatewayEvents;
using Lambda.SetMyPublicIp.Tests.Mocks;
using System;
using System.Threading.Tasks;
using Lambda.SetMyPublicIp.Models;

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
            Request request = null;

            // Act & assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(nameof(request), () => Function.SetMyPublicIp(request, null));
            Assert.Contains("The argument cannot be null. (Parameter 'request')", exception.Message);
        }

        [Fact]
        public async Task ArgumentExceptionForHostedZoneId()
        {
            // Arrange
            var request = new Request();

            // Act & assert
            var exception = await Assert.ThrowsAsync<ArgumentException>("request.HostedZoneId", () => Function.SetMyPublicIp(request, null));
            Assert.Contains("No HostedZoneId present. (Parameter 'request.HostedZoneId')", exception.Message);
        }

        [Fact]
        public async Task ArgumentExceptionForDomainName()
        {
            // Arrange
            var request = new Request();
            request.HostedZoneId = "123456789";

            // Act & assert
            var exception = await Assert.ThrowsAsync<ArgumentException>("request.DomainName", () => Function.SetMyPublicIp(request, null));
            Assert.Contains("No DomainName present. (Parameter 'request.DomainName')", exception.Message);
        }

        [Fact]
        public async Task ArgumentExceptionForPublicIps()
        {
            // Arrange
            var request = new Request();
            request.HostedZoneId = "123456789";
            request.DomainName = "test.com";

            // Act & assert
            var exception = await Assert.ThrowsAsync<ArgumentException>("request.PublicIps", () => Function.SetMyPublicIp(request, null));
            Assert.Contains("No PublicIps present. (Parameter 'request.PublicIps')", exception.Message);
        }

        [Fact]
        public void Success()
        {
            // Arrange
            var request = new Request();
            request.HostedZoneId = "123456789";
            request.DomainName = "test.com";
            request.PublicIps  = "127.0.0.1, 127.0.0.2";

            // Act
            var result = Function.SetMyPublicIp(request, null).Result;

            // Assert
            Assert.Equal("{\"ChangeRequestId\":\"007\",\"ChangeRequestStatus\":\"PENDING\",\"PublicIp\":\"127.0.0.1\"}", result);
        }
    }
}
