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

        [Tooltip("Kenar yumuşaklığı (0 = keskin, 1 = merkeze kadar noktalı geçiş). " +
                 "Negatif: SeeThroughController'daki varsayılan kullanılır. Küçük değer = tamamen açık alan daha geniş.")]
        [Range(-1f, 1f)] [SerializeField] float softness = -1f;

        public float Radius => radius;

        /// <summary>Negatifse varsayılan kullanılır.</summary>
        public float Softness => softness;
    }
}
