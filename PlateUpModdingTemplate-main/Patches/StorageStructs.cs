using KitchenData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
//This code was used from UrFriendKen https://github.com/UrFriendKen/PlateUpApplianceChest/tree/b2e9dab34f0424886240dadd0ed1db9724d7d5aa

namespace KitchenDeconstructor.Patches
{
    public class StorageStructs
    {
        public struct CStoredPlates : IApplianceProperty, IAttachableProperty, IComponentData
        {
            public int PlatesCount;
        }
        public struct CStoredTables : IApplianceProperty, IAttachableProperty, IComponentData
        {
            FixedListInt64 TableIDs;
            FixedListInt64 TableCounts;

            public CStoredTables()
            {
                TableIDs = new FixedListInt64();
                TableCounts = new FixedListInt64();
            }

            public int Add(int TableID, int count = 1)
            {
                if (!TableIDs.Contains(TableID))
                {
                    TableIDs.Add(TableID);
                    TableCounts.Add(0);
                }
                return TableCounts[TableIDs.IndexOf(TableID)] += count;
            }

            public int Remove(int TableID, int count = 1)
            {
                if (!TableIDs.Contains(TableID))
                {
                    return 0;
                }
                int index = TableIDs.IndexOf(TableID);
                int remaining = TableCounts[index] -= count;
                if (remaining == 0)
                {
                    TableIDs.RemoveAt(index);
                    TableCounts.RemoveAt(index);
                }
                return remaining;
            }

            public bool RemoveTable(int TableID)
            {
                if (!TableIDs.Contains(TableID))
                {
                    return false;
                }
                int index = TableIDs.IndexOf(TableID);
                TableIDs.RemoveAt(index);
                TableCounts.RemoveAt(index);
                return true;
            }

            public void Clear()
            {
                TableIDs.Clear();
                TableCounts.Clear();
            }

            public bool TryGet(int TableID, out int count)
            {
                return GetDictionary().TryGetValue(TableID, out count);
            }

            public Dictionary<int, int> GetDictionary()
            {
                Dictionary<int, int> dict = new Dictionary<int, int>();
                for (int i = 0; i < TableIDs.Length; i++)
                {
                    if (!dict.ContainsKey(TableIDs[i]))
                    {
                        dict.Add(TableIDs[i], 0);
                    }
                    dict[TableIDs[i]] += TableCounts[i];
                }
                return dict;
            }
        }
    }
}
