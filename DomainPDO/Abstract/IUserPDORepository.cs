using DatabaseWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainPDO.Abstract
{
    public interface IUserPDORepository
    {
        UserModel UserLogIn(string userName, string password);
        Vloga_PDO GetRoleByID(int id);
    }
}
