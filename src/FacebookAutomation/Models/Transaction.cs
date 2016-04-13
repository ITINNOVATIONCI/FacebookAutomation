using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookAutomation.Models
{
    public class Transactions
    {

        public string Id { get; set; }
        public string idUtilisateur { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTransaction { get; set; }
        public Produits produit { get; set; }
        public string idProduit { get; set; }
        public string Nom { get; set; }
        public string email { get; set; }
        public string NumeroClient { get; set; }
        public string buyer_name { get; set; }
        public string Numero { get; set; }
        public Double Montant { get; set; }
        public Double Quantite { get; set; }
        public Double Total { get; set; }
        public string numerotransaction { get; set; }
        public string log { get; set; }
        public string statuscinetpay { get; set; }
        public string TypeTransaction { get; set; }
        public string status { get; set; }
        public string Etat { get; set; }
        

    }
}