using Insolvency.CommonServices.WorldpayProxy.Interface;
using INSS.ODS.Common.Utilities.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;

namespace Insolvency.CommonServices.WorldpayProxy.Implementations
{
    public class WorldpayPaymentService: IPaymentService
    {
        private HttpWebRequest CreateHttpWebRequest(string requestBody)
        {
            //Get Worldpay url from config
            var worldpayUrl = ConfigurationManager.AppSettings["WorldpayPaymentServiceUrl"];

            //Create the request to Worldpay
            var request = (HttpWebRequest)WebRequest.Create(worldpayUrl);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            var bytes = Encoding.UTF8.GetBytes(requestBody);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";

            //Add Security headers
            var username = ConfigurationManager.AppSettings["WorldpayUsername"];
            var password = ConfigurationManager.AppSettings["WorldpayPassword"];
            var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);

            //Write the request data
            var requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();

            return request;
        }


        public HttpResponseMessage PostOrder(string requestBody)
        {
            var request = CreateHttpWebRequest(requestBody);

            //Submit the request and get the response
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseStream = response.GetResponseStream();
                var responseString = new StreamReader(responseStream).ReadToEnd();

                //Generate the response from the proxy with the same content and content type as the Worldpay response
                var formatter = new XmlMediaTypeFormatter { Indent = true };
                formatter.SupportedEncodings.Clear();
                formatter.SupportedEncodings.Add(Encoding.UTF8);

                var responseContent = new StringContent(responseString, Encoding.UTF8, "text/xml");

                var proxyResponse = new HttpResponseMessage { Content = responseContent };
                proxyResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(response.ContentType);
                return proxyResponse;
            }

            var errorStream = response.GetResponseStream();
            var errorString = new StreamReader(errorStream).ReadToEnd();
            Logger.LogError($"Error posting order to worldpay/proxy: {errorString}");
            var result = new HttpResponseMessage(HttpStatusCode.BadRequest) {Content = new StringContent(errorString)};
            return result;
        }


    }
}