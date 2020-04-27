using DatabaseWebService.DomainDW.Abstract;
using DatabaseWebService.ModelsDW.CashFlow_Skupno;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainDW.Concrete
{
    public class CashFlow_SkupnoRepository : ICashFlow_SkupnoRepository
    {
        DWEntities context = new DWEntities();

        public CashFlow_SkupnoRepository(DWEntities _entities)
        {
            context = _entities;
        }

        public int SaveCashFlow_Skupno(CashFlow_SkupnoModel model, bool updateRecord = true)
        {
            try
            {
                CashFlow_Skupno cashFlowSum = new CashFlow_Skupno();
                cashFlowSum.CashFlowSkupnoID = model.CashFlowSkupnoID;
                cashFlowSum.Datum = model.Datum.CompareTo(DateTime.MinValue) > 0 ? model.Datum : (DateTime?)null;
                cashFlowSum.DatumPlana = model.DatumPlana.CompareTo(DateTime.MinValue) > 0 ? model.DatumPlana : (DateTime?)null;
                cashFlowSum.PlacilaAvansov = model.PlacilaAvansov;
                cashFlowSum.PlacilaCassaSconto = model.PlacilaCassaSconto;
                cashFlowSum.PlacilaCassaScontoRocni = model.PlacilaCassaScontoRocni > 0 ? model.PlacilaCassaScontoRocni : (decimal?)null;
                cashFlowSum.PlacilaDDV = model.PlacilaDDV > 0 ? model.PlacilaDDV : (decimal?)null;
                cashFlowSum.PlacilaDobaviteljem = model.PlacilaDobaviteljem;
                cashFlowSum.PlacilaDobaviteljskiFaktoring = model.PlacilaDobaviteljskiFaktoring;
                cashFlowSum.PlacilaKredit = model.PlacilaKredit > 0 ? model.PlacilaKredit : (decimal?)null;
                cashFlowSum.PlacilaKupcev = model.PlacilaKupcev;
                cashFlowSum.PlacilaLeasing = model.PlacilaLeasing > 0 ? model.PlacilaLeasing : (decimal?)null;
                cashFlowSum.PlacilaOdkupov = model.PlacilaOdkupov > 0 ? model.PlacilaOdkupov : (decimal?)null;
                cashFlowSum.PlacilaOdkupovHR = model.PlacilaOdkupovHR > 0 ? model.PlacilaOdkupovHR : (decimal?)null;
                cashFlowSum.PlacilaPlace = model.PlacilaPlace > 0 ? model.PlacilaPlace : (decimal?)null;
                cashFlowSum.Vrsta = model.Vrsta;

                if (cashFlowSum.CashFlowSkupnoID == 0)
                {
                    context.CashFlow_Skupno.Add(cashFlowSum);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        CashFlow_Skupno original = context.CashFlow_Skupno.Where(cfs => cfs.CashFlowSkupnoID == cashFlowSum.CashFlowSkupnoID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(cashFlowSum);
                        context.SaveChanges();
                    }
                }

                return cashFlowSum.CashFlowSkupnoID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteCashFlow_Skupno(int cashFlowSkupnoID)
        {
            try
            {
                var cashFlowSkupno = context.CashFlow_Skupno.Where(cfs => cfs.CashFlowSkupnoID == cashFlowSkupnoID).FirstOrDefault();

                if (cashFlowSkupno != null)
                {
                    context.CashFlow_Skupno.Remove(cashFlowSkupno);
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

        public List<CashFlow_SkupnoModel> GetCashFlow_SkupnoByDatumPlana(DateTime datumPlana)
        {
            try
            {
                var query = from cash in context.CashFlow_Skupno
                            where cash.DatumPlana.Value.CompareTo(datumPlana) == 0
                            select new CashFlow_SkupnoModel
                            {
                                CashFlowSkupnoID = cash.CashFlowSkupnoID,
                                Datum = cash.Datum.HasValue ? cash.Datum.Value : DateTime.MinValue,
                                DatumPlana = cash.DatumPlana.HasValue ? cash.DatumPlana.Value : DateTime.MinValue,
                                PlacilaAvansov = cash.PlacilaAvansov,
                                PlacilaCassaSconto = cash.PlacilaCassaSconto,
                                PlacilaCassaScontoRocni = cash.PlacilaCassaScontoRocni.HasValue ? cash.PlacilaCassaScontoRocni.Value : 0,
                                PlacilaDDV = cash.PlacilaDDV.HasValue ? cash.PlacilaDDV.Value : 0,
                                PlacilaDobaviteljem = cash.PlacilaDobaviteljem,
                                PlacilaDobaviteljskiFaktoring = cash.PlacilaDobaviteljskiFaktoring,
                                PlacilaKredit = cash.PlacilaKredit.HasValue ? cash.PlacilaKredit.Value : 0,
                                PlacilaKupcev = cash.PlacilaKupcev,
                                PlacilaLeasing = cash.PlacilaLeasing.HasValue ? cash.PlacilaLeasing.Value : 0,
                                PlacilaOdkupov = cash.PlacilaOdkupov.HasValue ? cash.PlacilaOdkupov.Value : 0,
                                PlacilaOdkupovHR = cash.PlacilaOdkupovHR.HasValue ? cash.PlacilaOdkupovHR.Value : 0,
                                PlacilaPlace = cash.PlacilaPlace.HasValue ? cash.PlacilaPlace.Value : 0,
                                Vrsta = cash.Vrsta
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<CashFlow_SkupnoModel> GetCashFlow_SkupnoByDatum(DateTime datum)
        {
            try
            {
                var query = from cash in context.CashFlow_Skupno
                            where cash.Datum.Value.CompareTo(datum) == 0
                            select new CashFlow_SkupnoModel
                            {
                                CashFlowSkupnoID = cash.CashFlowSkupnoID,
                                Datum = cash.Datum.HasValue ? cash.Datum.Value : DateTime.MinValue,
                                DatumPlana = cash.DatumPlana.HasValue ? cash.DatumPlana.Value : DateTime.MinValue,
                                PlacilaAvansov = cash.PlacilaAvansov,
                                PlacilaCassaSconto = cash.PlacilaCassaSconto,
                                PlacilaCassaScontoRocni = cash.PlacilaCassaScontoRocni.HasValue ? cash.PlacilaCassaScontoRocni.Value : 0,
                                PlacilaDDV = cash.PlacilaDDV.HasValue ? cash.PlacilaDDV.Value : 0,
                                PlacilaDobaviteljem = cash.PlacilaDobaviteljem,
                                PlacilaDobaviteljskiFaktoring = cash.PlacilaDobaviteljskiFaktoring,
                                PlacilaKredit = cash.PlacilaKredit.HasValue ? cash.PlacilaKredit.Value : 0,
                                PlacilaKupcev = cash.PlacilaKupcev,
                                PlacilaLeasing = cash.PlacilaLeasing.HasValue ? cash.PlacilaLeasing.Value : 0,
                                PlacilaOdkupov = cash.PlacilaOdkupov.HasValue ? cash.PlacilaOdkupov.Value : 0,
                                PlacilaOdkupovHR = cash.PlacilaOdkupovHR.HasValue ? cash.PlacilaOdkupovHR.Value : 0,
                                PlacilaPlace = cash.PlacilaPlace.HasValue ? cash.PlacilaPlace.Value : 0,
                                Vrsta = cash.Vrsta
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<CashFlow_SkupnoModel> GetCashFlow_SkupnoByVrsta(string vrsta)
        {
            try
            {
                var query = from cash in context.CashFlow_Skupno
                            where cash.Vrsta.Equals(vrsta)
                            select new CashFlow_SkupnoModel
                            {
                                CashFlowSkupnoID = cash.CashFlowSkupnoID,
                                Datum = cash.Datum.HasValue ? cash.Datum.Value : DateTime.MinValue,
                                DatumPlana = cash.DatumPlana.HasValue ? cash.DatumPlana.Value : DateTime.MinValue,
                                PlacilaAvansov = cash.PlacilaAvansov,
                                PlacilaCassaSconto = cash.PlacilaCassaSconto,
                                PlacilaCassaScontoRocni = cash.PlacilaCassaScontoRocni.HasValue ? cash.PlacilaCassaScontoRocni.Value : 0,
                                PlacilaDDV = cash.PlacilaDDV.HasValue ? cash.PlacilaDDV.Value : 0,
                                PlacilaDobaviteljem = cash.PlacilaDobaviteljem,
                                PlacilaDobaviteljskiFaktoring = cash.PlacilaDobaviteljskiFaktoring,
                                PlacilaKredit = cash.PlacilaKredit.HasValue ? cash.PlacilaKredit.Value : 0,
                                PlacilaKupcev = cash.PlacilaKupcev,
                                PlacilaLeasing = cash.PlacilaLeasing.HasValue ? cash.PlacilaLeasing.Value : 0,
                                PlacilaOdkupov = cash.PlacilaOdkupov.HasValue ? cash.PlacilaOdkupov.Value : 0,
                                PlacilaOdkupovHR = cash.PlacilaOdkupovHR.HasValue ? cash.PlacilaOdkupovHR.Value : 0,
                                PlacilaPlace = cash.PlacilaPlace.HasValue ? cash.PlacilaPlace.Value : 0,
                                Vrsta = cash.Vrsta
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }
    }
}