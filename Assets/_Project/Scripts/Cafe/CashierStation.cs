using UnityEngine;
using UnityEngine.InputSystem;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Kasanın arkasındaki çalışma noktası. Oyuncu buraya girince alan sarı olur;
    /// müşteri bekliyorsa müşteriye tıklama / dokunma ya da E ile sipariş alınır, çubuk dolunca
    /// müşteri gider ve kasanın arkasına coin düşer.
    /// Bu script, kasanın arkasındaki görünmez trigger objesine eklenir.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CashierStation : MonoBehaviour
    {
        [Tooltip("Bu kasaya gelen müşteri")]
        [SerializeField] CafeCustomer customer;

        [Tooltip("Kasa göstergesi (yeşil alanı yanıp söndüren script)")]
        [SerializeField] CashRegisterIndicator indicator;

        [Tooltip("Oyuncu buradayken alanın rengi (etkileşim = sarı)")]
        [SerializeField] Color playerHereColor = new Color(0.94f, 0.78f, 0.16f);

        [Tooltip("Siparişin hazırlanma süresi (saniye)")]
        [SerializeField] float serveDuration = 3f;

        [Tooltip("Sipariş bitince düşecek coin")]
        [SerializeField] CoinPickup coinPrefab;

        [Tooltip("Coinlerin düşeceği yer")]
        [SerializeField] Transform coinDropPoint;

        [Tooltip("Her siparişte kaç coin düşsün")]
        [SerializeField] int coinsPerOrder = 2;

        [Tooltip("Her coinin değeri")]
        [SerializeField] int coinValue = 10;

        Collider zone;
        Collider player;
        bool serving;
        float progress;

        void Awake()
        {
            zone = GetComponent<Collider>();
            zone.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) player = other;
        }

        void OnTriggerExit(Collider other)
        {
            if (other == player) player = null;
        }

        void Update()
        {
            // Oyuncu kapatıldıysa ya da alanın dışına ışınlandıysa artık burada sayılmasın.
            if (player != null && (!player.enabled || !player.gameObject.activeInHierarchy || !zone.bounds.Intersects(player.bounds)))
                player = null;
            bool playerHere = player != null;

            if (indicator != null) indicator.SetHighlight(playerHere, playerHereColor);

            if (customer != null) customer.ShowHint(CanStartOrder);

            if (InteractPressed()) TryStartOrder();

            if (!serving) return;

            // Oyuncu kasadan ayrılırsa hazırlık bekler, dönünce devam eder.
            if (playerHere)
            {
                progress += Time.deltaTime / serveDuration;
                customer.SetServiceProgress(progress);
            }

            if (progress >= 1f)
            {
                serving = false;
                customer.FinishService();
                DropCoins();
            }
        }

        /// <summary>Oyuncu kasada ve müşteri bekliyorsa sipariş başlatılabilir.</summary>
        public bool CanStartOrder => player != null && !serving && customer != null && customer.IsWaiting;

        /// <summary>
        /// Siparişi başlatır (tıklama, E tuşu ya da ileride ekrandaki bir düğme çağırır).
        /// Başlatılamıyorsa false döner.
        /// </summary>
        public bool TryStartOrder()
        {
            if (!CanStartOrder) return false;
            serving = true;
            progress = 0f;
            customer.BeginService();
            return true;
        }

        void DropCoins()
        {
            if (coinPrefab == null || coinDropPoint == null) return;
            for (int i = 0; i < coinsPerOrder; i++)
            {
                Vector3 offset = coinDropPoint.right * (i * 0.6f);
                CoinPickup coin = Instantiate(coinPrefab, coinDropPoint.position + offset, Quaternion.identity, coinDropPoint);
                coin.Value = coinValue;
            }
        }

        // PC'de E tuşu ya da müşteriye fare ile tıklama, mobilde müşteriye dokunma.
        // Bağlı bütün cihazlara bakılır (birden fazla klavye/fare olabilir).
        bool InteractPressed()
        {
            foreach (InputDevice device in InputSystem.devices)
            {
                if (device is Keyboard keyboard && keyboard.eKey.wasPressedThisFrame) return true;
                // Pointer = fare ya da dokunmatik ekran
                if (device is Pointer pointer && pointer.press.wasPressedThisFrame && IsCustomerAt(pointer.position.ReadValue())) return true;
            }
            return false;
        }

        // Ekranda tıklanan noktadan ışın gönderir; bu kasanın müşterisine çarparsa true.
        // Böylece yere tıklayıp yürümek siparişi yanlışlıkla başlatmaz.
        bool IsCustomerAt(Vector2 screenPosition)
        {
            Camera cam = Camera.main;
            if (cam == null || customer == null) return false;

            Ray ray = cam.ScreenPointToRay(screenPosition);
            return Physics.Raycast(ray, out RaycastHit hit, 200f, ~0, QueryTriggerInteraction.Ignore)
                && hit.collider.GetComponentInParent<CafeCustomer>() == customer;
        }
    }
}
