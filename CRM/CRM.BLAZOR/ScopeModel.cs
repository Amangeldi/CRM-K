using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.BLAZOR
{
    public class ScopeModel
    {
        protected readonly IServiceScopeFactory _ServiceScopeFactory;
        public ScopeModel(IServiceScopeFactory serviceScopeFactory)
        {
            _ServiceScopeFactory = serviceScopeFactory;
        }
    }
}
