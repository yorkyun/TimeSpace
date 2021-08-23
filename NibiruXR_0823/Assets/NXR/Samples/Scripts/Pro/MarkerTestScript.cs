using Nxr.Internal;
using UnityEngine;
namespace NXR.Samples
{
    public class MarkerTestScript : MonoBehaviour
    {

        NibiruService nibiruService;
        // Use this for initialization
        void Start()
        {
            nibiruService = NxrViewer.Instance.GetNibiruService();
            NibiruMarker.OnMarkerFoundHandler = MarkerFound;
            NibiruMarker.OnMarkerLostHandler = MarkerLost;
            if (nibiruService != null)
            {
                nibiruService.StartMarkerRecognize();
            }
        }

        void MarkerFound()
        {
            Debug.Log("MarkerTestScript->onMarkerFound");
        }

        void MarkerLost()
        {
            Debug.Log("MarkerTestScript->onMarkerLost");
        }

        private void OnApplicationPause(bool pause)
        {
            Debug.LogError("Marker-OnApplicationPause=" + pause);
            if (pause && nibiruService != null)
            {
                nibiruService.StopMarkerRecognize();
            }
            else if (nibiruService != null)
            {
                nibiruService.StartMarkerRecognize();
            }
        }

        private void OnDestroy()
        {
            if (nibiruService != null && nibiruService.IsMarkerRecognizeRunning)
            {
                nibiruService.StopMarkerRecognize();
                Debug.Log("MarkerTestScript.OnDestroy");
            }
        }

        private void OnApplicationQuit()
        {
            if (nibiruService != null && nibiruService.IsMarkerRecognizeRunning)
            {
                nibiruService.StopMarkerRecognize();
                Debug.LogError("Marker-StopMarkerRecognize");
            }
            Debug.LogError("Marker-OnApplicationQuit");
        }

    }
}