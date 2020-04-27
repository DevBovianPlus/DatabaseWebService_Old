using DatabaseWebService.Models;
using DatabaseWebService.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainPDO.Abstract
{
    public interface IEmployeePDORepository
    {
        List<EmployeeFullModel> GetAllEmployees();
        List<EmployeeFullModel> GetAllEmployees(int roleID);
        EmployeeFullModel GetEmployeeByID(int employeeID);
        List<RoleModel> GetRoles();
        bool DeleteEmployee(int employeeID);
        int SaveEmployee(EmployeeFullModel model, bool updateRecord = true);
    }
}
