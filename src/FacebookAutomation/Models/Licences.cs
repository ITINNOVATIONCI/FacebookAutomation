using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookAutomation.Models
{
    public class Licences
    {

        public string Id { get; set; }
        public string idUtilisateur { get; set; }
        public string NoSerie { get; set; }
        public string Etat { get; set; }

    }
}