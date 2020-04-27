using DatabaseWebService.Models.FinancialControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.Domain.Abstract
{
    public interface IFinancialControlRepository
    {
        FinancialControlModel GetDataForFinancialDashboard();
    }
}
