namespace VW.Notification.Infrastructure.Persistence.InMemory;

public class InMemoryTemplateRepository : ITemplateRepository
{
    private readonly ILogger<ITemplateRepository> _logger;
    private readonly Dictionary<string, TemplateMessage> _templates = [];

    public InMemoryTemplateRepository(ILogger<ITemplateRepository> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(configuration);

        LoadFromConfig(configuration);
    }

    private void LoadFromConfig(IConfiguration configuration)
    {
        var path = configuration.GetValue<string>("Templates");

        var templateFoldersPath = Path.Combine(AppContext.BaseDirectory, path);

        if (Directory.Exists(templateFoldersPath))
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

    public void Add(TemplateMessage templateMessage)
    {
        if (_templates.ContainsKey(templateMessage.TemplateId))
        {
            _logger.LogWarning("Template with ID '{Id}' already exists for TemplateId {TemplateId}", templateMessage.Id, templateMessage.TemplateId);
            return;
        }

        _templates.Add(templateMessage.TemplateId, templateMessage);
    }

    public TemplateMessage? GetById(string templateId)
    {
        _templates.TryGetValue(templateId, out var template);

        if (template == null)
        {
            _logger.LogWarning("Template with ID '{Id}' not found.", templateId);
        }

        return template;
    }

    public List<TemplateMessage> GetAll()
    {
        return _templates.Values.ToList();
    }
}
