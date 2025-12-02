using Genealogy.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Application
{
    /// <summary>
    /// Add application services to the DI container
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IFamilyMemberService, FamilyMemberService>();
            services.AddScoped<UseCases.AddFamilyMember>();
            services.AddScoped<UseCases.GetFamilyTree>();
            return services;
        }
    }
}
