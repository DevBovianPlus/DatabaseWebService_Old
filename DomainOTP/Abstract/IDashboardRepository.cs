using DatabaseWebService.ModelsOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface IDashboardRepository
    {
        DashboardDataModel GetDashboardData();
    }
}
