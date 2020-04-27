using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseWebService.Models;

namespace DatabaseWebService.Domain.Abstract
{
    public interface IPostRepository
    {
        List<PostModel> GetAllPostInfo();
    }
}
