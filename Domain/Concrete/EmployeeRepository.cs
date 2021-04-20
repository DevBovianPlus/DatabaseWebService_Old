using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.Models;

using DatabaseWebService.Common;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.Resources;
using System.Data.Entity.Validation;
namespace DatabaseWebService.Domain.Concrete
{
    public class EmployeeRepository : IEmployeeRepository
    {
        AnalizaProdajeEntities context = new AnalizaProdajeEntities();
        public EmployeeFullModel GetEmployeeByID(int id)
        {
            try
            {
                var query = from emp in context.Osebe
                            where emp.idOsebe.Equals(id)
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

                                NadrejeniID = context.OsebeNadrejeni.Where(osn => osn.idOseba == id).FirstOrDefault() != null ? context.OsebeNadrejeni.Where(osn => osn.idOseba == id).FirstOrDefault().idNadrejeni : 0,
                                idNadrejeni = context.OsebeNadrejeni.Where(osn => osn.idOseba == id).FirstOrDefault() != null ? context.OsebeNadrejeni.Where(osn => osn.idOseba == id).FirstOrDefault().idNadrejeni : 0

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


        public List<EmployeeSimpleModel> GetAllEmployees()
        {
            try
            {
                var query = from emp in context.Osebe
                            where emp.idOsebe.Equals(emp.idOsebe)
                            group emp by emp into employee
                            select new EmployeeSimpleModel
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
                                Vloga = employee.Key.Vloga.Naziv
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public RoleModel GetRoleByID(int roleID)
        {
            try
            {
                var query = from role in context.Vloga
                            where role.idVloga.Equals(roleID)
                            select new RoleModel
                            {
                                idVloga = role.idVloga,
                                Koda = role.Koda,
                                Naziv = role.Naziv,
                                ts = role.ts.HasValue ? role.ts.Value : DateTime.MinValue,
                                tsIDOsebe = role.tsIDOsebe.HasValue ? role.tsIDOsebe.Value : 0
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
                Osebe record = new Osebe();
                OsebeNadrejeni nadrejeni = new OsebeNadrejeni();

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

                if (model.idVloga <= 0)
                    record.idVloga = null;
                else
                    record.idVloga = model.idVloga;

                if (record.idOsebe == 0)
                {
                    record.ts = DateTime.Now;
                    record.tsIDOsebe = model.tsIDOsebe;

                    context.Osebe.Add(record);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Osebe original = context.Osebe.Where(e => e.idOsebe == record.idOsebe).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(record);

                        context.SaveChanges();
                    }
                }

                if (model.NadrejeniID > 0)
                {
                    var supervisorExistForEmployee = context.OsebeNadrejeni.Where(osn => osn.idOseba == record.idOsebe).FirstOrDefault();

                    nadrejeni.idOseba = record.idOsebe;
                    nadrejeni.idNadrejeni = model.NadrejeniID;

                    if (supervisorExistForEmployee != null)
                    {
                        OsebeNadrejeni originalNadrejeni = context.OsebeNadrejeni.Where(e => e.idOseba == record.idOsebe).FirstOrDefault();
                        nadrejeni.OsebeNadrejeniID = originalNadrejeni.OsebeNadrejeniID;

                        context.Entry(originalNadrejeni).CurrentValues.SetValues(nadrejeni);
                    }
                    else
                    {
                        nadrejeni.ts = DateTime.Now;
                        nadrejeni.tsIDosebe = model.tsIDOsebe;
                        context.OsebeNadrejeni.Add(nadrejeni);
                    }
                    context.SaveChanges();
                }

                return record.idOsebe;
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
                var employee = context.Osebe.Where(e => e.idOsebe == employeeID).FirstOrDefault();
                if (employee != null)
                {
                    context.Osebe.Remove(employee);
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


        public List<EmployeeSimpleModel> GetEmployeesByRankID(int rankID)
        {
            /*var query = from employee in context.Employees
                        where employee.EmployeeRankID.Equals(rankID)
                        select new EmployeeModel
                        {
                            Address = employee.Address,
                            DateOfBirth = employee.DateOfBirth,
                            Email = employee.Email,
                            EmployeeRank = employee.EmployeeRank.Rank,
                            EmploymentDate = employee.EmploymentDate,
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            ID = employee.ID,
                            PostName = employee.Post.Name,
                            EmployeeRankID = employee.EmployeeRankID,
                            PostID = employee.PostID
                        };

            return query.ToList();*/
            return null;
        }



        public List<EmployeeSimpleModel> GetEmployeesByRoleID(int roleID)
        {
            try
            {
                var query = from employee in context.Osebe
                            where employee.idVloga.Value.Equals(roleID)
                            select new EmployeeSimpleModel
                            {
                                DatumRojstva = employee.DatumRojstva.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                DatumZaposlitve = employee.DatumZaposlitve.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                DelovnoMesto = employee.DelovnoMesto,
                                Email = employee.Email,
                                Geslo = employee.Geslo,
                                idOsebe = employee.idOsebe,
                                idVloga = employee.idVloga.HasValue ? employee.idVloga.Value : 0,
                                Ime = employee.Ime,
                                Naslov = employee.Naslov,
                                Priimek = employee.Priimek,
                                TelefonGSM = employee.TelefonGSM,
                                ts = employee.ts.HasValue ? employee.ts.Value : DateTime.MinValue,
                                tsIDOsebe = employee.tsIDOsebe.HasValue ? employee.tsIDOsebe.Value : 0,
                                UporabniskoIme = employee.UporabniskoIme,
                                Zunanji = employee.Zunanji.HasValue ? employee.Zunanji.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }



        public List<EmployeeSimpleModel> GetEmployeesByRoleCode(string roleCode)
        {
            try
            {
                var query = from employee in context.Osebe
                            where employee.Vloga.Koda.Equals(roleCode)
                            select new EmployeeSimpleModel
                            {
                                DatumRojstva = employee.DatumRojstva.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                DatumZaposlitve = employee.DatumZaposlitve.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                DelovnoMesto = employee.DelovnoMesto,
                                Email = employee.Email,
                                Geslo = employee.Geslo,
                                idOsebe = employee.idOsebe,
                                Ime = employee.Ime,
                                Naslov = employee.Naslov,
                                Priimek = employee.Priimek,
                                TelefonGSM = employee.TelefonGSM,
                                ts = employee.ts.HasValue ? employee.ts.Value : DateTime.MinValue,
                                tsIDOsebe = employee.tsIDOsebe.HasValue ? employee.tsIDOsebe.Value : 0,
                                UporabniskoIme = employee.UporabniskoIme,
                                Zunanji = employee.Zunanji.HasValue ? employee.Zunanji.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public List<RoleModel> GetAllRoles()
        {
            try
            {
                var query = from role in context.Vloga
                            where role.idVloga.Equals(role.idVloga)
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

        public List<EmployeeSimpleModel> GetEmployeeSupervisorByID(int employeeID)
        {
            try
            {
                var query = from employee in context.OsebeNadrejeni
                            where employee.idOseba.Equals(employeeID)
                            select new EmployeeSimpleModel
                            {
                                DatumRojstva = employee.Osebe1.DatumRojstva.HasValue ? employee.Osebe1.DatumRojstva.Value : DateTime.MinValue,
                                DatumZaposlitve = employee.Osebe1.DatumZaposlitve.HasValue ? employee.Osebe1.DatumZaposlitve.Value : DateTime.MinValue,
                                Email = employee.Osebe1.Email,
                                idOsebe = employee.OsebeNadrejeniID,
                                idVloga = employee.Osebe1.idVloga.HasValue ? employee.Osebe1.idVloga.Value : 0,
                                Ime = employee.Osebe1.Ime,
                                Priimek = employee.Osebe1.Priimek,
                                Naslov = employee.Osebe1.Naslov,
                                TelefonGSM = employee.Osebe1.TelefonGSM,
                                Vloga = employee.Osebe1.Vloga.Naziv
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<EmployeeSimpleModel> GetAllEmployeeSupervisors()
        {
            try
            {
                var query = from employee in context.Osebe
                            where !employee.Vloga.Koda.Equals("Salesman")
                            select new EmployeeSimpleModel
                            {
                                DatumRojstva = employee.DatumRojstva.HasValue ? employee.DatumRojstva.Value : DateTime.MinValue,
                                DatumZaposlitve = employee.DatumZaposlitve.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                Email = employee.Email,
                                idOsebe = employee.idOsebe,
                                idVloga = employee.idVloga.HasValue ? employee.idVloga.Value : 0,
                                Ime = employee.Ime,
                                Priimek = employee.Priimek,
                                Naslov = employee.Naslov,
                                TelefonGSM = employee.TelefonGSM,
                                Vloga = employee.Vloga.Naziv
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