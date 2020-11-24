using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DatabaseWebService.Common.Enums.Enums;

namespace DatabaseWebService.Common.EmailTemplates
{
    public static class TranslationHelper
    {
        public static string GetTranslateValueByContentAndLanguage(Language langT, EmailContentType _EmailCType)
        {
            string RetStr = "";

            switch (_EmailCType)
            {
                case EmailContentType.POZDRAV:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Dear";
                            break;
                        case Language.HRV:
                            RetStr = "Poštovani";
                            break;
                        case Language.SLO:
                            RetStr = "Spoštovani";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERMAIL_BODY:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "We are sending you a freight enquiry. Recall number:";
                            break;
                        case Language.HRV:
                            RetStr = "Šaljemo vam upit za cijeno prijevoza. Broj za opoziv:";
                            break;
                        case Language.SLO:
                            RetStr = "Pošiljamo vam povpraševanje za prevoz blaga. Številka odpoklica: ";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERMAIL_ADDTEXT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Click on the button below, you will open a form for quoting freight, which also contains shipping information";
                            break;
                        case Language.HRV:
                            RetStr = "Klikom na donji gumb otvorit ćete obrazac za prijavu prevoza, prijava koja također sadrži podatke o otpremi.";
                            break;
                        case Language.SLO:
                            RetStr = "S klikom na spodnji gumb odprete obrazec za prijavo cene za prevoz blaga, ki vsebuje tudi podatke o prevozu.";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.ZA_VPRASANJA:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "We are avaiable for any question at";
                            break;
                        case Language.HRV:
                            RetStr = "Stojimo vam na raspolaganju za svako pitanje";
                            break;
                        case Language.SLO:
                            RetStr = "Za kakršnokoli vprašanje smo vam na voljo";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.PODPIS1:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "©" + DateTime.Now.Year + " GrafolitOTP System. The content of the message is confidential. A message has been sent to ";
                            break;
                        case Language.HRV:
                            RetStr = "©" + DateTime.Now.Year + " GrafolitOTP sustav. Sadržaj poruke je povjerljiv. Poruka je poslana na naslov ";
                            break;
                        case Language.SLO:
                            RetStr = "©" + DateTime.Now.Year + " Sistem GrafolitOTP. Vsebina sporočila je zaupna. Sporočilo je bilo poslano na naslov ";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.PODPIS2:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "If the message missed the address, please let us know at";
                            break;
                        case Language.HRV:
                            RetStr = "Ako je poruka propustila primatelja, obavijestite nas na";
                            break;
                        case Language.SLO:
                            RetStr = "Če je sporočilo zgrešilo naslovnika, nam to sporočite na";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERMAIL_SUBJECT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Grafo Lit - Invitation to carrier enquiry";
                            break;
                        case Language.HRV:
                            RetStr = "Grafo Lit - Poziv za prijavu ponudbe";
                            break;
                        case Language.SLO:
                            RetStr = "Grafo Lit - Vabilo k oddaji cene";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERMAILORDER_SUBJECT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Grafo Lit - We are sending you a shipping order";
                            break;
                        case Language.HRV:
                            RetStr = "Grafo Lit - Šaljemo vam narudžbu otpreme";
                            break;
                        case Language.SLO:
                            RetStr = "Grafo Lit - Pošiljamo vam naročilnico za prevoz";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERMAILORDER_BODY:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "We are sending you a purchase order";
                            break;
                        case Language.HRV:
                            RetStr = "Šaljemo vam narudžbu za kupnju";
                            break;
                        case Language.SLO:
                            RetStr = "Pošiljamo vam naročilnico.";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERMAILORDER_ADDTEXT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Attached to this mail is an order form for a shipping service for route :";
                            break;
                        case Language.HRV:
                            RetStr = "U prilogu ove pošte nalazi se narudžba za uslugu prijevoza na relaciji :";
                            break;
                        case Language.SLO:
                            RetStr = "V prilogi tega maila je naročilnica za naročilo prevoza na relaciji :";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_SUBJECT_SELECT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Grafo Lit - Congratulations on being selected as our carrier";
                            break;
                        case Language.HRV:
                            RetStr = "Grafo Lit -Čestitamo što smo odabrani za našeg prijevoznika";
                            break;
                        case Language.SLO:
                            RetStr = "Grafo Lit - Čestitamo bili ste izbrani za našega prevoznika";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_SUBJECT_REJECT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Grafo Lit - Unfortunately you were not selected, better luck next time";
                            break;
                        case Language.HRV:
                            RetStr = "Grafo Lit - Nažalost niste izabrani, sretno sljedeći put";
                            break;
                        case Language.SLO:
                            RetStr = "Grafo Lit - Žal niste bili izbrani, več sreče prihodnjič";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_BODY_SELECT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "We would like to inform you that you have been selected as our carrier based on the declared price on";
                            break;
                        case Language.HRV:
                            RetStr = "Želimo vas obavijestiti, da ste odabrani kao naš prijevoznik na temelju deklarirane cijene na";
                            break;
                        case Language.SLO:
                            RetStr = "Obveščamo vas, da ste bili izbrani za našega prevoznika na podlagi prijavljene cene, dne";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_BODY_REJECT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "We are writing to inform you that you were unfortunately not selected based on the reported price.";
                            break;
                        case Language.HRV:
                            RetStr = "Obavještavamo vas da nažalost niste odabrani na temelju prijavljene cijene.";
                            break;
                        case Language.SLO:
                            RetStr = "Obveščamo vas, da žal niste bili izbrani na podlagi prijavljene cene.";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_ADDTEXT_SELECT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Please load the quantity that was on the tender, not less, as we will otherwise charge you for the costs that will be incurred, and stick to the date of loading.";
                            break;
                        case Language.HRV:
                            RetStr = "Molim vas nakladajte količinu koja je bila na natječaju i ne manja, jer ćemo vam u suprotnom naplatiti troškove koji će nastati i držite se datuma naklada.";
                            break;
                        case Language.SLO:
                            RetStr = "Prosimo vas, da nakladate količino, ki je bila v razpisu in ne manj, ker Vas bomo drugače bremenili za stroške, ki bodo nastali, ter da se držite datuma naklada.";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_ADDTEXT_REJECT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Thank you for your tender.";
                            break;
                        case Language.HRV:
                            RetStr = "Hvala na vašem izvještaju.";
                            break;
                        case Language.SLO:
                            RetStr = "Zahvaljujemo se vam za vašo prijavo.";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERCONGRATS_DATUMNAKLADA:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Load date";
                            break;
                        case Language.HRV:
                            RetStr = "Datum naklada";
                            break;
                        case Language.SLO:
                            RetStr = "Datum naklada";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERMAIL_REPORTPRICE:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Report the carrier price";
                            break;
                        case Language.HRV:
                            RetStr = "Prijavite cijenu prevoza";
                            break;
                        case Language.SLO:
                            RetStr = "Prijavi ceno za prevoz";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.EMAILTOSUPPLIER_MATERIAL:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Material";
                            break;
                        case Language.HRV:
                            RetStr = "Materjal";
                            break;
                        case Language.SLO:
                            RetStr = "Material";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.EMAILTOSUPPLIER_KOLICINA:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Qnt";
                            break;
                        case Language.HRV:
                            RetStr = "Količina";
                            break;
                        case Language.SLO:
                            RetStr = "Količina";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.EMAILTOSUPPLIER_OPOMBE:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Notes";
                            break;
                        case Language.HRV:
                            RetStr = "Opombe";
                            break;
                        case Language.SLO:
                            RetStr = "Opombe";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.EMAILTOSUPPLIER_THANKANDGREETING:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Thank you and best regards.";
                            break;
                        case Language.HRV:
                            RetStr = "Hvala i lijep pozdrav.";
                            break;
                        case Language.SLO:
                            RetStr = "Hvala in lep pozdrav.";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.EMAILTOSUPPLIER_FORCUSTOMER:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "For customer: ";
                            break;
                        case Language.HRV:
                            RetStr = "Za stranko: ";
                            break;
                        case Language.SLO:
                            RetStr = "Za stranko: ";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.POZDRAVPARTNER:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Dear partner";
                            break;
                        case Language.HRV:
                            RetStr = "Poštovani partner";
                            break;
                        case Language.SLO:
                            RetStr = "Spoštovani partner";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.BODYCARIERTENDER:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "I am sending you tender in the attachment. Next to the relation, write your price, the prices should not contain characters, so only a number. Enter in the green boxes.Please return the completed table to my address";
                            break;
                        case Language.HRV:
                            RetStr = "Šaljem poziv u privitku. Pored relacije napišite svoju cijenu, cijene ne bi trebale sadržavati znakove, već samo broj. Unesite u zelene okvire. Vratite popunjenu tablicu na moju adresu";
                            break;
                        case Language.SLO:
                            RetStr = "v priponki pošiljam razpis. Poleg relacije dopišite vašo ceno, cene naj ne vsebujejo znakov, torej samo številka. Vpišite v zelena polja. Izpolnjeno tabelo mi prosim vrnite na naslov";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.PODPISTENDER:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "In anticipation of successful cooperation, we warmly welcome you";
                            break;
                        case Language.HRV:
                            RetStr = "U očekivanju uspješne suradnje, toplo vas pozdravljamo";
                            break;
                        case Language.SLO:
                            RetStr = "V pričakovanju uspešnega sodelovanja, vas lepo pozdravljamo";
                            break;
                        default:
                            break;
                    }
                    break;
                case EmailContentType.CARRIRERTENDERMAIL_SUBJECT:
                    switch (langT)
                    {
                        case Language.ANG:
                            RetStr = "Grafo Lit - Invitation to fill tender file";
                            break;
                        case Language.HRV:
                            RetStr = "Grafo Lit - Poziv za oddaju ponudbe za prevoz";
                            break;
                        case Language.SLO:
                            RetStr = "Grafo Lit - Vabilo k oddaji cene za razpis";
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            return RetStr;
        }
    }
}