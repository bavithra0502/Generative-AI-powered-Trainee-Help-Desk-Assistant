using Microsoft.EntityFrameworkCore;
using TrainingHelpDeskApi.Data;
using TrainingHelpDeskApi.Models;
using TrainingHelpDeskApi.Services;

namespace TrainingHelpDeskApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------- Database (EF Core + SQL Server) ----------
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ---------- OpenAI / Azure OpenAI configuration ----------
            builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));

            // ---------- RAG pipeline services ----------
            builder.Services.AddScoped<IDocumentLoaderService, DocumentLoaderService>();
            builder.Services.AddScoped<IChunkingService, ChunkingService>();
            builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
            builder.Services.AddScoped<IRetrievalService, RetrievalService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IRagService, RagService>();

            // In-memory vector store for the current knowledge base build.
            builder.Services.AddSingleton<KnowledgeBaseService>();

            // ---------- CORS (allow the Angular app to call this API) ----------
            string allowedOrigin = builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:4200";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins(allowedOrigin)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Ensure the database and ChatLogs table exist.
            // (The SQLScripts/TrainingHelpDeskDb.sql script can also be run manually
            // against SQL Server to create the schema explicitly.)
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.EnsureCreatedAsync();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAngularApp");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
