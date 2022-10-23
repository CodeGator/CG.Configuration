
namespace CG.Configuration.AppConfig;

/// <summary>
/// This class is an app.config implementation of <see cref="IConfigurationSource"/>
/// </summary>
internal class AppConfigConfigurationSource : FileConfigurationSource
{
    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This constructor creates a new instance of the <see cref="AppConfigConfigurationSource"/>
    /// class.
    /// </summary>
    /// <param name="optional">True if the file is optional; false otherwise.</param>
    /// <param name="reloadOnChange">True if the provider should reload the 
    /// data from the underlying file changes; false otherwise.</param>
    public AppConfigConfigurationSource(
        bool optional = true,
        bool reloadOnChange = false
        )
    {
        // Save a path to the app.config file.
        Path = $"{AppDomain.CurrentDomain.FriendlyNameEx(false)}.config";

        // Save the value.
        Optional = optional;

        // Save the value.
        ReloadOnChange = reloadOnChange;
    }

    #endregion

    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method builds the <see cref="IConfigurationProvider"/> object 
    /// for this source.
    /// </summary>
    /// <param name="builder">The builder to use for the operation.</param>
    /// <returns>An <see cref="IConfigurationProvider"/> object.</returns>
    public override IConfigurationProvider Build(
        IConfigurationBuilder builder
        )
    {
        // Make sure the defaults exist.
        base.EnsureDefaults(builder);

        // Create the provider.
        var obj = new AppConfigConfigurationProvider(this);

        // Return the results.
        return obj;
    }

    #endregion
}

