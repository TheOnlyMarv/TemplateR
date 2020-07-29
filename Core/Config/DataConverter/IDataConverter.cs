using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateR.Core.Config.DataConverter
{
    public interface IDataConverter
    {
        Type GetDataType();
        string Convert(object data, string formatString, IFormatProvider formatProvider);
    }
}
