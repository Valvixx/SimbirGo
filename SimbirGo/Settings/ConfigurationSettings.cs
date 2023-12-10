using SimbirGo.Settings.Interfaces;

namespace SimbirGo.Settings;

public class ConfigurationSettings : IConfigurationSettings
{
    private readonly IConfiguration _configuration;

    public ConfigurationSettings(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public string DbConnectionsOwn => _configuration.GetSection("Postgres").Value;

}