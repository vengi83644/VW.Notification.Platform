var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add InMemory repositories as Singletons
builder.Services.AddSingleton<IRuleRepository<Rule>, InMemoryRuleRepository>();
builder.Services.AddSingleton<ITemplateRepository<TemplateMessage>, InMemoryTemplateRepository>();
builder.Services.AddSingleton<ICustomerRepository<Customer>, InMemoryCustomerRepository>();
builder.Services.AddSingleton<ICustomerRuleRepository<CustomerRule>, InMemoryCustomerRuleRepository>();

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
