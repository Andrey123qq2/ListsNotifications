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
		public static ERItem Create(SPItemEventProperties properties)
		{
			object[] ERItemParams = { properties };

			Type ERItemType = GetERItemType(properties);

			return (ERItem)Activator.CreateInstance(ERItemType, ERItemParams);
		}

		public static ERItem Create(SPList list)
		{
			object[] ERItemParams = { list };

			Type ERItemType = GetERItemType();

			return (ERItem)Activator.CreateInstance(ERItemType, ERItemParams);
		}



		private static Type GetERItemType(SPItemEventProperties properties)
		{
			Type ERItemType;
			string ERItemTypeName;

			ERItemTypeName = "ERType" + properties.EventType.ToString();

			string assemblyName = "ListsNotifications";

			ERItemType = Type.GetType(assemblyName + "." + ERItemTypeName);

			//if (ERItemType == null)
			//{
			//	ERItemTypeName = Type.GetType(assemblyName + ".ERItemTypeCommon");
			//}

			return ERItemType;
		}

		private static Type GetERItemType()
		{
			Type ERItemType;
			string ERItemTypeName;

			ERItemTypeName = "ERTypeItemPageSettings";

			string assemblyName = "ListsNotifications";

			ERItemType = Type.GetType(assemblyName + "." + ERItemTypeName);

			return ERItemType;
		}
	}
}
