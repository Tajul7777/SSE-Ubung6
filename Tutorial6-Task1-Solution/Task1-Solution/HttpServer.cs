using System;

namespace SSE
{
    public class HttpServer : TcpServer
    {
        /// <summary>
        /// Handles an incoming line of text. 
        /// </summary>
        /// <param name="line">Incoming data.</param>
        /// <returns>The answer to be sent back as a reaction to the received line or null.</returns>
        protected override string HandleRequest(string msg)
        {
            // parse message
            HttpMessage request = new HttpMessage(msg);

            // build answer message
            HttpMessage answer = ReceiveRequest(request);

            // send answer message
            Console.WriteLine("HTTP: Sending answer.");
            return answer.ToString();
        }

        /// <summary>
        /// Handle an incoming HTTP request.
        /// </summary>
        /// <param name="request">Incoming request.</param>
        /// <returns>The answer message to be sent back.</returns>
        protected virtual HttpMessage ReceiveRequest(HttpMessage request)
        {
            // parse relative URL in request
            Url requestUrl = new Url(request.Resource);

            // simple hardcoded HTML response
            if (requestUrl.Path == "/" && request.Method == HttpMessage.GET)
            {
                // access counter cookies
                var cookies = request.GetCookies();
                int counter = 0;
                if (cookies.ContainsKey("counter"))
                {
                    counter = Convert.ToInt32(cookies["counter"]);
                }

                // count up and add counter to served content
                counter++;
                var response = new HttpMessage("200", "Ok", null, "<html><body>This is your " + counter + ". visit to this site.<hr /></body></html>");
                response.SetCookie("counter", counter.ToString());

                return response;
            }
            // not found
            else
            {
                return new HttpMessage("404", "Not Found", null, "<html><body>The requested file could not be found.</body></html>");
            }
        }
    }
}
