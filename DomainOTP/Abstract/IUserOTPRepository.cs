using DatabaseWebService.Models;
using DatabaseWebService.ModelsOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface IUserOTPRepository
    {
        UserModel UserLogIn(string userName, string password);

        Vloga_OTP GetRoleByID(int id);

        string GetRoleNameByID(int id);

        AktivnostUporabnikaModel GetAktivnostUporabnikaByDateAndUser(UserModel usr, DateTime CurentDate);
        AktivnostUporabnikaModel GetAktivnostUporabnikaByDateAndUserID(int UserID, string CurentDateStr);
    }
}
