using Assets.Scripts.Shared.Datagrams.Handling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityMultiplayer.Shared.Networking.Datagrams.Handling
{
    public abstract class BaseBehaviourDatagramHandler : MonoBehaviour, IDatagramHandler
    {                
        public abstract void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel);
    }
}
