using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TemplateR.Core.Config.DataConverter
{
    class DefaultConverter : IDataConverter
    {
        public string Convert(object data, string formatString, IFormatProvider formatProvider)
        {
            if (data is IFormattable formattable)
                return formattable.ToString(formatString, formatProvider);
            else
                return data.ToString();
        }

        public Type GetDataType()
        {
            return typeof(object);
        }
    }
}
