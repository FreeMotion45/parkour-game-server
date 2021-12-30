using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class NetTransform
{
    public readonly static Dictionary<int, GameObject> networkObjects = new Dictionary<int, GameObject>();
    public readonly static Dictionary<GameObject, int> objectHash = new Dictionary<GameObject, int>();

    private static int currentHash;

    public static int RegisterNewNetObject(GameObject gameObject, int hash)
    {        
        networkObjects[hash] = gameObject;
        objectHash[gameObject] = hash;
        return hash;
    }

    public static int RegisterNewNetObject(GameObject gameObject)
    {
        return RegisterNewNetObject(gameObject, ++currentHash);
    }
}
