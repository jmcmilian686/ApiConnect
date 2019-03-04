using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiConnect.Models
{
    public class SortReq
    {
        public int messageNumber { get; set; }

        [Required(ErrorMessage = "Code Required!")]
        public string messageCode { get; set; }

        [Required(ErrorMessage = "Station Id Required!")]
        public string stationId { get; set; }

        [Required(ErrorMessage = "Scanned Item Type Required!")]
        public int scannedItemType { get; set; }

        [Required(ErrorMessage = "Scanned Item Barcode Required!")]
        public string scannedItemBarcode { get; set; }

        [Required(ErrorMessage = "Robot Id Required!")]
        public int robotId { get; set; }


        public DateTime messageTimestamp { get; set; }

        public SortResponse messResponse { get; set; }

    }

    public class SortResponse
    {
        public string status { get; set; }
        public int statusCode { get; set; }
        public string statusDesc { get; set; }
        public int messageNumber { get; set; }
        public SortRespPayload payload { get; set; }
        public string messageTimeStamp { get; set; }

    }

    public class SortRespPayload
    {
        public string messageCode { get; set; }
        public string stationId { get; set; }
        public string robotId { get; set; }
        public int systemAction { get; set; }
        public string userAction { get; set; }
        public string userMessage { get; set; }
        public string destinationId { get; set; }
        public int lastDivertToInductDistance { get; set; }
        public string lastDivertToInductTime { get; set; }
        public string messageTimestamp { get; set; }

    }
}