using Lambda.SetMyPublicIp.Tests.Mocks;
using System;
using System.Threading.Tasks;
using Lambda.SetMyPublicIp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lambda.SetMyPublicIp.Tests
{
    [TestClass]
    public class FunctionTest
    {
        public FunctionTest()
        {
            // Initialise the Function class by passing in a Mock RouteHandler
            Function.Initialise(new MockedRouteHandler());
        }

        [TestMethod]
        public async Task ArgumentNullExceptionForRequest()
        {
            // Arrange
            Request request = null;

            // Act & assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => Function.SetMyPublicIp(request, null));
            Assert.IsTrue(exception.Message.Contains("The argument cannot be null. (Parameter 'request')"));
        }

        [TestMethod]
        public async Task ArgumentExceptionForHostedZoneId()
        {
            // Arrange
            var request = new Request();

            // Act & assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() => Function.SetMyPublicIp(request, null));
            Assert.IsTrue(exception.Message.Contains("No HostedZoneId present. (Parameter 'request.HostedZoneId')"));
        }

        [TestMethod]
        public async Task ArgumentExceptionForDomainName()
        {
            // Arrange
            var request = new Request();
            request.HostedZoneId = "123456789";

            // Act & assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() => Function.SetMyPublicIp(request, null));
            Assert.IsTrue(exception.Message.Contains("No DomainName present. (Parameter 'request.DomainName')"));
        }

        [TestMethod]
        public async Task ArgumentExceptionForPublicIps()
        {
            // Arrange
            var request = new Request();
            request.HostedZoneId = "123456789";
            request.DomainName = "test.com";

            // Act & assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() => Function.SetMyPublicIp(request, null));
            Assert.IsTrue(exception.Message.Contains("No PublicIps present. (Parameter 'request.PublicIps')"));
        }

        [TestMethod]
        public void Success()
        {
            // Arrange
            var request = new Request();
            request.HostedZoneId = "123456789";
            request.DomainName = "test.com";
            request.PublicIps  = "127.0.0.1, 127.0.0.2";

            // Act
            var result = Function.SetMyPublicIp(request, null).Result;

            // Expected
            var expected = new Response()
            {
                ChangeRequestId = "007",
                ChangeRequestStatus = "PENDING",
                PublicIp = "127.0.0.1"
            };

            // Assert
            Assert.AreEqual(expected.ChangeRequestId, result.ChangeRequestId);
            Assert.AreEqual(expected.ChangeRequestStatus, result.ChangeRequestStatus);
            Assert.AreEqual(expected.PublicIp, result.PublicIp);
        }
    }
}
