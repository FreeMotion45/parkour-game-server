using Assets.Scripts.Shared;
using Assets.Scripts.Shared.Behaviours;
using Assets.Scripts.Shared.Datagrams.Messages;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMultiplayer.Server;
using UnityMultiplayer.Shared.Networking.Datagrams;

public class NetTransform : MonoBehaviour
{
    private static int currentHash;

    public static int NextHash()
    {
        return ++currentHash;
    }

    public static Dictionary<int, NetTransform> networkTransforms = new Dictionary<int, NetTransform>();

    public static GameObject NetInstantiate(string prefab, Vector3 position)
    {
        int hash = NextHash();

        GameObject instantiatedNetObject = GameObject.Instantiate(NetPrefabs.netPrefabs[prefab], position, Quaternion.identity);
        instantiatedNetObject.GetComponent<NetTransform>().hash = hash;
        networkTransforms[hash] = instantiatedNetObject.GetComponent<NetTransform>();

        GigaNetGlobals.PublishMessage(new NetInstantiateMessage(prefab, position, hash), DatagramType.Instantiate);
        return instantiatedNetObject;
    }

    public int hash;
    public NetworkMovement networkMovement;

    private TrackedVector3 position;
    private TrackedVector3 rotation;

    private void Start()
    {        
        position = new TrackedVector3(transform.position);
        rotation = new TrackedVector3(transform.eulerAngles);

        if (networkMovement == NetworkMovement.Interpolated)
        {
            GigaNetGlobals.interpolationManager.Add(this);
        }
    }

    private void Update()
    {
        position.Track(transform.position);
        rotation.Track(transform.eulerAngles);

        if (networkMovement == NetworkMovement.OnDemand)
        {
            MoveOnDemand();
        }
    }

    private void MoveOnDemand()
    {
        if (position.Changed() || rotation.Changed())
        {
            GigaNetGlobals.PublishMessage(new UpdateNetTransformMessage(transform.position, transform.eulerAngles,
                NetworkMovement.OnDemand, hash), DatagramType.Transform);
        }
    }
}

