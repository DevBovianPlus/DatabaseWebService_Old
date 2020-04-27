using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class UserModel
    {
        public int ID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public int RoleID { get; set; }
        public string Job { get; set; }
        public DateTime dateCreated { get; set; }

        public int LockedInquiryByUser { get; set; }

        public string profileImage { get; set; }
        public bool HasSupervisor { get; set; }

        public string Signature { get; set; }
    }
}