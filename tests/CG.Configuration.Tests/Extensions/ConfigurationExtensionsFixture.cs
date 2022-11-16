using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CG.Configuration
{
    /// <summary>
    /// This class is a test fixture for the <see cref="ConfigurationExtensions"/>
    /// class.
    /// </summary>
    [TestCategory("Unit")]
    [TestClass]
    public class ConfigurationExtensionsFixture
    {
        // *******************************************************************
        // Types.
        // *******************************************************************

        #region Types

        /// <summary>
        /// This class is used for internal testing purposes.
        /// </summary>
        class TestType
        {
            /// <summary>
            /// This property is used for internal testing purposes.
            /// </summary>
            public string A { get; set; }
        }

        #endregion

        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a populated configuration object.
        /// </summary>
        public IConfiguration Configuration { get; private set; }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method is called by the framework before each test run.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var testData = new Dictionary<string, string>()
            {
                { "A", "1" },
                { "list:0", "A" },
                { "list:1", "B" },
                { "list:2", "C" },
                { "bool", "true" },
                { "byte", "100" },
                { "char", "|" },
                { "datetime", "12/12/2000 12:12:12" },
                { "datetimeoffset", "12/12/2000 12:12:12 -5" },
                { "decimal", "1.1" },
                { "double", "1.2" },
                { "float", "1.3" },
                { "guid", "0EFCB2D7-72F6-4119-95D7-F27FA3EFF9FE" },
                { "int", "200" },
                { "long", "300" },
                { "single", "400" },
                { "timespan", "0.00:00:10" },
                { "uint", "500" },
                { "ulong", "600" },
            };
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(testData);
            Configuration = builder.Build();
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.SafeCopy{TObj, TProp}(IConfiguration, TObj, System.Linq.Expressions.Expression{Func{TObj, TProp}}, string, bool)"/>
        /// method safely copies a value from a location in the configuration to
        /// a location in a class, as specified by the delegate and the key
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_SafeCopy()
        {
            // Arrange ...
            var testType = new TestType();

            // Act ...
            Configuration.SafeCopy(testType, x => x.A, "A");

            // Assert ...
            Assert.IsTrue(testType.A == "1");
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.SafeCopy{TObj, TProp}(IConfiguration, TObj, System.Linq.Expressions.Expression{Func{TObj, TProp}}, string, bool)"/>
        /// method fails to copy a value when the source is missing, or null.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_SafeCopy_Missing()
        {
            // Arrange ...
            var testType = new TestType()
            {
                A = "A"
            };

            // Act ...
            Configuration.SafeCopy(testType, x => x.A, "notthere");

            // Assert ...
            Assert.IsTrue(testType.A == "A");
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsList{T}(IConfiguration, string, out IEnumerable{T})"/>
        /// method copies a list of values from the configuration as specified 
        /// by the key used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsList()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsList("list", out IEnumerable<string> value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value.Count() == 3);
            Assert.IsTrue(value.ElementAt(0) == "A");
            Assert.IsTrue(value.ElementAt(1) == "B");
            Assert.IsTrue(value.ElementAt(2) == "C");
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAs{T}(IConfiguration, string, out T)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAs()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAs("A", out string value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == "1");
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAs{T}(IConfiguration, string, T)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAs()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAs("A", "2");

            // Assert ...
            Assert.IsTrue(value == "1");
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAs{T}(IConfiguration, string, T)"/>
        /// method returns the default value when the key is missing or can't
        /// be parsed, or converted to the target type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAs_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAs("notthere", "2");

            // Assert ...
            Assert.IsTrue(value == "2");
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsBoolean(IConfiguration, string, out bool)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsBoolean()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsBoolean("bool", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == true);
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsByte(IConfiguration, string, out byte)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsByte()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsByte("byte", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == 100);
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsChar(IConfiguration, string, out char)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsChar()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsChar("char", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == '|');
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsDateTime(IConfiguration, string, out DateTime)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsDateTime()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsDateTime("datetime", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == DateTime.Parse("12/12/2000 12:12:12"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsDateTimeOffset(IConfiguration, string, out DateTimeOffset)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsDateTimeOffset()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsDateTime("datetimeoffset", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == DateTimeOffset.Parse("12/12/2000 12:12:12 -5"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsDecimal(IConfiguration, string, out decimal)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsDecimal()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsDecimal("decimal", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == decimal.Parse("1.1"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsDouble(IConfiguration, string, out double)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsDouble()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsDouble("double", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == double.Parse("1.2"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsFloat(IConfiguration, string, out float)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsFloat()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsFloat("float", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == float.Parse("1.3"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsGuid(IConfiguration, string, out Guid)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsGuid()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsGuid("guid", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == Guid.Parse("0EFCB2D7-72F6-4119-95D7-F27FA3EFF9FE"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsInt(IConfiguration, string, out int)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsInt()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsInt("int", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == int.Parse("200"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsLong(IConfiguration, string, out long)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsLong()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsLong("long", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == long.Parse("300"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsTimeSpan(IConfiguration, string, out TimeSpan)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsTimeSpan()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsTimeSpan("timespan", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == TimeSpan.Parse("0.00:00:10"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsUInt(IConfiguration, string, out uint)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsUInt()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsUInt("uint", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == uint.Parse("500"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.TryGetAsULong(IConfiguration, string, out ulong)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_TryGetAsULong()
        {
            // Arrange ...

            // Act ...
            var result = Configuration.TryGetAsULong("ulong", out var value);

            // Assert ...
            Assert.IsTrue(result == true);
            Assert.IsTrue(value == ulong.Parse("600"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsBoolean(IConfiguration, string, bool)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsBoolean()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsBoolean("bool", false);

            // Assert ...
            Assert.IsTrue(value == true);
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsBoolean(IConfiguration, string, bool)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsBoolean_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsBoolean("notthere", false);

            // Assert ...
            Assert.IsTrue(value == false);
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsByte(IConfiguration, string, byte)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsByte()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsByte("byte", 200);

            // Assert ...
            Assert.IsTrue(value == 100);
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsByte(IConfiguration, string, byte)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsByte_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsByte("notthere", 200);

            // Assert ...
            Assert.IsTrue(value == 200);
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsDateTime(IConfiguration, string, DateTime)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsDateTime()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsDateTime("datetime", DateTime.Parse("12/12/1930 12:12:12"));

            // Assert ...
            Assert.IsTrue(value == DateTime.Parse("12/12/2000 12:12:12"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsDateTime(IConfiguration, string, DateTime)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsDateTime_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsByte("notthere", 200);

            // Assert ...
            Assert.IsTrue(value == 200);
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsDateTimeOffset(IConfiguration, string, DateTimeOffset)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsDateTimeOffset()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsDateTimeOffset("datetimeoffset", DateTimeOffset.Parse("12/12/1900 12:12:12 -5"));

            // Assert ...
            Assert.IsTrue(value == DateTimeOffset.Parse("12/12/2000 12:12:12 -5"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsDateTimeOffset(IConfiguration, string, DateTimeOffset)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsDateTimeOffset_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsDateTimeOffset("notthere", DateTimeOffset.Parse("12/12/1900 12:12:12 -5"));

            // Assert ...
            Assert.IsTrue(value == DateTimeOffset.Parse("12/12/1900 12:12:12 -5"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsDecimal(IConfiguration, string, decimal)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsDecimal()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsDecimal("decimal", decimal.Parse("3.3"));

            // Assert ...
            Assert.IsTrue(value == decimal.Parse("1.1"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsDecimal(IConfiguration, string, decimal)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsDecimal_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsDecimal("notthere", decimal.Parse("3.3"));

            // Assert ...
            Assert.IsTrue(value == decimal.Parse("3.3"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsGuid(IConfiguration, string, Guid)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsGuid()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsGuid("guid", Guid.Parse("9E6880CB-C463-44E0-B567-F7FAA50EC36C"));

            // Assert ...
            Assert.IsTrue(value == Guid.Parse("0EFCB2D7-72F6-4119-95D7-F27FA3EFF9FE"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsGuid(IConfiguration, string, Guid)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsGuid_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsGuid("notthere", Guid.Parse("9E6880CB-C463-44E0-B567-F7FAA50EC36C"));

            // Assert ...
            Assert.IsTrue(value == Guid.Parse("9E6880CB-C463-44E0-B567-F7FAA50EC36C"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsInt(IConfiguration, string, int)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsInt()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsInt("int", int.Parse("300"));

            // Assert ...
            Assert.IsTrue(value == int.Parse("200"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsInt(IConfiguration, string, int)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsInt_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsInt("notthere", int.Parse("300"));

            // Assert ...
            Assert.IsTrue(value == int.Parse("300"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsLong(IConfiguration, string, long)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsLong()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsLong("long", long.Parse("400"));

            // Assert ...
            Assert.IsTrue(value == long.Parse("300"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsLong(IConfiguration, string, long)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsLong_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsLong("notthere", long.Parse("400"));

            // Assert ...
            Assert.IsTrue(value == long.Parse("400"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsTimeSpan(IConfiguration, string, TimeSpan)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsTimeSpan()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsTimeSpan("timespan", TimeSpan.Parse("0.00:00:20"));

            // Assert ...
            Assert.IsTrue(value == TimeSpan.Parse("0.00:00:10"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsTimeSpan(IConfiguration, string, TimeSpan)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsTimeSpan_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsTimeSpan("notthere", TimeSpan.Parse("0.00:00:20"));

            // Assert ...
            Assert.IsTrue(value == TimeSpan.Parse("0.00:00:20"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsUInt(IConfiguration, string, uint)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsUInt()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsUInt("uint", uint.Parse("600"));

            // Assert ...
            Assert.IsTrue(value == uint.Parse("500"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsUInt(IConfiguration, string, uint)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsUInt_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsUInt("notthere", uint.Parse("600"));

            // Assert ...
            Assert.IsTrue(value == uint.Parse("600"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsULong(IConfiguration, string, ulong)"/>
        /// method copies a value from the configuration as specified by the key 
        /// used in the operation.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsULong()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsULong("ulong", ulong.Parse("700"));

            // Assert ...
            Assert.IsTrue(value == ulong.Parse("600"));
        }

        // *******************************************************************

        /// <summary>
        /// This method verifies that the <see cref="ConfigurationExtensions.GetAsULong(IConfiguration, string, ulong)"/>
        /// method copies a default value if the key is missing, or can't be 
        /// parsed, or can't be converted to the desired type.
        /// </summary>
        [TestMethod]
        public void ConfigurationExtensions_GetAsULong_Default()
        {
            // Arrange ...

            // Act ...
            var value = Configuration.GetAsULong("notthere", ulong.Parse("700"));

            // Assert ...
            Assert.IsTrue(value == ulong.Parse("700"));
        }

        #endregion
    }
}
