using GenericCharts.DataAccess;

namespace GenericCharts
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddScoped<IChartBusinessUnit, ChartBusinessUnit>();
            builder.Services.AddScoped<IChartDataAccess, ChartDataAccess>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS konfigurasyonu
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin() // Herhangi bir origin'e izin verir
                            .AllowAnyMethod()  // Herhangi bir HTTP metoduna izin verir
                            .AllowAnyHeader(); // Herhangi bir header'a izin verir
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // HTTPS yönlendirmesini kullanmak istiyorsanız, bu satır kalabilir.
            // app.UseHttpsRedirection(); 

            // CORS politikası uygulama
            app.UseCors("AllowAll");

            // Authorization middleware'ini ekleyin
            app.UseAuthorization();

            // Route'ları map et
            app.MapControllers();

            // Uygulamayı başlat
            app.Run();
        }
    }
}