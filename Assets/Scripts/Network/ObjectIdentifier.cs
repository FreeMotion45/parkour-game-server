using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectIdentifier : MonoBehaviour
{    
    private static int _id = 0;    

    public readonly int itemId;

    public ObjectIdentifier()
    {
        itemId = ++_id;
    }    
}
