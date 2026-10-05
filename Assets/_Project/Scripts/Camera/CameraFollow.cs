using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Kamerayı hedefin (oyuncunun) arkasında sabit bir açıyla tutar ve yumuşakça takip eder.
    /// Kamera dönmez, sadece kayar; My Perfect Hotel tarzı eğik görünüm.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Tooltip("Takip edilecek obje (Player)")]
        [SerializeField] Transform target;

        [Tooltip("Kameranın hedefe göre konumu. Y yükseklik, Z ne kadar geride olduğu.")]
        [SerializeField] Vector3 offset = new Vector3(0f, 14f, -11f);

        [Tooltip("Kameranın hedefin hangi yüksekliğine bakacağı")]
        [SerializeField] float lookHeight = 1f;

        [Tooltip("Takip gecikmesi (saniye). Küçük = daha sıkı takip.")]
        [SerializeField] float smoothTime = 0.15f;

        Vector3 velocity;

        void Start()
        {
            SnapToTarget();
        }

        void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }

        /// <summary>Kamerayı beklemeden hedefin üstüne yerleştirir ve açısını ayarlar.</summary>
        public void SnapToTarget()
        {
            if (target == null) return;
            transform.position = target.position + offset;
            Vector3 lookPoint = target.position + Vector3.up * lookHeight;
            transform.rotation = Quaternion.LookRotation(lookPoint - transform.position);
            velocity = Vector3.zero;
        }

        // Inspector'da değer değişince kamerayı Scene görünümünde de yerine koyar.
        void OnValidate()
        {
            if (!Application.isPlaying) SnapToTarget();
        }
    }
}
