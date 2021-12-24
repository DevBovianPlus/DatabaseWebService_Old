using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class AktivnostUporabnikaRepository : IAktivnostUporabnikaRepository
    {
        GrafolitOTPEntities context;

        public AktivnostUporabnikaRepository(GrafolitOTPEntities _context)
        {
            context = _context;
        }

        public AktivnostUporabnikaModel GetUserActivity(int UserId)
        {
            //Akt  tmpUsers = context.Osebe_OTP.Where(u => u.UporabniskoIme.CompareTo(userName) == 0 && u.Geslo.CompareTo(password) == 0).FirstOrDefault();
            //UserModel model = null;
            //if (tmpUsers != null)
            //{
            //    if (String.Compare(tmpUsers.UporabniskoIme, userName, false) != 0 && String.Compare(tmpUsers.Geslo, password) != 0)
            //        return null;

            //    model = new UserModel();

            //    model.ID = tmpUsers.idOsebe;

            //    model.firstName = tmpUsers.Ime;
            //    model.lastName = tmpUsers.Priimek;
            //    model.email = tmpUsers.Email;
            //    model.dateCreated = tmpUsers.ts.HasValue ? tmpUsers.ts.Value : DateTime.MinValue;
            //    model.Job = tmpUsers.DelovnoMesto;
            //    model.profileImage = tmpUsers.ProfileImage;
            //    model.OTPPantheonUsrID = Common.DataTypesHelper.ParseInt(tmpUsers.OTPPantheonUsrID);

            //    if (tmpUsers.idVloga != null)
            //    {
            //        model.RoleID = tmpUsers.idVloga.Value;
            //        model.Role = tmpUsers.Vloga_OTP.Koda;
            //        model.RoleName = tmpUsers.Vloga_OTP.Naziv;
            //    }
                return null;
            }



        public AktivnostUporabnikaModel SaveUserActivity(int id)
        {
            return null;
        }

        //public string GetRoleNameByID(int id)
        //{
        //    Vloga_OTP role = GetRoleByID(id);
        //    return role != null ? role.Naziv : "";
        //}

        //private Osebe_OTP GetEmployeeByID(int id)
        //{
        //    return context.Osebe_OTP.Where(e => e.idOsebe == id).FirstOrDefault();
        //}
    }
}