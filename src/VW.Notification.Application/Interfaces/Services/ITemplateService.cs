namespace VW.Notification.Application.Interfaces.Services;

public interface ITemplateService<TModel>
{
    Task<string> GetTemplateAsync(string templateName, TModel model);
}
