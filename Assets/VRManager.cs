using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class VRManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float scale;
    void Start()
    {
        UnityEngine.XR.XRSettings.enabled = false;
        QualitySettings.resolutionScalingFixedDPIFactor = scale;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
