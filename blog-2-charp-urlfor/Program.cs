using blog_2_charp_urlfor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITemplateParser, TemplateParser>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/Example", (ITemplateParser parser) =>
{
    var name = "Victor";
    var template = "<p>My name is {{name}}</p>";
   var templateText = parser.Render(new { name = "Tino" }, template);
   return templateText;

});

app.MapGet("/ExampleUrl", (ITemplateParser parser, HttpContext context) =>
{
    var template = $@"<a href=""/examples/{23}"">Hardcoded url</a>";
    var templateText = parser.Render(new { }, template);
    context.Response.ContentType = "text/html";
    context.Response.WriteAsync(templateText);
});

app.MapGet("/GetUrl", (ITemplateParser parser, HttpContext context) =>
{
    var template = @"<a href=""{{url_for 'GetExamplesUrlFor' 23}}"">The href was generated using our custom url_for</a>";
   var templateText = parser.Render(new { }, template);
   context.Response.ContentType = "text/html";
   context.Response.WriteAsync(templateText);

});

app.MapGet("/examples/{id}", (int id, ITemplateParser parser, HttpContext context) =>
{
    var template = $@"<p>The routing param from the url is {id}</p> <br> <p>End of Demo</p>";
   var templateText = parser.Render(new { }, template);
   context.Response.ContentType = "text/html";
   context.Response.WriteAsync(templateText);
})
    .WithName("GetExamplesUrlFor");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
