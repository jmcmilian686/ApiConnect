using ApiConnect.Domain.Abstract;
using ApiConnect.Domain.Entities;
using ApiConnect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Web.Helpers;
using System.IO;

namespace ApiConnect.Controllers
{
    public class WaveController : Controller
    {
        private IFieldsRepository fieldsRepo;
        private IDataFieldRepository datafieldRepo;

        private IDictionary<string,List<Elements>> filterV;
        public int randSeed { get; set; }
        public int Level2 = 0;
        public List<string> comparer;
        public List<string> picked;

        public class InValue
        {

            public int Field { get; set; }

        }

        public WaveController(IFieldsRepository fieldRepository, IDataFieldRepository datafieldRepository)
        {
            this.fieldsRepo = fieldRepository;
            this.datafieldRepo = datafieldRepository;
        }

        // GET: Wave
        public ActionResult Index()
        {

            ViewBag.ItemsW = fieldsRepo.Fields.Where(p =>  p.Level == 1 && p.HasData).ToList();
            ViewBag.ItemsO = fieldsRepo.Fields.Where(p => p.Level == 2 && p.HasData).ToList();
            ViewBag.ItemsC = fieldsRepo.Fields.Where(p => p.Level == 3 && p.HasData).ToList();
            ViewBag.ItemsL = fieldsRepo.Fields.Where(p => p.Level == 4 && p.HasData).ToList();
            if (Session["token"] != null)
            {
                ViewBag.Token = true;
            }
            else
            {
                ViewBag.Token = true;   //============change this to false when we get the token
            }

            return View();
        }

        //============ Combos================
        //Combo box show ajax
        [HttpPost]
        public ActionResult Comboshow(InValue paramets)
        {

            List<DataField> fieldsval = new List<DataField>();

            fieldsval = datafieldRepo.DataFields.Where(p => p.FieldID == paramets.Field).ToList();
            
            return Json(fieldsval);

        }

        [HttpPost]
        public ActionResult Nameshow(InValue mod)
        {
            int param = Convert.ToInt32(mod.Field);

            Field val = new Field();

            val = fieldsRepo.Fields.Where(p => p.ID == param).FirstOrDefault();
            
            return Json(val.Name);

        }

        //============ End of Combos=========

        //================ Wave Creation ==============

        [HttpPost]
        public ActionResult WaveGen(WavGenViewModel model) {

            //=====Duplicated arrays===================
            List<string> supra_duplicated = new List<string>();
            List<string> ordDuplicated = new List<string>();
            List<string> contDuplicated = new List<string>();
            List<string> lineDuplicated = new List<string>();

            try
            {

                if (model.NLine == 0)
                {
                    model.NLine = 1;
                }
                //if (model.NCont == 0)
                //{
                //    model.NCont = 1;
                //}
                if (model.NOrd == 0)
                    {
                        model.NOrd = 1;
                    }
                //==========Filter values area===========
                filterV = new Dictionary<string, List<Elements>>();
                comparer = new List<string>();
                picked = new List<string>();
                if (model.FiltValues != null)
                {
                    foreach (var filt in model.FiltValues)
                    {

                        Field field = new Field();
                        DataField dataFil = new DataField();

                        if (filt.Type == "text")
                        {
                            field = fieldsRepo.Fields.Where(p => p.Name == filt.Name).FirstOrDefault();
                                                        
                        }
                        else
                        {

                            int id = Convert.ToInt32(filt.Val);
                            field = datafieldRepo.DataFields.Where(p => p.Id == id).FirstOrDefault().Field;
                            dataFil = datafieldRepo.DataFields.Where(p => p.Id == id).FirstOrDefault();
                            filt.Val = dataFil.Value;
                            filt.Name = field.Name;
                        }

                        if (filt.Name== "requestedQty") //analizing Qty range 
                        {
                            var valToBreak = filt.Val;
                            if (valToBreak.ToString().Contains(','))// specific values
                            {
                                var spec = valToBreak.Split(',');
                                foreach (var item2 in spec)
                                {
                                    comparer.Add(item2);
                                }
                            }
                            else if (valToBreak.ToString().Contains('-'))// range
                            {
                                var rang = valToBreak.Split('-');

                                if (Convert.ToInt32(rang[1])> Convert.ToInt32(rang[0]))
                                {
                                    for (int i = Convert.ToInt32(rang[0]); i <= Convert.ToInt32(rang[1]); i++)
                                    {
                                        comparer.Add(i.ToString());
                                    }
                                }
                            }


                        }

                        if (filterV.ContainsKey(field.Level.ToString()))
                        {
                            filterV[field.Level.ToString()].Add(filt);
                        }
                        else
                        {

                            filterV.Add(field.Level.ToString(), new List<Elements>());
                            filterV[field.Level.ToString()].Add(filt);

                        }
                    }

                }
                //=============End of filter values area=======

                WAVE wavConv = new WAVE(1);
                List<ORDER> ordersConv = new List<ORDER>();


                WAVE wave = new WAVE(model.NOrd);

                    WAVE_HEADER wavHead = new WAVE_HEADER();
                    TimesT TStamp = new TimesT();
                    ORDER[] ordersList = new ORDER[model.NOrd];


                    //==== using the filter values=====



                    FillFilter(filterV, wave, "1");
                    FillFilter(filterV, wavHead, "1");


                    FillFromData(wave, 1, ref supra_duplicated);

                    wave.message_ts = TStamp.TimeStamp;
                    wave.waveHeader = wavHead;

                    CopyFromPar(wave.waveHeader, wave);

                //===if no wave every order is a new message===
                    if (model.NoWave)
                    {
                      wave.messageNumber = "";
                    }

                    for (int i = 0; i < model.NOrd; i++)
                    {
                        ordDuplicated.Clear();
                        lineDuplicated.Clear(); 
                        ORDER_HEADER header = new ORDER_HEADER();
                        ORDER order;
                        if (model.NCont == 0)
                        {
                            order = new ORDER(model.NLine);
                        }
                        else
                        {
                            order = new ORDER(model.NCont, model.NLine);
                        }
                        if (model.NoWave)
                        {
                            //header.message_ts = TStamp.TimeStamp;
                            //order.message_ts = header.message_ts;
                            order.message_ts = TStamp.TimeStamp;
                        }
                        
                        CopyFromPar(header, wave);
                        CopyFromPar(header, wavHead);
                        FillFilter(filterV, header, "2");
                        FillFromData(header, 2, ref ordDuplicated);

                        CopyFromPar(order, wave); //replicate wave data to its orders
                        FillFilter(filterV, order, "2");
                        FillFromData(order, 2, ref ordDuplicated);
                        order.orderHeader = header;// order header completed
                        if (model.NoWave)
                        {
                            ordersList[i] = order;
                        }
                        else
                        {
                            wave.waveDetail[i] = order;
                        }
                        
                        //====Starting Container Headers
                        if (model.NCont > 0)
                        {
                            for (int j = 0; j < model.NCont; j++)
                            {

                                CONT_HEADER contHeader = new CONT_HEADER();

                                
                                FillFilter(filterV, contHeader, "3");
                                CopyFromPar(contHeader, header);
                                FillFromData(contHeader, 3, ref contDuplicated);
                                if (model.NoWave)
                                {
                                    ordersList[i].orderContainers[j] = contHeader;
                                }
                                else
                                {
                                    wave.waveDetail[i].orderContainers[j] = contHeader;
                                }
                               
                            }
                        }

                        //====== End Container Headers

                        //=====Starting Order Lines
                        
                        for (int k = 0; k < model.NLine; k++)
                        {

                            LINE_DETAIL linDetail = new LINE_DETAIL();

                            FillFilter(filterV, linDetail, "4");
                            CopyFromPar(linDetail, order);
                            linDetail.orderId = order.orderHeader.orderId;//order has no orderId
                            FillFromData(linDetail, 4, ref lineDuplicated);
                            if (model.NoWave)
                            {
                                ordersList[i].orderLineDetail[k] = linDetail;
                            
                            }
                            else {
                                wave.waveDetail[i].orderLineDetail[k] = linDetail;
                            }
                            
                        }
                        picked.Clear();// clearing the Qty

                        Field lineItem = fieldsRepo.Fields.Where(n => n.Name == "lineItemId").FirstOrDefault();
                        if (lineItem!=null)
                        {
                            lineItem.Counter = 1;
                            fieldsRepo.SaveField(lineItem);
                        }

                        
                    }
                    wavConv = wave;
                    ordersConv = ordersList.ToList();



                //====Starting Orders===========================================



                //============Creating and sending the Message=====
                string json = "";
                string addFile = TStamp.TimeStamp.Replace('-', '_').Replace(' ', '_').Replace(':', '_');

                string path = @"c:\tkfile\" + addFile + ".json";
                
                
                    // Create a file to write to.
               using (StreamWriter sw = new StreamWriter(path, false))
                {
                        
                  

                if (model.NoWave) {
                    string respMessage = "";
                    int mesCounter = 0;
                    foreach (var item in ordersConv)
                    {
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
                                else if (newJson.Length>0 && (newJson[newJson.Length - 1] == '{'))
                                {
                                    newJson = newJson + itemJ + ",";
                                    continue;
                                }
                                else if (newJson.Length > 0 && (newJson[newJson.Length - 1] == ','))
                                {
                                    newJson = newJson + itemJ;
                                    continue;
                                }
                                else if (last != '}') {
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
                                    newJson = newJson +","+ patch[0] + ":{";
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

                            
                        
                        if (!model.SavFile)
                        {


                           RestClient client;

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
                              respMessage = respMessage + mesCounter.ToString() + ">>" + (desResponse.status + desResponse.statusCode + desResponse.statusDesc) + "--";

                        }

                        }
                    return Json(respMessage);
                   // return Json(json);

                }
                else {
                    json = JsonConvert.SerializeObject(wavConv);
                    string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    RestClient client;

                    client = Session["MessageAddress"].ToString() != "NOAD" ? new RestClient(Session["MessageAddress"].ToString()) : new RestClient("https://qa.sensorthink.com/iot/integ/message");

                    var request = new RestRequest(Method.POST);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("x-authorization", "Bearer " + Session["token"]);
                    request.AddParameter("application/json", json);
                    IRestResponse response = client.Execute(request);

                    //============End of Message======================
                    if (response.ErrorMessage != null)
                        response.StatusCode = HttpStatusCode.BadRequest;
                    return Json(response.StatusDescription + response.StatusCode);
                    //return Json(json);
                }

                }


            }
            catch (Exception)
            {

               
               // return Json("error");
                throw;
            }

            
        }

        public void FillFromData( Object obj, int level, ref List<string> duplicated ) //fill data from DB
        {
            FieldInfo[] fieldInfo = obj.GetType().GetFields();
            
            foreach (var prop in fieldInfo)
            {
                Field actItem = fieldsRepo.Fields.Where(m => m.Name == prop.Name).FirstOrDefault();

                if ((((prop.GetValue(obj) == null)||(prop.GetValue(obj).ToString()==""))&&(actItem != null) && (actItem.HasData)))// if no value look for it in DB
                {
                    Random rand = new Random();

                    List<DataField> search = datafieldRepo.DataFields.Where(p => p.Field.Name == prop.Name).ToList();
                    int amount = search.Count();
                    bool found = false;
                   
                    if (amount > 1)
                    {
                        if (actItem.UniqueV)// Unique value (Items)
                        {
                            int counter = amount*2;
                            while (!found)
                            {
                                var selitem = search.Skip(rand.Next(0, amount)).First().Value;

                                if (!duplicated.Contains(selitem.ToString()))
                                {
                                    prop.SetValue(obj, selitem);
                                    found = true;
                                    duplicated.Add(selitem.ToString());
                                }
                                else if (counter > 0)
                                {
                                    counter--;
                                    continue;
                                }
                                else {
                                    break;
                                }

                            }
                        }
                        else
                        {
                            prop.SetValue(obj, search.Skip(rand.Next(0, amount)).First().Value);
                        }
                        
                    }
                    else if(amount ==1)
                    {
                        prop.SetValue(obj, search[0].Value);
                    }

                }
                else if ((actItem != null) && (!actItem.HasData)&& ((prop.GetValue(obj) == null)||(prop.GetValue(obj).ToString() == "")))
                {
                    var counter = fieldsRepo.Fields.Where(t => t.Name == prop.Name).FirstOrDefault();
                    if (counter.Counter == 0)
                    {
                        counter.Counter++;
                    }
                    //if (prop.Name == "orderId")
                    //{
                       
                    //    DateTime dt = DateTime.Now;
                        
                    //    string dateIdent = dt.Month.ToString("d2") + dt.Day.ToString("d2");
                    //    prop.SetValue(obj, "Ord-"+dateIdent+counter.Counter.ToString());

                    //}
                    else
                    {
                        if (prop.Name== "orderId")
                        {
                            prop.SetValue(obj, "TR"+counter.Counter.ToString());
                        }
                        else
                        {
                            prop.SetValue(obj, counter.Counter.ToString());
                        }
                        

                    }
                    
                    counter.Counter++;
                  
                    fieldsRepo.SaveField(counter);
                   
                } 

                LinkVal(obj);
            }

        }

        public void LinkVal(Object obj) // fill linked values
        {
            FieldInfo[] fieldInfo = obj.GetType().GetFields();

            var fieldinfoLst = fieldInfo.ToList();

            foreach (var item in fieldInfo)
            {
               
                if ((item.FieldType.Name =="String") && item.GetValue(obj)!= null)//type can't be other than string
                {
                    var value = item.GetValue(obj);
                    DataField datain = datafieldRepo.DataFields.Where(k => k.Field.Name == item.Name && k.Value == value.ToString()).FirstOrDefault();  //get the datafield 
                    if (datain != null)
                    {
                        List<DataField> linkd = datafieldRepo.DataFields.Where(p => p.Field.Name != item.Name && p.Link_S != null && p.Link_S == datain.Link_S).ToList(); //get linked values

                        if (linkd.Count>0)
                        {
                            foreach (var elem in linkd)
                            {
                                FieldInfo updateElem = fieldInfo.Where(m => m.Name == elem.Field.Name).FirstOrDefault();
                                updateElem.SetValue(obj,elem.Value);
                            }
                        }
                    }
                    
                }

            }

        }

        public void CopyFromPar(Object child, Object parent) //Update nested Obj with parents data
        {

            List<FieldInfo> parentFinfo = parent.GetType().GetFields().ToList();
            FieldInfo[] childFinfo = child.GetType().GetFields();

            foreach (var item in childFinfo)
            {
                var parValue = parentFinfo.Where(k => k.Name == item.Name).FirstOrDefault();
                if ((parValue!= null) && ((item.GetValue(child) == null)||((item.GetValue(child).ToString()) =="")))
                {                  
                    item.SetValue(child, parValue.GetValue(parent));
                }
            }
            
        }


        public void FillFilter(IDictionary<string, List<Elements>> filter, Object obj, string level)
        {
            if (filter.ContainsKey(level))
            {
                
                foreach (var item in filter[level])
                {
                    List<FieldInfo> fieldInfo = obj.GetType().GetFields().ToList();
                  

                    var wavelem = fieldInfo.Where(p => p.Name == item.Name).FirstOrDefault();
                  
                    if (wavelem != null)
                    {
                        if (wavelem.Name == "requestedQty" && comparer.Count()>1)
                        {
                            int amount = comparer.Count();
                            int reverse = amount * 6;
                            Random rand = new Random();
                            bool flag = false;
                            string selitem = "";
                            while (!flag)
                            {
                                selitem = comparer.Skip(rand.Next(0, amount)).First();
                                if (!picked.Contains(selitem)) //if value was found
                                {
                                    wavelem.SetValue(obj, selitem);
                                    picked.Add(selitem);
                                    flag = true;
                                }
                                reverse--;
                                if (reverse == 0) // if too many loops
                                {
                                    flag = true;
                                    wavelem.SetValue(obj, selitem);
                                }
                            }
                            
                        }
                        else
                        {
                            wavelem.SetValue(obj, item.Val);
                        }

                       
                    }

                }

                LinkVal(obj);  //finding linked values to added filters

            }
        }
        //================ End of Wave Creation =======


        //=================Pick Tote Message==================

        public ActionResult IndexPT()
        {

            List<Field> modelOut = new List<Field>();
            var list1 = fieldsRepo.Fields.Where(p => p.Level == 1 && p.HasData).ToList();
            var list2 = fieldsRepo.Fields.Where(p => p.Level == 3 && p.HasData).ToList();
            modelOut = list1.Concat(list2).ToList();
            ViewBag.ItemsC = modelOut;
            ViewBag.ItemsL = fieldsRepo.Fields.Where(p => p.Level == 4 && p.HasData).ToList();

            if (Session["token"] != null)
            {
                ViewBag.Token = true;
            }
            else
            {
                ViewBag.Token = true;   //============change this to false when we get the token
            }

            return View();
        }

        //=================end of Pick Tote Message===========
    }
}