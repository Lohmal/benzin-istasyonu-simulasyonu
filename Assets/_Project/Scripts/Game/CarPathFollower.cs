using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Arabayı bir nokta (waypoint) listesi boyunca döngü hâlinde sürer.
    /// Son noktadan sonra ilk noktaya döner; istenen noktalarda bir süre bekler.
    /// Örnek: yolun sonunda dur, karşı şeride geç, geri dön, yolun başında dur...
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

        [Tooltip("Arabanın sırayla gideceği noktalar. Sondan sonra başa döner.")]
        [SerializeField] Waypoint[] waypoints;

        [Tooltip("Oyun başlarken hangi noktaya doğru gideceği (0'dan başlar)")]
        [SerializeField] int startIndex;

        [Tooltip("Düz yoldaki hız (metre/saniye)")]
        [SerializeField] float speed = 10f;

        [Tooltip("Dönüş hızı (derece/saniye)")]
        [SerializeField] float turnSpeed = 180f;

        int currentIndex;
        float waitTimer;

        void Start()
        {
            currentIndex = Mathf.Clamp(startIndex, 0, waypoints.Length - 1);

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
            Vector3 target = waypoint.point.position;
            Vector3 toTarget = target - transform.position;
            toTarget.y = 0f;

            // Önce gideceği yöne dön. Keskin dönüşlerde (şerit değiştirirken) yavaşla.
            float speedFactor = 1f;
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(toTarget);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
                float angle = Quaternion.Angle(transform.rotation, look);
                speedFactor = Mathf.Lerp(1f, 0.3f, angle / 90f);
            }

            transform.position = Vector3.MoveTowards(transform.position, target, speed * speedFactor * Time.deltaTime);

            if ((transform.position - target).sqrMagnitude < 0.0025f)
            {
                waitTimer = waypoint.waitTime;
                currentIndex = (currentIndex + 1) % waypoints.Length;
            }
        }

        // Scene penceresinde yolu çizgiyle gösterir (sadece editörde görünür).
        void OnDrawGizmosSelected()
        {
            if (waypoints == null) return;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Transform a = waypoints[i].point;
                Transform b = waypoints[(i + 1) % waypoints.Length].point;
                if (a == null || b == null) continue;
                Gizmos.DrawSphere(a.position, 0.4f);
                Gizmos.DrawLine(a.position, b.position);
            }
        }
    }
}
