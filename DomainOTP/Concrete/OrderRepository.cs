using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.ModelsOTP.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class OrderRepository : IOrderRepository
    {
        GrafolitOTPEntities context;

        public OrderRepository(GrafolitOTPEntities _context)
        {
            context = _context;
        }

        public List<OrderModel> GetAllOrders()
        {
            var query = from order in context.Narocilo
                        group order by order into o
                        select new OrderModel
                        {
                            Cena = o.Key.Cena.HasValue ? o.Key.KolicinaSkupaj.Value : 0,
                            KolicinaSkupaj = o.Key.KolicinaSkupaj,
                            NarociloID = o.Key.NarociloID,
                            NarociloStevilka = o.Key.NarociloStevilka,
                            Relacija = o.Key.Relacija
                        };

            List<OrderModel> model = query.ToList();

            foreach (var item in model)
            {
                item.NarociloPozicija = (from op in context.NarociloPozicija
                                         where op.NarociloID == item.NarociloID
                                         select new OrderPositionModel
                                         {
                                             Cena = op.Cena,
                                             Kolicina = op.Kolicina,
                                             NarociloID = op.NarociloID,
                                             NarociloPozicijaID = op.NarociloPozicijaID,                                             
                                         }).ToList();
            }

            return model;
        }
        public List<OrderPositionModel> GetAllOrdersPositions()
        {
            var query = (from op in context.NarociloPozicija
                         select new OrderPositionModel
                         {
                             Cena = op.Cena,
                             Kolicina = op.Kolicina,
                             NarociloID = op.NarociloID,
                             NarociloPozicijaID = op.NarociloPozicijaID
                         });

            return query.ToList();
        }
    }
}