using DatabaseWebService.Common;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class UserOTPRepository : IUserOTPRepository
    {
        GrafolitOTPEntities context;

        public UserOTPRepository(GrafolitOTPEntities _context)
        {
            context = _context;
        }

        public UserModel UserLogIn(string userName, string password)
        {
            Osebe_OTP tmpUsers = context.Osebe_OTP.Where(u => u.UporabniskoIme.CompareTo(userName) == 0 && u.Geslo.CompareTo(password) == 0).FirstOrDefault();
            UserModel model = null;
            if (tmpUsers != null)
            {
                if (String.Compare(tmpUsers.UporabniskoIme, userName, false) != 0 && String.Compare(tmpUsers.Geslo, password) != 0)
                    return null;

                model = new UserModel();

                model.ID = tmpUsers.idOsebe;

                model.firstName = tmpUsers.Ime;
                model.lastName = tmpUsers.Priimek;
                model.email = tmpUsers.Email;
                model.dateCreated = tmpUsers.ts.HasValue ? tmpUsers.ts.Value : DateTime.MinValue;
                model.Job = tmpUsers.DelovnoMesto;
                model.profileImage = tmpUsers.ProfileImage;
                model.OTPPantheonUsrID = Common.DataTypesHelper.ParseInt(tmpUsers.OTPPantheonUsrID);

                if (tmpUsers.idVloga != null)
                {
                    model.RoleID = tmpUsers.idVloga.Value;
                    model.Role = tmpUsers.Vloga_OTP.Koda;
                    model.RoleName = tmpUsers.Vloga_OTP.Naziv;
                }


                //login into logging table
                AktivnostUporabnikaModel aktUporab = GetAktivnostUporabnikaByDateAndUser(model, DateTime.Now.Date);

                if (aktUporab == null)
                {
                    AddUpdateAktivnostUporabnika(model, aktUporab, false);
                }
                else
                {
                    AddUpdateAktivnostUporabnika(model, aktUporab, true);
                }
            }

            return model;
        }

        public AktivnostUporabnikaModel GetAktivnostUporabnikaByDateAndUser(UserModel usr, DateTime CurentDate)
        {
            var query = from akt in context.AktivnostUporabnika
                        where akt.OsebaID.Equals(usr.ID) && akt.TS.Year == CurentDate.Year
                            && akt.TS.Month == CurentDate.Month
                            && akt.TS.Day == CurentDate.Day
                        select new AktivnostUporabnikaModel
                        {
                            AktivnostUporabnikaID = akt.AktivnostUporabnikaID,
                            AktivnostUporabnikaStatusID = akt.AktivnostUporabnikaStatusID,
                            OsebaID = akt.OsebaID,
                            Opis = akt.Opis,
                            ts = akt.TS,
                        };

            return query.FirstOrDefault();
        }

        public AktivnostUporabnikaModel GetAktivnostUporabnikaByDateAndUserID(int UserID, string CurentDateStr)
        {
            DateTime? CurentDate = DataTypesHelper.ParseDateTime(CurentDateStr);

            var query = from akt in context.AktivnostUporabnika
                        where akt.OsebaID.Equals(UserID) && akt.TS.Year == CurentDate.Value.Year
                            && akt.TS.Month == CurentDate.Value.Month
                            && akt.TS.Day == CurentDate.Value.Day
                        select new AktivnostUporabnikaModel
                        {
                            AktivnostUporabnikaID = akt.AktivnostUporabnikaID,
                            AktivnostUporabnikaStatusID = akt.AktivnostUporabnikaStatusID,
                            OsebaID = akt.OsebaID,
                            Opis = akt.Opis,
                            ts = akt.TS,
                        };

            return query.FirstOrDefault();
        }

        public void AddUpdateAktivnostUporabnika(UserModel usr, AktivnostUporabnikaModel aktUporab, bool updateRecord = true)
        {
            AktivnostUporabnika record = new AktivnostUporabnika();
            record.AktivnostUporabnikaID = aktUporab==null ? 0 : aktUporab.AktivnostUporabnikaID;
            record.OsebaID = usr.ID;
            record.AktivnostUporabnikaStatusID = 1;
            record.Opis = "Prijava";

            if (!updateRecord)
            {
                record.TS = DateTime.Now;

                context.AktivnostUporabnika.Add(record);
                context.SaveChanges();
            }
            else
            {
                record.TS = DateTime.Now;
                AktivnostUporabnika original = context.AktivnostUporabnika.Where(e => e.AktivnostUporabnikaID == aktUporab.AktivnostUporabnikaID).FirstOrDefault();
                context.Entry(original).CurrentValues.SetValues(record);

                context.SaveChanges();
            }
        }




        public Vloga_OTP GetRoleByID(int id)
        {
            return context.Vloga_OTP.Where(r => r.idVloga == id).FirstOrDefault();
        }

        public string GetRoleNameByID(int id)
        {
            Vloga_OTP role = GetRoleByID(id);
            return role != null ? role.Naziv : "";
        }

        private Osebe_OTP GetEmployeeByID(int id)
        {
            return context.Osebe_OTP.Where(e => e.idOsebe == id).FirstOrDefault();
        }
    }
}