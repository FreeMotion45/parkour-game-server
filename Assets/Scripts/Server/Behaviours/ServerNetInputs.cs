using Assets.Scripts.Server.Inputs;
using Assets.Scripts.Shared;
using Assets.Scripts.Shared.Datagrams.Messages;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public struct Axis
{
    public string name;
    public string positive;
    public string negative;
}

class ServerNetInputs : MonoBehaviour, INetInputSource
{
    [SerializeField] private bool clientControlsRotation;

    [Space]
    
    public string[] netInputs;
    public string[] keyDowns;
    public Axis[] definedAxis;

    private List<INetInputReceiver> inputControlled = new List<INetInputReceiver>();
    private Rigidbody inputRigidbody;
    private BitReader bitReader = new BitReader();

    private HashSet<string> held = new HashSet<string>();
    private HashSet<string> down = new HashSet<string>();

    private void Start()
    {
        TryGetComponent(out inputRigidbody);
        foreach (MonoBehaviour behaviour in GetComponents<MonoBehaviour>())
        {
            if (behaviour is INetInputReceiver)
                inputControlled.Add((INetInputReceiver)behaviour);
        }
    }

    public void SimulateByInput(NetInputMessage netInputMessage)
    {
        PrepareInputs(netInputMessage);
        SimulateInputReceivers();        
        ClearInputs();
    }

    public void ClearInputs()
    {
        held.Clear();
        down.Clear();
    }

    public void SimulatePhysics()
    {
        if (inputRigidbody != null)
        {
            GigaNetGlobals.physics.Simulate(inputRigidbody);
        }
    }

    public void SimulateInputReceivers()
    {
        foreach (INetInputReceiver inputReceiver in inputControlled)
        {
            inputReceiver.Tick(this);
        }
    }

    public void PrepareInputs(NetInputMessage netInputMessage)
    {        
        bitReader.Update(netInputMessage.inputBits, 0);

        IEnumerable<int> netheldInputs = bitReader.Take(netInputs.Length);
        IEnumerable<int> netkeyDowns = bitReader.Skip(netInputs.Length).Take(keyDowns.Length);        

        UpdateCurrentlyPressedKeys(netheldInputs, netInputs, held);
        UpdateCurrentlyPressedKeys(netkeyDowns, keyDowns, down);

        if (clientControlsRotation)
        {
            transform.eulerAngles = netInputMessage.eulerAngles.Get();
        }
    }

    public void UpdateCurrentlyPressedKeys(IEnumerable<int> bits, string[] netInputs, HashSet<string> verified)
    {
        foreach ((int, string) bitInput in bits.Zip(netInputs, (bit, netIn) => (bit, netIn)))
        {
            int isOn = bitInput.Item1;
            string inputName = bitInput.Item2;

            if (isOn == 1)
                verified.Add(inputName);
        }
    }

    public bool GetButton(string btn)
    {
        return held.Contains(btn);
    }

    public bool GetButtonDown(string btn)
    {
        return down.Contains(btn);
    }

    public bool GetButtonUp(string btn)
    {
        throw new NotImplementedException();
    }

    public int GetAxisRaw(string axisName)
    {
        int value = 0;
        Axis axis = definedAxis.FirstOrDefault(x => x.name == axisName);
        if (held.Contains(axis.positive))
        {
            value++;
        }
        if (held.Contains(axis.negative))
        {
            value--;
        }
        return value;
    }
}

