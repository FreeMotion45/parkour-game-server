using Assets.Scripts.Network.Messages.ServerOrigin.PickUp;
using Assets.Scripts.Network.Messages.ServerOrigin.PickUp.PickUpData;
using Assets.Scripts.ServerLogic;
using Assets.Scripts.ServerLogic.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams;

class AmmoMagazinePickUp : BasePickUp
{
    public int magazinesToRecover = 2;

    public override void OnPickUp(GameObject player)
    {
        player.GetComponent<Gun>().gunLogic.MagazinesAvailable += magazinesToRecover;

        AmmoMagazinePickUpData data = new AmmoMagazinePickUpData(magazinesToRecover);
        PickUpPickedUpMessage message = new PickUpPickedUpMessage(GamePlayers.GetChannelID(player), PickUpID, data);
        GamePlayers.Publish(message, DatagramType.PickUpPickedUp);

        Destroy(gameObject);
    }
}
