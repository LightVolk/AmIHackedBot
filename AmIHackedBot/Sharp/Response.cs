using System;
using System.Collections.Generic;
using System.Text;

namespace AmIHackedBot.Sharp
{
    public class Response
    {
        public string StatusCode { get; set; }
        public string Body { get; set; }
        public string HttpException { get; set; }
    }
}
