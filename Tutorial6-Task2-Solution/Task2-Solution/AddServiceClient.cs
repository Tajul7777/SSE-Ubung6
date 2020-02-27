using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SSE
{
    public class AddServiceClient
    {
        private readonly string _serviceLocation;
        
        public AddServiceClient(string serviceLcoation)
        {
            _serviceLocation = serviceLcoation;
        }

        /// <summary>
        /// Sends a SOAP request via HTTP to a Web service endpoint.
        /// </summary>
        public async Task<int> Add(int a, int b)
        {
            // prepare request
            XNamespace ws = "http://vsr.informatik.tu-chemnitz.de/edu/2008/pvs/soapwebservice";
            XElement request = new XElement(ws + "Add",
                new XElement(ws + "a", a),
                new XElement(ws + "b", b));

            // build soap message
            string content =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">" +
                "<soap12:Body>" +
                request +
                "</soap12:Body></soap12:Envelope>";

            // send over HTTP
            Dictionary<string, string> para = new Dictionary<string, string>();
            para["content-type"] = "application/soap+xml; charset=utf-8";
            HttpMessage answer = await HttpRequest.Post(_serviceLocation, content, para);

            // parse body content of resulting XML
            var doc = new XPathDocument(new StringReader(answer.Content));
            var navigator = doc.CreateNavigator();
            return Int32.Parse(navigator.SelectSingleNode("//*[1]/*[1]/*[1]/*[1]").Value);
        }
    }
}