using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.Models;

namespace DatabaseWebService.Domain.Abstract
{
    public interface IUserRepository
    {
        UserModel UserLogIn(string userName, string password);

        Vloga GetRoleByID(int id);

        string GetRoleNameByID(int id);
    }
}
