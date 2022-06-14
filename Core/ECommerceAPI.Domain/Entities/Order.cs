using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceAPI.Domain.Entities.Common;

namespace ECommerceAPI.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; }

        public ICollection<Product> Products { get; set; }  //bir order'in birden fazla product'i olabilir.
        public Customer Customer { get; set; }  //bir orderin bir customeri vardır
    }
}
