using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.ModelsPDO.Settings;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainPDO.Concrete
{
    public class SettingsRepository : ISettingsRepository
    {
        GrafolitPDOEntities context;

        public SettingsRepository(GrafolitPDOEntities _context)
        {
            context = _context;
        }

        public SettingsModel GetLatestSettings()
        {
            try
            {
                if (context.Nastavitve.Count() <= 0)
                {
                    Nastavitve n = new Nastavitve();
                    n.NastavitveID = 0;
                    n.Opombe = "";
                    n.PovprasevanjeStevilcenjePredpona = DateTime.Now.Date.Year.ToString();
                    n.PovprasevanjeStevilcenjeStev = 0;
                    n.ts = DateTime.Now;
                    n.tsIDOsebe = 0;
                    n.tsUpdate = DateTime.Now;
                    n.tsUpdateUserID = 0;

                    context.Nastavitve.Add(n);
                    context.SaveChanges();
                }

                var query = (from s in context.Nastavitve
                             orderby s.ts descending
                             select new SettingsModel
                             {
                                 NastavitveID = s.NastavitveID,
                                 Opombe = s.Opombe,
                                 PovprasevanjeStevilcenjePredpona = s.PovprasevanjeStevilcenjePredpona,
                                 PovprasevanjeStevilcenjeStev = s.PovprasevanjeStevilcenjeStev.HasValue ? s.PovprasevanjeStevilcenjeStev.Value : 0,
                                 EmailSifriranjeSSL = s.EmailSifriranjeSSL.HasValue ? s.EmailSifriranjeSSL.Value : false,
                                 EmailStreznik = s.EmailStreznik,
                                 EmailVrata = s.EmailVrata.HasValue ? s.EmailVrata.Value : 0,
                                 ts = s.ts.HasValue ? s.ts.Value : DateTime.MinValue,
                                 tsIDOsebe = s.tsIDOsebe.HasValue ? s.tsIDOsebe.Value : 0,
                                 tsUpdate = s.tsUpdate.HasValue ? s.tsUpdate.Value : DateTime.MinValue,
                                 tsUpdateUserID = s.tsUpdateUserID.HasValue ? s.tsUpdateUserID.Value : 0,
                                 PosiljanjePoste = s.PosiljanjePoste.HasValue ? s.PosiljanjePoste.Value : false
                             }).FirstOrDefault();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveSettings(SettingsModel model, bool updateRecord = true)
        {
            try
            {
                Nastavitve n = new Nastavitve();
                n.NastavitveID = model.NastavitveID;
                n.Opombe = model.Opombe;
                n.PovprasevanjeStevilcenjePredpona = model.PovprasevanjeStevilcenjePredpona;
                n.PovprasevanjeStevilcenjeStev = model.PovprasevanjeStevilcenjeStev;
                n.EmailSifriranjeSSL = model.EmailSifriranjeSSL;
                n.EmailStreznik = model.EmailStreznik;
                n.EmailVrata = model.EmailVrata > 0 ? model.EmailVrata : (int?)null;
                n.tsUpdate = DateTime.Now;
                n.tsUpdateUserID = model.tsUpdateUserID;
                n.PosiljanjePoste = model.PosiljanjePoste;
                n.ts = model.ts.Equals(DateTime.MinValue) ? (DateTime?)null : model.ts;
                n.tsIDOsebe = model.tsIDOsebe;

                if (n.NastavitveID == 0)
                {
                    n.ts = DateTime.Now;
                    n.tsIDOsebe = n.tsIDOsebe;
                    context.Nastavitve.Add(n);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Nastavitve original = context.Nastavitve.Where(nas => nas.NastavitveID == n.NastavitveID).FirstOrDefault();

                        context.Entry(original).CurrentValues.SetValues(n);
                        context.SaveChanges();
                    }
                }

                return n.NastavitveID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteSettings(int nId)
        {
            try
            {
                var settings = context.Nastavitve.Where(n => n.NastavitveID == nId).FirstOrDefault();

                if (settings != null)
                {
                    context.Nastavitve.Remove(settings);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }



        public bool RunSQLString(string sSQL)
        {
            try
            {
                  if (sSQL != null)
                {
                   var objectRet = context.Database.ExecuteSqlCommand(sSQL);                    
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }
    }
}