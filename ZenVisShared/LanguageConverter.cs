using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace ZenVis.Shared
{
    public class LanguageConverter : TypeConverter
    {
        public LanguageConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != null)
            {
                FieldInfo[] fields = typeof(Languages).GetFields();
                for (int i = 0; i < (int)fields.Length; i++)
                {
                    FieldInfo fieldInfo = fields[i];
                    Language language = (Language)fieldInfo.GetValue(null);
                    if (fieldInfo.Name == value.ToString() || language.Name == value.ToString())
                    {
                        return language;
                    }
                }
            }
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                return null;
            }
            return ((Language)value).Name;
        }
    }
}