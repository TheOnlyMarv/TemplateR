using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateR.Core.Config.DataConverter
{
    public interface IEnumerationDataPicker
    {
        Type GetDataType();

        object GetObjectInCollection(object collection, int index);
    }
}
