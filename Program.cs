using RozgarNowAPIs.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------- SERVICES ----------------

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔥 MongoDB Service inject
builder.Services.AddSingleton<MongoDbService>();

// 🔥 CORS (React frontend ke liye MUST)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// ---------------- PIPELINE ----------------

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔥 CORS enable karo yahan
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();