using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Shared.Networking;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.Shared.Datagrams.Handling
{
    public interface IDatagramHandler
    {
        void Handle(DatagramHolder deserializedDatagram, NetworkChannel networkChannel);
    }
}
