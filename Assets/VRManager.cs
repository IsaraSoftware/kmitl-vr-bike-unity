using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;

public class VRManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isVR;

    public NetworkManager manager;
    void Start()
    {
        UnityEngine.XR.XRSettings.enabled = isVR;
        
    }

    public void StartHost()
    {
        manager.StartHost();
    }

    public void StartClient()
    {
        manager.StartClient();
    }

    // Update is called once per frame
    void Update()
    {
        //print("IP:" + manager.networkAddress);
        
    }
}
