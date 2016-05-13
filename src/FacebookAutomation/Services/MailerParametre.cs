using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookAutomation.Services
{
    public class MailerParametre
    {

        public  string fromAddress = "mathieu.boa@itinnovation-ci.net";
        public string recipients { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
        public string html { get; set; }
        public string templateEngine { get; set; }


        public Dictionary<string, string> Substitution { get; set; }


    }
}
