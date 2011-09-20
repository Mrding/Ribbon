using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Luna.Core
{
    /// <summary>
    /// TypeConverter for enum types which supports the DisplayName-attribute
    /// </summary>
    public class EnumTypeConverter : EnumConverter
    {
        #region Private class

        private sealed class MappingContainer
        {
            public ICollection standardValues;
            public IDictionary<CultureInfo, MappingPerCulture> mappingsPerCulture;
        }

        private sealed class MappingPerCulture
        {
            public bool fieldDisplayNameFound;
            public IDictionary<object, string> mappings;
            public IDictionary<string, object> reverseMappings;
        }

        #endregion

        #region Members

        private static IDictionary<Type, MappingContainer> mappings;
        private static IDictionary<Type, ResourceManager> resourceManagers;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumTypeConverter"/> class.
        /// </summary>
        /// <param name="type">Eine <see cref="T:System.Type"></see>-Klasse, die den Typ der Enumeration darstellt, der diesem Enumerationskonverter zugeordnet werden soll.</param>
        public EnumTypeConverter(Type type)
            : base(type)
        {
        }

        #endregion

        #region Methods

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            //
            // we support converting between string type an enum
            //
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            //
            // we support converting between string type an enum
            //
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result = null;

            //
            // source value should be a string
            //
            if (value != null &&
                value is string)
            {
                // ensure that the mapping table is available for the enumeration type
                EnsureMappingsAvailable(context.PropertyDescriptor.PropertyType, culture);
                MappingContainer container = mappings[context.PropertyDescriptor.PropertyType];
                MappingPerCulture mapping = container.mappingsPerCulture[culture];
                string valueStr = value.ToString();

                if (mapping.fieldDisplayNameFound)
                {
                    if (valueStr.IndexOf(',') < 0)
                    {
                        // simple value
                        if (mapping.reverseMappings.ContainsKey(valueStr))
                        {
                            result = mapping.reverseMappings[valueStr];
                        }
                        else
                        {
                            throw GetConvertFromException(valueStr);
                        }
                    }
                    else
                    {
                        // concated values (Flags enumeration)
                        string[] valueParts = valueStr.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                        string tmp = valueParts[0];
                        int tmpResult;
                        if (mapping.reverseMappings.ContainsKey(tmp))
                        {
                            tmpResult = (int)mapping.reverseMappings[tmp];
                        }
                        else
                        {
                            throw GetConvertFromException(valueStr);
                        }
                        for (int index = 1; index < valueParts.Length; index++)
                        {
                            tmp = valueParts[index];
                            if (mapping.reverseMappings.ContainsKey(tmp))
                            {
                                tmpResult |= (int)mapping.reverseMappings[tmp];
                            }
                            else
                            {
                                throw GetConvertFromException(valueStr);
                            }
                        }
                        result = Enum.ToObject(context.PropertyDescriptor.PropertyType, tmpResult);
                    }
                }
                else
                {
                    result = Enum.Parse(context.PropertyDescriptor.PropertyType, valueStr);
                }
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = value;

            //
            // source value have to be a enumeration type or string, destination a string type
            //
            if (destinationType == typeof(string) &&
                value != null)
            {
                if (value.GetType().IsEnum)
                {
                    // ensure that the mapping table is available for the enumeration type
                    EnsureMappingsAvailable(value.GetType(), culture);
                    MappingContainer container = mappings[value.GetType()];
                    MappingPerCulture mapping = container.mappingsPerCulture[culture];
                    string valueStr = value.ToString();

                    if (mapping.fieldDisplayNameFound)
                    {
                        if (valueStr.IndexOf(',') < 0)
                        {
                            // simple enum value
                            if (mapping.mappings.ContainsKey(valueStr))
                            {
                                result = mapping.mappings[valueStr];
                            }
                            else
                            {
                                throw GetConvertToException(valueStr, destinationType);
                            }
                        }
                        else
                        {
                            // flag enum with more then one enum value
                            string[] parts = valueStr.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                            System.Text.StringBuilder builder = new System.Text.StringBuilder();
                            string tmp;
                            for (int index = 0; index < parts.Length; index++)
                            {
                                tmp = parts[index];
                                if (mapping.mappings.ContainsKey(tmp))
                                {
                                    builder.Append(mapping.mappings[tmp]);
                                    builder.Append(", ");
                                }
                                else
                                {
                                    throw GetConvertToException(valueStr, destinationType);
                                }
                            }

                            builder.Length -= 2;
                            result = builder.ToString();
                        }
                    }
                    else
                    {
                        result = value.ToString();
                    }
                }
            }
            else
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //
            // enumerations support standard values which are a list of the fields of the enumeration
            //
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // ensure that the mapping table is available for the enumeration type
            // it builds the standard value collection too
            EnsureMappingsAvailable(EnumType, CultureInfo.CurrentCulture);
            MappingContainer container = mappings[EnumType];
            // wrap it with the right type. it is also possible to use Enum.GetValues
            TypeConverter.StandardValuesCollection values = new StandardValuesCollection(container.standardValues);
            return values;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return base.IsValid(context, value);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        delegate string GetFieldName(string key);

        /// <summary>
        /// Build the mappings between the field values of the enumeration type and the display name for the field
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        private void EnsureMappingsAvailable(Type enumType, CultureInfo culture)
        {
            if (mappings == null)
                mappings = new Dictionary<Type, MappingContainer>();

            MappingContainer container;
            if (mappings.ContainsKey(enumType))
            {
                container = mappings[enumType];
                if (container.mappingsPerCulture.ContainsKey(culture))
                    return;
            }
            else
            {
                container = new MappingContainer();
                container.mappingsPerCulture = new Dictionary<CultureInfo, MappingPerCulture>();
            }

            MappingPerCulture mapping = new MappingPerCulture();
            IDictionary<object, string> enumTypeValueMapping = new Dictionary<object, string>();
            IDictionary<string, object> enumTypeValueReverseMapping = new Dictionary<string, object>();
            List<object> enumTypeStandardValues = new List<object>();

            ResourceManager man;
            GetFieldName getDisplayName;
            // look for a resource manager
            if (resourceManagers != null &&
                resourceManagers.ContainsKey(enumType))
            {
                man = resourceManagers[enumType];
                getDisplayName = delegate(string key) { string displayName = man.GetString(key, culture); if (displayName == null) return key; return displayName; };
            }
            else
            {
                getDisplayName = delegate(string key) { return key; };
            }

            // build the mapping (reflection of the enum type)
            string enumTypeFullNameForKey = enumType.FullName.Replace('.', '_');
            FieldInfo[] fields = enumType.GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType == enumType)
                {
                    object fieldValue = Enum.Parse(enumType, field.Name);
                    string key = enumTypeFullNameForKey + "_" + field.Name;
                    string displayName = getDisplayName(key);
                    if (String.Compare(key, displayName) == 0)
                    {
                        object[] attributes = field.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                        if (attributes != null &&
                            attributes.Length > 0)
                        {
                            DisplayNameAttribute attrib = (DisplayNameAttribute)(attributes[0]);
                            displayName = attrib.DisplayName;
                            mapping.fieldDisplayNameFound = true;
                        }
                        else
                        {
                            displayName = field.Name;
                        }
                    }
                    else
                    {
                        mapping.fieldDisplayNameFound = true;
                    }
                    if (!enumTypeValueMapping.ContainsKey(field.Name))
                        enumTypeValueMapping.Add(field.Name, displayName);
                    enumTypeValueReverseMapping.Add(displayName, fieldValue);
                    enumTypeStandardValues.Add(fieldValue);
                }
            }

            mapping.reverseMappings = enumTypeValueReverseMapping;
            if (mapping.fieldDisplayNameFound)
                mapping.mappings = enumTypeValueMapping;

            // add mapping for culture to the mapping container for the enum type
            lock (container.mappingsPerCulture)
            {
                if (!container.mappingsPerCulture.ContainsKey(culture))
                    container.mappingsPerCulture.Add(culture, mapping);
            }
            // if standardValues == null the container is the new
            if (container.standardValues == null)
            {
                // lock the mapping dictionary because it is static and should be thread safe
                lock (mappings)
                {
                    container.standardValues = enumTypeStandardValues;
                    // double check is necessary because of concurrent threads
                    if (!mappings.ContainsKey(enumType))
                    {
                        mappings.Add(enumType, container);
                    }
                    else
                    {
                        // a new container for this enum type is added before our new container
                        // but we have to check that a mapping for the current culture is available
                        container = mappings[enumType];
                        if (!container.mappingsPerCulture.ContainsKey(culture))
                        {
                            lock (container.mappingsPerCulture)
                            {
                                if (!container.mappingsPerCulture.ContainsKey(culture))
                                    container.mappingsPerCulture.Add(culture, mapping);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods - Static

        public static void RegisterResourceManager(Type enumType, ResourceManager resourceManager)
        {
            if (resourceManagers == null)
                resourceManagers = new Dictionary<Type, ResourceManager>();
            resourceManagers[enumType] = resourceManager;
            if (mappings != null)
            {
                if (mappings.ContainsKey(enumType))
                {
                    lock (mappings)
                    {
                        if (mappings.ContainsKey(enumType))
                        {
                            mappings.Remove(enumType);
                        }
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Attribute which can be used to provide a good name for a field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class FieldDisplayNameAttribute : DisplayNameAttribute
    {
        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDisplayNameAttribute"/> class.
        /// </summary>
        /// <param name="displayName">the name which should be displayed</param>
        public FieldDisplayNameAttribute(string displayName)
            : base(displayName)
        {
        }

        #endregion

        #region Methods

        #endregion
    }
}