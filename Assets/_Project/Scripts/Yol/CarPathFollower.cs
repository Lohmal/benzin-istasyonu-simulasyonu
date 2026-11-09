using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Arabayı verilen noktalara doğru dümdüz sürer.
    /// Tümsekte (SpeedBump) yavaşlar ve son noktaya ulaştığında yok olur (Sink).
    /// </summary>
    public class CarPathFollower : MonoBehaviour
    {
        [System.Serializable]
        public class Waypoint
        {
            [Tooltip("Gidilecek nokta")]
            public Transform point;

            [Tooltip("Bu noktaya varınca kaç saniye beklesin (0 = beklemeden devam)")]
            public float waitTime;
        }

        [Tooltip("Arabanın sırayla gideceği noktalar.")]
        [SerializeField] Waypoint[] waypoints;

        [Tooltip("Oyun başlarken hangi noktaya doğru gideceği (0'dan başlar)")]
        [SerializeField] int startIndex;

        [Tooltip("Düz yoldaki hız (metre/saniye)")]
        [SerializeField] float speed = 10f;

        // --- Tümsek Ayarları ---
        [Tooltip("Tümsek alanına (SpeedBump) girildiğinde düşülecek hız")]
        [SerializeField] float speedBumpSpeed = 3f;

        int currentIndex;
        float waitTimer;

        // Arabanın o anki aktif hızı
        float currentActiveSpeed;

        // Spawner'dan yolu alma fonksiyonu
        public void SetWaypoints(Transform[] points)
        {
            waypoints = new Waypoint[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                waypoints[i] = new Waypoint { point = points[i], waitTime = 0f };
            }
            currentIndex = 0;
        }

        void Start()
        {
            if (waypoints != null && waypoints.Length > 0)
            {
                currentIndex = Mathf.Clamp(startIndex, 0, waypoints.Length - 1);
            }

            // Başlangıçta aktif hızı normal hıza eşitliyoruz
            currentActiveSpeed = speed;

            // Fizik motoru arabayı itip düşürmesin; hareketi bu script yapıyor.
            if (TryGetComponent(out Rigidbody body)) body.isKinematic = true;
        }

        void Update()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            if (waitTimer > 0f)
            {
                waitTimer -= Time.deltaTime;
                return;
            }

            Waypoint waypoint = waypoints[currentIndex];
            if (waypoint.point == null) return;

            Vector3 target = waypoint.point.position;
            Vector3 toTarget = target - transform.position;
            toTarget.y = 0f;

            // Yüzünü doğrudan hedefe çevir (açısal yavaşlama olmadan)
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(toTarget);
            }

            // Aktif hızla (normal hız veya tümsek hızı) dümdüz ilerle
            transform.position = Vector3.MoveTowards(transform.position, target, currentActiveSpeed * Time.deltaTime);

            // Hedef noktaya ulaştı mı?
            if ((transform.position - target).sqrMagnitude < 0.0025f)
            {
                waitTimer = waypoint.waitTime;
                currentIndex++;

                // Son noktaya (WP_1 veya WP_3) ulaştıysa arabayı YOK ET
                if (currentIndex >= waypoints.Length)
                {
                    Destroy(gameObject);
                }
            }
        }

        // --- Tetikleyici Kontrolleri (Tümsek Yavaşlama) ---

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("SpeedBump"))
            {
                currentActiveSpeed = speedBumpSpeed;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("SpeedBump"))
            {
                currentActiveSpeed = speed;
            }
        }

        // Scene penceresinde yolu çizgiyle gösterir (sadece editörde görünür).
        void OnDrawGizmosSelected()
        {
            if (waypoints == null) return;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Transform a = waypoints[i].point;
                Transform b = waypoints[i + 1].point;
                if (a == null || b == null) continue;
                Gizmos.DrawSphere(a.position, 0.4f);
                Gizmos.DrawLine(a.position, b.position);
            }
        }
    }
}