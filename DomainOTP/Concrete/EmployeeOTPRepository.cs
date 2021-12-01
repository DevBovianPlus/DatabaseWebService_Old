using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class EmployeeOTPRepository : IEmployeeOTPRepository
    {

        GrafolitOTPEntities context;

        public EmployeeOTPRepository(GrafolitOTPEntities _context)
        {
            context = _context;
        }

        public List<EmployeeFullModel> GetAllEmployees()
        {
            try
            {
                var query = from emp in context.Osebe_OTP
                            group emp by emp into employee
                            select new EmployeeFullModel
                            {
                                DatumRojstva = employee.Key.DatumRojstva.HasValue ? employee.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                DatumZaposlitve = employee.Key.DatumZaposlitve.HasValue ? employee.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                DelovnoMesto = employee.Key.DelovnoMesto,
                                Email = employee.Key.Email,
                                Geslo = employee.Key.Geslo,
                                idOsebe = employee.Key.idOsebe,
                                Ime = employee.Key.Ime,
                                Naslov = employee.Key.Naslov,
                                Priimek = employee.Key.Priimek,
                                TelefonGSM = employee.Key.TelefonGSM,
                                ts = employee.Key.ts.HasValue ? employee.Key.ts.Value : DateTime.MinValue,
                                tsIDOsebe = employee.Key.tsIDOsebe.HasValue ? employee.Key.tsIDOsebe.Value : 0,
                                UporabniskoIme = employee.Key.UporabniskoIme,
                                Zunanji = employee.Key.Zunanji.HasValue ? employee.Key.Zunanji.Value : 0,
                                idVloga = employee.Key.idVloga.HasValue ? employee.Key.idVloga.Value : 0,
                                ProfileImage = employee.Key.ProfileImage,
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

        public List<EmployeeFullModel> GetAllEmployees(int roleID)
        {
            try
            {
                var query = from emp in context.Osebe_OTP
                            where emp.idVloga.Equals(roleID)
                            group emp by emp into employee
                            select new EmployeeFullModel
                            {
                                DatumRojstva = employee.Key.DatumRojstva.HasValue ? employee.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                DatumZaposlitve = employee.Key.DatumZaposlitve.HasValue ? employee.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                DelovnoMesto = employee.Key.DelovnoMesto,
                                Email = employee.Key.Email,
                                Geslo = employee.Key.Geslo,
                                idOsebe = employee.Key.idOsebe,
                                Ime = employee.Key.Ime,
                                Naslov = employee.Key.Naslov,
                                Priimek = employee.Key.Priimek,
                                TelefonGSM = employee.Key.TelefonGSM,
                                ts = employee.Key.ts.HasValue ? employee.Key.ts.Value : DateTime.MinValue,
                                tsIDOsebe = employee.Key.tsIDOsebe.HasValue ? employee.Key.tsIDOsebe.Value : 0,
                                UporabniskoIme = employee.Key.UporabniskoIme,
                                Zunanji = employee.Key.Zunanji.HasValue ? employee.Key.Zunanji.Value : 0,
                                idVloga = employee.Key.idVloga.HasValue ? employee.Key.idVloga.Value : 0,
                                ProfileImage = employee.Key.ProfileImage,
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
                var query = from emp in context.Osebe_OTP
                            where emp.idOsebe.Equals(employeeID)
                            group emp by emp into employee
                            select new EmployeeFullModel
                            {
                                DatumRojstva = employee.Key.DatumRojstva.HasValue ? employee.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                DatumZaposlitve = employee.Key.DatumZaposlitve.HasValue ? employee.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                DelovnoMesto = employee.Key.DelovnoMesto,
                                Email = employee.Key.Email,
                                Geslo = employee.Key.Geslo,
                                idOsebe = employee.Key.idOsebe,
                                Ime = employee.Key.Ime,
                                Naslov = employee.Key.Naslov,
                                Priimek = employee.Key.Priimek,
                                TelefonGSM = employee.Key.TelefonGSM,
                                ts = employee.Key.ts.HasValue ? employee.Key.ts.Value : DateTime.MinValue,
                                tsIDOsebe = employee.Key.tsIDOsebe.HasValue ? employee.Key.tsIDOsebe.Value : 0,
                                UporabniskoIme = employee.Key.UporabniskoIme,
                                Zunanji = employee.Key.Zunanji.HasValue ? employee.Key.Zunanji.Value : 0,
                                idVloga = employee.Key.idVloga.HasValue ? employee.Key.idVloga.Value : 0,
                                OTPPantheonUsrID = employee.Key.OTPPantheonUsrID.HasValue ? employee.Key.OTPPantheonUsrID.Value : 0,
                                OTPPantheonUsrChar = employee.Key.OTPPantheonUsrChar,
                                ProfileImage = employee.Key.ProfileImage,
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

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public int SaveEmployeeOTP(EmployeeFullModel model, bool updateRecord = true)
        {
            try
            {
                Osebe_OTP record = new Osebe_OTP();

                record.idOsebe = model.idOsebe;
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

                record.ts = model.ts.Equals(DateTime.MinValue) ? (DateTime?)null : model.ts;
                record.tsIDOsebe = model.tsIDOsebe;

                record.OTPPantheonUsrID = model.OTPPantheonUsrID;
                record.OTPPantheonUsrChar = model.OTPPantheonUsrChar;
                record.idVloga = model.idVloga;

                if (record.idOsebe == 0)
                {
                    record.ts = DateTime.Now;
                    record.tsIDOsebe = model.tsIDOsebe;

                    context.Osebe_OTP.Add(record);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Osebe_OTP original = context.Osebe_OTP.Where(e => e.idOsebe == record.idOsebe).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(record);

                        context.SaveChanges();
                    }
                }

                return record.idOsebe;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public List<RoleModel> GetRolesOTP()
        {
            try
            {
                var query = from role in context.Vloga_OTP
                            select new RoleModel
                            {
                                idVloga = role.idVloga,
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