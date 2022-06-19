namespace ClearBank.DeveloperTest.Domain.Interfaces
{
    public interface IBaseClass
    {
        protected T GetService<T>();
    }
}
