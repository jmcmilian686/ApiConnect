using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ApiConnect.Domain.Abstract;
using ApiConnect.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ApiConnect.Controllers
{
    
    public class SendController : Controller
    {
        private IDataFieldRepository datafieldRepo;

        public SendController(IDataFieldRepository dataFieldRepository)
        {
            this.datafieldRepo = dataFieldRepository;
        }

        public class InValue
        {

            public int Timer { get; set; }

        }

        public class InValue2
        {

            public int BatchSize { get; set; }

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
                            //foreach (var iterObj in auxJobj)
                            //{
                            //    if (iterObj.Key== "facilityId")
                            //    {
                            //        ordAux.FacilityId = iterObj.Value.ToString();
                            //    }

                            //    if (iterObj.Value.HasValues)
                            //    {
                            //        var childObj = JObject.Parse(iterObj.Value.ToString());
                                                                        
                            //        foreach (var item2 in childObj)
                            //        {
                            //            if (item2.Key== "orderId")
                            //            {
                            //                ordAux.OrderId = item2.Value.ToString();
                            //                ordAux.Message = jsonLine;
                            //                messages.Add(ordAux);
                                            
                            //            }
                            //            break;
                            //        }
                            //    }

                            //}

                        }
                        //JObject o1 = JObject.Parse(content);
                        ////IFormFile reader = null;
                        //var desc = o1.Descendants();
                        //var propert = o1.Properties();

                        //foreach (var item in o1)
                        //{
                            

                        //    if (item.Value.HasValues)
                        //    {
                        //        var p = item.Value;

                        //        foreach (var item2 in p)
                        //        {
                        //            var subItem = item2;
                        //        }
                        //    }
                        //}
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
            MessageList = messages;
            return RedirectToAction("Index","Send");
        }

        public ActionResult PickTote(InValue2 mod)
        {

            List<string> orderList = (List<string>)Session["SendingList"];
            List<Dictionary<string, int>> batches = new List<Dictionary<string, int>>();
            
            List<PickToteDS> pickTotes = new List<PickToteDS>();
            if (mod.BatchSize>0)
            {
                for (int i = 0; i < orderList.Count(); i = i + mod.BatchSize)
                {
                    Dictionary<string, int> itemList = new Dictionary<string, int>();
                    var items = orderList.Skip(i).Take(mod.BatchSize);
                    foreach (var iterator in items)
                    {
                        JObject obj1= JObject.Parse(iterator);
                        var lineItems = obj1["orderLineDetail"];

                        foreach (var lines in lineItems)
                        {
                            var sku = lines["sku"].ToString();
                            if (itemList.ContainsKey(sku))
                            {
                                itemList[sku] = itemList[sku] + Convert.ToInt32(lines["requestedQty"]);
                            }
                            else
                            {
                                itemList.Add(sku, Convert.ToInt32(lines["requestedQty"]));
                            }
                        }
                    }
                    batches.Add(itemList);
                }

                int bcounter = 1;
                foreach (var itemBatch in batches)
                {
                    if (mod.Containers < itemBatch.Count())
                    {
                        int itemsTote = Convert.ToInt32(Math.Round((double)itemBatch.Count() / (double)mod.Containers));
                        int itemsRem = itemBatch.Count() % mod.Containers;

                        for (int j = 0; j < itemBatch.Count(); j = j + itemsTote)
                        {
                            var prods = itemBatch.Skip(j).Take(itemsTote);
                            PickToteDS pt = new PickToteDS();
                            pt.Product = new Dictionary<string, int>();
                            pt.Batch = bcounter;
                            if (prods.Count()>=itemsTote)
                            {
                                foreach (var elemP in prods)
                                {

                                    pt.Product.Add(elemP.Key, elemP.Value);
                                }
                            }
                            else if (prods.Count()>0)
                           {
                                foreach (var item2 in prods)
                                {
                                    pickTotes.LastOrDefault().Product.Add(item2.Key, item2.Value);
                                }
                                
                            }
                           
                            pickTotes.Add(pt);
                        }
                        bcounter++;
                    }

                }

            }
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