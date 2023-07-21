using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;
using Insolvency.CommonServices.MockWorldpay.Data;

namespace Insolvency.CommonServices.MockWorldpay.Controllers
{
    //Mocks Worldpay's receipt of an order and sends back a redirect message       
    public class PaymentServiceController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            //parse request body for the order number etc
            var reader = new StreamReader(Request.Content.ReadAsStreamAsync().Result);
            var xml = reader.ReadToEnd();
            var orderData = ParseResult(xml);

            if (orderData == null)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorResponse.Content = new StringContent("Error parsing request");
                return errorResponse;
                //return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (orderData is OrderData)
            {
                return PostOrder(orderData as OrderData);
            }

            if (orderData is RefundData)
            {
                return PostRefundData(orderData as RefundData);
            }

            if (orderData is CancelData)
            {
                return PostCancelData(orderData as CancelData);
            }



            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }


        private HttpResponseMessage PostOrder(OrderData orderData)
        {
            var orderKey = $"{orderData.MerchantCode}^{orderData.OrderCode}";
            var redirectUrl = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}/selectpaymentmethod?orderKey={orderKey}";

            OrderDataStore.Add(orderKey, orderData);

            using (var stringWriter = new StringWriter())
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    //document start and doc type declaration
                    var dtdPath = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}/content/paymentService_v1.dtd";
                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN", dtdPath, null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");

                    writer.WriteStartElement("reply");

                    writer.WriteStartElement("orderStatus");

                    writer.WriteAttributeString("orderCode", orderData.OrderCode);

                    writer.WriteStartElement("reference");

                    /*
                    The id attribute of the reference element can be used as a payment reference if the shopper is
                    expected to make a payment with an off-line payment method like a bank transfer or Accept Giro. In the
                    case of Accept Giro, the reference id number should be printed on the Accept Giros as the payment
                    reference.
                    */

                    writer.WriteAttributeString("id", "");

                    writer.WriteString(redirectUrl);

                    writer.WriteEndElement(); //reference

                    writer.WriteEndElement(); //order status

                    writer.WriteEndElement(); //reply
                    writer.WriteEndElement(); //payment service
                    writer.WriteEndDocument(); //end doc
                }

                //Return UTF-8 XML response
                var responseContent = new StringContent(stringWriter.ToString(), Encoding.UTF8, "text/xml");
                var response = new HttpResponseMessage { Content = responseContent };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

                return response;

            }
        }




        private HttpResponseMessage PostRefundData(RefundData orderData)
        {
            //var orderKey = $"{orderData.MerchantCode}^{orderData.OrderCode}";
            //var redirectUrl = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}/selectpaymentmethod?orderKey={orderKey}";

            //OrderDataStore.Add(orderKey, orderData);

            using (var stringWriter = new StringWriter())
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    //document start and doc type declaration
                    var dtdPath = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}/content/paymentService_v1.dtd";
                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN", dtdPath, null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");

                    writer.WriteStartElement("reply");

                    writer.WriteStartElement("ok");

                    writer.WriteStartElement("refundReceived");

                    writer.WriteAttributeString("orderCode", orderData.OrderCode);

                    writer.WriteStartElement("amount");

                    writer.WriteAttributeString("value", orderData.RefundValue);
                    writer.WriteAttributeString("currencyCode", orderData.Currency);
                    writer.WriteAttributeString("exponent", "2");
                    writer.WriteAttributeString("debitCreditIndicator","credit");

                    writer.WriteEndElement(); //amount

                    writer.WriteEndElement(); //refundReceived
                    writer.WriteEndElement(); //ok
                    writer.WriteEndElement(); //reply
                    writer.WriteEndElement(); //payment service
                    writer.WriteEndDocument(); //end doc
                }

                //Return UTF-8 XML response
                var responseContent = new StringContent(stringWriter.ToString(), Encoding.UTF8, "text/xml");
                var response = new HttpResponseMessage { Content = responseContent };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

                return response;

            }
        }




        private HttpResponseMessage PostCancelData(CancelData orderData)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    //document start and doc type declaration
                    var dtdPath = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}/content/paymentService_v1.dtd";
                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN", dtdPath, null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");

                    writer.WriteStartElement("reply");

                    writer.WriteStartElement("ok");

                    writer.WriteStartElement("voidReceived");

                    writer.WriteAttributeString("orderCode", orderData.OrderCode);

                    writer.WriteEndElement(); //voidReceived
                    writer.WriteEndElement(); //ok
                    writer.WriteEndElement(); //reply
                    writer.WriteEndElement(); //payment service
                    writer.WriteEndDocument(); //end doc
                }

                //Return UTF-8 XML response
                var responseContent = new StringContent(stringWriter.ToString(), Encoding.UTF8, "text/xml");
                var response = new HttpResponseMessage { Content = responseContent };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

                return response;

            }
        }


        private static IWorldpayRequest ParseResult(string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode root = doc.DocumentElement;

                if (root.SelectSingleNode("submit") != null)
                {
                    return ParseOrderData(xml);
                }

                var modifyNode = root.SelectSingleNode("modify");
                var orderModificationNode = modifyNode.SelectSingleNode("orderModification");

                if (orderModificationNode != null)
                {
                    if (orderModificationNode.SelectSingleNode("refund")!=null)
                    {
                        return ParseRefundData(xml);
                    }
                    else if(orderModificationNode.SelectSingleNode("cancelOrRefund") != null)
                    {
                        return ParseCancelOrRefundData(xml);
                    }

                    
                }

                throw new XmlException("Could not parse xml");

            }
            catch (Exception)
            {
                return null;
            }
        }

        private static OrderData ParseOrderData(string xml)
        {
            try
            {
                var result = new OrderData();

                var doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode root = doc.DocumentElement;

                result.MerchantCode = root.Attributes["merchantCode"].Value;

                var nSubmit = root.SelectSingleNode("submit");
                var nOrder = nSubmit.SelectSingleNode("order");
                var nDescription = nOrder.SelectSingleNode("description");
                var nAmount = nOrder.SelectSingleNode("amount");

                result.OrderCode = nOrder.Attributes["orderCode"].Value;
                result.Description = nDescription.InnerText;
                result.Value = nAmount.Attributes["value"].Value;
                result.Currency = nAmount.Attributes["currencyCode"].Value;

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static RefundData ParseRefundData(string xml)
        {
            try
            {
                var result = new RefundData();

                var doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode root = doc.DocumentElement;

                var nModify = root.SelectSingleNode("modify");
                var nOrderModification = nModify.SelectSingleNode("orderModification");
                var nRefund = nOrderModification.SelectSingleNode("refund");
                var nAmount = nRefund.SelectSingleNode("amount");

                result.OrderCode = nOrderModification.Attributes["orderCode"].Value;
                result.RefundValue = nAmount.Attributes["value"].Value;
                result.Currency = nAmount.Attributes["currencyCode"].Value;

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static CancelData ParseCancelOrRefundData(string xml)
        {
            try
            {
                var result = new CancelData();

                var doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode root = doc.DocumentElement;

                var nModify = root.SelectSingleNode("modify");
                var nOrderModification = nModify.SelectSingleNode("orderModification");
                var nRefund = nOrderModification.SelectSingleNode("cancelOrRefund");
                result.OrderCode = nOrderModification.Attributes["orderCode"].Value;

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

}
