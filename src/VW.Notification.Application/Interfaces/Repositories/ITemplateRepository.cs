namespace VW.Notification.Application.Interfaces.Repositories;

public interface ITemplateRepository
{
    void Add(TemplateMessage templateMessage);

    TemplateMessage? GetById(string templateId);

    List<TemplateMessage> GetAll();
}
