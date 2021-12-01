using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseWebService.Models;
using DatabaseWebService.Domain;
using DatabaseWebService.Models.Employee;

namespace DatabaseWebService.Domain.Abstract
{
    public interface IEmployeeRepository
    {
        EmployeeFullModel GetEmployeeByID(int id);

        List<EmployeeSimpleModel> GetAllEmployees();

        int SaveEmployee(EmployeeFullModel model, bool updateRecord = true);        

        bool DeleteEmployee(int employeeID);

        List<EmployeeSimpleModel> GetEmployeesByRankID(int rankID);


        List<EmployeeSimpleModel> GetEmployeesByRoleID(int roleID);

        List<EmployeeSimpleModel> GetEmployeesByRoleCode(string roleCode);
        RoleModel GetRoleByID(int roleID);
        List<RoleModel> GetAllRoles();

        List<EmployeeSimpleModel> GetEmployeeSupervisorByID(int employeeID);

        List<EmployeeSimpleModel> GetAllEmployeeSupervisors();
    }
}
