namespace PlexSSO.Common.Service.Config
{
    public interface IConfigurationService
    {
        string GetConfigurationFile();
        string GetConfigurationDirectory();
        T GetConfiguration<T>();
    }
}
