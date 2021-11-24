using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NetObject : MonoBehaviour
{
    private static int currentHash = 0;    

    protected virtual void Start()
    {
        //hash = NextHash();
    }

    public static int NextHash()
    {
        return ++currentHash;
    }
}
