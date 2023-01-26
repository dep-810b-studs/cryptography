using Cryptography.Algorithms.RSA;
using Cryptography.Algorithms.Utils;
using Cryptography.Arithmetic.ResidueNumberSystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cryptography.WebInterface.ServerSideApp.Rsa;

public static class ConfigurationExtensions
{
    public static void ConfigureRsa(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RSASettings>(settings => configuration.GetSection("RSASettings").Bind(settings));
        services.AddSingleton<IResidueNumberSystem, ResidueNumberSystem>();
        services.AddSingleton<IRSACipher, RSACipher>();
        services.AddSingleton<IMessageConvertor, MessageConvertor>();
        services.AddSingleton<IRsaEncryptionSystem, RsaEncryptionSystem>();
    }
}