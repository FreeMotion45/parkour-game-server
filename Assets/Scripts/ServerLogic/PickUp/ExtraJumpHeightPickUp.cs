using Assets.Scripts.Network.Messages.ServerOrigin.PickUp;
using Assets.Scripts.Network.Messages.ServerOrigin.PickUp.PickUpData;
using Assets.Scripts.ServerLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams;

public class ExtraJumpHeightPickUp : BasePickUp
{
    public float extraJumpForce = 1.5f;

    public ExtraJumpHeightPickUp()
    {
        pickUpType = Assets.Scripts.Game.Shared.PickUpType.ExtraJumpHeight;
    }

    public override void OnPickUp(GameObject player)
    {
        ExtraJumpHeightPickUpData data = new ExtraJumpHeightPickUpData(extraJumpForce);
        PickUpPickedUpMessage message = new PickUpPickedUpMessage(GamePlayers.GetChannelID(player), PickUpID, data);
        GamePlayers.Publish(message, DatagramType.PickUpPickedUp);

        Destroy(gameObject);
    }
}
