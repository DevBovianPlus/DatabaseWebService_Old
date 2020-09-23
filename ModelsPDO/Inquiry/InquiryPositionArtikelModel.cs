using DatabaseWebService.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO.Inquiry
{
    public class InquiryPositionArtikelModel
    {
        public int PovprasevanjePozicijaArtikelID { get; set; }
        public int PovprasevanjePozicijaID { get; set; }
        public int IzbranDobaviteljID { get; set; }
        public ClientFullModel Dobavitelj { get; set; }
        public string KategorijaNaziv { get; set; }
        public string Naziv { get; set; }
        public decimal Kolicina1 { get; set; }
        public string EnotaMere1 { get; set; }
        public decimal Kolicina2 { get; set; }
        public string EnotaMere2 { get; set; }
        public string Opombe { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
        public DateTime tsUpdate { get; set; }
        public int tsUpdateUserID { get; set; }
        public bool NewAdd { get; set; }


        public DepartmentModel Oddelek { get; set; }
        public int OddelekID { get; set; }
        public String OddelekNaziv { get; set; }
        public bool PrikaziKupca { get; set; }
        public bool IzbranArtikel { get; set; }
        
        public string IzbraniArtikelNaziv_P { get; set; }
        public string IzbraniArtikelIdent_P { get; set; }
        public decimal ArtikelCena { get; set; }

        public decimal KolicinavKG { get; set; }
        public decimal KolicinaVPOL { get; set; }
        public string EnotaMere { get; set; }
        public string NarEnotaMere2 { get; set; }
        public decimal Rabat { get; set; }
        public string OpombaNarocilnica { get; set; }
        public DateTime DatumDobavePos { get; set; }

        public List<ProductModel> ArtikliPantheon { get; set; }

        public int DobaviteljID { get; set; }
        public string DobaviteljNaziv_PA { get; set; }

    }
}