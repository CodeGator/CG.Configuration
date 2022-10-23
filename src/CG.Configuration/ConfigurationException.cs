
namespace CG.Configuration;

/// <summary>
/// This class represents a configuration related exception.
/// </summary>
[Serializable]
public class ConfigurationException : Exception
{
    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This constructor creates a new instance of the <see cref="ConfigurationException"/>
    /// class.
    /// </summary>
    public ConfigurationException()
    {

    }

    // *******************************************************************

    /// <summary>
    /// This constructor creates a new instance of the <see cref="ConfigurationException"/>
    /// class.
    /// </summary>
    /// <param name="message">The message to use for the exception.</param>
    /// <param name="innerException">An optional inner exception reference.</param>
    public ConfigurationException(
        string message,
        Exception innerException
        ) : base(message, innerException)
    {

    }

    // *******************************************************************

    /// <summary>
    /// This constructor creates a new instance of the <see cref="ConfigurationException"/>
    /// class.
    /// </summary>
    /// <param name="message">The message to use for the exception.</param>
    public ConfigurationException(
        string message
        ) : base(message)
    {

    }

    // *******************************************************************

    /// <summary>
    /// This constructor creates a new instance of the <see cref="ConfigurationException"/>
    /// class.
    /// </summary>
    /// <param name="info">The serialization info to use for the exception.</param>
    /// <param name="context">The streaming context to use for the exception.</param>
    public ConfigurationException(
        SerializationInfo info,
        StreamingContext context
        ) : base(info, context)
    {

    }

    #endregion
}
