using System;
using System.Collections;

namespace TemplateR.Core.Config.DataConverter
{
    class DefaultEnumerationDataPicker : IEnumerationDataPicker
    {
        public Type GetDataType()
        {
            return typeof(IEnumerable);
        }

        public object GetObjectInCollection(object collection, int index)
        {
            object pickedObject = null;
            bool found = false;
            if (collection is IEnumerable enumerable)
            {
                int counter = 0;
                foreach (var item in enumerable)
                {
                    if (counter++ == index)
                    {
                        pickedObject = item;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new IndexOutOfRangeException($"The index '{index}' is out of enumeration range. Max index{counter - 1}");
                }
            }
            return pickedObject;
        }
    }
}
