using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookAutomation.Models
{
    public class Produits
    {

        public string Id { get; set; }
        public string Description { get; set; }
        public Double Prix { get; set; }
        public Double Remise { get; set; }
        public Double PrixVente { get; set; }
        public string Etat { get; set; }
        public List<Transactions> Transactions { get; set; }

    }
}