using Assets.Scripts.Network.Messages.ServerOrigin.PickUp;
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

        PickUpPickedUpMessage message = new PickUpPickedUpMessage(PlayerDatabase.GetChannelID(player), PickUpID);
        PlayerDatabase.Publish(message, DatagramType.PickUpTaken);

        Destroy(gameObject);
    }    
}
