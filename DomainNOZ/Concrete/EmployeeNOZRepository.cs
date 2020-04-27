using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainNOZ.Concrete
{
    public class EmployeeNOZRepository : IEmployeeNOZRepository
    {
        GrafolitNOZEntities context;

        public EmployeeNOZRepository(GrafolitNOZEntities _context)
        {
            context = _context;
        }

        public List<EmployeeFullModel> GetAllEmployees()
        {
            try
            {
                var query = from emp in context.Osebe_NOZ
                            group emp by emp into employee
                            select new EmployeeFullModel
                            {
                                DatumRojstva = employee.Key.DatumRojstva.HasValue ? employee.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                DatumZaposlitve = employee.Key.DatumZaposlitve.HasValue ? employee.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                DelovnoMesto = employee.Key.DelovnoMesto,
                                Email = employee.Key.Email,
                                Geslo = employee.Key.Geslo,
                                idOsebe = employee.Key.OsebaID,
                                Ime = employee.Key.Ime,
                                Naslov = employee.Key.Naslov,
                                Priimek = employee.Key.Priimek,
                                TelefonGSM = employee.Key.TelefonGSM,
                                ts = employee.Key.ts.HasValue ? employee.Key.ts.Value : DateTime.MinValue,
                                tsIDOsebe = employee.Key.tsIDOsebe.HasValue ? employee.Key.tsIDOsebe.Value : 0,
                                UporabniskoIme = employee.Key.UporabniskoIme,
                                Zunanji = employee.Key.Zunanji.HasValue ? employee.Key.Zunanji.Value : 0,
                                idVloga = employee.Key.VlogaID,
                                ProfileImage = employee.Key.ProfileImage,
                                NOZDostop = employee.Key.NOZDostop.HasValue ? employee.Key.NOZDostop.Value : false,
                                Podpis = employee.Key.Podpis
                                //idNadrejeni = context.OsebeNadrejeni.Where(osn => osn.idOseba == id).FirstOrDefault() != null ? context.OsebeNadrejeni.Where(osn => osn.idOseba == id).FirstOrDefault().idNadrejeni : (int?)null
                                /*Vloga = (from role in employee
                                         group role by role.Vloga into userRole
                                         select new RoleModel
                                         {
                                             idVloga = userRole.Key.idVloga,
                                             Koda = userRole.Key.Koda,
                                             Naziv = userRole.Key.Naziv,
                                             ts = userRole.Key.ts.HasValue ? userRole.Key.ts.Value : DateTime.MinValue,
                                             tsIDOsebe = userRole.Key.tsIDOsebe.HasValue ? userRole.Key.tsIDOsebe.Value : 0
                                         }).FirstOrDefault()*/,
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<EmployeeFullModel> GetAllEmployees(int roleID)
        {
            try
            {
                var query = from emp in context.Osebe_NOZ
                            where emp.VlogaID.Equals(roleID)
                            group emp by emp into employee
                            select new EmployeeFullModel
                            {
                                DatumRojstva = employee.Key.DatumRojstva.HasValue ? employee.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                DatumZaposlitve = employee.Key.DatumZaposlitve.HasValue ? employee.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                DelovnoMesto = employee.Key.DelovnoMesto,
                                Email = employee.Key.Email,
                                Geslo = employee.Key.Geslo,
                                idOsebe = employee.Key.OsebaID,
                                Ime = employee.Key.Ime,
                                Naslov = employee.Key.Naslov,
                                Priimek = employee.Key.Priimek,
                                TelefonGSM = employee.Key.TelefonGSM,
                                ts = employee.Key.ts.HasValue ? employee.Key.ts.Value : DateTime.MinValue,
                                tsIDOsebe = employee.Key.tsIDOsebe.HasValue ? employee.Key.tsIDOsebe.Value : 0,
                                UporabniskoIme = employee.Key.UporabniskoIme,
                                Zunanji = employee.Key.Zunanji.HasValue ? employee.Key.Zunanji.Value : 0,
                                idVloga = employee.Key.VlogaID,
                                ProfileImage = employee.Key.ProfileImage,
                                NOZDostop = employee.Key.NOZDostop.HasValue ? employee.Key.NOZDostop.Value : false,
                                Podpis = employee.Key.Podpis
                                //idNadrejeni = context.OsebeNadrejeni.Where(osn => osn.idOseba == id).FirstOrDefault() != null ? context.OsebeNadrejeni.Where(osn => osn.idOseba == id).FirstOrDefault().idNadrejeni : (int?)null
                                /*Vloga = (from role in employee
                                         group role by role.Vloga into userRole
                                         select new RoleModel
                                         {
                                             idVloga = userRole.Key.idVloga,
                                             Koda = userRole.Key.Koda,
                                             Naziv = userRole.Key.Naziv,
                                             ts = userRole.Key.ts.HasValue ? userRole.Key.ts.Value : DateTime.MinValue,
                                             tsIDOsebe = userRole.Key.tsIDOsebe.HasValue ? userRole.Key.tsIDOsebe.Value : 0
                                         }).FirstOrDefault()*/
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public EmployeeFullModel GetEmployeeByID(int employeeID)
        {
            try
            {
                var query = from employee in context.Osebe_NOZ
                            where employee.OsebaID.Equals(employeeID)
                            select new EmployeeFullModel
                            {
                                DatumRojstva = employee.DatumRojstva.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                DatumZaposlitve = employee.DatumZaposlitve.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                DelovnoMesto = employee.DelovnoMesto,
                                Email = employee.Email,
                                Geslo = employee.Geslo,
                                idOsebe = employee.OsebaID,
                                Ime = employee.Ime,
                                Naslov = employee.Naslov,
                                Priimek = employee.Priimek,
                                TelefonGSM = employee.TelefonGSM,
                                ts = employee.ts.HasValue ? employee.ts.Value : DateTime.MinValue,
                                tsIDOsebe = employee.tsIDOsebe.HasValue ? employee.tsIDOsebe.Value : 0,
                                UporabniskoIme = employee.UporabniskoIme,
                                Zunanji = employee.Zunanji.HasValue ? employee.Zunanji.Value : 0,
                                idVloga = employee.VlogaID,
                                ProfileImage = employee.ProfileImage,
                                Vloga = (from role in context.Vloga_NOZ
                                         where role.VlogaID == employee.VlogaID
                                         select new RoleModel
                                         {
                                             idVloga = role.VlogaID,
                                             Koda = role.Koda,
                                             Naziv = role.Naziv,
                                             ts = role.ts.HasValue ? role.ts.Value : DateTime.MinValue,
                                             tsIDOsebe = role.tsIDOsebe.HasValue ? role.tsIDOsebe.Value : 0
                                         }).FirstOrDefault(),
                                NadrejeniID = context.OsebeNadrejeni_NOZ.Where(osn => osn.OsebaID == employeeID).FirstOrDefault() != null ? context.OsebeNadrejeni_NOZ.Where(osn => osn.OsebaID == employeeID).FirstOrDefault().OsebeNadrejeniID : 0,
                                NOZDostop = employee.NOZDostop.HasValue ? employee.NOZDostop.Value : false,
                                /*EmailGeslo = employee.EmailGeslo,
                                EmailSifriranjeSSL = employee.EmailSifriranjeSSL.HasValue?employee.EmailSifriranjeSSL.Value : false,
                                EmailStreznik = employee.EmailStreznik,
                                EmailVrata = employee.EmailVrata.HasValue ? employee.EmailVrata.Value : 0,*/
                                Podpis = employee.Podpis,
                                PantheonUsrID = employee.PantheonUsrID
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveEmployee(EmployeeFullModel model, bool updateRecord = true)
        {
            try
            {
                Osebe_NOZ record = new Osebe_NOZ();
                OsebeNadrejeni_NOZ nadrejeni = new OsebeNadrejeni_NOZ();

                record.OsebaID = model.idOsebe;
                record.Naslov = model.Naslov;
                record.Email = model.Email;
                record.DelovnoMesto = model.DelovnoMesto;
                record.DatumZaposlitve = model.DatumZaposlitve;
                record.Ime = model.Ime;
                record.Priimek = model.Priimek;
                record.DatumRojstva = model.DatumRojstva;
                record.UporabniskoIme = model.UporabniskoIme;
                record.Geslo = model.Geslo;
                record.TelefonGSM = model.TelefonGSM;
                record.Zunanji = model.Zunanji;
                record.ProfileImage = model.ProfileImage;

                record.VlogaID = model.idVloga;
                record.tsUpdate = DateTime.Now;
                record.tsUpdateUserID = model.tsIDOsebe;
                record.ts = model.ts.Equals(DateTime.MinValue) ? (DateTime?)null : model.ts;
                record.tsIDOsebe = model.tsIDOsebe;
                record.NOZDostop = model.NOZDostop;
                /*record.EmailGeslo = model.EmailGeslo;
                record.EmailSifriranjeSSL = model.EmailSifriranjeSSL;
                record.EmailStreznik = model.EmailStreznik;
                record.EmailVrata = model.EmailVrata > 0 ? model.EmailVrata : (int?)null;*/
                record.Podpis = model.Podpis;
                record.PantheonUsrID = model.PantheonUsrID;

                if (record.OsebaID == 0)
                {
                    record.ts = DateTime.Now;
                    record.tsIDOsebe = model.tsIDOsebe;

                    context.Osebe_NOZ.Add(record);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Osebe_NOZ original = context.Osebe_NOZ.Where(e => e.OsebaID == record.OsebaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(record);

                        context.SaveChanges();
                    }
                }

                if (model.NadrejeniID > 0)
                {
                    var supervisorExistForEmployee = context.OsebeNadrejeni_NOZ.Where(osn => osn.OsebaID == record.OsebaID).FirstOrDefault();

                    nadrejeni.OsebaID = record.OsebaID;
                    nadrejeni.NadrejeniID = model.NadrejeniID;

                    if (supervisorExistForEmployee != null)
                    {
                        OsebeNadrejeni_NOZ originalNadrejeni = context.OsebeNadrejeni_NOZ.Where(e => e.OsebaID == record.OsebaID).FirstOrDefault();
                        nadrejeni.OsebeNadrejeniID = originalNadrejeni.OsebeNadrejeniID;


                        context.Entry(originalNadrejeni).CurrentValues.SetValues(nadrejeni);
                    }
                    else
                    {
                        nadrejeni.ts = DateTime.Now;
                        nadrejeni.tsIDosebe = model.tsIDOsebe;
                        context.OsebeNadrejeni_NOZ.Add(nadrejeni);
                    }
                    context.SaveChanges();
                }

                return record.OsebaID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }


        public bool DeleteEmployee(int employeeID)
        {
            try
            {
                var employee = context.Osebe_NOZ.Where(e => e.OsebaID == employeeID).FirstOrDefault();
                if (employee != null)
                {
                    context.Osebe_NOZ.Remove(employee);
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

        public List<RoleModel> GetRoles()
        {
            try
            {
                var query = from role in context.Vloga_NOZ
                            select new RoleModel
                            {
                                idVloga = role.VlogaID,
                                Koda = role.Koda,
                                Naziv = role.Naziv,
                                ts = role.ts.HasValue ? role.ts.Value : DateTime.MinValue,
                                tsIDOsebe = role.tsIDOsebe.HasValue ? role.tsIDOsebe.Value : 0
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