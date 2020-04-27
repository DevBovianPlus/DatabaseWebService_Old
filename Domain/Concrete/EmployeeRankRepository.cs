using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.Models;

namespace DatabaseWebService.Domain.Concrete
{
    public class EmployeeRankRepository : IEmployeeRankRepository
    {
        AnalizaProdajeEntities context = new AnalizaProdajeEntities();

        public List<EmployeeRankModel> GetAllEmployeeRanks()
        {
           /* var query = from employeeRank in context.EmployeeRank
                        where employeeRank.ID.Equals(employeeRank.ID)
                        select new EmployeeRankModel
                        {
                            ID = employeeRank.ID,
                            Rank = employeeRank.Rank,
                            Description = employeeRank.Description                            
                        };

            return query.ToList();*/

            return null;
        }
    }
}