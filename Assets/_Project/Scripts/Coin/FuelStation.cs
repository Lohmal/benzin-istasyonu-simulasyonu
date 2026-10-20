using UnityEngine;
using UnityEngine.InputSystem; // Yeni Unity Input System paketini kullanıyoruz

namespace BenzinIstasyonu
{
    /// <summary>
    /// Benzin istasyonu istasyon alanını ve benzin doldurma sürecini yönetir.
    /// Müşteri arabası alana girince durur, oyuncu yaklaşınca UI ipucu çıkar,
    /// 'E' tuşuna basıldığında dolum gerçekleşir ve işlem bitince coin düşer.
    /// </summary>
    public class FuelStation : MonoBehaviour
    {
        [Header("Coin Ayarları")]
        [Tooltip("Doğurulacak Coin Prefabi veya Sahnedeki Coin Şablonu")]
        [SerializeField] private GameObject coinPrefab;

        [Tooltip("Coin'in doğacağı/düşeceği konum (Örn: Coin_Alan_1 Transform'u)")]
        [SerializeField] private Transform coinDropPoint;

        [Header("Mesafe ve Dolum Ayarları")]
        [Tooltip("Oyuncunun 'E' tuşuyla etkileşime girebilmesi için maksimum uzaklığı (metre)")]
        [SerializeField] private float interactionDistance = 4.0f;

        [Tooltip("Benzin dolumunun tamamlanması için gereken süre (saniye)")]
        [SerializeField] private float serveDuration = 2.0f;

        // Durum Değişkenleri (State Variables)
        private bool isCarInZone = false;           // Araba benzin alanında mı?
        private bool isFueling = false;             // Benzin dolumu devam ediyor mu?
        private bool isFueled = false;              // Benzin dolduruldu mu?
        private bool isFinishedForThisCar = false;  // Mevcut arabanın işlemi tamamen bitti mi?
        private float timer = 0f;                   // Dolum süresi sayacı

        // Bileşen ve Nesne Referansları
        private MonoBehaviour carFollower;          // Arabanın yol takip script'i
        private Transform playerTransform;          // Oyuncunun Transform referansı

        // UI Görselleri İçin Doku
        private Texture2D fillTexture;

        private void Start()
        {
            // Sahnedeki oyuncuyu (Player/Karakter) otomatik olarak bul
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj == null) playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null) playerObj = GameObject.Find("Karakter");

            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }

            // Dolum barı için kullanılacak renk dokularını hazırla
            MakeTextures();
        }

        /// <summary>
        /// OnGUI üzerinden çizilecek dolum barı için 1x1 piksellik dinamik renk dokusu oluşturur.
        /// </summary>
        private void MakeTextures()
        {
            fillTexture = new Texture2D(1, 1);
            fillTexture.SetPixel(0, 0, new Color(0.95f, 0.5f, 0.1f, 0.9f)); // Turuncu dolum rengi
            fillTexture.Apply();
        }

        /// <summary>
        /// Müşteri arabası benzinliğin Trigger alanına girdiğinde tetiklenir.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // Bu arabanın işlemi zaten bittiyse tekrar durdurma yapma
            if (isFinishedForThisCar) return;

            // Gelen nesne 'Vehicle' tag'ine veya 'MusteriArabasi' ismine sahipse
            if (other.CompareTag("Vehicle") || other.name.Contains("MusteriArabasi") || (other.transform.parent != null && other.transform.parent.name.Contains("MusteriArabasi")))
            {
                Debug.Log("🚗 Araba benzinliğe geldi ve durdu.");

                // Arabanın yol takip mekanizmasını pasif hale getirerek durduruyoruz
                carFollower = other.GetComponentInParent<MonoBehaviour>();
                if (carFollower != null)
                {
                    carFollower.enabled = false;
                }

                isCarInZone = true;
                isFueled = false;
            }
        }

        /// <summary>
        /// Araba benzinlik alanından tamamen çıktığında alanı sonraki müşteri için sıfırlar.
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Vehicle") || other.name.Contains("MusteriArabasi") || (other.transform.parent != null && other.transform.parent.name.Contains("MusteriArabasi")))
            {
                ResetFuelArea();
            }
        }

        private void Update()
        {
            // Araba alanda değilse, işlem bittiyse veya benzin zaten dolduysa güncelleme yapma
            if (!isCarInZone || isFueled || isFinishedForThisCar) return;

            // Oyuncu istasyona yeterince yakın mı kontrol et
            bool isPlayerNearby = false;
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                if (distance <= interactionDistance)
                {
                    isPlayerNearby = true;
                }
            }

            // Oyuncu yakındaysa ve dolum henüz başlamadıysa tuş girdisini dinle
            if (isPlayerNearby && !isFueling)
            {
                if (IsEPressed())
                {
                    StartFueling();
                }
            }

            // Dolum süreci devam ediyorsa zamanlayıcıyı ilerlet
            if (isFueling)
            {
                timer += Time.deltaTime;
                if (timer >= serveDuration)
                {
                    CompleteFueling();
                }
            }
        }

        /// <summary>
        /// Dünya uzayındaki benzinlik simgesini ve [E] ipucunu ekrana (GUI) çizer.
        /// </summary>
        private void OnGUI()
        {
            if (isFinishedForThisCar || !isCarInZone) return;

            // Oyuncu yakında mı kontrol et
            bool isPlayerNearby = false;
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                if (distance <= interactionDistance) isPlayerNearby = true;
            }

            // Oyuncu uzaktaysa ve dolum yapılmıyorsa arayüzü çizme
            if (!isPlayerNearby && !isFueling) return;

            // Arabanın 4.5 metre yukarısını ekran koordinatlarına çevir
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 4.5f);
            if (screenPos.z <= 0) return; // Kamera arkasında kalıyorsa çizme

            // UI Simge boyutları (26x26 piksel - Minimalist)
            float size = 26f;
            float x = screenPos.x - (size / 2f);
            float y = Screen.height - screenPos.y - (size / 2f);

            // Dolum esnasında aşağıdan yukarıya doğru dolan turuncu bar çizimi
            if (isFueling)
            {
                float fillPct = Mathf.Clamp01(timer / serveDuration);
                GUI.color = new Color(0.98f, 0.5f, 0.05f);
                GUI.DrawTexture(new Rect(x - 2, y + (size * (1f - fillPct)), size + 4, size * fillPct), fillTexture);
            }

            // Benzin İkonu Metni
            GUI.color = Color.white;
            GUIStyle iconStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            GUI.Label(new Rect(x, y, size, size), "⛽", iconStyle);

            // Dolum yapılmıyorken altında beliren [E] Doldur İpucu
            if (!isFueling)
            {
                GUIStyle textStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 10,
                    fontStyle = FontStyle.Bold
                };
                textStyle.normal.textColor = Color.white;
                GUI.Label(new Rect(x - 25, y + size + 2, size + 50, 16), "[E] Doldur", textStyle);
            }
        }

        /// <summary>
        /// Yeni Input System paketiyle klavye veya fare girdisini kontrol eder.
        /// </summary>
        private bool IsEPressed()
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) return true;
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
            return false;
        }

        /// <summary>
        /// Benzin dolum sürecini başlatır.
        /// </summary>
        private void StartFueling()
        {
            isFueling = true;
            timer = 0f;
            Debug.Log("⛽ Benzin dolumu BAŞLADI!");
        }

        /// <summary>
        /// Benzin dolumu tamamlandığında çalışır, coin düşürür ve arabayı hareket ettirir.
        /// </summary>
        private void CompleteFueling()
        {
            isFueling = false;
            isFueled = true;
            isFinishedForThisCar = true; // Bu araba için işlem tamamlandı

            Debug.Log("✅ Benzin DOLDU! Coin düşürülüyor...");

            // Coin doğurma işlemini çağır
            SpawnCoin();

            // Arabanın yol takibini tekrar aktif ederek gitmesini sağla
            if (carFollower != null)
            {
                carFollower.enabled = true;
            }
        }

        /// <summary>
        /// Benzin dolumu bitince coin nesnesini sahnede oluşturur (Instantiate).
        /// </summary>
        private void SpawnCoin()
        {
            if (coinPrefab != null && coinDropPoint != null)
            {
                Instantiate(coinPrefab, coinDropPoint.position, Quaternion.identity).SetActive(true);
            }
            else if (coinDropPoint != null)
            {
                // Prefab atanmadıysa sahnede hazır bulunan noktayı kopyalar
                GameObject newCoin = Instantiate(coinDropPoint.gameObject, coinDropPoint.position, Quaternion.identity);
                newCoin.SetActive(true);
            }
        }

        /// <summary>
        /// Yeni araba geldiğinde kullanılmak üzere benzinlik durum değişkenlerini sıfırlar.
        /// </summary>
        public void ResetFuelArea()
        {
            isCarInZone = false;
            isFueling = false;
            isFueled = false;
            isFinishedForThisCar = false;
            timer = 0f;
        }
    }
}