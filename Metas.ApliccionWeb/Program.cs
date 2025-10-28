using Metas.AplicacionWeb.Utilidades.AutoMapper;
using Metas.BLL.Implementacion;
using Metas.BLL.Interfaces;
using Metas.DAL.Implementacion;
using Metas.DAL.Interfaces;
using Metas.Entity;
using Metas.IOC;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IGenericRepository<Comentario>, GenericRepository<Comentario>>();
builder.Services.AddScoped<IProgramacionService, ProgramacionService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option => {
        option.LoginPath = "/Acceso/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.InyectarDependencia(builder.Configuration);


builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
