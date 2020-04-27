using DatabaseWebService.Models;
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
    }
}
