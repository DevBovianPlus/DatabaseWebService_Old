using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainPDO.Concrete
{
    public class UserPDORepository : IUserPDORepository
    {
        GrafolitPDOEntities context;

        public UserPDORepository(GrafolitPDOEntities _context)
        {
            context = _context;
        }

        public Vloga_PDO GetRoleByID(int id)
        {
            try
            {
                return context.Vloga_PDO.Where(r => r.VlogaID == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        public UserModel UserLogIn(string userName, string password)
        {
            Osebe_PDO tmpUsers = context.Osebe_PDO.Where(u => u.UporabniskoIme.CompareTo(userName) == 0 && u.Geslo.CompareTo(password) == 0).FirstOrDefault();
            UserModel model = null;
            if (tmpUsers != null)
            {
                if (String.Compare(tmpUsers.UporabniskoIme, userName, false) != 0 && String.Compare(tmpUsers.Geslo, password) != 0)
                    throw new Exception("Napačna prijava! Ponovno vnesi geslo in uporabniško ime!");

                var allowLogin = tmpUsers.PDODostop.HasValue ? tmpUsers.PDODostop.Value : false;
                if (!allowLogin)
                    throw new Exception("Nimate dodeljenega dostopa do aplikacije. Obrnite se na administratorja");

                model = new UserModel();

                model.ID = tmpUsers.OsebaID;

                model.firstName = tmpUsers.Ime;
                model.lastName = tmpUsers.Priimek;
                model.email = tmpUsers.Email;
                model.dateCreated = tmpUsers.ts.HasValue ? tmpUsers.ts.Value : DateTime.MinValue;
                model.Job = tmpUsers.DelovnoMesto;
                model.profileImage = tmpUsers.ProfileImage;


                model.RoleID = tmpUsers.VlogaID;
                model.Role = tmpUsers.Vloga_PDO.Koda;
                model.RoleName = tmpUsers.Vloga_PDO.Naziv;

                model.Signature = tmpUsers.Podpis;
            }
            else
                throw new Exception("Napačna prijava! Ponovno vnesi geslo in uporabniško ime!");

            return model;
        }
    }
}