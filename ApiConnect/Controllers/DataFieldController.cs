using ApiConnect.Domain.Abstract;
using ApiConnect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using ExcelDataReader;
using System.Net;
using System.Data;
using ApiConnect.Models;

namespace ApiConnect.Controllers
{
    public class DataFieldController : Controller
    {
        private IFieldsRepository fieldsRepo;
        private IDataFieldRepository datafieldRepo;

        public DataFieldController(IFieldsRepository fieldRepository, IDataFieldRepository datafieldRepository)
        {
            this.fieldsRepo = fieldRepository;
            this.datafieldRepo = datafieldRepository;

        }


        // GET: Datafield
        public ActionResult Index(int? id = 0)
        {
            List<Field> fields = fieldsRepo.Fields.ToList();

            if ((fields.Count > 0) && (id == 0))
            {
                ViewBag.FirstId = fields[0].ID;
            }
            else if (id > 0)
            {

                ViewBag.FirstId = id;
            }


            return View(fields);
        }


        public ActionResult Details(int id)
        {
            List<DataField> datafields;
            if (id == 0)
            {
                datafields = new List<DataField>();
            }
            else
            {
                datafields = datafieldRepo.DataFields.Where(p => p.FieldID == id).ToList();
            }

            return PartialView("_DetTable", datafields);
        }


        [HttpGet]
        public ActionResult Create(int? id)
        {

            List<Field> fields = fieldsRepo.Fields.ToList();

            List<DataField> datafields = new List<DataField>();

            ViewBag.FieldName = fields;

            return View(datafields);
        }


        [HttpPost]
        public ActionResult Create(TableViewModel data)
        {
            try
            {
                if (data.DataValue.Count() > 1)
                { // more than one element, linked elements

                    string textName = "", textValue = "";

                    foreach (var e in data.DataValue)
                    {
                        textName += e.Name;
                        textValue += e.Val;
                    }

                    MD5 md5Hasher = MD5.Create();
                    var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(textName));
                    var Link_P = BitConverter.ToInt32(hashed, 0);

                    hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(textValue));
                    var Link_S = BitConverter.ToInt32(hashed, 0);

                    foreach (var j in data.DataValue)
                    {
                        DataField dataf = new DataField
                        {
                            FieldID = fieldsRepo.Fields.Where(p => p.Name == j.Name).FirstOrDefault().ID,
                            Value = j.Val,
                            Link_P = Link_P,
                            Link_S = Link_S
                        };
                        datafieldRepo.SaveDataField(dataf);
                    }

                }
                else
                {// only one element
                    string name = data.DataValue[0].Name;
                    string value = data.DataValue[0].Val;
                    DataField dataf = new DataField
                    {

                        FieldID = fieldsRepo.Fields.Where(p => p.Name == name).FirstOrDefault().ID,
                        Value = value,

                    };
                    datafieldRepo.SaveDataField(dataf);
                }
                return Json("ok");

            }
            catch (Exception e)
            {

                return Json(e.Data.Values);
            }
        }
        

        public ActionResult Table(TableViewModel data = null)
        {



            TableViewModel mod = new TableViewModel();
            List<Elements> dict = new List<Elements>();
            if (data.DataValue != null && data.DataValue.Count() > 0)
            {
                dict = data.DataValue;

            }
            if (data.ID > 0)
            {
                Field field = fieldsRepo.Fields.Where(p => p.ID == data.ID).FirstOrDefault();
                dict.Add(new Elements { Name = field.Name, Val = "" });

            }
            else
            {

            }


            mod.ID = 0;
            mod.DataValue = dict;

            return PartialView("_Table", mod);


        }

        public ActionResult LinkedList(int id)
        {

            DataField elem = datafieldRepo.DataFields.Where(p => p.Id == id).FirstOrDefault();
            List<DataField> linkedData = new List<DataField>();
           
                linkedData = datafieldRepo.DataFields.Where(p => p.Link_S == elem.Link_S).ToList();
          


            List<Elements> dict = new List<Elements>();

            foreach (var m in linkedData)
            {

                dict.Add(new Elements { Name = m.Field.Name, Val = m.Value });

            }
            TableViewModel mod = new TableViewModel();
            mod.ID = 0;
            mod.DataValue = dict;

            return PartialView("_Table", mod);


        }

        public ActionResult Delete(int id, string returnUrl)
        {

            try
            {

                if (id > 0)
                {

                    DataField datafield = datafieldRepo.DataFields.Where(p => p.Id == id).FirstOrDefault();

                    if (datafield.Link_P != null)
                    {

                        List<DataField> lstData = datafieldRepo.DataFields.Where(p => p.Link_S == datafield.Link_S).ToList();
                        List<int> lstDataId = new List<int>();
                        foreach (var m in lstData)
                        {

                            lstDataId.Add(m.Id);

                        }

                        foreach (int i in lstDataId)
                        {

                            datafieldRepo.DeleteDataField(id);

                        }


                    }
                    else
                    {

                        datafieldRepo.DeleteDataField(id);

                    }

                    return Json(returnUrl);

                }

                return Json("Index 0");

            }
            catch (Exception e)
            {

                return Json(e.Message);

            }
            
        }

        public JsonResult UpFile()
        {

            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        IExcelDataReader reader = null;

                        if (fileContent.FileName.EndsWith(".xls"))
                        {

                            //reads the excel with .xls extension
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);


                        }
                        else if (fileContent.FileName.EndsWith(".xlsx"))
                        {
                            //reads the excel with .xlsx extension

                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                        }
                        else
                        {

                            return Json("Document not supported");

                        }

                        DataSet result = reader.AsDataSet();

                        //get the tables name

                        List<string> names = new List<string>();

                        Dictionary<string, string> loadable;

                        for (int i = 0; i < result.Tables[0].Rows.Count; i++)
                        {

                            var dat_row = result.Tables[0].Rows[i].ItemArray;

                            if (i == 0)
                            { // taking values from the first row

                                foreach (var m in dat_row)
                                {
                                    names.Add(m.ToString());

                                }


                            }
                            else
                            {

                                loadable = new Dictionary<string, string>();

                                for (int j = 0; j < dat_row.Count(); j++)
                                {

                                    loadable.Add(names[j], dat_row[j].ToString());

                                }

                                if (loadable.Count() > 0)
                                {

                                    var chainName = "";
                                    var chainValue = "";

                                    foreach (var drun in loadable)
                                    {// creating strings for hash

                                        chainName += drun.Key;
                                        chainValue += drun.Value;

                                    }

                                    MD5 md5Hasher = MD5.Create();
                                    var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(chainName));
                                    var Link_P = BitConverter.ToInt32(hashed, 0);

                                    hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(chainValue));
                                    var Link_S = BitConverter.ToInt32(hashed, 0);

                                    int Level2 = 0;

                                    foreach (var crfile in loadable)
                                    {

                                        Field field = fieldsRepo.Fields.Where(p => p.Name == crfile.Key).FirstOrDefault();
                                        DataField newdata = new DataField();
                                        if (field != null)
                                        {

                                            if (loadable.Count()>1)
                                            {
                                                
                                                newdata.Link_S = Link_S;
                                            }
                                            newdata.Value = crfile.Value;
                                            newdata.FieldID = field.ID;

                                            datafieldRepo.SaveDataField(newdata);


                                        }
                                        else
                                        {

                                            return Json("Some of the value names are not in database ");

                                        }

                                    }

                                    if (Level2 != 0)
                                    {

                                        foreach (var crfile2 in loadable)
                                        {


                                            Field field = fieldsRepo.Fields.Where(p => p.Name == crfile2.Key).FirstOrDefault();

                                                var dataF = datafieldRepo.DataFields.Where(m => m.Link_S == Link_S && m.FieldID == field.ID).FirstOrDefault();
                                                dataF.Link_P = Level2;

                                                datafieldRepo.SaveDataField(dataF);
                                           

                                            

                                        }


                                    }
                                    

                                }

                            }

                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            return Json("File uploaded successfully");
        }
    }
}