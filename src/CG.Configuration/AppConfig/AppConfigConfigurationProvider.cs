
namespace CG.Configuration.AppConfig;

/// <summary>
/// This class is an app.config implementation of <see cref="ConfigurationProvider"/>
/// </summary>
internal class AppConfigConfigurationProvider : FileConfigurationProvider
{
    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This constructor creates a new instance of the <see cref="AppConfigConfigurationProvider"/>
    /// class.
    /// </summary>
    /// <param name="source">The source to use for the operation</param>
    public AppConfigConfigurationProvider(
        AppConfigConfigurationSource source
        ) : base(source)
    {

    }

    #endregion

    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method loads or reloads the data for the provider.
    /// </summary>
    /// <param name="stream">The stream to use for the operation.</param>
    public override void Load(
        Stream stream
        )
    {
        // Cleay any old data.
        Data.Clear();

        // Read any data from the stream.
        Data = ReadAppConfig(stream);
    }

    #endregion

    // *******************************************************************
    // Private methods.
    // *******************************************************************

    #region Private methods

    /// <summary>
    /// This method looks for a local app.config file and, if it finds one, 
    /// reads it and returns the contents as an enumerable sequence of 
    /// key-value-pair objects.
    /// </summary>
    /// <param name="stream">The stream to use for the operation</param>
    /// <returns>The contents of any local app.config file.</returns>
    private static IDictionary<string, string> ReadAppConfig(
        Stream stream
        )
    {
        var table = new Dictionary<string, string>();

        // Open the xml stream.
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(stream);

        // Look for any local app-settings.
        var appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");

        // Did we find anything?
        if (appSettingsNode != null)
        {
            // Loop and process the app-settings.
            foreach (XmlNode childNode in appSettingsNode.ChildNodes)
            {
                // What we do depends on the node type.
                switch (childNode.Name.ToLower())
                {
                    // Add the node to the list.
                    case "add":
                        var key = childNode.Attributes["key"].Value;
                        var value = childNode.Attributes["value"].Value;
                        table[key] = value;
                        break;

                    // Clear the list.
                    case "clear":
                        table.Clear();
                        break;

                    // Remove a specific node from the list.
                    case "remove":
                        key = childNode.Attributes["key"].Value;
                        var find = table.FirstOrDefault(x => x.Key == key);
                        if (find.Key != null)
                        {
                            table.Remove(find.Key);
                        }
                        break;

                    // Eeek! no idea what kind of node this is!
                    default:
                        break;
                }
            }
        }

        // Look for any local connection strings.
        var connectionStringsNode = xmlDoc.SelectSingleNode("configuration/connectionStrings");

        // Did we find anything?
        if (connectionStringsNode != null)
        {
            // Loop and process the connection strings.
            foreach (XmlNode childNode in connectionStringsNode.ChildNodes)
            {
                // What we do depends on the node type.
                switch (childNode.Name.ToLower())
                {
                    // Add the node to the list.
                    case "add":
                        var key = childNode.Attributes["name"].Value;
                        var value = childNode.Attributes["connectionString"].Value;
                        table[$"ConnectionStrings:{key}"] = value;
                        break;

                    // Clear the list.
                    case "clear":
                        table.Clear();
                        break;

                    // Remove a specific node from the list.
                    case "remove":
                        key = childNode.Attributes["name"].Value;

                        var find = table.FirstOrDefault(x => x.Key == $"ConnectionStrings:{key}");
                        if (find.Key != null)
                        {
                            table.Remove(find.Key);
                        }
                        break;

                    // Eeek! no idea what kind of node this is!
                    default:
                        break;
                }
            }
        }

        // Return the results.
        return table;
    }

    #endregion
}
