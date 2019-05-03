using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ApiConnect.Domain.Abstract;
using ApiConnect.Domain.Entities;
using ApiConnect.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ApiConnect.Controllers
{
    
    public class SendController : Controller
    {
        private IDataFieldRepository datafieldRepo;
        private IFieldsRepository fieldRepo;
        struct prod
        {
            public string sku;
            public string barcode;
            public int qty;
        };

        public SendController(IDataFieldRepository dataFieldRepository, IFieldsRepository fieldsRepository)
        {
            this.datafieldRepo = dataFieldRepository;
            this.fieldRepo = fieldsRepository;
        }

        public class InValue
        {

            public int Timer { get; set; }

        }

        public class InValue2
        {

            public object file { get; set; }
            public int Containers { get; set; }

        }

        public class PickToteDS
        {

            public Dictionary<string, int> Product { get; set; }
            public int Batch { get; set; }

        }
        public List<string> SendingList;
        public List<OrdMess> MessageList;
        [HttpGet]
        public ActionResult Index()
        {
            List<OrdMess> ord1 = new List<OrdMess>();
            SendModel sendModel1 = new SendModel();
            //if (Session["Messages"].ToString()!=null)
            //{

                MessageList = (List<OrdMess>)Session["Messages"];
            //}
           
            if (MessageList != null)
            {
                sendModel1.Messages = MessageList;
                sendModel1.OrdQty = MessageList.Count();
                sendModel1.Timer = 1;
                sendModel1.MessageType = Session["MessageType"].ToString();
            }
            else
            {
                sendModel1.Messages = ord1;
                sendModel1.OrdQty = 0;
                sendModel1.Timer = 0;
            }


            RedirectToAction("Index", "Send");
            return View(sendModel1);
        }


        public ActionResult SendTimed(InValue mod)
        {
            string respMessage = "";
            if (mod.Timer > 0)
            {

                //RestClient client;
                RestClient client;
                client = Session["MessageAddress"].ToString() != "NOAD" ? new RestClient(Session["MessageAddress"].ToString()) : new RestClient("https://qa.sensorthink.com/iot/integ/message");

                List<string> sendlist = (List<string>)Session["SendingList"];
                if (sendlist.Count > 0)
                {
                    int mesCounter = 0;
                    
                    foreach (var json in sendlist)
                    {
                        //JObject o1 = JObject.Parse(json);
                        //var subitem = JObject.Parse(o1["orderHeader"].ToString());
                        //string orderId = subitem["orderId"].ToString();

                        mesCounter++;
                        client = Session["MessageAddress"].ToString() != "NOAD" ? new RestClient(Session["MessageAddress"].ToString()) : new RestClient("https://qa.sensorthink.com/iot/integ/message");
                        
                        var request = new RestRequest(Method.POST);

                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("X-Authorization", "Bearer " + Session["token"]);
                        request.AddParameter("application/json", json, ParameterType.RequestBody);
                        IRestResponse response = client.Execute(request);

                        // ============ End of Message======================
                        if (response.ErrorMessage != null)
                            response.StatusCode = HttpStatusCode.BadRequest;

                        RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                        var desResponse = deserial.Deserialize<MsgResponse>(response);
                        respMessage += /*orderId +*/ "---" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +">>" + (desResponse.status + desResponse.statusCode + desResponse.statusDesc) + "--";
                        //respMessage += orderId + "---" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine;
                        System.Threading.Thread.Sleep(mod.Timer * 10);

                    }
                }
            }

            return Json(respMessage);

        }


        [HttpPost]
        public ActionResult UpFile()// uploading file to process
        {
            Session["Messages"] = "";
            Session["SendingList"] = "";
            Session["MessageType"] = "";
            string mType = "";
            if (SendingList != null)
            {
                SendingList.Clear();

            }
            else
            {
                SendingList = new List<string>();
            }
           

            List<OrdMess> messages = new List<OrdMess>();
            
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        TextReader tr = new StreamReader(stream);
                        string content = tr.ReadToEnd();
                        string[] contLines = tr.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        contLines = content.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        foreach (var jsonLine in contLines)
                        {
                            
                            SendingList.Add(jsonLine);
                            JObject auxJobj = JObject.Parse(jsonLine);
                            OrdMess ordAux = new OrdMess();
                            ordAux.Message = jsonLine;
                            messages.Add(ordAux);
                            foreach (var iterObj in auxJobj)
                            {
                                if (iterObj.Key == "messageCode")
                                {
                                    if (iterObj.Value.ToString() == "2002")
                                    {
                                        mType = "Order";

                                    }
                                    if (iterObj.Value.ToString() == "3007")
                                    {
                                        mType = "Batch";
                                    }
                                    if (iterObj.Value.ToString() == "3005")
                                    {
                                        mType = "Container";
                                    }
                                }

                               

                            }

                        }
                     
                    }  
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            SendModel newSendMod = new SendModel();
            newSendMod.Messages = messages;
            newSendMod.OrdQty = messages.Count();
            Session["Messages"] = messages;
            Session["SendingList"] = SendingList;
            Session["MessageType"] = mType;
            MessageList = messages;
            return RedirectToAction("Index","Send");
        }

        [HttpPost]

        public ActionResult UpBatch()
        {
            int contQty = Convert.ToInt32(Request["cont"]);
            List<CONTAINER2> outpCont = new List<CONTAINER2>();
            List<string> usedBarcodes = new List<string>();
            List<prod> usedProds = new List<prod>();
            List<prod> actualProds = new List<prod>();

            try
            {
                //foreach (string file in Request.Files)
                //{
                //    var fileContent = Request.Files[file];
                //    if (fileContent != null && fileContent.ContentLength > 0)
                //    {
                //        // get a stream
                //        var stream = fileContent.InputStream;
                //        TextReader tr = new StreamReader(stream);
                //        string content = tr.ReadToEnd();
                //        string[] contLines = tr.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                //        contLines = content.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        var messagesSess = (List<OrdMess>)Session["Messages"];

                        foreach (var contLineItem in messagesSess)
                        {
                            BATCH_PUT batch = JsonConvert.DeserializeObject<BATCH_PUT>(contLineItem.Message.ToString());
                            Dictionary<string, int> products = new Dictionary<string, int>();

                            if (batch != null && batch.orderContainers.Count > 0)
                            {
                                foreach (var item in batch.orderContainers)
                                {
                                    foreach (var ordC in item.containerDetail)// including products to the Products List
                                    {
                                        prod newProd = new prod();

                                        newProd.barcode = ordC.productCode;
                                        newProd.sku = ordC.sku;
                                        newProd.qty = Convert.ToInt32(ordC.requestedQty);

                                        if (actualProds.Where(m => m.sku == newProd.sku).Count() == 0)
                                        {
                                            actualProds.Add(newProd);
                                        }
                                        else
                                        {
                                            var foudPr = actualProds.Find(l => l.barcode == newProd.barcode);
                                            actualProds.Remove(foudPr);
                                            foudPr.qty += newProd.qty;
                                            actualProds.Add(foudPr);

                                        }
                                        //if (!products.ContainsKey(ordC.productCode))
                                        //{
                                        //    products.Add(ordC.productCode, Convert.ToInt32(ordC.requestedQty));
                                        //}
                                        //else {

                                        //    products[ordC.productCode] += Convert.ToInt32(ordC.requestedQty);

                                        //}


                                    }

                                }
                            }

                            if (actualProds.Count > 0)
                            {

                                if (actualProds.Count > contQty) // determine the Qty of products per container
                                {
                                    int amItemCont = 0;
                                    int rest = 0;

                                    amItemCont = actualProds.Count / contQty;
                                    rest = actualProds.Count % contQty;

                                    for (int i = 0; i < contQty; i++)
                                    {
                                        int amntDetails = 0;
                                        if (rest != 0  && i == contQty - 1)
                                        {
                                            amItemCont += rest;
                                            
                                        }

                                        CONTAINER2 containerOut = new CONTAINER2(amItemCont);
                                        //=====================Start populating the container message details=====================
                                        containerOut.companyId = batch.companyId;
                                        containerOut.facilityId = batch.facilityId;
                                        var mNumber = fieldRepo.Fields.Where(k => k.Name == "messageNumber").FirstOrDefault();
                                        containerOut.messageNumber = mNumber.Counter.ToString();
                                        mNumber.Counter++;
                                        fieldRepo.SaveField(mNumber);
                                        containerOut.containerHeader = new CONT_HEADER2();
                                        containerOut.containerHeader.batchId = batch.batchId;
                                        containerOut.containerHeader.unitSortZone = batch.zone;
                                        containerOut.containerHeader.detailCount = amItemCont.ToString();
                                        containerOut.containerHeader.user = "PC_generated";
                                        DataField barcode = datafieldRepo.DataFields.Where(k => k.Field.Name == "Container" && !usedBarcodes.Contains(k.Value)).FirstOrDefault();
                                        if (barcode != null)
                                        {
                                            containerOut.containerHeader.containerBarCode = barcode.Value;
                                            usedBarcodes.Add(barcode.Value);
                                        }
                                        containerOut.containerHeader.wmsContainerType = "BATCH";
                                        containerOut.message_ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        //bool flag = false;
                                        for (int j = 0; j < amItemCont; j++)
                                        {
                                            //if (rest != 0 && j == amItemCont - 1 && i== contQty -1 && !flag)
                                            //{
                                            //    amItemCont += rest;
                                            //    flag = true;
                                            //}
                                            LINE_DETAIL_C linedetail = new LINE_DETAIL_C();

                                            linedetail.batchId = batch.batchId;
                                            linedetail.containerBarCode = barcode.Value;
                                            linedetail.lineItemId = (j + 1).ToString();
                                            linedetail.wmsLocationId = "cl-205";
                                            linedetail.wmsPickZone = "cl";

                                            var firstP = actualProds[0];
                                            actualProds.Remove(firstP);
                                            usedProds.Add(firstP);
                                            linedetail.sku = firstP.sku;
                                            linedetail.productCode = firstP.barcode;
                                            linedetail.requestedQty = firstP.qty.ToString();
                                            linedetail.actualQty = "0";
                                            linedetail.user = containerOut.containerHeader.user;
                                            containerOut.containerLineDetail[j] = linedetail;


                                        }

                                        outpCont.Add(containerOut);
                                        //========================================================================================
                                    }
                                    

                                }


                            }
                        }
                        
                       
                    //}
                    TimesT TStamp = new TimesT();
                    string json = "";
                    string addFile = TStamp.TimeStamp.Replace('-', '_').Replace(' ', '_').Replace(':', '_');
                    string path = @"c:\tkfile\" + addFile +"_"+"CONTAINER"+ ".json";
                    using (StreamWriter sw = new StreamWriter(path, false))
                    {

                            string respMessage = "";
                            int mesCounter = 0;
                            foreach (var item in outpCont)
                            {
                                respMessage += "---" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>" + (item.messageNumber) + "--" + (item.containerHeader.batchId);
                                 mesCounter++;
                                 json = JsonConvert.SerializeObject(item);
                                string[] explJson = json.Split(',');

                                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                string newJson = "";
                                int index = 0;
                                foreach (var itemJ in explJson)
                                {
                                    if (!itemJ.Contains(":null"))
                                    {
                                        char last = itemJ[itemJ.Length - 1];


                                        if (index == 0)
                                        {
                                            newJson = newJson + itemJ;
                                            index++;
                                            continue;
                                        }
                                        else if (newJson.Length > 0 && (newJson[newJson.Length - 1] == '{'))
                                        {
                                            newJson = newJson + itemJ + ",";
                                            continue;
                                        }
                                        else if (newJson.Length > 0 && (newJson[newJson.Length - 1] == ','))
                                        {
                                            newJson = newJson + itemJ;
                                            continue;
                                        }
                                        else if (last != '}')
                                        {
                                            newJson = newJson + "," + itemJ;
                                            continue;
                                        }


                                        else
                                        {
                                            newJson = newJson + "," + itemJ + ",";
                                        }
                                    }
                                    else
                                    {
                                        char last = itemJ[itemJ.Length - 1];
                                        char first = itemJ[0];

                                        if (last == ']')
                                        {
                                            newJson = newJson + "}]";
                                            continue;
                                        }
                                        if (last == '}')
                                        {
                                            newJson = newJson + "}";
                                            continue;
                                        }
                                        if (itemJ.Contains("[{"))
                                        {
                                            var patch = itemJ.Split('[');
                                            newJson = newJson + patch[0] + "[{";
                                            index = 0;
                                            continue;
                                        }
                                        if (itemJ.Contains(":{"))
                                        {
                                            var patch = itemJ.Split(':');
                                            newJson = newJson + "," + patch[0] + ":{";
                                            index = 0;
                                            continue;
                                        }
                                        if (first == '{')
                                        {
                                            newJson = newJson + "{";
                                            continue;
                                        }

                                    }

                                }
                                json = newJson.Substring(0, newJson.Length - 1);
                                //=====================Saving to a file==================
                                sw.WriteLine(json);
                                //=====================end Saving to a file



                            }
                            return Json(respMessage);
                            // return Json(json);

                    }
                  
               
            }
            catch (Exception ex)
            {

                return Json(ex.ToString());
            }
          
        }


        [HttpPost]
        public ActionResult PickTote()
        {

            var mod = Request.Files;
            List<string> orderList = (List<string>)Session["SendingList"];
            List<Dictionary<string, int>> batches = new List<Dictionary<string, int>>();
            
            List<PickToteDS> pickTotes = new List<PickToteDS>();
            //if (mod.BatchSize>0)
            //{
            //    for (int i = 0; i < orderList.Count(); i = i + mod.BatchSize)
            //    {
            //        Dictionary<string, int> itemList = new Dictionary<string, int>();
            //        var items = orderList.Skip(i).Take(mod.BatchSize);
            //        foreach (var iterator in items)
            //        {
            //            JObject obj1= JObject.Parse(iterator);
            //            var lineItems = obj1["orderLineDetail"];

            //            foreach (var lines in lineItems)
            //            {
            //                var sku = lines["sku"].ToString();
            //                if (itemList.ContainsKey(sku))
            //                {
            //                    itemList[sku] = itemList[sku] + Convert.ToInt32(lines["requestedQty"]);
            //                }
            //                else
            //                {
            //                    itemList.Add(sku, Convert.ToInt32(lines["requestedQty"]));
            //                }
            //            }
            //        }
            //        batches.Add(itemList);
            //    }

            //    int bcounter = 1;
            //    foreach (var itemBatch in batches)
            //    {
            //        if (mod.Containers < itemBatch.Count())
            //        {
            //            int itemsTote = Convert.ToInt32(Math.Round((double)itemBatch.Count() / (double)mod.Containers));
            //            int itemsRem = itemBatch.Count() % mod.Containers;

            //            for (int j = 0; j < itemBatch.Count(); j = j + itemsTote)
            //            {
            //                var prods = itemBatch.Skip(j).Take(itemsTote);
            //                PickToteDS pt = new PickToteDS();
            //                pt.Product = new Dictionary<string, int>();
            //                pt.Batch = bcounter;
            //                if (prods.Count()>=itemsTote)
            //                {
            //                    foreach (var elemP in prods)
            //                    {

            //                        pt.Product.Add(elemP.Key, elemP.Value);
            //                    }
            //                }
            //                else if (prods.Count()>0)
            //               {
            //                    foreach (var item2 in prods)
            //                    {
            //                        pickTotes.LastOrDefault().Product.Add(item2.Key, item2.Value);
            //                    }
                                
            //                }
                           
            //                pickTotes.Add(pt);
            //            }
            //            bcounter++;
            //        }

            //    }

            //}
            TimesT TStamp = new TimesT();

            string json = JsonConvert.SerializeObject(pickTotes);
            string addFile = TStamp.TimeStamp.Replace('-', '_').Replace(' ', '_').Replace(':', '_');

            string path = @"c:\tkfile\" + "BATCH"+addFile + ".json";


            // Create a file to write to.
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.WriteLine(json);

            }
            

            return Json(json);
        }


    }
}