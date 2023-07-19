using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopSolution.Entities.Exceptions
{
    public class EShopException : Exception
    {
        public EShopException() { }

        public EShopException(string message)
            : base(message) { }

        public EShopException(string message, Exception inner)
            : base(message, inner) { }
    }
}
