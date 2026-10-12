using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Objeyi her karede kameraya döndürür. Karakterlerin başının üstündeki çubuk ve yazılar için.
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        Camera cam;

        void LateUpdate()
        {
            if (cam == null) cam = Camera.main;
            if (cam != null) transform.rotation = cam.transform.rotation;
        }
    }
}
