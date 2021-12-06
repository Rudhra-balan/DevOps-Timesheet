using System;
using System.Net;
using HI.DevOps.DomainCore.Extensions;

namespace HI.DevOps.DomainCore.Helper
{
    public class WebClientEx : WebClient
    {
        public WebClientEx() : this(ConfigReader.QuickRead("WebClientTimeout").ToInt())
        {
        }

        public WebClientEx(int timeout)
        {
            Timeout = timeout;
        }

        /// <summary>
        ///     Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null) request.Timeout = Timeout;
            return request;
        }
    }
}