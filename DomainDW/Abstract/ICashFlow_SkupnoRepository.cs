using DatabaseWebService.ModelsDW.CashFlow_Skupno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainDW.Abstract
{
    public interface ICashFlow_SkupnoRepository
    {
        List<CashFlow_SkupnoModel> GetCashFlow_SkupnoByDatumPlana(DateTime datumPlana);
        List<CashFlow_SkupnoModel> GetCashFlow_SkupnoByDatum(DateTime datum);
        List<CashFlow_SkupnoModel> GetCashFlow_SkupnoByVrsta(string vrsta);
        int SaveCashFlow_Skupno(CashFlow_SkupnoModel model, bool updateRecord = true);
        bool DeleteCashFlow_Skupno(int cashFlowSkupnoID);
    }
}
