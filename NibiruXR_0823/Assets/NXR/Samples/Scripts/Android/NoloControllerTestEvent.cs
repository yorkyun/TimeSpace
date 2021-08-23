using Nxr.Internal;
using UnityEngine;
namespace NXR.Samples
{
    public class NoloControllerTestEvent : MonoBehaviour
    { 
        public TextMesh textMesh_Controller; 
        
        public void onMenuDown(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onMenuDown." + deviceType;
        }
        public void onMenuUp(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onMenuUp." + deviceType.ToString();
        }

        public void onSystemDown(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onSystemDown." + deviceType;
        }
        public void onSystemUp(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onSystemUp." + deviceType.ToString();
        }

        public void onTriggerDown(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onTriggerDown." + deviceType.ToString();
        }
        public void onTriggerUp(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onTriggerUp." + deviceType.ToString();
        }

        public void onGripDown(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onGripDown." + deviceType.ToString();
        }

        public void onGripUp(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onGripUp." + deviceType.ToString();
        }

        public void onTouchpadDown(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onTouchpadDown." + deviceType.ToString();
        }

        public void onTouchpadUp(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onTouchpadUp." + deviceType.ToString();
        }

        public void onTouchpadTouch(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onTouchpadTouch." + deviceType.ToString();
        }

        public void onTouchpadRelease(NxrInstantNativeApi.NibiruDeviceType deviceType)
        {
            textMesh_Controller.text = "onTouchpadRelease." + deviceType.ToString();
        }

        public void onTouchPadPosition(NxrInstantNativeApi.NibiruDeviceType deviceType, float x, float y)
        {
            textMesh_Controller.text = "onTouchpadPosition." + x + "," + y + "." + deviceType.ToString();
        }

      
    }
}