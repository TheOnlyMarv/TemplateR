using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using TemplateR.Core.Config.DataConverter;

namespace TemplateR.Core.Config
{
    public sealed class Configuration
    {
        public int MaxObjectDepth { get; set; }
        public IFormatProvider FormatProvider { get; set; }

        private IDictionary<string, IDataConverter> converters;
        private IDictionary<string, IEnumerationDataPicker> enumerationDataPickers;
        private DefaultConverter defaultConverter;
        private DefaultEnumerationDataPicker defaultEnumerationDataPicker;

        public Configuration()
        {
            defaultConverter = new DefaultConverter();
            defaultEnumerationDataPicker = new DefaultEnumerationDataPicker();
            converters = new Dictionary<string, IDataConverter>();
            enumerationDataPickers = new Dictionary<string, IEnumerationDataPicker>();
            MaxObjectDepth = int.MaxValue;
            FormatProvider = CultureInfo.CurrentCulture;
        }

        public Configuration RegisterConverter(IDataConverter dataConverter)
        {
            string typeString = dataConverter.GetDataType().FullName;
            converters[typeString] = dataConverter;
            return this;
        }

        public Configuration UnRegisterConverter(IDataConverter dataConverter)
        {
            string typeString = dataConverter.GetDataType().FullName;
            if (converters.ContainsKey(typeString))
            {
                converters.Remove(typeString);
            }
            return this;
        }

        public Configuration RegisterEnumerationDataPicker(IEnumerationDataPicker enumerationDataPicker)
        {
            string typeString = enumerationDataPicker.GetDataType().FullName;
            enumerationDataPickers[typeString] = enumerationDataPicker;
            return this;
        }

        public Configuration UnRegisterEnumerationDataPicker(IEnumerationDataPicker enumerationDataPicker)
        {
            string typeString = enumerationDataPicker.GetDataType().FullName;
            if (enumerationDataPickers.ContainsKey(typeString))
            {
                enumerationDataPickers.Remove(typeString);
            }
            return this;
        }

        internal IDataConverter GetDataConverter(Type type)
        {
            IDataConverter dataConverter = defaultConverter;
            if (converters.ContainsKey(type.FullName))
            {
                dataConverter = converters[type.FullName];
            }
            else
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                if (underlyingType != null && converters.ContainsKey(underlyingType.FullName))
                {
                    dataConverter = converters[underlyingType.FullName];
                }
            }
            return dataConverter;
        }

        internal IEnumerationDataPicker GetDataPicker(Type type)
        {
            IEnumerationDataPicker enumerationDataPicker = defaultEnumerationDataPicker;
            if (enumerationDataPickers.ContainsKey(type.FullName))
            {
                enumerationDataPicker = enumerationDataPickers[type.FullName];
            }
            return enumerationDataPicker;
        }
    }
}
