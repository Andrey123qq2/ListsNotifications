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
            switch (item.listItem.ParentList.Fields.GetField(fieldTitle).FieldValueType.Name)
            {
                case "DateTime":
                    return new SPItemFieldTypeDateTime(item, fieldTitle, valueAfterParam);
                case "Double":
                    return new SPItemFieldTypeDouble(item, fieldTitle, valueAfterParam);
                case "SPFieldUserValueCollection":
                    return new SPItemFieldTypeSPFieldUserValueCollection(item, fieldTitle, valueAfterParam);
                case "SPFieldUserValue":
                    return new SPItemFieldTypeSPFieldUserValue(item, fieldTitle, valueAfterParam);
                case "SPFieldLookupValueCollection":
                    return new SPItemFieldTypeSPFieldLookupValueCollection(item, fieldTitle, valueAfterParam);
                default:
                    return new SPItemFieldTypeCommon(item, fieldTitle, valueAfterParam);
            }
        }
    }
}
