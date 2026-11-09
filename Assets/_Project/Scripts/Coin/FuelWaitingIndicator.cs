using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Bir yakıt alanında benzin bekleyen araba varken, alanın üstünde (çatının üzerinde) zıplayan bir araba ikonu gösterir.
    /// Böylece çatı altında kalan arabalar uzaktan da fark edilir. Oyuncu pompanın yanına gelince ikon kaybolur
    /// (orada zaten FuelStation'ın kendi "[E] Doldur" ipucu çıkıyor).
    /// Genelde FuelWaitingIndicatorSpawner tarafından her FuelStation için otomatik oluşturulur.
    /// </summary>
    public class FuelWaitingIndicator : MonoBehaviour
    {
        [Tooltip("Takip edilen yakıt alanı. Spawner kullanılıyorsa otomatik atanır.")]
        [SerializeField] FuelStation station;

        [Tooltip("Görünen kısım (ikon). Kameraya döner, ölçeklenir ve zıplar.")]
        [SerializeField] Transform visual;

        [Header("Konum")]
        [Tooltip("Araba görünüyorken ikonun alanın ne kadar üstünde duracağı (metre).")]
        [SerializeField] float heightAboveCar = 3.5f;

        [Tooltip("Araba çatı altında gizliyken kullanılan dünya yüksekliği (çatının biraz üstü). İkon, kameradan arabaya " +
                 "bakan çizginin bu yüksekliği kestiği noktaya konur; böylece ekranda tam gizli arabanın üstünde görünür.")]
        [SerializeField] float height = 10.2f;

        [Tooltip("İkonun ekrandaki boyutu (ekran yüksekliğinin oranı). Kameraya uzaklıktan bağımsız sabit kalır. 0: prefab ölçeği kullanılır.")]
        [Range(0f, 0.5f)] [SerializeField] float screenSize = 0.1f;

        [Tooltip("Visual ölçeği 1 iken ikonun dünya birimi cinsinden çapı (dış halka).")]
        [SerializeField] float visualDiameter = 1.12f;

        [Header("Görünürlük")]
        [Tooltip("Oyuncu pompaya bu mesafeden yakınsa ikon gizlenir. 0 veya altı: FuelStation'ın etkileşim mesafesi + 1 kullanılır.")]
        [SerializeField] float hideWhenPlayerWithin = 0f;

        [Tooltip("Açıksa ikon sadece araba kameradan görünmüyorken (çatı altında kaldığında) çıkar.")]
        [SerializeField] bool onlyWhenHidden = false;

        [Header("Animasyon")]
        [SerializeField] float bobAmplitude = 0.25f;
        [SerializeField] float bobSpeed = 2.5f;
        [SerializeField] float pulseAmount = 0.06f;
        [SerializeField] float pulseSpeed = 5f;
        [Tooltip("Belirme / kaybolma hızı (saniyede 0-1).")]
        [SerializeField] float popSpeed = 6f;

        Transform player;
        Camera cam;
        Vector3 baseScale = Vector3.one;
        float shown;

        public void Init(FuelStation fuelStation) => station = fuelStation;

        void Start()
        {
            if (visual == null && transform.childCount > 0) visual = transform.GetChild(0);
            if (visual != null)
            {
                baseScale = visual.localScale;
                visual.localScale = Vector3.zero;
                visual.gameObject.SetActive(false);
            }
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        void LateUpdate()
        {
            if (station == null || visual == null) return;
            if (cam == null) cam = Camera.main;

            bool want = station.IsCarWaiting && !IsPlayerNear() && (!onlyWhenHidden || IsHiddenFromCamera());
            shown = Mathf.MoveTowards(shown, want ? 1f : 0f, popSpeed * Time.deltaTime);

            bool active = shown > 0.001f;
            if (visual.gameObject.activeSelf != active) visual.gameObject.SetActive(active);
            if (!active) return;

            float t = Time.time;
            Vector3 bob = (cam != null ? cam.transform.up : Vector3.up) * (Mathf.Sin(t * bobSpeed) * bobAmplitude);
            transform.position = GetAnchor() + bob;
            if (cam != null) visual.rotation = cam.transform.rotation;

            Vector3 scale = baseScale;
            if (screenSize > 0f && cam != null && !cam.orthographic)
            {
                // İstenen ekran boyutu için gereken dünya çapı: 2 * mesafe * tan(fov/2) * oran
                float dist = Vector3.Dot(transform.position - cam.transform.position, cam.transform.forward);
                float worldSize = 2f * Mathf.Max(dist, 0.1f) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * screenSize;
                scale = Vector3.one * (worldSize / Mathf.Max(visualDiameter, 0.01f));
            }

            float pulse = 1f + Mathf.Sin(t * pulseSpeed) * pulseAmount;
            visual.localScale = scale * (EaseOutBack(shown) * pulse);
        }

        // İkonun duracağı nokta:
        // - Arabanın üstündeki nokta kameradan görünüyorsa: arabanın tam üstü.
        // - Görünmüyorsa (çatı altı): kameradan arabaya giden çizginin 'height' yüksekliğini kestiği nokta,
        //   yani ekranda gizli arabanın tam üstüne denk gelen, çatının üzerindeki nokta.
        Vector3 GetAnchor()
        {
            Vector3 car = station.transform.position + Vector3.up * 0.8f;
            Vector3 aboveCar = station.transform.position + Vector3.up * heightAboveCar;
            if (cam == null || IsVisibleFromCamera(aboveCar)) return aboveCar;

            Vector3 c = cam.transform.position;
            if (c.y <= height + 0.01f) return new Vector3(car.x, height, car.z);
            float k = (c.y - height) / (c.y - car.y);
            return c + (car - c) * k;
        }

        bool IsVisibleFromCamera(Vector3 point)
        {
            Vector3 from = cam.transform.position;
            Vector3 dir = point - from;
            float dist = dir.magnitude;
            return !Physics.Raycast(from, dir / dist, dist - 0.3f, ~0, QueryTriggerInteraction.Ignore);
        }

        bool IsPlayerNear()
        {
            if (player == null) return false;
            float limit = hideWhenPlayerWithin > 0f ? hideWhenPlayerWithin : station.InteractionDistance + 1f;
            Vector3 d = player.position - station.transform.position;
            d.y = 0f;
            return d.sqrMagnitude <= limit * limit;
        }

        // Kameradan alana bakan çizgi, arabaya varmadan bir şeye (ör. çatı) çarpıyorsa araba görünmüyor demektir.
        bool IsHiddenFromCamera()
        {
            if (cam == null) return false;
            Vector3 from = cam.transform.position;
            Vector3 to = station.transform.position + Vector3.up * 0.8f;
            Vector3 dir = to - from;
            float dist = dir.magnitude;
            if (!Physics.Raycast(from, dir / dist, out RaycastHit hit, dist, ~0, QueryTriggerInteraction.Ignore))
                return false;
            return hit.distance < dist - 3f; // arabanın kendisi ~3 m içinde kalır
        }

        static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f, c3 = c1 + 1f;
            float p = x - 1f;
            return 1f + c3 * p * p * p + c1 * p * p;
        }
    }
}
