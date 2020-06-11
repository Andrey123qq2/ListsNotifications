using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.CSharp.RuntimeBinder;

namespace ListsNotifications
{
    class Config
    {
        public void SetStringAttribute(SPList List, out string ConfAttribute, string ListPropertyName)
        {
            ConfAttribute = "";

            Hashtable ListRootFolderProperties = List.RootFolder.Properties;
            if (ListRootFolderProperties.ContainsKey(ListPropertyName))
            {
                string listPropertyValue = ListRootFolderProperties[ListPropertyName].ToString();
                if (listPropertyValue != "")
                {
                    ConfAttribute = listPropertyValue;
                }
            }
        }

        public void SetListAttribute(SPList List, out List<string> ConfAttribute, List<string> Fields, string ListPropertyName)
        {
            ConfAttribute = new List<string>();
            ConfAttribute.AddRange(Fields);

            Hashtable ListRootFolderProperties = List.RootFolder.Properties;
            if (ListRootFolderProperties.ContainsKey(ListPropertyName))
            {
                string listPropertyValue = ListRootFolderProperties[ListPropertyName].ToString();
                if (listPropertyValue != "")
                {
                    ConfAttribute.AddRange(listPropertyValue.Split(','));
                }
            }
        }
    }
}
