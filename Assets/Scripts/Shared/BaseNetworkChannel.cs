﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityMultiplayer.Shared.Networking.Datagrams;

namespace Assets.Scripts.Shared
{
    public abstract class BaseNetworkChannel
    {
        public abstract void Send(DatagramHolder data, TransportType transportType = TransportType.Reliable);

        public void Send(object data, DatagramType datagramType, TransportType transportType = TransportType.Reliable)
        {
            Send(new DatagramHolder(datagramType, data), transportType);
        }
    }
}