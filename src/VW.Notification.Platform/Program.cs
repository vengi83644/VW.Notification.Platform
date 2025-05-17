var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddRazorPages();
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Add("/Templates/{0}" + RazorViewEngine.ViewExtension);
});

// Add InMemory repositories as Singletons
builder.Services.AddSingleton<IRuleRepository<Rule>, InMemoryRuleRepository>();
builder.Services.AddSingleton<ICustomerRepository<Customer>, InMemoryCustomerRepository>();
builder.Services.AddSingleton<ICustomerRuleRepository<CustomerRule>, InMemoryCustomerRuleRepository>();

builder.Services.AddSingleton<ITemplateRepository, InMemoryTemplateRepository>();

builder.Services.AddSingleton<INotificationStrategyFactory, NotificationStrategyFactory>();

builder.Services.AddTransient<ITemplateService<NotificationTemplateData>, RazorViewEngine<NotificationTemplateData>>();

builder.Services.AddScoped<IEventService, EventService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
