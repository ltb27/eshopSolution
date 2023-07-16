using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EShopSolution.Data.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime DOB { get; set; }

        public List<Order> Orders { get; set; }
        public List<Cart> Carts { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
