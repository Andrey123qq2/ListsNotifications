using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.CSharp.RuntimeBinder;

namespace ListsNotifications
{
    public class ERItemConfigMethods
    {
        public void SetAttribute(SPList List, out string ConfAttribute, string ListPropertyName)
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

        public void SetAttribute(SPList List, out List<string> ConfAttribute, List<string> Fields, string ListPropertyName)
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

        public void SetAttribute(SPList List, out Dictionary<string, string> ConfAttribute, string ListPropertyName)
        {
            ConfAttribute = new Dictionary<string, string>();

            Hashtable ListRootFolderProperties = List.RootFolder.Properties;
            if (ListRootFolderProperties.ContainsKey(ListPropertyName))
            {
                string listPropertyValue = ListRootFolderProperties[ListPropertyName].ToString();
                if (listPropertyValue != "")
                {
                    string[] propertyValues = listPropertyValue.Split(',');
                    foreach (string value in propertyValues)
                    {
                        string[] valueParts = value.Split('=');
                        string valueParts1 = (valueParts.Length == 2) ? valueParts[1] : "";
                        ConfAttribute.Add(valueParts[0], valueParts1);
                    }
                }
            }
        }
    }
}
