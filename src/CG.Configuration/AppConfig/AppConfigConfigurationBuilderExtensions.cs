using CG.Configuration.AppConfig;
using System;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IConfigurationBuilder"/>
    /// type, for registering types related to configuration builders.
    /// </summary>
    public static class AppConfigConfigurationBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method injects an app.config source into the specified builder.
        /// </summary>
        /// <param name="builder">The builder to use for the operation.</param>
        /// <param name="optional">True if the file is optional; false otherwise.</param>
        /// <param name="reloadOnChange">True if the provider should reload the 
        /// data from the underlying file changes; false otherwise.</param>
        /// <returns>An <see cref="IConfigurationSource"/> object.</returns>
        public static IConfigurationSource AddAppConfigSource(
            this IConfigurationBuilder builder,
            bool optional = true,
            bool reloadOnChange = false
            )
        {
            // Create the source.
            var source = new AppConfigConfigurationSource(
                optional, 
                reloadOnChange
                );

            // Add the source to the builder.
            builder.Add(source);

            // Return the source.
            return source;
        }

        #endregion
    }
}
