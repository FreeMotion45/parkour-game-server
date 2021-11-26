using Assets.Scripts.Shared.Datagrams.Handling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityMultiplayer.Shared.Networking.Datagrams.Handling
{    
    [Serializable]
    public class DatagramHandler
    {
        public DatagramType type;
        public BaseBehaviourDatagramHandler handler;
    }

    public class DatagramHandlerResolver : MonoBehaviour
    {
        public DatagramHandler[] handlers;

        private Dictionary<DatagramType, IDatagramHandler> _typeHandlingMap = new Dictionary<DatagramType, IDatagramHandler>();

        private void Start()
        {
            HashSet<DatagramType> forbiddenByDefault = new HashSet<DatagramType>
            {
                DatagramType.Handshake, DatagramType.Disconnect, DatagramType.UnreliableKeepAlive,
            };

            foreach (DatagramHandler handler in handlers)
            {
                if (forbiddenByDefault.Contains(handler.type))
                {
                    throw new ArgumentException($"Datagram handler of type {handler.type} is forbidden by default.");
                }

                if (handler.handler == null)
                {
                    throw new ArgumentNullException($"Handler of type {handler.type} can't null.");
                }

                _typeHandlingMap[handler.type] = handler.handler;
            }
        }

        public void AddHandler(DatagramType type, IDatagramHandler handler)
        {
            _typeHandlingMap[type] = handler;
        }

        public void RemoveHandler(DatagramType datagramType)
        {
            _typeHandlingMap.Remove(datagramType);
        }

        public IDatagramHandler Resolve(DatagramType type)
        {
            if (!_typeHandlingMap.ContainsKey(type))
            {
                Debug.LogError("Received datagram of type " + type + " which is not mapped in the resolver. Ignoring it.");
            }
            return _typeHandlingMap[type];
        }
    }
}
