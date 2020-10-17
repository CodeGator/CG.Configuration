using CG.DataAnnotations;
using CG.DataAnnotations.Options;
using CG.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// type.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method attempts to configure the specified options type with 
        /// the service collection. It then returns a bound and validated options
        /// instance.
        /// </summary>
        /// <typeparam name="TOptions">The type of associated options.</typeparam>
        /// <param name="serviceCollection">The service collection to use for the 
        /// operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>True if the options were configured; false otherwise.</returns>
        public static bool TryConfigure<TOptions>(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            ) where TOptions : class, new()
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Create the options.
            var options = new TOptions();

            // Bind the options to the configuration.
            configuration.Bind(options);

            // Are the options verifiable?
            if (options is OptionsBase)
            {
                // Are the options valid?
                if ((options as OptionsBase).IsValid())
                {
                    // Add the options to the DI container.
                    serviceCollection.TryAddSingleton<IOptions<TOptions>>(
                        new OptionsWrapper<TOptions>(options)
                        );

                    // Return the results.
                    return true;
                }
            }

            // Return the results.
            return false;
        }

        // *******************************************************************

        /// <summary>
        /// This method attempts to configure the specified options type with 
        /// the service collection. It then returns a bound and validated options
        /// instance.
        /// </summary>
        /// <typeparam name="TOptions">The type of associated options.</typeparam>
        /// <param name="serviceCollection">The service collection to use for the 
        /// operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <param name="options">The bound and validated options instance.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        /// <exception cref="ValidationException">This exception is thrown if the options
        /// fail validation after the binding operation.</exception>
        public static IServiceCollection Configure<TOptions>(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            out TOptions options
            ) where TOptions : class, new()
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Create the options.
            options = new TOptions();

            // Bind the options to the configuration.
            configuration.Bind(options);

            // Verify the result - if possible.
            (options as OptionsBase)?.ThrowIfInvalid();

            // Add the options to the DI container.
            serviceCollection.TryAddSingleton<IOptions<TOptions>>(
                new OptionsWrapper<TOptions>(options)
                );

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
