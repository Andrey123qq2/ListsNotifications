﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsNotifications
{
    class SPItemFieldTypeCommon : SPItemField
    {
        public SPItemFieldTypeCommon(params object[] attributes) : base(attributes)
        {
        }
        public override void GetFieldValuesToStringForCompare()
        {
            fieldValueBeforeToStringForCompare = (fieldValueBefore != null) ? (string)fieldValueBefore : "";
            fieldValueAfterToStringForCompare = (fieldValueAfter != null) ? (string)fieldValueAfter : "";
        }

        public override void GetFriendlyFieldValues(string fieldValueString, out string friendlyFieldValue)
        {
            friendlyFieldValue = fieldValueString;
        }
    }
}
