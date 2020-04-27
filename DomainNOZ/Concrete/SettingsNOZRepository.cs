using DatabaseWebService.DomainNOZ;
using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.ModelsNOZ.Settings;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainPDO.Concrete
{
    public class SettingsNOZRepository : ISettingsNOZRepository
    {
        GrafolitNOZEntities context;

        public SettingsNOZRepository(GrafolitNOZEntities _context)
        {
            context = _context;
        }

        public SettingsNOZModel GetLatestSettings()
        {
            try
            {
                if (context.Nastavitve_NOZ.Count() <= 0)
                {
                    Nastavitve_NOZ n = new Nastavitve_NOZ();
                    n.NastavitveID = 0;
                    n.Opombe = "";
                    n.NarociloOptimalnihZalogPredpona = DateTime.Now.Date.Year.ToString();
                    n.NarociloOptimalnihZalogStev = 0;
                    n.ts = DateTime.Now;
                    n.tsIDOsebe = 0;
                    n.tsUpdate = DateTime.Now;
                    n.tsUpdateUserID = 0;

                    context.Nastavitve_NOZ.Add(n);
                    context.SaveChanges();
                }

                var query = (from s in context.Nastavitve_NOZ
                             orderby s.ts descending
                             select new SettingsNOZModel
                             {
                                 NastavitveID = s.NastavitveID,
                                 Opombe = s.Opombe,
                                 NarociloOptimalnihZalogPredpona = s.NarociloOptimalnihZalogPredpona,
                                 NarociloOptimalnihZalogStev = s.NarociloOptimalnihZalogStev.HasValue ? s.NarociloOptimalnihZalogStev.Value : 0,
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

        public int SaveSettings(SettingsNOZModel model, bool updateRecord = true)
        {
            try
            {
                Nastavitve_NOZ n = new Nastavitve_NOZ();
                n.NastavitveID = model.NastavitveID;
                n.Opombe = model.Opombe;
                n.NarociloOptimalnihZalogPredpona = model.NarociloOptimalnihZalogPredpona;
                n.NarociloOptimalnihZalogStev = model.NarociloOptimalnihZalogStev;
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
                    context.Nastavitve_NOZ.Add(n);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Nastavitve_NOZ original = context.Nastavitve_NOZ.Where(nas => nas.NastavitveID == n.NastavitveID).FirstOrDefault();

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
                var settings = context.Nastavitve_NOZ.Where(n => n.NastavitveID == nId).FirstOrDefault();

                if (settings != null)
                {
                    context.Nastavitve_NOZ.Remove(settings);
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
    }
}