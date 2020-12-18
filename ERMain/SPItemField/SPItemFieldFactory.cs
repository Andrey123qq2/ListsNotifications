using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPSCommon.ERItem;

namespace ListsNotifications
{
    class SPItemFieldFactory
    {
        public static SPItemField create(IERItem item, string fieldTitle, bool valueAfterParam = true)
        {
            object[] SPItemFieldParams = { item, fieldTitle, valueAfterParam };

            Type SPItemFieldType = GetSPItemFieldType(item, fieldTitle);

            return (SPItemField)Activator.CreateInstance(SPItemFieldType, SPItemFieldParams);
        }

        private static Type GetSPItemFieldType(IERItem item, string fieldTitle)
        {
            Type SPItemFieldType;
            string SPItemFieldTypeName;

            try
            {
                SPItemFieldTypeName = "SPItemFieldType" + item.listItem.ParentList.Fields.GetField(fieldTitle).FieldValueType.Name;
            }
            catch {
                SPItemFieldTypeName = "SPItemFieldType" + item.listItem.ParentList.Fields.GetField(fieldTitle).FieldTypeDefinition.TypeName;
                SPItemFieldTypeName = SPItemFieldTypeName.Replace("File", "String");
            }
            string assemblyName = "ListsNotifications";

            SPItemFieldType = Type.GetType(assemblyName + "." + SPItemFieldTypeName);

            if (SPItemFieldType == null)
            {
                SPItemFieldType = Type.GetType(assemblyName + ".SPItemFieldTypeCommon");
            }

            return SPItemFieldType;
        }
    }
}