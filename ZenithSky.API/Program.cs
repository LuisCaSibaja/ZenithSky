using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(
    options =>
    {
        //Esto lo que hace es que si un DTO no pasa las validaciones, responde con error 400 con formato JSON
        options.InvalidModelStateResponseFactory = context =>
        {
            string message = context.ModelState.Values
                .SelectMany(entry => entry.Errors)
                .Select(error => error.ErrorMessage)
                .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text)) ?? "Los datos enviados no son válidos.";
            return new BadRequestObjectResult(new { message });
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Permitir peticiones desde el emulador de Android y redes locales
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Habilita http://localhost:5175/swagger
}

app.UseCors("AllowAll");

// Comentado en desarrollo para permitir tráfico HTTP desde Android (10.0.2.2:5175)
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();