using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Metas.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using Metas.DAL.Interfaces;
using Metas.DAL.Implementacion;
using Metas.BLL.Interfaces;
using Metas.BLL.Implementacion;

namespace Metas.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MetasContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL")));

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IDepartamentoService, DepartamentoService>();
            services.AddScoped<IFechasService, FechasService>();
            services.AddScoped<IProgramacionService, ProgramacionService>();
        }
    }
}
