var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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