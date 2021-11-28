﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMultiplayer.Shared.Networking.Datagrams
{
    public enum DatagramType
    {
        // For default handlers.
        Handshake,
        Disconnect,
        UnreliableKeepAlive,

        // Everything else below.
        RequestJoin,
        PlayerJoin,
        MoveTransform,
        TransformsUpdate,
        WorldState,
        MovePlayer,
        PlayerShoot,
        PlayersUpdate,
        LinkNameToID,
        PlayerHealthChange,
        PlayerDeath,
        PlayerSpawn,
    }
}
