using Assets.Scripts.Game.Shared;
using Assets.Scripts.Network.Messages.ServerOrigin.PickUp;
using Assets.Scripts.ServerLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams;

abstract class BasePickUp : MonoBehaviour
{
    public static int currentId;

    public PickUpType pickUpType;

    public BasePickUp()
    {
        PickUpID = ++currentId;
    }    

    public int PickUpID { get; }

    public abstract void OnPickUp(GameObject player);    

    private void OnEnable()
    {
        PickUpSpawnedMessage spawnMessage = new PickUpSpawnedMessage(PickUpID, pickUpType, transform.position);
        PlayerDatabase.Publish(spawnMessage, DatagramType.PickUpSpawned);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnPickUp(other.gameObject);
        }
    }  
}
