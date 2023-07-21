using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Insolvency.CommonServices.WorldpayProxy.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Insolvency.CommonServices.WorldpayProxy.Tests.Controllers
{
    [TestClass]
    public class OrderUpdateControllerTests
    {
        protected Mock<HttpContextBase> HttpContextBaseMock;
        protected Mock<HttpRequestBase> HttpRequestMock;
        protected Mock<HttpResponseBase> HttpResponseMock;
        protected Mock<NameValueCollection> FormMock;

        [TestInitialize]
        public void Setup()
        {
            HttpContextBaseMock = new Mock<HttpContextBase>();
            HttpRequestMock = new Mock<HttpRequestBase>();

            HttpResponseMock = new Mock<HttpResponseBase>();
            FormMock = new Mock<NameValueCollection>();
            HttpContextBaseMock.SetupGet(x => x.Request).Returns(HttpRequestMock.Object);
            HttpContextBaseMock.SetupGet(x => x.Response).Returns(HttpResponseMock.Object);
            HttpContextBaseMock.Setup(ctx => ctx.Request.Form).Returns(FormMock.Object);

            //Mock the Request.Url property
            var uriMock = new Uri("http://localmock");
            HttpContextBaseMock.SetupGet(r => r.Request.Url).Returns(uriMock);
        }

        [TestMethod]
        public void PaymentReceiptControllerPostTest()
        {
            var mockPaymentService = new Mock<IPaymentReceiptService>();
            mockPaymentService.Setup(x => x.UpdatePaymentReceipt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.Run(() =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    return response;
                }));

            var controller = new WorldpayProxy.Controllers.OrderUpdateController(mockPaymentService.Object);

            var mockRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(GetTestOrderUpdateXml(), Encoding.UTF8, "text/xml")
            };

            var result = controller.Post(mockRequest).Result;
            Assert.IsTrue(result.IsSuccessStatusCode);

            var content = result.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(content, "[OK]");

        }

        private string GetTestOrderUpdateXml()
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    //document start and doc type declaration
                    var dtdPath = $"";
                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN", dtdPath,
                        null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");

                    writer.WriteStartElement("notify");

                    writer.WriteStartElement("orderStatusEvent");

                    writer.WriteAttributeString("orderCode", "ADJ1234567-0001");

                    writer.WriteStartElement("payment");

                    writer.WriteElementString("paymentMethod", "ECMC-SSL"); //mastercard

                    writer.WriteStartElement("amount");
                    writer.WriteAttributeString("value", "1234");
                    writer.WriteAttributeString("currencyCode", "GBP");
                    writer.WriteAttributeString("exponent", "2");
                    writer.WriteAttributeString("debitCreditIndicator", "credit");
                    writer.WriteEndElement(); //amount

                    writer.WriteElementString("lastEvent", "AUTHORISED");

                    writer.WriteStartElement("balance");
                    writer.WriteAttributeString("accountType", "IN_PROCESS_AUTHORISED"); //? not sure about this?

                    writer.WriteStartElement("amount");
                    writer.WriteAttributeString("value", "1234");
                    writer.WriteAttributeString("currencyCode", "GBP");
                    writer.WriteAttributeString("exponent", "2");
                    writer.WriteAttributeString("debitCreditIndicator", "credit");
                    writer.WriteEndElement(); //amount

                    writer.WriteEndElement(); //balance

                    writer.WriteElementString("cardNumber", "5255********2490");
                    writer.WriteElementString("riskScore", "0");

                    writer.WriteEndElement(); //payment

                    writer.WriteEndElement(); //orderStatusEvent

                    writer.WriteEndElement(); //notify
                    writer.WriteEndElement(); //payment service
                    writer.WriteEndDocument(); //end doc
                }


                return stringWriter.ToString();
            }

        }

    }
}
