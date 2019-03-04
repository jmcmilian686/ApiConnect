using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ApiConnect.Models;
using RestSharp;

namespace ApiConnect.Controllers
{
    public class SortController : Controller
    {
       


        // =============================Sort Request Area===================================================================================================
        public ActionResult SortRequest()
        {
            SortReq sreq = new SortReq();
            sreq.messResponse = new SortResponse();
            sreq.messResponse.payload = new SortRespPayload();
            sreq.messageCode = "SORTREQUEST";
            if (Session["token"] != null)
            {
                ViewBag.Connected = "ok";
            }

            return View(sreq);
        }

        [HttpPost]
        public ActionResult SortRequest(SortReq sreq)
        {
            ViewBag.Connected = "ok";
           
            if (ModelState.IsValid)
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //var client = new RestClient("https://qa.sensorthink.com/iot/integ/message");
                RestClient client;

                if (Session["MessageAddress"].ToString() != "NOAD")
                {
                    client = new RestClient(Session["MessageAddress"].ToString());
                }
                else {
                    client = new RestClient("https://qa.sensorthink.com/iot/integ/message");
                }
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("x-authorization", "Bearer " + Session["token"]);
                request.AddParameter("application/json", "{\n\t\"messageNumber\":" + sreq.messageNumber + ",\n\t\"messageCode\":\"" + sreq.messageCode + "\",\n\t\"stationId\": \"" + sreq.stationId + "\",\n\t\"scannedItemType\":" + sreq.scannedItemType + ",\n\t\"scannedItemBarcode\":\"" + sreq.scannedItemBarcode + "\",\n\t\"robotId\":\"" + sreq.robotId + "\",\n\t\"messageTimestamp\":\"" + date + "\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                    var desResponse = deserial.Deserialize<SortResponse>(response);

                    sreq.messResponse = desResponse;
                   
                }
                else {
                    var content = response.Content;
                    ViewBag.Error = 0;
                    ViewBag.Response = content.ToString();
                }
               


               

                return View("SortRequest", sreq);

            }
            else {

                return View("SortRequest", sreq);
            }

        }

        public PartialViewResult _SortResponsePartial(SortResponse response) {

            if (response != null)
            {
                return PartialView(response);
            }
            else {
                SortResponse sortmodel = new SortResponse();
                sortmodel.payload = new SortRespPayload();

                return PartialView(sortmodel);
            }
            

        }

        //============================================End of Sort Request Area====================================================================

        //=============================================Sort Confirmation Area===============================================================
        public ActionResult SortConfirmation()
        {
            SortConf sconf = new SortConf();
            sconf.messResponse = new SortConfResp();
            sconf.messageCode = "SORTCONFIRMATION";
            if (Session["token"] != null)
            {
                ViewBag.Connected = "ok";
            }

            return View(sconf);
        }

        [HttpPost]
        public ActionResult SortConfirmation(SortConf sconf)
        {
            ViewBag.Connected = "ok";
            if (ModelState.IsValid)
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                RestClient client;

                if (Session["MessageAddress"].ToString() != "NOAD")
                {
                    client = new RestClient(Session["MessageAddress"].ToString());
                }
                else
                {
                    client = new RestClient("https://qa.sensorthink.com/iot/integ/message");
                }
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("x-authorization", "Bearer " + Session["token"]);
                request.AddParameter("application/json", "{\n\t\"messageNumber\":" + sconf.messageNumber + ",\n\t\"messageCode\":\"" + sconf.messageCode + "\",\n\t\"scannedItemType\":" + sconf.scannedItemType + ",\n\t\"scannedItemBarcode\":\"" + sconf.scannedItemBarcode + "\",\n\t\"robotId\":\"" + sconf.robotId + "\",\n\"destinationId\":\"" + sconf.destinationId + "\",\n\"inductToDeliveryDistance\":" + sconf.inductToDeliveryDistance + ",\n\"inductToDeliveryTime\":" + sconf.inductToDeliveryTime + ",\n\"reasonCode\":" + sconf.reasonCode + ",\n\t\"messageTimestamp\":\"" + date + "\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                    var desResponse = deserial.Deserialize<SortConfResp>(response);

                    sconf.messResponse = desResponse;

                }
                else
                {
                    var content = response.Content;
                    ViewBag.Error = 0;
                    ViewBag.Response = content.ToString();
                }





                return View("SortConfirmation", sconf);

            }
            else
            {

                return View("SortConfirmation", sconf);
            }
           
        }

        public PartialViewResult _SortConfirmationPartial(SortConfResp response)
        {

            if (response != null)
            {
                return PartialView(response);
            }
            else
            {
                SortConfResp sortmodel = new SortConfResp();
                

                return PartialView(sortmodel);
            }


        }
        //=========================================End of Sort Confirmation Area====================================================

        //=========================================Reassign Sort Request Area========================================================
        public ActionResult ReasSortRequest()
        {
            SortReq sreq = new SortReq();
            sreq.messResponse = new SortResponse();
            sreq.messResponse.payload = new SortRespPayload();
            sreq.messageCode = "REASSIGNSORTREQUEST";

            if (Session["token"] != null)
            {
                ViewBag.Connected = "ok";
            }

            return View(sreq);
        }

        [HttpPost]
        public ActionResult ReasSortRequest(SortReq sreq)
        {
            ViewBag.Connected = "ok";
            if (ModelState.IsValid)
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                RestClient client;

                if (Session["MessageAddress"].ToString() != "NOAD")
                {
                    client = new RestClient(Session["MessageAddress"].ToString());
                }
                else
                {
                    client = new RestClient("https://qa.sensorthink.com/iot/integ/message");
                }
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("x-authorization", "Bearer " + Session["token"]);
                request.AddParameter("application/json", "{\n\t\"messageNumber\":" + sreq.messageNumber + ",\n\t\"messageCode\":\"" + sreq.messageCode + "\",\n\t\"stationId\": \"" + sreq.stationId + "\",\n\t\"scannedItemType\":" + sreq.scannedItemType + ",\n\t\"scannedItemBarcode\":\"" + sreq.scannedItemBarcode + "\",\n\t\"robotId\":\"" + sreq.robotId + "\",\n\t\"messageTimestamp\":\"" + date + "\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                    var desResponse = deserial.Deserialize<SortResponse>(response);

                    sreq.messResponse = desResponse;

                }
                else
                {
                    var content = response.Content;
                    ViewBag.Error = 0;
                    ViewBag.Response = content.ToString();
                }





                return View("SortRequest", sreq);

            }
            else
            {

                return View("SortRequest", sreq);
            }

        }

        public PartialViewResult _ReasSortRequestPartial(SortResponse response)
        {

            if (response != null)
            {
                return PartialView(response);
            }
            else
            {
                SortResponse sortmodel = new SortResponse();
                sortmodel.payload = new SortRespPayload();

                return PartialView(sortmodel);
            }


        }

        //=========================================End of Reassign Request Area======================================================
    }
}