using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiConnect.Models
{
    public class SortConf
    {

        public int messageNumber { get; set; }

        [Required(ErrorMessage = "Code Required!")]
        public string messageCode { get; set; }

        [Required(ErrorMessage = "Scanned Item Type Required!")]
        public int scannedItemType { get; set; }

        [Required(ErrorMessage = "Scanned Item Barcode Required!")]
        public string scannedItemBarcode { get; set; }

        [Required(ErrorMessage = "Robot Id Required!")]
        public int robotId { get; set; }

        [Required(ErrorMessage = "Destination Id Required!")]
        public string destinationId { get; set; }

        [Required(ErrorMessage = "Distance to Deliv. Required!")]
        public int inductToDeliveryDistance { get; set; }

        [Required(ErrorMessage = "Time to Deliv Required!")]
        public string inductToDeliveryTime { get; set; }

        [Required(ErrorMessage = "Resason Code Required!")]
        public int reasonCode { get; set; }
                
        public DateTime messageTimestamp { get; set; }

        public SortConfResp messResponse { get; set; }

    }

    public class SortConfResp
    {
        public string status { get; set; }

        public string statusCode { get; set; }

        public string statusDesc { get; set; }

        public string messageNumber { get; set; }

        public DateTime messageTimeStamp { get; set; }
    }
}