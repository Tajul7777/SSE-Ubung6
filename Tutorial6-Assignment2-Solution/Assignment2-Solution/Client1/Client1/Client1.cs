using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using HttpLib;
using Vsr.Teaching.Pvs.Server;

namespace Client1
{
    class Client1: TcpServer
    {
        protected override string ReceiveMessage(string message, string endpoint)
        {
            XDocument doc = XDocument.Parse( message );
            var headers = doc.XPathSelectElements( "//*[1]/*[1]/*" );
            bool encrypted = headers.Any(x => x.Name.Equals( XNamespace.Get( "http://security.org/" ) + "Encryption"));
            string company;
            string price;
            if(encrypted)
            {
                var soapNs = XNamespace.Get( "http://www.w3.org/2001/09/soap-envelope");
                var secNs = XNamespace.Get( "http://security.org/" );
                string encryptedContent = doc.Element( soapNs + "Envelope" ).Element( soapNs + "Body" ).Element( secNs + "EncryptedContent").Value;
                var decrypted = Encryption.Decrypt(encryptedContent, "password");
                var content = XElement.Parse( decrypted );
                company = content.XPathSelectElement( "//*[1]" ).Value;
                price = content.XPathSelectElement( "//*[2]" ).Value;
            }
            else
            {
                company = doc.XPathSelectElement( "//*[1]/*[2]/*[1]/*[1]" ).Value;
                price = doc.XPathSelectElement( "//*[1]/*[2]/*[1]/*[2]" ).Value;
            }
            Console.WriteLine( "Client 1: incoming {2}update: {0},{1}", 
                company, 
                price,
                encrypted ? "encrypted " : "");
            return "";
        }

        static void Main(string[] args)
        {
            Client1 server = new Client1();
            Console.WriteLine("Client1 started...");
            server.Run("127.0.0.1", 15001);
        }
    }

}
