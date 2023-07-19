﻿using System.Collections.Generic;

namespace EShopSolution.Application.Catalog.Products.Dto
{
    public class PagedResult<T>
    {
        public int Total { get; set; }
        public List<T> Items { get; set; }
    }
}
