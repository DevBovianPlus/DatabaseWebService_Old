using DatabaseWebService.ModelsNOZ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainNOZ.Abstract
{
    public interface IDashboardNOZRepository
    {
        DashboardNOZModel GetDashboardData();
    }
}
