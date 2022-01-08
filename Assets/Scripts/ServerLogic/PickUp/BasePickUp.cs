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

public abstract class BasePickUp : MonoBehaviour
{
    public static int currentId;
        
    public float pickUpRadius;
    public LayerMask playerLayerMask;

    [HideInInspector] public PickUpType pickUpType;        

    public BasePickUp()
    {
        PickUpID = ++currentId;
    }    

    public int PickUpID { get; }

    public abstract void OnPickUp(GameObject player);    

    private void OnEnable()
    {
        PickUpSpawnedMessage spawnMessage = new PickUpSpawnedMessage(PickUpID, pickUpType, transform.position);
        GamePlayers.Publish(spawnMessage, DatagramType.PickUpSpawned);
    }

    protected virtual void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickUpRadius, playerLayerMask);

        // 1 because we are ignoring collision with ourself.
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                OnPickUp(collider.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
    }
}
