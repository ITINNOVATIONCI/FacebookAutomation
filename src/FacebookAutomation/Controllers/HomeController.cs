﻿using System;
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
using System.Security.Claims;
using Microsoft.Azure.NotificationHubs;

namespace FacebookAutomation.Controllers
{
    public class HomeController : Controller, IEmailSender
    {
        private TelemetryClient telemetry = new TelemetryClient();
        protected string URISignature = "http://api.sandbox.cinetpay.com/v1/?method=getSignatureByPost";
        protected string URIStatus = "http://api.sandbox.cinetpay.com/v1/?method=checkPayStatus";

        private UserManager<ApplicationUser> _userManager;
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
            //CREER ROLE
            //_dbContext.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
            //{
            //    Name = "ADMIN"
            //});
            //_dbContext.SaveChanges();

            //ATTRIBUER ROLE
            //ApplicationUser user = _dbContext.Users.Where(u => u.UserName.Equals("boamathieu@yahoo.fr", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            //_userManager.AddToRoleAsync (user, "ADMIN");



            //SendNotificationAsync("BOA MATHIEU");

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

                                    SendNotificationAsync("Facebookpub achat effectué. ID:" + trans.Id + "");
                                    //HelperSMS.SendSMS(Config.adminNumber2, "Facebookpub achat effectué. montant:"+trans.Total);
                                    //HelperSMS.SendSMS(Config.adminNumber, "Facebookpub achat effectué. montant:"+trans.Total);

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
        public async void Register(RegisterViewModel model, Transactions trans)
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

                    var result1 = await _userManager.UpdateAsync(user);

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


        public IActionResult Contact(string email_address, string message, string name, string telephone)
        {
            //var rep= SendEmailAsync()
            return View();
        }
        public IActionResult Contacts(string email, string message, string subject)
        {
            //var rep = SendEmailAsync(email, subject, message);

            MailerParametre mparam = new MailerParametre();

            mparam.recipients = email;
            mparam.subject = "FacebookPub - Message client " + subject;
            mparam.text = "<b style=\"color:#333333;font-size:medium;\">" + message + "</b>";
            mparam.html = "<b style=\"color:#333333;font-size:medium;\"> " + message + ".</b>";

            //mparam.templateEngine = "76bc2231-bf48-4266-9277-9a4f978c3e6e";
            //mparam.Substitution = new Dictionary<string, string>();
            //mparam.Substitution.Add("iti_subject", "Réinitialiser mot de passe");
            //mparam.Substitution.Add("iti_callback", callbackUrl);




            Mailer.SendMail(mparam);

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

        [Authorize]
        public IActionResult ListeUtilisateur()
        {
            var liste = _dbContext.Transactions.Where(t => t.statuscinetpay.ToUpper().Equals("SUCCES") && t.Etat == "ACTIF"
            && t.status.ToUpper() == "TERMINER").OrderBy(t => t.DateTransaction);

            return View(liste.ToList());
        }
        [Authorize]
        public IActionResult CreerUtilisateur(string id)
        {

            //RegisterViewModel model = new RegisterViewModel();


            try

            {
                Transactions trans = _dbContext.Transactions.Where(c => c.Id == id && c.Etat == "ACTIF"
                && c.status == "Terminer" && c.statuscinetpay.ToUpper().Equals("SUCCES")).FirstOrDefault();

                if (trans != null)
                {
                    if (trans.TypeTransaction == "COMMANDE")
                    {
                        try
                        {
                            ViewBag.Login = trans.Nom;
                            string pass = GetUniqueKey(6);

                            ViewBag.Password = pass;
                            ViewBag.Email = trans.email;
                            ViewBag.ConfirmPassword = pass;

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


                            ViewBag.NbreTotalLicence = (int)(trans.Quantite * Nbre);
                            ViewBag.idtrans = id;

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
                    HelperSMS.SendSMS(Config.adminNumber2, "Trans null");

                }
            }
            catch (Exception)
            {

                HelperSMS.SendSMS(Config.adminNumber2, "Try Trans null");
            }


            //return Content("<form action='Register' controller='Account' id='frmTest' method='post'><input type='hidden' name='Email' value='" + model.Email + "' /><input type='hidden' name='Password' value='" + model.Password + "' /><input type='hidden' name='ConfirmPassword' value='" + model.ConfirmPassword + "' /></form><script>document.getElementById('frmTest').submit();</script>");

            //return RedirectToAction("Register", "Account", model);
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
                Html = "<p>Nouveau Mail reçu</p><br />" + message
            }));

            return task;
        }

        public Task SendWelcomeEmail(string firstName, string email)
        {
            throw new NotImplementedException();
        }


        private static async void SendNotificationAsync(string message)
        {
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://itiappns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=7DGSFfearJ1P7lwJwAIx31gC98lTQuC2ZplGXYCiLTA=", "etransfert");


            Dictionary<string, string> templateParams = new Dictionary<string, string>();

            //templateParams["message"] = "{ \"data\" : {\"message\":\"Hello from Azure!\"}}";
            var res = await hub.SendGcmNativeNotificationAsync("{ \"data\" : {\"message\":\"" + message + "\"}}", new List<string>() { "iti" });
            //var res = await hub.SendTemplateNotificationAsync(templateParams, "etransfert");

        }
    }
}
