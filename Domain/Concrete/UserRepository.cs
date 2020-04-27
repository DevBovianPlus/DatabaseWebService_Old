using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.Models;

namespace DatabaseWebService.Domain.Concrete
{
    public class UserRepository : IUserRepository
    {
        AnalizaProdajeEntities context;
        private readonly IEmployeeRepository userRepo;

        public UserRepository(IEmployeeRepository userRepository, AnalizaProdajeEntities dummyEntites)
        {
            userRepo = userRepository;
            context = dummyEntites;
        }

        public UserModel UserLogIn(string userName, string password)
        {
            Osebe tmpUsers = context.Osebe.Where(u => u.UporabniskoIme.CompareTo(userName) == 0 && u.Geslo.CompareTo(password) == 0).FirstOrDefault();
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

                model.HasSupervisor = (tmpUsers.OsebeNadrejeni != null ? (tmpUsers.OsebeNadrejeni.Count > 0 ? true : false) : false);

                if(tmpUsers.idVloga != null)
                {
                    model.RoleID = tmpUsers.idVloga.Value;
                    model.Role = tmpUsers.Vloga.Koda;
                    model.RoleName = tmpUsers.Vloga.Naziv;
                }
            }

            return model;
        }

        public Vloga GetRoleByID(int id)
        {
            return context.Vloga.Where(r => r.idVloga == id).FirstOrDefault();
        }

        public string GetRoleNameByID(int id)
        {
            Vloga role = GetRoleByID(id);
            return role != null ? role.Naziv : ""; 
        }

        public Osebe GetEmployeeByID(int id)
        {
            return context.Osebe.Where(e => e.idOsebe == id).FirstOrDefault();
        }
    }
}