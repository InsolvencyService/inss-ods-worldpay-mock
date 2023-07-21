using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Insolvency.CommonServices.WorldpayProxy.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Insolvency.CommonServices.WorldpayProxy.Models;
using Insolvency.CommonServices.WorldpayProxy.Helpers;

namespace Insolvency.CommonServices.WorldpayProxy.Tests.Controllers
{
    [TestClass]
    public class ProxyControllerTests
    {
        protected Mock<HttpContextBase> HttpContextBaseMock;
        protected Mock<HttpRequestBase> HttpRequestMock;
        protected Mock<HttpResponseBase> HttpResponseMock;


        [TestInitialize]
        public void Setup()
        {
            HttpContextBaseMock = new Mock<HttpContextBase>();
            HttpRequestMock = new Mock<HttpRequestBase>();
            HttpResponseMock = new Mock<HttpResponseBase>();
            HttpContextBaseMock.SetupGet(x => x.Request).Returns(HttpRequestMock.Object);
            HttpContextBaseMock.SetupGet(x => x.Response).Returns(HttpResponseMock.Object);
        }

        [TestMethod]
        public void ProxyControllerPostTest()
        {
            var mockPaymentService = new Mock<IPaymentService>();
            mockPaymentService.Setup(x => x.PostOrder(It.IsAny<string>()))
                .Returns(new HttpResponseMessage(HttpStatusCode.OK));

            var controller = new WorldpayProxy.Controllers.ProxyController(mockPaymentService.Object);

            var mockRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent("<test>TEST</test>", Encoding.UTF8, "text/xml")
            };

            var result = controller.Post(mockRequest).Result;
           
            Assert.IsTrue(result.IsSuccessStatusCode);

        }
    }
}
