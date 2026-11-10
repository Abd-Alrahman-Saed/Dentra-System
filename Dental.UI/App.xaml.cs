using Dental.Application.Services;
using Dental.Core.Entities;
using Dental.Core.Enums;
using Dental.Infrastructure.Data;
using Dental.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Windows;

namespace Dental.UI
{
    public partial class App : System.Windows.Application
    {
        public static ServiceProvider Services { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register services
            services.AddScoped<PatientService>();
            services.AddScoped<ToothService>();
            services.AddScoped<ToothRecordService>();

            Services = services.BuildServiceProvider();

           
            var Window = new DashboardWindow();
            Window.Show();


            base.OnStartup(e);
        }
    }
}