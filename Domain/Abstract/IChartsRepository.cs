using DatabaseWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.Domain.Abstract
{
    public interface IChartsRepository
    {
        ChartRenderModel GetDataForChart(int clientID, int categorieID, int period, int type, DateTime? dateFROM = null, DateTime? dateTO = null);
        List<ChartRenderModel> GetDataChartAllTypes(int clientID, int categorieID, int period, DateTime dateFROM, DateTime dateTO);

        ChartRenderModel GetDataForChartFromSQLFunction(int clientID, int categorieID, int period, int type, DateTime? dateFROM = null, DateTime? dateTO = null);
        List<ChartRenderModel> GetDataChartAllTypesSQLFunction(int clientID, int categorieID, int period, DateTime? dateFROM, DateTime? dateTO);
    }
}
