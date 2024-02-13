using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace ProjectX.Core
{
    public class EnvironmentVariableLoader
    {
        private readonly EnvironmentVariablesConfigurationProvider _provider;
        public EnvironmentVariableLoader()
        {
            _provider = new EnvironmentVariablesConfigurationProvider();
            _provider.Load();
        }
        public string FromEnvironmentVariable(string envVariable)
        {
            string? value = null;
            if (!_provider.TryGet(envVariable, out value) || string.IsNullOrEmpty(value))
            {
                throw new Exception($"Blow up: cannot get compulsory market data api key from environment variable '${envVariable}'");
            }

            return value;
        }
    }

}
