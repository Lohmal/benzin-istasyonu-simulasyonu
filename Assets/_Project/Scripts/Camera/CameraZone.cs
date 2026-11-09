using UnityEngine;

public class CameraZone : MonoBehaviour
{
    [SerializeField] private GameObject zoneCamera;

    void Start()
    {
        // Inspector'dan atanmadýysa sahnedeki CM_ZoneCamera'yý otomatik bulur
        if (zoneCamera == null)
        {
            GameObject foundCam = GameObject.Find("CM_ZoneCamera");
            if (foundCam != null)
            {
                zoneCamera = foundCam;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneCamera != null)
        {
            zoneCamera.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneCamera != null)
        {
            zoneCamera.SetActive(false);
        }
    }
}