using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.HttpClientGenerator;

namespace Lykke.Service.DepositAccumulation.Client
{
    [PublicAPI]

    public static class AutofacExtensions
    {
        /// <summary>
        /// Registers Refit client of type IDepositAccumulationClient.
        /// </summary>
        public static void RegisterDepositAccumulationClient(
            [NotNull] this ContainerBuilder builder,
            [NotNull] string serviceUrl
            )

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
