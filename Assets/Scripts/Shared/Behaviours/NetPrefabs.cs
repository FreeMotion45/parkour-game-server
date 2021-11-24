using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class NamedPrefab
{
    public string name;
    public GameObject prefab;
}

class NetPrefabs : MonoBehaviour
{
    public static Dictionary<string, GameObject> netPrefabs = new Dictionary<string, GameObject>();

    public NamedPrefab[] prefabs;

    private void Start()
    {
        foreach (NamedPrefab namedPrefab in prefabs)
        {
            netPrefabs[namedPrefab.name] = namedPrefab.prefab;
        }
    }
}

