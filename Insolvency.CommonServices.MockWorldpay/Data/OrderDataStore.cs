using System.Collections.Generic;
using System.Linq;

namespace Insolvency.CommonServices.MockWorldpay.Data
{
    public static class OrderDataStore
    {
        private static readonly Dictionary<string, OrderData> _dataStore;

        static OrderDataStore()
        {
            _dataStore = new Dictionary<string, OrderData>();
        }

        public static void Add(string orderKey, OrderData data)
        {
            if (_dataStore.ContainsKey(orderKey))
            {
                _dataStore[orderKey] = data;
            }
            else
            {
                _dataStore.Add(orderKey, data);
            }
        }

        public static OrderData Get(string orderKey)
        {
            if (_dataStore.ContainsKey(orderKey))
            {
                return _dataStore[orderKey];
            }

            return null;
        }

        public static List<OrderData> GetAll()
        {
            return _dataStore.Values.ToList();
        }

    }
}