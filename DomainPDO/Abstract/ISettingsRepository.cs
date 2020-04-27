using DatabaseWebService.ModelsPDO.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainPDO.Abstract
{
    public interface ISettingsRepository
    {
        SettingsModel GetLatestSettings();
        int SaveSettings(SettingsModel model, bool updateRecord = true);
        bool DeleteSettings(int nId);
        bool RunSQLString(string sSQL);
    }
}
