﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Management;

[System.Serializable]
public class BasisXRManagement
{
    public XRManagerSettings xRManagerSettings;
    public XRGeneralSettings xRGeneralSettings;
    // Define the event
    public event System.Action<BasisBootedMode> CheckForPass;

    // Store the initial list of loaders
    [SerializeField]
    public List<XRLoader> initialLoaders = new List<XRLoader>();

    public void Initalize()
    {
        // Debug.Log("Begin Load of XR");
        xRGeneralSettings = XRGeneralSettings.Instance;
        xRManagerSettings = xRGeneralSettings.Manager;
    }
    public void BeginLoad()
    {
        Debug.Log("Starting LoadXR");
        BasisDeviceManagement.Instance.StartCoroutine(LoadXR());
    }

    public IEnumerator LoadXR()
    {
        // Initialize the XR loader
        yield return xRManagerSettings.InitializeLoader();
        BasisBootedMode result = BasisBootedMode.Desktop;
        // Check the result
        if (xRManagerSettings.activeLoader != null)
        {
            xRManagerSettings.StartSubsystems();
            result = GetLoaderType(xRManagerSettings.activeLoader?.name);
        }
        Debug.Log("Found Loader " + result);
        CheckForPass?.Invoke(result);
    }
    public BasisBootedMode GetLoaderType(string loaderName)
    {
        if (loaderName == BasisBootedMode.OpenVRLoader.ToString()) return BasisBootedMode.OpenVRLoader;
        if (loaderName == BasisBootedMode.OpenXRLoader.ToString()) return BasisBootedMode.OpenXRLoader;
        return BasisBootedMode.SuccessButUnknown;
    }

    public void StopXR()
    {
        if (xRManagerSettings != null)
        {
            xRManagerSettings.StopSubsystems();
            xRManagerSettings.DeinitializeLoader();
        }
        CheckForPass?.Invoke(BasisBootedMode.Desktop);
    }
}