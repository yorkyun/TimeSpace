using NibiruTask;
using UnityEngine;

namespace NXR.Samples
{
    public class Apis : MonoBehaviour {
    public TextMesh macAddress;
    public TextMesh deviceId;
    public TextMesh sixDofStatus;

    public void LaunchSDKDemo()
    {
        Debug.Log("LaunchSDKDemo");
        NibiruTaskApi.LaunchAppByPkgName("com.nibiru.vr.lib2.test");
    }

	// Use this for initialization
	void Start () {
        if(macAddress != null) macAddress.text = "MacAddress: " + NibiruTaskApi.GetMacAddress();
        if (deviceId != null) deviceId.text = "DeviceId: " + NibiruTaskApi.GetDeviceId();
        if (sixDofStatus != null) sixDofStatus.text = "SixDof Plugin Status: [Declared " + NibiruTaskApi.IsPluginDeclared(Nxr.Internal.PLUGIN_ID.SIX_DOF)
            + "], [Suppored " + NibiruTaskApi.IsPluginSupported(Nxr.Internal.PLUGIN_ID.SIX_DOF) + "]";
    }
	
 
}


}