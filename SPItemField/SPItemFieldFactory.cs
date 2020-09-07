using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsNotifications
{
    class SPItemFieldFactory
    {
        public static SPItemField create(ERItem item, string fieldTitle, bool valueAfterParam = true)
        {
            object[] SPItemFieldParams = { item, fieldTitle, valueAfterParam };

            Type SPItemFieldType = GetSPItemFieldType(item, fieldTitle);

            if (SPItemFieldType != null)
            {
                return (SPItemField)Activator.CreateInstance(SPItemFieldType, SPItemFieldParams);
            }
            else
            {
                return new SPItemFieldTypeCommon(SPItemFieldParams);
            }
        }

        private static Type GetSPItemFieldType(ERItem item, string fieldTitle)
        {
            string SPItemFieldTypeName = "SPItemFieldType" + item.listItem.ParentList.Fields.GetField(fieldTitle).FieldValueType.Name;
            string assemblyName = "ListsNotifications";

            Type SPItemFieldType = Type.GetType(assemblyName + "." + SPItemFieldTypeName);

            return SPItemFieldType;
        }
    }
}