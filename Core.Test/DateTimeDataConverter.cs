using System;
using TemplateR.Core.Config.DataConverter;

namespace TemplateR.Core.Test
{
    public class DateTimeDataConverter : IDataConverter
    {
        public string Convert(object data, string formatString, IFormatProvider formatProvider)
        {
            string result = string.Empty;
            if (data is DateTime dt)
            {
                result = dt.ToString("yyyy");
            }
            return result;
        }

        public Type GetDataType()
        {
            return typeof(DateTime);
        }
    }
}
