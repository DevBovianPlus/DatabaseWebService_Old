using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models.FinancialControl;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Domain.Concrete
{
    public class FinancialControlRepository : IFinancialControlRepository
    {
        AnalizaProdajeEntities context = new AnalizaProdajeEntities();

        public FinancialControlRepository(AnalizaProdajeEntities dummyEntites)
        {
            context = dummyEntites;
        }
        public FinancialControlModel GetDataForFinancialDashboard()
        {
            try
            {
                var query = (from fc in context.FinancniDashboard
                             select new FinancialControlModel
                             {
                                 Dobavitelji = fc.Dobavitelji.HasValue ? fc.Dobavitelji.Value : 0,
                                 Dobicek = fc.Dobicek.HasValue ? fc.Dobicek.Value : 0,
                                 FinancniDashboardID = fc.FinancniDashboardID,
                                 Investicije = fc.Investicije.HasValue ? fc.Investicije.Value : 0,
                                 Investicijsko_vzdrzevanje = fc.Investicijsko_vzdrzevanje.HasValue ? fc.Investicijsko_vzdrzevanje.Value : 0,
                                 Krediti = fc.Krediti.HasValue ? fc.Krediti.Value : 0,
                                 Kupci = fc.Kupci.HasValue ? fc.Kupci.Value : 0,
                                 Skupaj = fc.Skupaj.HasValue ? fc.Skupaj.Value : 0,
                                 Timestmp = fc.Timestmp.HasValue ? fc.Timestmp.Value : DateTime.MinValue,
                                 Zaloga = fc.Zaloga.HasValue ? fc.Zaloga.Value : 0,
                                 StDniDobavitelji = fc.StDniDobavitelji.HasValue ? fc.StDniDobavitelji.Value : 0,
                                 StDniKupci = fc.StDniKupci.HasValue ? fc.StDniKupci.Value : 0
                             });
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }
    }
}