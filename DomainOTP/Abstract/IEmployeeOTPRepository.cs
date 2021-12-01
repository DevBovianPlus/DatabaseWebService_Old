using DatabaseWebService.Models;
using DatabaseWebService.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface IEmployeeOTPRepository
    {
        List<EmployeeFullModel> GetAllEmployees();
        List<EmployeeFullModel> GetAllEmployees(int roleID);
        EmployeeFullModel GetEmployeeByID(int employeeID);

        int SaveEmployeeOTP(EmployeeFullModel model, bool updateRecord = true);

        List<RoleModel> GetRolesOTP();
    }
}
