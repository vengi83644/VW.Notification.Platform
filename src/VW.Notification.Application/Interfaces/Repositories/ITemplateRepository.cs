namespace VW.Notification.Application.Interfaces.Repositories;

public interface ITemplateRepository
{
    void Add(TemplateMessage template);

    TemplateMessage? GetById(string templateId);

    List<TemplateMessage> GetAll();
}
