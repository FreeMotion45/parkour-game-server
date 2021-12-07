using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementingPositionByTransformContainer : MonoBehaviour
{
    public Transform spawnpointsContainer;

    private Transform[] spawnpoints;
    private int currentIndex;

    private void OnEnable()
    {
        spawnpoints = new Transform[spawnpointsContainer.childCount];

        int counter = 0;
        foreach (Transform spawnpoint in spawnpointsContainer)
        {
            spawnpoints[counter++] = spawnpoint;            
        }
    }

    public Vector3 GetNextPosition()
    {
        currentIndex++;
        return spawnpoints[currentIndex % spawnpoints.Length].position;
    }
}
