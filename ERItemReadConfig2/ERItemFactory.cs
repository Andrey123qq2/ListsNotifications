using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ListsNotifications
{
    class ERItemFactory
    {
		public static ERItemNotifications Create(SPItemEventProperties properties)
		{
			object[] ERItemParams = { properties };

			Type ERItemType = GetERItemType(properties);

			return (ERItemNotifications)Activator.CreateInstance(ERItemType, ERItemParams);
		}

		private static Type GetERItemType(SPItemEventProperties properties)
		{
			Type ERItemType;
			string ERItemTypeName;

			ERItemTypeName = "ERItemNotifications" + properties.EventType.ToString();

			string assemblyName = "ListsNotifications";

			ERItemType = Type.GetType(assemblyName + "." + ERItemTypeName);

			//if (ERItemType == null)
			//{
			//	ERItemTypeName = Type.GetType(assemblyName + ".ERItemTypeCommon");
			//}

			return ERItemType;
		}
	}
}
