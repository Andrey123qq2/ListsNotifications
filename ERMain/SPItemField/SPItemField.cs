using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using SPSCommon.ERItem;
using SPSCommon.SPCustomExtensions;

namespace ListsNotifications
{
    /// <summary>
    /// Creates object for particular item field with useful params
    /// </summary>
    abstract class SPItemField
    {
        public readonly string FieldTitle;
        public readonly bool IsChanged;
        public string FriendlyFieldValueBefore;
        public string FriendlyFieldValueAfter;
        public dynamic FieldValueAfter;
        public dynamic FieldValueBefore;

        protected IERItem Item;
        protected readonly bool ValueAfter;

        protected string FieldValueAfterToStringForCompare;
        protected string FieldValueBeforeToStringForCompare;
        protected string FieldValueAfterToStringForFriendly;
        protected string FieldValueBeforeToStringForFriendly;
        
        public SPItemField(params object[] attributes)
        {
            Item = (IERItem)attributes[0];
            FieldTitle = (string)attributes[1];
            ValueAfter = (bool)attributes[2];

            FieldValueAfter = ValueAfter ? Item.GetFieldValueAfter(FieldTitle) : null;
            FieldValueBefore = Item.listItem.GetFieldValue(FieldTitle);

            GetFieldValuesToStringForCompare();

            IsChanged = FieldIsChanged();

            if (!IsChanged)
                return;

            GetFieldValuesToStringForFriendly();
            
            if (FriendlyFieldValueAfter != "-")
                GetFriendlyFieldValues(FieldValueAfterToStringForFriendly, out FriendlyFieldValueAfter);
            if (FriendlyFieldValueBefore != "-")
                GetFriendlyFieldValues(FieldValueBeforeToStringForFriendly, out FriendlyFieldValueBefore);
            if (Item.eventType.Contains("Added"))
                FriendlyFieldValueAfter = FriendlyFieldValueBefore;
        }

        abstract public void GetFieldValuesToStringForCompare();
        abstract public void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue);

        private bool FieldIsChanged()
        {
            if (Item.eventType.Contains("Added") && FieldValueBeforeToStringForCompare != null && FieldValueBeforeToStringForCompare != "")
                return true;

            if (FieldValueAfterToStringForCompare != FieldValueBeforeToStringForCompare)
                return true;
            else
                return false;
        }

        private void GetFieldValuesToStringForFriendly()
        {
            try
            {
                FieldValueAfterToStringForFriendly = (FieldValueAfter != null) ? (string)FieldValueAfter : "";
            }
            catch
            {
                FieldValueAfterToStringForFriendly = (FieldValueAfter != null) ? FieldValueAfter.ToString() : "";
            }


            if (FieldValueAfterToStringForFriendly == "" || FieldValueAfterToStringForFriendly == null)
                FriendlyFieldValueAfter = "-";


            //if (!valueAfter)
            //{
            //    return;
            //}

            try
            {
                FieldValueBeforeToStringForFriendly = (FieldValueBefore != null) ? (string)FieldValueBefore : "";
            }
            catch
            {
                FieldValueBeforeToStringForFriendly = (FieldValueBefore != null) ? FieldValueBefore.ToString() : "";
            }


            if (FieldValueBeforeToStringForFriendly == "" || FieldValueBeforeToStringForFriendly == null)
                FriendlyFieldValueBefore = "-";
        }
    }
}
