using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetTestScript : MonoBehaviour
{
    public string prefab;
    public Transform spawnpoint;

    public NetTransform netTransform;
    public float moveSpeed = 1;
    public float rotSpeed = 10;

    public Vector3 rot;

    // Start is called before the first frame update
    public void CreateNetTransform()
    {
        netTransform = NetTransform.NetInstantiate(prefab, spawnpoint.position).GetComponent<NetTransform>();
    }

    //private void Update()
    //{
    //    if (netTransform == null) return;
    //    netTransform.transform.position = Vector3.zero + Mathf.Sin(Time.time * moveSpeed) * Vector3.up * 2;
    //    rot += Vector3.one * Time.deltaTime * rotSpeed;
    //    netTransform.transform.eulerAngles = rot;
    //}
}
