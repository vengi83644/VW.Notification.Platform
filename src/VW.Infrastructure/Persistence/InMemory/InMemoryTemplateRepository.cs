namespace VW.Notification.Infrastructure.Persistence.InMemory;

public class InMemoryTemplateRepository : ITemplateRepository<TemplateMessage>
{
    private readonly ILogger<ITemplateRepository<TemplateMessage>> _logger;
    private readonly Dictionary<Guid, TemplateMessage> _templates = [];

    public InMemoryTemplateRepository(ILogger<ITemplateRepository<TemplateMessage>> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(configuration);

        LoadFromDisk(configuration);
    }

    private void LoadFromDisk(IConfiguration configuration)
    {
        var path = configuration.GetValue<string>("Templates");

        var templateFoldersPath = Path.Combine(AppContext.BaseDirectory, path);

        if (!Directory.Exists(templateFoldersPath))
        {
            var templateFiles = Directory.GetFiles(templateFoldersPath, "*.cshtml");

            foreach (var templateFile in templateFiles)
            {
                var templateId = Path.GetFileNameWithoutExtension(templateFile);

                var content = File.ReadAllText(templateFile);

                var templateMessage = new TemplateMessage(templateId, content);

                Add(templateMessage);
            }
        }
    }

    public void Add(TemplateMessage template)
    {
        if (_templates.ContainsKey(template.Id))
        {
            _logger.LogWarning("Template with ID '{Id}' already exists for TemplateId {TemplateId}", template.Id, template.TemplateId);
            return;
        }

        _templates.Add(template.Id, template);
    }

    public TemplateMessage? GetById(Guid id)
    {
        _templates.TryGetValue(id, out var template);

        if (template == null)
        {
            _logger.LogWarning("Template with ID '{Id}' not found.", id);
        }

        return template;
    }

    public List<TemplateMessage> GetAll()
    {
        return _templates.Values.ToList();
    }
}
