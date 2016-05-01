using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using FacebookAutomation.Models;
using Microsoft.ApplicationInsights;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;
using Microsoft.AspNet.Authorization;
using FacebookAutomation.ViewModels.Account;
using Microsoft.AspNet.Identity;
using FacebookAutomation.Services;
using System.Security.Cryptography;
using Mandrill.Requests.Messages;
using Mandrill;
using Mandrill.Models;

namespace FacebookAutomation.Controllers
{
    public class HomeController : Controller,IEmailSender
    {
        private TelemetryClient telemetry = new TelemetryClient();
        protected string URISignature = "http://api.sandbox.cinetpay.com/v1/?method=getSignatureByPost";
        protected string URIStatus = "http://api.sandbox.cinetpay.com/v1/?method=checkPayStatus";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private ApplicationDbContext _dbContext;
        public string currentUserId { get; set; }
        public ApplicationUser currentUser { get; set; }
        public string Name { get; set; }
        string signature;
        public string message { get; set; }


        public static MandrillApi _mandrill = new MandrillApi("PqeG2o_2NPgYpX_PTLqAMg");
        public static string EmailFromAddress = "olaressource@gmail.com";
        public static string EmailFromName = "FacebookPub infos";

        public HomeController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, TelemetryClient Telemetry)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            telemetry = Telemetry;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Faq()
        {

            return View();
        }

        public IActionResult Feature()
        {

            return View();
        }

        public IActionResult Galerie()
        {

            return View();
        }

        public IActionResult Pricing()
        {

            return View();
        }

        public IActionResult Shopcart(string id)
        {
            Transactions trans = new Transactions();
            trans.idProduit = id;
            trans.produit = _dbContext.Produits.Where(c => c.Id == id).FirstOrDefault();

            return View(trans);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Notification(NotifParametre notif)
        {
            //HelperSMS.SendSMS(Config.adminNumber, "Notif");
            ViewBag.Message = "Notification";
            telemetry.TrackEvent("Notification");

            using (WebClient client = new WebClient())
            {
                NameValueCollection data = new NameValueCollection();
                data.Add("apikey", Config.apikey);
                data.Add("cpm_site_id", notif.cpm_site_id);
                data.Add("cpm_trans_id", notif.cpm_trans_id);

                byte[] responsebytes = client.UploadValues(Config.URIStatus, "POST", data);
                string res = Encoding.UTF8.GetString(responsebytes);
                JsonResponse ci = JsonConvert.DeserializeObject<JsonResponse>(res);

                //debut test si ok cpm_result==00

                if (ci.transaction.cpm_result == "00")
                {
                    try
                    {
                        Transactions trans = _dbContext.Transactions.Where(c => c.Id == ci.transaction.cpm_trans_id && c.Etat == "ACTIF" && c.status != "Terminer").FirstOrDefault();

                        if (trans != null)
                        {
                            if (trans.TypeTransaction == "COMMANDE")
                            {
                                try
                                {
                                    telemetry.TrackEvent("Notification:COMMAND");
                                    trans.status = "Terminer";
                                    trans.statuscinetpay = ci.transaction.cpm_error_message;
                                    trans.buyer_name = ci.transaction.buyer_name;

                                    _dbContext.SaveChanges();
                                    
                                    RegisterViewModel model = new RegisterViewModel();
                                    model.Email = trans.email;
                                    string pass = GetUniqueKey(6);
                                    model.Password = pass;
                                    model.ConfirmPassword = pass;

                                    Register(model,trans);

                                }
                                catch (Exception ex)
                                {
                                    trans.log = ex.Message;
                                    _dbContext.SaveChanges();

                                }

                            }
                        }
                        else
                        {
                            HelperSMS.SendSMS(Config.adminNumber, "Trans null");

                        }
                    }
                    catch (Exception)
                    {

                        HelperSMS.SendSMS(Config.adminNumber, "Try Trans null");
                    }

                }
                else
                {
                    //log
                }

                //HelperSMS.SendSMS(Config.adminNumber, ci.transaction.buyer_name + " " + ci.transaction.cel_phone_num + " " + ci.transaction.cpm_custom + " " + ci.transaction.cpm_error_message + " " + ci.transaction.cpm_payid + " " + ci.transaction.cpm_result + " " + ci.transaction.cpm_trans_status);

                ViewBag.Notif = res;
            }


            return Ok();
        }

        //[HttpPost]
        //[AllowAnonymous]
        public async void Register(RegisterViewModel model,Transactions trans)
        {
            //HelperSMS.SendSMS(Config.adminNumber, "Creation user");

            try
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //HelperSMS.SendSMS(Config.adminNumber, "succes 1");

                    double Nbre = 0;
                    if (trans.idProduit == "1")
                    {
                        Nbre = 1;
                    }
                    else if (trans.idProduit == "2")
                    {
                        Nbre = 5;
                    }
                    else
                    {
                        Nbre = 10;
                    }

                    user.NbreTotalLicence += (int)(trans.Quantite * Nbre);
                    //HelperSMS.SendSMS(Config.adminNumber, "succes 2");
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                        "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    await _emailSender.SendWelcomeEmail(user.Email, user.Email);
                    //HelperSMS.SendSMS(Config.adminNumber, "succes 3");

                }
                else
                {
                    //HelperSMS.SendSMS(Config.adminNumber, "Error 2");
                }
            }
            catch (Exception ex)
            {
                //HelperSMS.SendSMS(Config.adminNumber, ex.Message);
            }

        }

        public ActionResult Paiement(Transactions trans)
        {

            trans.produit = _dbContext.Produits.Where(c => c.Id == trans.idProduit).FirstOrDefault();
            trans.Id = Guid.NewGuid().ToString();
            trans.Date = DateTime.UtcNow.Date;
            trans.DateTransaction = DateTime.UtcNow;
            trans.TypeTransaction = "COMMANDE";
            //trans.TypeTransfert = "RAPIDE";
            trans.Etat = "ACTIF";

            //trans.Pourcentage = 0;
            //trans.Total = trans.Montant;
            trans.status = "En Attente du Paiement";
            _dbContext.Transactions.Add(trans);
            _dbContext.SaveChanges();

            return View(PaiementRapide(trans, "Achat de " + trans.produit.Description));

        }

        public PaiementData PaiementRapide(Transactions trans, string Description)
        {
            string signature;
            string id = DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            using (WebClient client = new WebClient())
            {

                Config.cpm_designation = Description;

                NameValueCollection data = new NameValueCollection();
                data.Add("apikey", "106612574455953b2d0e7775.94466351");
                data.Add("cpm_site_id", Config.cpm_site_id);
                data.Add("cpm_currency", "CFA");
                data.Add("cpm_page_action", "PAYMENT");
                data.Add("cpm_payment_config", "SINGLE");
                data.Add("cpm_version", "V1");
                data.Add("cpm_language", "fr");
                data.Add("cpm_trans_date", id);
                data.Add("cpm_trans_id", trans.Id.ToString());
                data.Add("cpm_designation", Config.cpm_designation);
                data.Add("cpm_amount", trans.Total.ToString());
                data.Add("cpm_custom", HttpContext.User.Identity.Name);

                byte[] responsebytes = client.UploadValues(URISignature, "POST", data);
                signature = Encoding.UTF8.GetString(responsebytes);
                signature = JsonConvert.DeserializeObject<string>(signature);

            }

            Dictionary<string, object> postData = new Dictionary<string, object>();
            postData.Add("apikey", Config.apikey);
            postData.Add("cpm_site_id", Config.cpm_site_id);
            postData.Add("cpm_currency", Config.cpm_currency);
            postData.Add("cpm_page_action", Config.cpm_page_action);
            postData.Add("cpm_payment_config", Config.cpm_payment_config);
            postData.Add("cpm_version", Config.cpm_version);
            postData.Add("cpm_language", Config.cpm_language);
            postData.Add("cpm_trans_date", id);
            postData.Add("cpm_trans_id", trans.Id.ToString());
            postData.Add("cpm_designation", Config.cpm_designation);
            postData.Add("cpm_amount", trans.Total.ToString());
            postData.Add("cpm_custom", HttpContext.User.Identity.Name);
            postData.Add("signature", signature);

            postData.Add("return_url", "http://facebookautomation.azurewebsites.net");
            postData.Add("notify_url", "http://facebookautomation.azurewebsites.net/Home/Notification");

            PaiementData pay = new PaiementData();
            pay.data = postData;

            return pay;
        }


        public IActionResult Contact(string email_address,string message,string name,string telephone)
        {
            //var rep= SendEmailAsync()
            return View();
        }
        public IActionResult Contacts(string email, string message, string subject)
        {
            var rep = SendEmailAsync(email, subject, message);
            return View("Contact");
        }


        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Congratulation()
        {

            DateTime dt = DateTime.UtcNow;
             ViewBag.jour = dt.Day;
            ViewBag.mois = dt.ToString("MMM").ToUpper();
            //int annee = dt.Year;
            ViewBag.longdate = dt.ToLongDateString();
            return View();
        }

        public string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var lst = new List<Mandrill.Models.EmailAddress>();
            //lst.Add(new Mandrill.Models.EmailAddress(email));
            lst.Add(new Mandrill.Models.EmailAddress(EmailFromAddress));
            var task = _mandrill.SendMessage(new SendMessageRequest(new EmailMessage
            {
                FromEmail = email,
                FromName = EmailFromName,
                Subject = subject,
                To = lst,               
                Html = "<p>Nouveau Mail reçu</p><br />"+ message
            }));

            return task;
        }

        public Task SendWelcomeEmail(string firstName, string email)
        {
            throw new NotImplementedException();
        }
    }
}
