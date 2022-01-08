using Assets.Scripts.Network.Messages.ServerOrigin.PickUp;
using Assets.Scripts.Network.Messages.ServerOrigin.PickUp.PickUpData;
using Assets.Scripts.ServerLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams;

public class HealthPickUp : BasePickUp
{
    public int healthRecovery = 25;

    public HealthPickUp()
    {
        pickUpType = Assets.Scripts.Game.Shared.PickUpType.Health;
    }

    public override void OnPickUp(GameObject player)
    {
        player.GetComponent<PlayerStats>().Heal(healthRecovery);

        HealthPickUpData data = new HealthPickUpData(healthRecovery);
        PickUpPickedUpMessage message = new PickUpPickedUpMessage(GamePlayers.GetChannelID(player), PickUpID, data);
        GamePlayers.Publish(message, DatagramType.PickUpPickedUp);

        Destroy(gameObject);
    }
}
