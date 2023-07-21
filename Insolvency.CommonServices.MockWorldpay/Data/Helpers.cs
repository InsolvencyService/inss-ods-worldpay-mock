using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Insolvency.CommonServices.MockWorldpay.Data
{
    public static class Helpers
    {
        public static string CreateMacHash(string orderKey, string paymentAmount, string paymentCurrency, string paymentStatus)
        {
            var mac = orderKey + paymentAmount + paymentCurrency + paymentStatus + Constants.MacSecret;
            return CalculateMD5Hash(mac);
        }

        private static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }


        public static string GeneratePaymentStatusUpdateXml(OrderData order)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    //document start and doc type declaration
                    var dtdPath =
                        $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}/content/paymentService_v1.dtd";
                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN", dtdPath,
                        null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");

                    writer.WriteStartElement("notify");

                    writer.WriteStartElement("orderStatusEvent");

                    writer.WriteAttributeString("orderCode", order.OrderCode);

                    writer.WriteStartElement("payment");

                    writer.WriteElementString("paymentMethod", "ECMC-SSL"); //mastercard

                    writer.WriteStartElement("amount");
                    writer.WriteAttributeString("value", order.Value);
                    writer.WriteAttributeString("currencyCode", order.Currency);
                    writer.WriteAttributeString("exponent", "2");
                    writer.WriteAttributeString("debitCreditIndicator", "credit");
                    writer.WriteEndElement(); //amount

                    writer.WriteElementString("lastEvent", order.Status);

                    writer.WriteStartElement("balance");
                    writer.WriteAttributeString("accountType", "IN_PROCESS_AUTHORISED"); //? not sure about this?

                    writer.WriteStartElement("amount");
                    writer.WriteAttributeString("value", order.Value);
                    writer.WriteAttributeString("currencyCode", order.Currency);
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

        public static async Task<bool> SendOrderStatusUpdate(string xml)
        {
            var requestContent = new StringContent(xml, Encoding.UTF8, "text/xml");

            using (var client = GetHttpClient())
            {
                try
                {
                    var result = await client.PostAsync("", requestContent);

                    return result.IsSuccessStatusCode;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }


        private static  HttpClient GetHttpClient()
        {
            var uri = ConfigurationManager.AppSettings["OrderUpdateServiceUrl"];
            var client = new HttpClient { BaseAddress = new Uri(uri) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}