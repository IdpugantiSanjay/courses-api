namespace SharedModule;

public interface IAppDbContext
{
    static abstract string Schema { get; }
}