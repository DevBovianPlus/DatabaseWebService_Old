using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.Models;

namespace DatabaseWebService.Domain.Concrete
{
    public class PostRepository : IPostRepository
    {
        AnalizaProdajeEntities context = new AnalizaProdajeEntities();
        public List<PostModel> GetAllPostInfo()
        {
            /*var query = from post in context.Post
                        where post.ID.Equals(post.ID)
                        select new PostModel 
                        {
                            ID = post.ID,
                            Name = post.Name,
                            Code = post.Code,
                            Description = post.Description,
                            DateCreated = post.DateCreated,
                            DateUpdated = post.DateUpdated
                        };

            return query.ToList();*/
            return null;
        }
    }
}