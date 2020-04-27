using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Common.Enums
{
    public class Enums
    {
        public enum LoginValidation
        {
            OK = 1,
            FAILED = 0,
            NOT_FOUND = -1
        }

        public enum SystemMessageEventCodes
        {
            NewMessage,
            AUTO,
            EVENT_DOGODEK,
            EVENT_PRIPRAVA_OPOZORILO,
            EVENT_PRIPRAVA_OPOZORILO_NADREJENI,
            EVENT_PRIPRAVA_OPOZORILO_NADREJENI_DIREKTOR,
            EVENT_POROCILO_OPOZORILO,
            EVENT_POROCILO_OPOZORILO_NADREJENI,
            EVENT_POROCILO_OPOZORILO_NADREJENI_DIREKTOR
        }

        public enum SystemMessageEventStatus
        {
            UnProcessed = 0,
            Processed = 1,
            Error = 2
        }

        public enum SystemEmailMessageStatus
        {
            UnProcessed = 0,
            Processed = 1,
            Error = 2,
            RecipientError = 3
        }

        public enum ChartRenderPeriod
        {
            MESECNO = 1,
            LETNO = 2,
            TEDENSKO = 3
        }

        public enum ChartRenderType
        {
            PRODAJA = 1,
            KOLICINA = 2,
            RVC = 3,
            RVC_ODSTOTEK = 4
        }

        public enum EventMeetingType
        {
            PRIPRAVA,
            POROCILO
        }

        public enum TypeOfClient
        {
            DOBAVITELJ,
            PREVOZNIK,
            SKLADISCE,
            KUPEC
        }

        public enum TypeOfRecall
        {

        }

        public enum StatusOfRecall
        {
            DELOVNA,
            ZAVRNJEN,
            V_ODOBRITEV,
            POTRJEN,
            ZAVRNJEN_KONTROLA,
            POSLAN,
            NEZNAN,
            PREVZET,
            DELNO_PREVZET,
            PONOVNI_ODPOKLIC,
            RAZPIS_PREVOZNIK,
            POTRJEN_PREVOZNIK,
            USTVARJENO_NAROCILO,
            KREIRAN_POSLAN_PDF,
            ERR_ADMIN_MAIL, // 5x napaka pri pošiljanju naročilnice
            ERR_ORDER_NO_SEND // kerirana naročilnica, vendar še ni bil poslana
        }

        public enum TransportType
        {
            KAMION,
            LETALO,
            LADJA,
            KOMBI,
            ZBIRNIK
        }

        public enum UserRole
        {
            Logistics
        }

        public enum StatusOfInquiry
        {
            DELOVNA,
            ZAVRNJEN,
            POTRJEN,
            POSLAN,
            NEZNAN,
            ODDANO,
            NAROCENO,
            PREVERJENI_ARTIKLI,
            USTVARJENO_NAROCILO,
            POSLANO_V_NABAVO,
            PRIPRAVLJENO, // povpraševanje je bilo zaključeno
            ERR_ADMIN_MAIL // 5x napaka pri pošiljanju naročilnice            
        }

        public enum Language
        {
            ANG = 1,
            HRV = 2,
            SLO = 3
        }

        public enum EmailContentType
        {
            POZDRAV = 1,
            CARRIRERMAIL_SUBJECT = 7,
            CARRIRERMAIL_BODY = 2,
            CARRIRERMAIL_ADDTEXT = 3,
            CARRIRERMAIL_REPORTPRICE = 18,
            ZA_VPRASANJA = 4,
            PODPIS1 = 5,
            PODPIS2 = 6,
            CARRIRERMAILORDER_SUBJECT = 8,
            CARRIRERMAILORDER_BODY = 9,
            CARRIRERMAILORDER_ADDTEXT =  10,            
            CARRIRERCONGRATS_OR_BETTERLUCK_SUBJECT_SELECT = 11,
            CARRIRERCONGRATS_OR_BETTERLUCK_SUBJECT_REJECT = 12,
            CARRIRERCONGRATS_OR_BETTERLUCK_BODY_SELECT = 13,
            CARRIRERCONGRATS_OR_BETTERLUCK_BODY_REJECT = 14,
            CARRIRERCONGRATS_OR_BETTERLUCK_ADDTEXT_SELECT = 15,
            CARRIRERCONGRATS_OR_BETTERLUCK_ADDTEXT_REJECT = 16,
            CARRIRERCONGRATS_DATUMNAKLADA = 17,
            EMAILTOSUPPLIER_MATERIAL = 19,
            EMAILTOSUPPLIER_KOLICINA = 20,
            EMAILTOSUPPLIER_OPOMBE = 21,

        }

       

        public enum PrintType
        {
            A0Q = 1,
            A0U = 2,
            A10
        }

        public enum OptimalStockHierarchyLevel
        {
            Kategorija,
            Gloss,
            Gramatura,
            Velikost,
            Tek
        }

        public enum StatusOfOptimalStock
        {
            /*DELOVNA,
            ZAVRNJEN,
            POTRJEN,
            NAROCENO,*/
            NEZNAN,
            USTVARJENO_NAROCILO,
            ODDANO,
            KOPIRANO_NAROCILO
        }
    }
}