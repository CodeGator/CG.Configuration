﻿
namespace Microsoft.Extensions.Configuration;

/// <summary>
/// This class contains extension methods related to the <see cref="IConfiguration"/>
/// type, for registering types related to configurations.
/// </summary>
public static partial class ConfigurationExtensions
{
    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method determines if the specified field is missing, or not.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <param name="fieldName">The field name to use for the operation.</param>
    /// <returns>True if the field is missing; False otherwise.</returns>
    public static bool FieldIsMissing(
        this IConfiguration configuration,
        string fieldName
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(fieldName, nameof(fieldName));

        // Is the field missing and NOT an array?
        var result = null == configuration[fieldName] &&
            null == configuration[$"{fieldName}:0"];

        // Return the results.
        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method determines if the specified field contains an array, or
    /// not.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <param name="fieldName">The field name to use for the operation.</param>
    /// <returns>True if the field contains an array; False otherwise.</returns>
    public static bool FieldIsArray(
        this IConfiguration configuration,
        string fieldName
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(fieldName, nameof(fieldName));

        // Is the field an array?
        var result = null != configuration[$"{fieldName}:0"];

        // Return the results.
        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method determines if the specified configuration section has
    /// any child nodes, or not.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <returns>True if the configuration section has child nodes; False 
    /// otherwise.</returns>
    public static bool HasChildren(
        this IConfiguration configuration
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

        // Are there any child nodes?
        var result = configuration.GetChildren().Any();

        // Return the results.
        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method returns the parent section of the specified configuration
    /// object. If there is no parent, the same section is returned.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <returns>The parent section for the specified <see cref="IConfiguration"/>
    /// object.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static IConfigurationSection GetParentSection(
        this IConfiguration configuration
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

        // Get the root configuration.
        var root = configuration.GetRoot();

        // Get the current configuration path.
        var path = configuration.GetPath();

        // Figure out the path to the parent section (if any).
        var parentPath = string.IsNullOrEmpty(path) || !path.Contains(":")
            ? path
            : path.Substring(0, path.LastIndexOf(":"));

        // Get the parent section (if possible).
        var parentSection = string.IsNullOrEmpty(parentPath)
            ? configuration
            : root.GetSection(parentPath);

        // Return the parent section.
        return parentSection as IConfigurationSection;
    }

    // *******************************************************************

    /// <summary>
    /// This method always returns the root section of the specified 
    /// configuration hierarchy.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <returns>The root section for the specified <see cref="IConfiguration"/>
    /// object.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static IConfiguration GetRoot(
        this IConfiguration configuration
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

        // This is, admittedly, a hack. But, it's also the only practical
        //   way, that I know of, to get to the root of the configuration
        //   tree, from anywhere within that tree. If you know a better way
        //   then feel free to share with the rest of the class.

        // Get the inner root instance, cast to the type we need.
        var root = configuration.GetFieldValue(
            "_root",
            true
            ) as IConfiguration;

        // Return the result.
        return root;
    }

    // *******************************************************************

    /// <summary>
    /// This method returns the path for the specified <see cref="IConfiguration"/>
    /// object.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <returns>The path for the specified <see cref="IConfiguration"/>
    /// object.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static string GetPath(
        this IConfiguration configuration
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

        // Get the current section.
        var section = configuration.GetSection("");

        // Get the path from the section.
        var path = section.Path.TrimEnd(':');

        // Return the path.
        return path;
    }

    // *******************************************************************

    /// <summary>
    /// This method returns the value for the specified <see cref="IConfiguration"/>
    /// object.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <returns>The value for the specified <see cref="IConfiguration"/>
    /// object.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static string GetValue(
        this IConfiguration configuration
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

        // Get the current section.
        var section = configuration.GetSection("");

        // Get the value from the section.
        var value = section.Value;

        // Return the value.
        return value;
    }

    // *******************************************************************

    /// <summary>
    /// This method safely copies a value from the specified key of the 
    /// configuration to the property on the target oject using a LINQ
    /// expression to identify the property to change. If the configuration
    /// key is missing, or the value is NULL, then no copy operation is
    /// performed - unless the <paramref name="allowSetNulls"/> property is
    /// set to TRUE.
    /// </summary>
    /// <typeparam name="TObj">The target object type.</typeparam>
    /// <typeparam name="TProp">The target property type.</typeparam>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name = "target" > The target object to use for the operation.</param>
    /// <param name="selector">The LINQ expression to use for the operation.</param>
    /// <param name="key">The configuration key to use for the operation.</param>
    /// <param name="allowSetNulls">Indicates whether to allow a NULL value to be
    /// copied to the specified property on the target object.</param>
    /// <returns>The value of the <paramref name="configuration"/> parameter.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static IConfiguration SafeCopy<TObj, TProp>(
        this IConfiguration configuration,
        TObj target,
        Expression<Func<TObj, TProp>> selector,
        string key,
        bool allowSetNulls = false
        ) where TObj : class
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key))
            .ThrowIfNull(target, nameof(target))
            .ThrowIfNull(selector, nameof(selector));

        // Is the expression what we're expecting it to be?
        if (selector.Body is MemberExpression)
        {
            // Does the expression point to a property?
            if ((selector.Body as MemberExpression).Member is PropertyInfo)
            {
                // Get the reflection info for the property.
                var pi = (selector.Body as MemberExpression).Member as PropertyInfo;

                // Get the underlying property type.
                var tProp = typeof(TProp);

                // Read the value associated with the configuration key.
                var value = configuration[key];

                // Watch out for value types, they are persnickety about
                //   parsing sometimes.
                if (tProp.IsValueType)
                {
                    // Parse the value using the appropriate parsing method.
                    if (tProp.IsEnum)
                    {
                        var eValue = Enum.Parse(tProp, value);
                        if (eValue != null || allowSetNulls)
                        {
                            pi.SetValue(target, eValue);
                        }
                    }
                    else if (tProp == typeof(bool) || tProp == typeof(bool?))
                    {
                        if (bool.TryParse(value, out bool bValue))
                        {
                            pi.SetValue(target, bValue);
                        }
                    }
                    else if (tProp == typeof(TimeSpan) || tProp == typeof(TimeSpan?))
                    {
                        if (TimeSpan.TryParse(value, out TimeSpan tsValue))
                        {
                            pi.SetValue(target, tsValue);
                        }
                    }
                    else if (tProp == typeof(DateTime) || tProp == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(value, out DateTime dtValue))
                        {
                            pi.SetValue(target, dtValue);
                        }
                    }
                    else if (tProp == typeof(double) || tProp == typeof(double?))
                    {
                        if (double.TryParse(value, out double dValue))
                        {
                            pi.SetValue(target, dValue);
                        }
                    }
                    else if (tProp == typeof(int) || tProp == typeof(int?))
                    {
                        if (int.TryParse(value, out int iValue))
                        {
                            pi.SetValue(target, iValue);
                        }
                    }
                    else if (tProp == typeof(long) || tProp == typeof(long?))
                    {
                        if (long.TryParse(value, out long iValue))
                        {
                            pi.SetValue(target, iValue);
                        }
                    }
                    else if (tProp != pi.PropertyType)
                    {
                        var cnvValue = Convert.ChangeType(value, pi.PropertyType);
                        if (cnvValue != null || allowSetNulls)
                        {
                            pi.SetValue(target, cnvValue, null);
                        }
                    }
                }
                else
                {
                    // No need to parse, it's a ref type.
                    if (value != null || allowSetNulls)
                    {
                        pi.SetValue(target, value, null);
                    }
                }
            }
        }

        // Return the configuration.
        return configuration;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a list of key-value-pairs and converts them to
    /// the specified type before returning.
    /// </summary>
    /// <typeparam name="T">The type to use for the operation.</typeparam>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="value">The list of values read from the key.</param>
    /// <returns>True if the setting was read and converted; false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsList<T>(
        this IConfiguration configuration,
        string key,
        out IEnumerable<T> value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a false result.
        var result = false;

        // Start with an empty list.
        value = Array.Empty<T>();

        var list = new List<T>();
        var index = 0;

        // Loop and read/parse/convert the list settings.
        while (configuration.TryGetAs($"{key}:{index}", out T item))
        {
            // Add the item to the list.
            list.Add(item);

            // Bump up the index.
            index++;
        }

        // Save the resulting list.
        value = list.ToArray();

        // What is the result?
        result = list.Count > 0;

        // Return the results of the operation.
        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as <typeparamref name="T"/> value. 
    /// </summary>
    /// <typeparam name="T">The type to use for the operation.</typeparam>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="value">The list of values read from the key.</param>
    /// <returns>True if the setting was read and converted; false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAs<T>(
        this IConfiguration configuration,
        string key,
        out T value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a false result.
        var result = false;

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return result;
        }

        // Next we must parse/convert the setting to a T (hah, see what I did there?)

        // Is T a value type?
        if (typeof(T).IsValueType)
        {
            // If we get here then T is a value type so the parsing could be 
            //   tricky. As a result, we'll defer to the type's TryGetAs overload 
            //   (if there is one) and then convert the results (if any), to a T.

            if (typeof(string) == typeof(T))
            {
                value = (T)Convert.ChangeType(setting, typeof(T));
                result = !string.IsNullOrEmpty($"{value}");
            }
            else if (typeof(T) == typeof(bool))
            {
                result = configuration.TryGetAsBoolean(key, out var bValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(bValue, typeof(T));
                }
            }
            else if (typeof(T).IsEnum)
            {
                value = (T)Convert.ChangeType(Enum.Parse(typeof(T), setting), typeof(T));
                result = !EqualityComparer<T>.Default.Equals(value, default);
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                result = configuration.TryGetAsTimeSpan(key, out var tsValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(tsValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(DateTime))
            {
                result = configuration.TryGetAsDateTime(key, out var dtValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(dtValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(DateTimeOffset))
            {
                result = configuration.TryGetAsDateTimeOffset(key, out var dtoValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(dtoValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(char))
            {
                result = configuration.TryGetAsChar(key, out var cValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(cValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(byte))
            {
                result = configuration.TryGetAsByte(key, out var bValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(bValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(int))
            {
                result = configuration.TryGetAsInt(key, out var iValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(iValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(uint))
            {
                result = configuration.TryGetAsUInt(key, out var uValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(uValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(long))
            {
                result = configuration.TryGetAsLong(key, out var lValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(lValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(ulong))
            {
                result = configuration.TryGetAsULong(key, out var ulValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(ulValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(float))
            {
                result = configuration.TryGetAsFloat(key, out var fValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(fValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(double))
            {
                result = configuration.TryGetAsDouble(key, out var dValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(dValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(decimal))
            {
                result = configuration.TryGetAsDecimal(key, out var dValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(dValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(Guid))
            {
                result = configuration.TryGetAsGuid(key, out var gValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(gValue, typeof(T));
                }
            }
            else
            {
                // If we get here then T is a kind of value type that we 
                //   don't know about, so, we'll use change type and hope 
                //   for the best.

                value = (T)Convert.ChangeType(setting, typeof(T));
                result = !EqualityComparer<T>.Default.Equals(value, default);
            }
        }
        else
        {
            // If we get here then T is a ref type so we only have to convert
            //   the value to T and we're done.
            value = (T)Convert.ChangeType(setting, typeof(T));
            result = !EqualityComparer<T>.Default.Equals(value, default);
        }

        // Return the results of the operation.
        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a boolean value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a boolean value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsBoolean(
        this IConfiguration configuration,
        string key,
        out bool value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (bool.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a char value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a char value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsChar(
        this IConfiguration configuration,
        string key,
        out char value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (char.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a TimeSpan value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a TimeSpan value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsTimeSpan(
        this IConfiguration configuration,
        string key,
        out TimeSpan value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (TimeSpan.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a DateTime value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a DateTime value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsDateTime(
        this IConfiguration configuration,
        string key,
        out DateTime value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (DateTime.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a DateTimeOffset value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a DateTimeOffset value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsDateTimeOffset(
        this IConfiguration configuration,
        string key,
        out DateTimeOffset value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (DateTimeOffset.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as an int value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to an int value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsInt(
        this IConfiguration configuration,
        string key,
        out int value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (int.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a uint value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a uint value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsUInt(
        this IConfiguration configuration,
        string key,
        out uint value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (uint.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a long value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a long value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsLong(
        this IConfiguration configuration,
        string key,
        out long value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (long.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a ulong value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a ulong value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsULong(
        this IConfiguration configuration,
        string key,
        out ulong value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (ulong.TryParse(setting, out value))
        {
            // We suceeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a byte value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a byte value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsByte(
        this IConfiguration configuration,
        string key,
        out byte value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (byte.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a float value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a float value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsFloat(
        this IConfiguration configuration,
        string key,
        out float value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (float.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a double value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a double value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsDouble(
        this IConfiguration configuration,
        string key,
        out double value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (double.TryParse(setting, out value))
        {
            // We converted the value.
            return true;
        }

        // We failed to convert the value.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a decimal value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a decimal value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsDecimal(
        this IConfiguration configuration,
        string key,
        out decimal value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        if (decimal.TryParse(setting, out value))
        {
            // We succeeded.
            return true;
        }

        // We failed.
        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a GUID value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a GUID value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsGuid(
        this IConfiguration configuration,
        string key,
        out Guid value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Start with a default value.
        value = default;

        // First we must read the setting from the configuration.
        var setting = configuration[key];

        // Did we fail?
        if (string.IsNullOrEmpty(setting))
        {
            // Return the results of the operation.
            return false;
        }

        // Try to parse the value.
        var result = Guid.TryParse(setting, out var gValue);

        // Did we succeed?
        if (result)
        {
            value = (Guid)Convert.ChangeType(gValue, typeof(Guid));
        }

        // Return the results.
        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool GetAsBoolean(
        this IConfiguration configuration,
        string key,
        bool defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsBoolean(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static int GetAsInt(
        this IConfiguration configuration,
        string key,
        int defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsInt(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static uint GetAsUInt(
        this IConfiguration configuration,
        string key,
        uint defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsUInt(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static long GetAsLong(
        this IConfiguration configuration,
        string key,
        long defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsLong(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static ulong GetAsULong(
        this IConfiguration configuration,
        string key,
        ulong defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsULong(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static byte GetAsByte(
        this IConfiguration configuration,
        string key,
        byte defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsByte(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static Guid GetAsGuid(
        this IConfiguration configuration,
        string key,
        Guid defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsGuid(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static TimeSpan GetAsTimeSpan(
        this IConfiguration configuration,
        string key,
        TimeSpan defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsTimeSpan(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static DateTime GetAsDateTime(
        this IConfiguration configuration,
        string key,
        DateTime defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsDateTime(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static DateTimeOffset GetAsDateTimeOffset(
        this IConfiguration configuration,
        string key,
        DateTimeOffset defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsDateTimeOffset(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static decimal GetAsDecimal(
        this IConfiguration configuration,
        string key,
        decimal defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAsDecimal(key, out var retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <typeparam name="T">The type to use for the operation.</typeparam>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static T GetAs<T>(
        this IConfiguration configuration,
        string key,
        T defaultValue
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        // Attempt to read/parse/convert the value.
        if (!configuration.TryGetAs(key, out T retValue))
        {
            // If we fail, use the default value.
            retValue = defaultValue;
        }

        // Return the results.
        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method converts the given configuration object into equivalent 
    /// JSON text.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="level">The level for the operation.</param>
    /// <returns>Formatted JSON text.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever 
    /// one or more arguments is missing or invalid.</exception>
    public static string ToJson(
        this IConfiguration configuration,
        int level = 1
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfLessThanOrEqualZero(level, nameof(level));

        // Start with the opening brace.
        var sb = new StringBuilder("{" + Environment.NewLine);

        // Create some tabs for formatting.
        var tabs = new string('\t', level);

        // Loop through the children.
        foreach (var child in configuration.GetChildren())
        {
            // Append the child as JSON.
            if (child.HasChildren())
            {
                sb.AppendLine($"{tabs}\"{child.Key}\": {child.ToJson(level + 1)},");
            }
            else
            {
                sb.AppendLine($"{tabs}\"{child.Key}\": \"{child.Value}\",");
            }
        }

        // Remove the trailing comma.
        sb.Remove(sb.Length - 1, 1);

        // Add the final brace.
        sb.AppendLine("}" + Environment.NewLine);

        // Return the formatted JSON.
        return sb.ToString();
    }

    // *******************************************************************

    /// <summary>
    /// This method blasts the contents of the given configuration object
    /// to the specified file, as formatted JSON text.  
    /// </summary>
    /// <param name="configuration">The configuration object to use for 
    /// the operation.</param>
    /// <param name="filePath">The file to write to, or create if needed.</param>
    /// <exception cref="ArgumentException">This exception is thrown whenever 
    /// one or more arguments is missing or invalid.</exception>
    /// <remarks>
    /// <para>
    /// The file is created if needed. The contents of the file are overwritten 
    /// without warning. The contents of the configuration object are not 
    /// distributed between environments.
    /// </para>
    /// </remarks>
    public static void WriteAsJSON(
        this IConfiguration configuration,
        string filePath
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(filePath, nameof(filePath));

        // Convert the configuration to JSON.
        var json = configuration.ToJson();

        // Write the file.
        File.WriteAllText(filePath, json);
    }

    // *******************************************************************

    /// <summary>
    /// This method blasts the contents of the given configuration object
    /// to the specified file, as formatted JSON text.  
    /// </summary>
    /// <param name="configuration">The configuration object to use for 
    /// the operation.</param>
    /// <param name="filePath">The file to write to, or create if needed.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task to perform the operation.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever 
    /// one or more arguments is missing or invalid.</exception>
    /// <remarks>
    /// <para>
    /// The file is created if needed. The contents of the file are overwritten 
    /// without warning. The contents of the configuration object are not 
    /// distributed between environments.
    /// </para>
    /// </remarks>
    public static async Task WriteAsJSONAsync(
        this IConfiguration configuration,
        string filePath,
        CancellationToken cancellationToken = default
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(filePath, nameof(filePath));

        // Convert the configuration to JSON.
        var json = configuration.ToJson();

        // Write the file.
        await File.WriteAllTextAsync(
            filePath,
            json,
            cancellationToken
            ).ConfigureAwait(false);
    }

    #endregion

    // *******************************************************************
    // Private methods.
    // *******************************************************************

    #region Private methods

    /// <summary>
    /// This method reads a field value from the specified object.
    /// </summary>
    /// <param name="obj">The object to use for the operation.</param>
    /// <param name="fieldName">The field to use for the operation.</param>
    /// <param name="includeProtected">Determines if protected fields are included 
    /// in the search.</param>
    /// <returns>The value of the field.</returns>
    /// <remarks>
    /// <para>The idea, with this method, is to use reflection to go find
    /// and return a field value from an object at runtime. The intent is 
    /// to use this sparingly because the performance isn't great. I see
    /// this approach as something useful for things like unit testing.</para>
    /// </remarks>
    private static object? GetFieldValue(
        this object obj,
        string fieldName,
        bool includeProtected = false
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(obj, nameof(obj))
            .ThrowIfNullOrEmpty(fieldName, nameof(fieldName));

        // Get the object type.
        var type = obj.GetType();

        // Find the reflection info for the field.
        var fi = type.GetField(
            fieldName,
            BindingFlags.Static |
            BindingFlags.Instance |
            BindingFlags.Public |
            (includeProtected ? BindingFlags.NonPublic : BindingFlags.Public)
            );

        // Did we fail?
        if (fi == null)
        {
            return null;
        }

        // Get the field value.
        var value = fi.GetValue(obj);

        // Get the actual return type.
        var valType = value?.GetType();

        // Deal with weak reference types.
        if (valType == typeof(WeakReference))
        {
            return (value as WeakReference)?.Target;
        }

        // Return the results.
        return value;
    }

    #endregion
}
