
namespace OS.Communication;


public interface IDataService
{
    T Load<T>(string fileName) where T : new();
    void Save<T>(string fileName, T data);

}
