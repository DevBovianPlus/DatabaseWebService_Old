using DatabaseWebService.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ.OptimalStockOrder
{
    public class OptimalStockTreeHierarchy
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
        public decimal KolicinaOptimalna { get; set; }
        public decimal KolicinaZaloga { get; set; }
        public decimal TrenutnaZalogaProdukt { get; set; }
        public decimal RazlikaZalogaOptimalna { get; set; }
        public decimal KolicinaNarocenoVTeku { get; set; }
        public decimal VsotaZalNarRazlikaOpt { get; set; }
        public decimal VsotaZalNarKolicnikOpt { get; set; }// Z + N / O
        public decimal KolicinaNarocilo { get; set; }// količina, ki jo bomo naročili
        public string NazivPodkategorije { get; set; }
        public string NazivPodkategorijeFilter { get; set; }
        public string Gloss { get; set; }

        public OptimalStockTreeHierarchy Parent { get; set; }
        public List<OptimalStockTreeHierarchy> Child { get; set; }
        public bool IsLeaf { get; set; }
        public bool IsProcessed { get; set; }
        

        public GetProductsByOptimalStockValuesModel Product { get; set; }
        public bool IsProduct { get; set; }
        /// <summary>
        /// Nastavimo na true, kadar za izbranega dobavitelja in izbrano podskupino ni produkta (produkt obstaja vendar za drugega dobavitelja).
        /// </summary>
        public bool OpenNewCodeForProductInPantheon { get; set; }
    }
}