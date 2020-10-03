using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections;

namespace ListsNotifications
{
    static class ERConfFactory<T>
    {
        public static T Create(string RootFolderPropertyName, SPList List)
        {
            string RootFolderPropertyValue;
            T ERConfByType;

            Hashtable ListRootFolderProperties = List.RootFolder.Properties;

            if (!ListRootFolderProperties.ContainsKey(RootFolderPropertyName))
            {
                Type ERType = typeof(T);
                ERConfByType = (T)Activator.CreateInstance(ERType);
            }
            else
            {
                RootFolderPropertyValue = ListRootFolderProperties[RootFolderPropertyName].ToString();
                ERConfByType = JsonSerializer.Deserialize<T>(RootFolderPropertyValue);
            }
            
            return ERConfByType;
        }

    }
}
