using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Oyuncuyu bu obje (veya alt objeleri) kapattığında, delik varsayılandan farklı büyüklükte açılır.
    /// Örnek: benzinlik çatısı gibi geniş ve yüksek yapılarda daha büyük delik.
    /// Collider'ın bulunduğu objeye ya da onun bir üst objesine eklenir.
    /// </summary>
    public class SeeThroughRadiusOverride : MonoBehaviour
    {
        [Tooltip("Bu obje kapattığında deliğin yarıçapı, ekran yüksekliğinin oranı olarak.")]
        [Range(0.02f, 0.5f)] [SerializeField] float radius = 0.22f;

        public float Radius => radius;
    }
}
