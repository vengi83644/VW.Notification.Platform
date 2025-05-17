namespace VW.Notification.Application.Interfaces.Repositories;

public interface IRepository<T>
{
    void Add(T obj);
    T? GetById(Guid id);
    List<T> GetAll();
}
