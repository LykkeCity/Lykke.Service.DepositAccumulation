using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.HttpClientGenerator;

namespace Lykke.Service.DepositAccumulation.Client
{
    [PublicAPI]
    /*
    public static class AutofacExtension
    {
        /// <summary>
        /// Registers <see cref="IDepositAccumulationClient"/> in Autofac container using <see cref="DepositAccumulationServiceClientSettings"/>.
        /// </summary>
        /// <param name="builder">Autofac container builder.</param>
        /// <param name="settings">DepositAccumulation client settings.</param>
        /// <param name="builderConfigure">Optional <see cref="HttpClientGeneratorBuilder"/> configure handler.</param>
        public static void RegisterDepositAccumulationClient(
            [NotNull] this ContainerBuilder builder,
            [NotNull] DepositAccumulationServiceClientSettings settings,
            [CanBeNull] Func<HttpClientGeneratorBuilder, HttpClientGeneratorBuilder> builderConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (string.IsNullOrWhiteSpace(settings.ServiceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(DepositAccumulationServiceClientSettings.ServiceUrl));

            builder.RegisterClient<IDepositAccumulationClient>(settings?.ServiceUrl, builderConfigure);
        }
    }
    */


    public static class AutofacExtensions
    {
        /// <summary>
        /// Registers Refit client of type IDepositAccumulationClient.
        /// </summary>
        public static void RegisterDepositAccumulationClient(
            [NotNull] this ContainerBuilder builder,
            [NotNull] string serviceUrl,
            [CanBeNull] Func<HttpClientGeneratorBuilder, HttpClientGeneratorBuilder> builderConfigure = null)

        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be empty.", nameof(serviceUrl));

            builder.RegisterInstance(
                    new DepositAccumulationClient(HttpClientGenerator.HttpClientGenerator.BuildForUrl(serviceUrl)
                        .WithoutRetries()
                        .Create())
                )
                .As<IDepositAccumulationClient>()
                .SingleInstance();
        }
    }
}
