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
using System.Linq;

namespace ListsNotifications
{
    //public class ERItemConfigMethods
    //{
    //    public void SetAttribute(SPList List, out string ConfAttribute, string ListPropertyName)
    //    {
    //        ConfAttribute = "";

    //        Hashtable ListRootFolderProperties = List.RootFolder.Properties;
    //        if (ListRootFolderProperties.ContainsKey(ListPropertyName))
    //        {
    //            string listPropertyValue = ListRootFolderProperties[ListPropertyName].ToString();
    //            if (listPropertyValue != "")
    //            {
    //                ConfAttribute = listPropertyValue;
    //            }
    //        }
    //    }

    //    public void SetAttribute(SPList List, out List<string> ConfAttribute, string ListPropertyName, bool checkIsSPField = false)
    //    {
    //        ConfAttribute = GetListRootFolderPropertyToList(List, ListPropertyName);
    //        ConfAttribute = (checkIsSPField) ? FilterExistsSPFields(List, ConfAttribute) : ConfAttribute;
    //    }

    //    public void SetAttribute(SPList List, out Dictionary<string, string> ConfAttribute, string ListPropertyName, bool checkIsSPField = false)
    //    {
    //        ConfAttribute = GetListRootFolderPropertyToHash(List, ListPropertyName);
    //        ConfAttribute = (checkIsSPField) ? FilterExistsSPFields(List, ConfAttribute) : ConfAttribute;
    //    }


    //    private List<string> GetListRootFolderPropertyToList(SPList List, string ListPropertyName)
    //    {
    //        List<string> FolderProperty = new List<string>();

    //        Hashtable ListRootFolderProperties = List.RootFolder.Properties;
    //        if (ListRootFolderProperties.ContainsKey(ListPropertyName))
    //        {
    //            string listPropertyValue = ListRootFolderProperties[ListPropertyName].ToString();
    //            if (listPropertyValue != "")
    //            {
    //                FolderProperty.AddRange(listPropertyValue.Split(','));
    //            }
    //        }

    //        return FolderProperty;
    //    }

    //    private Dictionary<string, string> GetListRootFolderPropertyToHash(SPList List, string ListPropertyName)
    //    {
    //        Dictionary<string, string> FolderProperty = new Dictionary<string, string>();

    //        Hashtable ListRootFolderProperties = List.RootFolder.Properties;
    //        if (ListRootFolderProperties.ContainsKey(ListPropertyName))
    //        {
    //            string listPropertyValue = ListRootFolderProperties[ListPropertyName].ToString();
    //            if (listPropertyValue != "")
    //            {
    //                string[] propertyValues = listPropertyValue.Split(',');
    //                foreach (string value in propertyValues)
    //                {
    //                    string[] valueParts = value.Split('=');
    //                    string valueParts1 = (valueParts.Length == 2) ? valueParts[1] : "";
    //                    FolderProperty.Add(valueParts[0], valueParts1);
    //                }
    //            }
    //        }

    //        return FolderProperty;
    //    }

    //    private Dictionary<string, string> FilterExistsSPFields(SPList List, Dictionary<string, string> ConfAttribute)
    //    {
    //        return ConfAttribute.Where(f => List.Fields.ContainsField(f.Key)).ToDictionary(p => p.Key, p => p.Value);
    //    }

    //    private List<string> FilterExistsSPFields(SPList List, List<string> ConfAttribute)
    //    {
    //        return ConfAttribute.Where(f => List.Fields.ContainsField(f)).ToList();
    //    }
    //}
}
