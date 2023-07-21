using System;
using Microsoft.AspNetCore.Identity;

namespace EShopSolution.Data.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
    }
}