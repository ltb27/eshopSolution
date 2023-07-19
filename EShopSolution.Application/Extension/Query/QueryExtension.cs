using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopSolution.Application.Extension.Query
{
    public static class QueryExtension
    {
        public static IQueryable<T> Pagination<T>(
            this IQueryable<T> queryable,
            int pageSize,
            int pageIndex
        )
        {
            return queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
    }
}
