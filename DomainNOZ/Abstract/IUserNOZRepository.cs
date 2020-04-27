using DatabaseWebService.DomainNOZ;
using DatabaseWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainNOZ.Abstract
{
    public interface IUserNOZRepository
    {
        UserModel UserLogIn(string userName, string password);
        Vloga_NOZ GetRoleByID(int id);
    }
}
