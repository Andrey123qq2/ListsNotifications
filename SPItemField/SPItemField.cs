using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    abstract public class SPItemField
    {
        public readonly string fieldTitle;
        public readonly bool IsChanged;
        public string friendlyFieldValueBefore;
        public string friendlyFieldValueAfter;

        protected IERItem item;
        protected readonly bool valueAfter;

        protected dynamic fieldValueAfter;
        protected dynamic fieldValueBefore;

        protected string fieldValueAfterToStringForCompare;
        protected string fieldValueBeforeToStringForCompare;

        protected string fieldValueAfterToStringForFriendly;
        protected string fieldValueBeforeToStringForFriendly;


        
        public SPItemField(params object[] attributes)
        {
            item = (IERItem)attributes[0];
            fieldTitle = (string)attributes[1];
            valueAfter = (bool)attributes[2];

            fieldValueAfter = valueAfter ? item.GetFieldValue(fieldTitle, valueAfter) : null;
            fieldValueBefore = item.GetFieldValue(fieldTitle, false);

            GetFieldValuesToStringForCompare();
            IsChanged = FieldIsChanged();

            if (IsChanged == false)
            {
                return;
            }

            GetFieldValuesToStringForFriendly();
            
            if (friendlyFieldValueAfter != "-")
            {
                GetFriendlyFieldValues(fieldValueAfterToStringForFriendly, out friendlyFieldValueAfter);
            }
            if (friendlyFieldValueBefore != "-")
            {
                GetFriendlyFieldValues(fieldValueBeforeToStringForFriendly, out friendlyFieldValueBefore);
            }

            if (item.eventType.Contains("Added"))
            {
                friendlyFieldValueAfter = friendlyFieldValueBefore;
            }
        }

        abstract public void GetFieldValuesToStringForCompare();
        abstract public void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue);

        private bool FieldIsChanged()
        {
            if (item.eventType.Contains("Added") && fieldValueBeforeToStringForCompare != null && fieldValueBeforeToStringForCompare != "")
            {
                
                return true;
            }

            if (fieldValueAfterToStringForCompare != fieldValueBeforeToStringForCompare)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GetFieldValuesToStringForFriendly()
        {
            try
            {
                fieldValueAfterToStringForFriendly = (fieldValueAfter != null) ? (string)fieldValueAfter : "";
            }
            catch
            {
                fieldValueAfterToStringForFriendly = (fieldValueAfter != null) ? fieldValueAfter.ToString() : "";
            }


            if (fieldValueAfterToStringForFriendly == "" || fieldValueAfterToStringForFriendly == null)
            {
                friendlyFieldValueAfter = "-";
            }


            //if (!valueAfter)
            //{
            //    return;
            //}

            try
            {
                fieldValueBeforeToStringForFriendly = (fieldValueBefore != null) ? (string)fieldValueBefore : "";
            }
            catch
            {
                fieldValueBeforeToStringForFriendly = (fieldValueBefore != null) ? fieldValueBefore.ToString() : "";
            }


            if (fieldValueBeforeToStringForFriendly == "" || fieldValueBeforeToStringForFriendly == null)
            {
                friendlyFieldValueBefore = "-";
            }
        }
    }
}
