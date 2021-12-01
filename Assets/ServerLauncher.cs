using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityMultiplayer.Server;

public class ServerLauncher : MonoBehaviour
{
    [SerializeField] UnityMultiplayerServer server;
    public InputField ip;
    public InputField port;

    public void Launch()
    {
        server._hostIP = ip.text;
        server._hostPort = int.Parse(port.text);
        server.enabled = true;
    }
}
