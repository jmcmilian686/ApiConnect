using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using RestSharp;
using ApiConnect.Models;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Configuration;

namespace ApiConnect.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        public string responseSt { get; set; }
        public string SessionToken { get; set; }

        public class AccessToken
        {
            public string token { get; set; }
            public string refreshToken { get; set; }
        }

        public class Expiry
        {
            public string sub { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string companyCode { get; set; }
            public string companyTimeZone { get; set; }
            public string scopes { get; set; }
            public string iss { get; set; }
            public string iat { get; set; }
            public string exp { get; set; }
        }


        // GET: Home
        public ActionResult Index(string messg)
        {
           
          
             
                if (messg != null)
                {
                  ViewBag.Response = messg;
                }
                else
                {
                  ViewBag.Response = "Not Connected";
                }

                if (Session["token"] != null)
                {
                  ViewBag.Connected = "ok";
                  
                }
                return View();
           

           
            

           // ViewBag.Message = Webcall();

            
        }

        

        public  string Webcall(Credentials cred)
        {
            // var client = new RestClient("https://qa.sensorthink.com/iot/integ/auth/login");
            var client = new RestClient(cred.host);
            var request = new RestRequest(Method.POST);
            //request.AddHeader("postman-token", "0b05fc80-dd65-8df2-4c8e-58509a677fa5");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            string sendCred = "{\"username\":\"" + cred.username + "\",\"password\":\"" + cred.password + "\"}";
            request.AddParameter("application/json", sendCred, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {

                var content = response.Content; // raw content as string

                var isToken = content.Contains("token");
                if (isToken)
                {
                    // deserialize
                    RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                    var des = deserial.Deserialize<AccessToken>(response);

                    string refToken = DesToken(des.refreshToken);
                    string tokenPs = DesToken(des.refreshToken);

                    string[] newAr = refToken.Split('{');

                    var jsonMsg = "{" + newAr[2];

                    var espObj = JsonConvert.DeserializeObject<Expiry>(jsonMsg);



                    Session["decToken"] = tokenPs;
                    Session["decRefToken"] = refToken;
                                        
                    Session["token"] = des.token;
                    Session["refreshToken"] = des.refreshToken;
                    if (cred.MessageAddress != null)
                    {
                        Session["MessageAddress"] = cred.MessageAddress;
                    }
                    else {
                        Session["MessageAddress"] = "NOAD"; // no addres provided
                    }
                    


                    
                }

                
                return response.Content;

            }
            else
            {
                Session["decToken"] = "Disconnected";
                Session["decRefToken"] = "Disconnected";

                Session["token"] = "Disconnected";
                Session["refreshToken"] = "Disconnected";
                return response.Content;
            }

            
        }

        [HttpPost]
        public ActionResult Connect(Credentials cred)
        {
            string response = "";
            Credentials newCred = new Credentials();

            if (cred.username!= null && cred.password!=null && cred.host!=null )
            {
                response =Webcall(cred);
                newCred.username = cred.username;
                newCred.password = cred.password;
                newCred.host = cred.host;
                newCred.MessageAddress = cred.MessageAddress;
                    

            }
            else
            {
                Credentials webConfCred = new Credentials();
                webConfCred.username = ConfigurationManager.AppSettings.Get("username");
                webConfCred.password = ConfigurationManager.AppSettings.Get("password");
                webConfCred.host = ConfigurationManager.AppSettings.Get("host");
                webConfCred.MessageAddress = ConfigurationManager.AppSettings.Get("MessageAddress");


                newCred.username = webConfCred.username;
                newCred.password = webConfCred.password;
                newCred.host = webConfCred.host;
                newCred.MessageAddress = webConfCred.MessageAddress;

                response = Webcall(webConfCred);
            };

            
            newCred.Token = Session["token"].ToString();
            newCred.RefreshToken = Session["refreshToken"].ToString();
            newCred.RefreshTokenDEc = Session["decRefToken"].ToString();
            newCred.TokenDec = Session["decToken"].ToString();


            cred = newCred;

            return PartialView("RespArea", cred);
            
        }

        public ActionResult Disconnect()
        {

            Session.Clear();

            return RedirectToAction("Index");

        }

        public ActionResult Connect()
        {
            Credentials cred = new Credentials();
            if (Session["token"] != null)
            {
                cred.Token = Session["token"].ToString();
                cred.RefreshToken = Session["refreshToken"].ToString();
                cred.RefreshTokenDEc = Session["decRefToken"].ToString();
                cred.TokenDec = Session["decToken"].ToString();
            }
            return PartialView("RespArea", cred);

        }



        // decode token
        public string DesToken(string tokenIn) {

            var jwtHandler =new JwtSecurityTokenHandler();
            var jwtInput = tokenIn;
            string txtJwtOut = "";
                       
            //Check if readable token (string is in a JWT format)
            var readableToken = jwtHandler.CanReadToken(jwtInput);

            if (readableToken != true)
            {
                txtJwtOut = "The token doesn't seem to be in a proper JWT format.";
            }
            if (readableToken == true)
            {
                var token = jwtHandler.ReadJwtToken(jwtInput);

                //Extract the headers of the JWT
                var headers = token.Header;
                var jwtHeader = "{";
                foreach (var h in headers)
                {
                    jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";
                }
                jwtHeader += "}";
                txtJwtOut = "Header:\r\n" + JToken.Parse(jwtHeader).ToString(Formatting.Indented);

                //Extract the payload of the JWT
                var claims = token.Claims;
                var jwtPayload = "{";
                foreach (Claim c in claims)
                {
                    jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";
                }
                jwtPayload += "}";
                txtJwtOut += "\r\nPayload:\r\n" + JToken.Parse(jwtPayload).ToString(Formatting.Indented);
            }

            return txtJwtOut;
        }
    }
}