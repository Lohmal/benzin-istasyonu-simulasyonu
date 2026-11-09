using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Yere tıklayınca (mobilde dokununca) oyuncuyu o noktaya yürütür.
    /// Kameradan tıklanan yere ışın (Raycast) gönderilir; yere çarparsa orası hedef olur.
    /// Müşteriye tıklamak yürütmez (o tıklama siparişi alır).
    /// WASD / gamepad ile yürümeye başlayınca tıklama hedefi iptal olur.
    /// Bu script, PlayerMovement ile aynı objeye (Player) eklenir.
    /// </summary>
    [DefaultExecutionOrder(-10)] // PlayerMovement'tan önce çalışsın, yönü aynı karede versin
    [RequireComponent(typeof(PlayerMovement))]
    public class ClickToMove : MonoBehaviour
    {
        [Tooltip("Tıklanabilen yüzeyler (varsayılan: hepsi)")]
        [SerializeField] LayerMask groundMask = ~0;

        [Tooltip("Hedefe bu kadar yaklaşınca durur (metre)")]
        [SerializeField] float stopDistance = 0.15f;

        [Tooltip("Oyuncunun ayağından bu kadar yüksekteki yüzeyler (benzinlik çatısı, raf üstü...) tıklamada yok sayılır, " +
                 "ışın altındaki zemine kadar devam eder.")]
        [SerializeField] float maxGroundHeightAbovePlayer = 1f;

        [Tooltip("Tıklanan yerde görünen işaretin prefab'ı (boş bırakılabilir)")]
        [SerializeField] Transform markerPrefab;

        [Tooltip("İşaretin büyüyüp küçülme hızı")]
        [SerializeField] float markerPulseSpeed = 6f;

        PlayerMovement movement;
        Camera cam;
        Transform marker;
        Vector3 target;
        bool hasTarget;
        Vector3 markerScale;
        readonly RaycastHit[] rayHits = new RaycastHit[16];

        // Bir yere takılırsa (duvar, tezgâh) sonsuza kadar itmesin diye.
        float stuckTimer;
        float lastDistance;

        void Awake()
        {
            movement = GetComponent<PlayerMovement>();
            cam = Camera.main;
            if (markerPrefab != null)
            {
                // İşaret oyuncuyla birlikte kaymasın diye sahnede ayrı bir obje olarak oluşturulur.
                marker = Instantiate(markerPrefab);
                marker.name = "ClickMarker";
                markerScale = marker.localScale;
                marker.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            // Klavye / gamepad her zaman önceliklidir.
            if (movement.HasDirectInput) Stop();

            Pointer pointer = Pointer.current; // fare ya da dokunmatik ekran
            if (pointer != null && pointer.press.wasPressedThisFrame)
                TrySetTarget(pointer.position.ReadValue());

            if (!hasTarget) return;

            Vector3 toTarget = target - transform.position;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;

            if (distance <= stopDistance || IsStuck(distance))
            {
                Stop();
                return;
            }

            movement.AutoMove = toTarget / distance;
            PulseMarker();
        }

        void TrySetTarget(Vector2 screenPosition)
        {
            if (cam == null) return;

            // Arayüzün (HUD) üstüne tıklandıysa yürüme.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = cam.ScreenPointToRay(screenPosition);
            if (!TryFindGround(ray, out RaycastHit hit)) return;

            target = hit.point;
            hasTarget = true;
            stuckTimer = 0f;
            lastDistance = float.MaxValue;

            if (marker != null)
            {
                marker.position = hit.point + Vector3.up * 0.02f;
                marker.gameObject.SetActive(true);
            }
        }

        // Işının çarptığı yüzeyleri yakından uzağa gezer ve ilk "zemin" yüzeyini bulur.
        // Oyuncunun çok üstünde kalan yüzeyler (benzinlik çatısı gibi) ve dikey yüzeyler (duvar yüzü,
        // tabela) atlanır; böylece ekranda görünen yere tıklanınca oraya gidilir.
        bool TryFindGround(Ray ray, out RaycastHit ground)
        {
            ground = default;
            int count = Physics.RaycastNonAlloc(ray, rayHits, 200f, groundMask, QueryTriggerInteraction.Ignore);
            System.Array.Sort(rayHits, 0, count, HitDistanceComparer.Instance);

            float maxY = transform.position.y + maxGroundHeightAbovePlayer;
            for (int i = 0; i < count; i++)
            {
                RaycastHit h = rayHits[i];
                if (h.collider.transform.IsChildOf(transform)) continue; // oyuncunun kendisi

                // Müşteriye tıklamak sipariş içindir, yürümek için değil.
                if (h.collider.CompareTag("Customer") || h.collider.GetComponentInParent<CafeCustomer>() != null) return false;

                // Sadece yere benzeyen (yukarı bakan) ve oyuncunun yetişebileceği yükseklikteki yüzeyler.
                if (h.normal.y < 0.7f || h.point.y > maxY) continue;

                ground = h;
                return true;
            }
            return false;
        }

        sealed class HitDistanceComparer : System.Collections.Generic.IComparer<RaycastHit>
        {
            public static readonly HitDistanceComparer Instance = new HitDistanceComparer();
            public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
        }

        bool IsStuck(float distance)
        {
            if (lastDistance - distance > 0.01f)
            {
                stuckTimer = 0f;
                lastDistance = distance;
                return false;
            }
            stuckTimer += Time.deltaTime;
            return stuckTimer > 0.5f;
        }

        void PulseMarker()
        {
            if (marker == null) return;
            float s = 1f + 0.15f * Mathf.Sin(Time.time * markerPulseSpeed);
            marker.localScale = new Vector3(markerScale.x * s, markerScale.y, markerScale.z * s);
        }

        void Stop()
        {
            if (!hasTarget) return;
            hasTarget = false;
            movement.AutoMove = Vector3.zero;
            if (marker != null) marker.gameObject.SetActive(false);
        }
    }
}
