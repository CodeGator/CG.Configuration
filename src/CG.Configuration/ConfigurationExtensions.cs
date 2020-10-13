using CG.Validations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IConfiguration"/>
    /// type.
    /// </summary>
    public static partial class ConfigurationExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

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

                    // Get the underyling property type.
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
            value = new T[0];

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
            value = default(T);

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
                    result = !EqualityComparer<T>.Default.Equals(value, default(T));
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
                else if (typeof(T) == typeof(Single))
                {
                    result = configuration.TryGetAsSingle(key, out var sValue);
                    if (result)
                    {
                        value = (T)Convert.ChangeType(sValue, typeof(T));
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
                    result = !EqualityComparer<T>.Default.Equals(value, default(T));
                }
            }
            else
            {
                // If we get here then T is a ref type so we only have to convert
                //   the value to T and we're done.
                value = (T)Convert.ChangeType(setting, typeof(T));
                result = !EqualityComparer<T>.Default.Equals(value, default(T));
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
            value = default(bool);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = bool.TryParse(setting, out var bValue);

            // Did we succeed?
            if (result)
            {
                value = (bool)Convert.ChangeType(bValue, typeof(bool));
            }

            // Return the results.
            return result;
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
            value = default(char);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = char.TryParse(setting, out var cValue);

            // Did we succeed?
            if (result)
            {
                value = (char)Convert.ChangeType(cValue, typeof(char));
            }

            // Return the results.
            return result;
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
            value = default(TimeSpan);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = TimeSpan.TryParse(setting, out var tsValue);

            // Did we succeed?
            if (result)
            {
                value = (TimeSpan)Convert.ChangeType(tsValue, typeof(TimeSpan));
            }

            // Return the results.
            return result;
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
            value = default(DateTime);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = DateTime.TryParse(setting, out var tsValue);

            // Did we succeed?
            if (result)
            {
                value = (DateTime)Convert.ChangeType(tsValue, typeof(DateTime));
            }

            // Return the results.
            return result;
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
            value = default(DateTimeOffset);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = DateTimeOffset.TryParse(setting, out var tsValue);

            // Did we succeed?
            if (result)
            {
                value = (DateTimeOffset)Convert.ChangeType(tsValue, typeof(DateTimeOffset));
            }

            // Return the results.
            return result;
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
            value = default(int);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = int.TryParse(setting, out var iValue);

            // Did we succeed?
            if (result)
            {
                value = (int)Convert.ChangeType(iValue, typeof(int));
            }

            // Return the results.
            return result;
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
            value = default(uint);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = uint.TryParse(setting, out var uiValue);

            // Did we succeed?
            if (result)
            {
                value = (uint)Convert.ChangeType(uiValue, typeof(uint));
            }

            // Return the results.
            return result;
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
            value = default(long);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = long.TryParse(setting, out var uiValue);

            // Did we succeed?
            if (result)
            {
                value = (long)Convert.ChangeType(uiValue, typeof(long));
            }

            // Return the results.
            return result;
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
            value = default(ulong);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = ulong.TryParse(setting, out var ulValue);

            // Did we succeed?
            if (result)
            {
                value = (ulong)Convert.ChangeType(ulValue, typeof(ulong));
            }

            // Return the results.
            return result;
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
            value = default(byte);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = byte.TryParse(setting, out var bValue);

            // Did we succeed?
            if (result)
            {
                value = (byte)Convert.ChangeType(bValue, typeof(byte));
            }

            // Return the results.
            return result;
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
            value = default(float);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = float.TryParse(setting, out var fValue);

            // Did we succeed?
            if (result)
            {
                value = (float)Convert.ChangeType(fValue, typeof(float));
            }

            // Return the results.
            return result;
        }

        // *******************************************************************

        /// <summary>
        /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
        /// object, parses it, and returns the value as a single value. 
        /// </summary>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="value">The value read by the operation.</param>
        /// <returns>True is the setting was read and converted to a single value; 
        /// false otherwise.</returns>
        public static bool TryGetAsSingle(
            this IConfiguration configuration,
            string key,
            out Single value
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
                .ThrowIfNullOrEmpty(key, nameof(key));

            // Start with a default value.
            value = default(Single);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = Single.TryParse(setting, out var sValue);

            // Did we succeed?
            if (result)
            {
                value = (Single)Convert.ChangeType(sValue, typeof(Single));
            }

            // Return the results.
            return result;
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
            value = default(double);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = double.TryParse(setting, out var dValue);

            // Did we succeed?
            if (result)
            {
                value = (double)Convert.ChangeType(dValue, typeof(double));
            }

            // Return the results.
            return result;
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
            value = default(decimal);

            // First we must read the setting from the configuration.
            var setting = configuration[key];

            // Did we fail?
            if (string.IsNullOrEmpty(setting))
            {
                // Return the results of the operation.
                return false;
            }

            // Try to parse the value.
            var result = decimal.TryParse(setting, out var dValue);

            // Did we succeed?
            if (result)
            {
                value = (decimal)Convert.ChangeType(dValue, typeof(decimal));
            }

            // Return the results.
            return result;
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
            value = default(Guid);

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
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <param name="key">The key to use for the operation.</param>
        /// <param name="defaultValue">The value to return if the key is missing 
        /// or the value can't be parsed.</param>
        /// <returns>The value associated with the specified key, in the configuration,
        /// or the default value if the key is missing, or can't be parsed into 
        /// the desired type.</returns>
        public static Single GetAsSingle(
            this IConfiguration configuration,
            string key,
            Single defaultValue
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
                .ThrowIfNullOrEmpty(key, nameof(key));

            // Attempt to read/parse/convert the value.
            if (!configuration.TryGetAsSingle(key, out var retValue))
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

        #endregion
    }
}
