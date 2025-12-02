using Genealogy.Domain.Repositories;
using Genealogy.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Infrastructure
{
    /// <summary>
    /// Add infrastructure services to the DI container
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IFamilyMemberRepository, InMemoryFamilyMemberRepository>();
            return services;
        }
    }
}
