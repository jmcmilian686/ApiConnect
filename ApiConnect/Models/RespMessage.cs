using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Models
{
    // for deserialization of login response- Use json2csharp.com to define the class. It can handle embedded JSON as well
    public class AccessToken
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
    }

    // for deserialization of Generic message response
    public class GenericMessageResponse
    {
        public string status { get; set; }
        public string statusCode { get; set; }
        public string statusDesc { get; set; }
        public string messageNumber { get; set; }
        public string messageTimestamp { get; set; }
    }

    // for deserialization of Wave message response
    public class WaveMessageResponse
    {
        public string status { get; set; }
        public string statusCode { get; set; }
        public string messageNumber { get; set; }
        public string messageTimestamp { get; set; }
    }

    // for deserialization of Wave header response
    public class WaveHeaderResponse
    {
        public string status { get; set; }
        public string statusCode { get; set; }
        public string statusDesc { get; set; }
        public string messageNumber { get; set; }
        public string messageTimestamp { get; set; }
    }

    // for deserialization of Order message response
    public class OrderMessageResponse
    {
        public string status { get; set; }
        public string statusCode { get; set; }
        public string messageNumber { get; set; }
        public string messageTimestamp { get; set; }
    }

    // for deserialization of Order Header message response
    public class OrderHeaderMessageResponse
    {
        public string status { get; set; }
        public string statusCode { get; set; }
        public string messageNumber { get; set; }
        public string messageTimestamp { get; set; }
    }

    // for deserialization of Container message response
    public class ContainerMessageResponse
    {
        public string status { get; set; }
        public string statusCode { get; set; }
        public string messageNumber { get; set; }
        public string messageTimestamp { get; set; }
    }

    // for deserialization of Container header message response
    public class ContainerHeaderMessageResponse
    {
        public string status { get; set; }
        public string statusCode { get; set; }
        public string messageNumber { get; set; }
        public string messageTimestamp { get; set; }
    }

    // for deserialization of Line Detail message response
    public class LineDetailMessageResponse
    {
        public string status { get; set; }
        public string statusCode { get; set; }
        public string messageNumber { get; set; }
        public string messageTimestamp { get; set; }
    }

    // for deserialization of Pack Complete message response
    public class PackCompleteMessageResponse
    {
        public string status { get; set; }
        public string statusCode { get; set; }
        public string messageNumber { get; set; }
        public string messageTimestamp { get; set; }
    }
}