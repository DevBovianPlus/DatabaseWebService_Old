using DatabaseWebService.ModelsNOZ.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainNOZ.Abstract
{
    public interface ISettingsNOZRepository
    {
        SettingsNOZModel GetLatestSettings();
        int SaveSettings(SettingsNOZModel model, bool updateRecord = true);
        bool DeleteSettings(int nId);
    }
}
