using UnityEngine;
using UnityEngine.InputSystem;

namespace Market
{
    public class MarketCashierStation : MonoBehaviour
    {
        [SerializeField] MarketCustomerHareket customer;
        [SerializeField] OdemeAlani odemeAlani;

        [Tooltip("Siparişin hazırlanma süresi (saniye)")]
        [SerializeField] float serveDuration = 3f;

        [Header("Coin")]
        [SerializeField] GameObject coinPrefab;
        [SerializeField] Transform coinDropPoint;
        [SerializeField] int coinsPerOrder = 2;
        [SerializeField] int coinValue = 10;

        bool serving;
        bool hintAcik;
        float progress;

        bool PlayerHere => odemeAlani != null && odemeAlani.OyuncuAlandaMi();

        // Müşteri BU kasaya mı geldi?
        bool CustomerAtMyStation => customer != null && customer.Kasa == odemeAlani;

        public bool CanStartOrder =>
            PlayerHere && !serving && CustomerAtMyStation && customer.IsWaiting;

        void Update()
        {
            // Müşteri başka kasaya gidiyorsa bu istasyon hiçbir şeye dokunmaz
            if (!CustomerAtMyStation) return;

            // İpucu: sadece bu istasyon açtıysa kapatır
            if (CanStartOrder)
            {
                customer.ShowHint(true);
                hintAcik = true;
            }
            else if (hintAcik)
            {
                customer.ShowHint(false);
                hintAcik = false;
            }

            if (InteractPressed()) TryStartOrder();

            if (!serving) return;

            // Oyuncu bu kasanın alanından ayrılırsa bar durur, dönünce devam eder
            if (PlayerHere)
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

        public bool TryStartOrder()
        {
            if (!CanStartOrder) return false;
            serving = true;
            progress = 0f;
            hintAcik = false;
            customer.BeginService();
            return true;
        }

        void DropCoins()
        {
            if (coinPrefab == null || coinDropPoint == null) return;
            for (int i = 0; i < coinsPerOrder; i++)
            {
                Vector3 offset = coinDropPoint.right * (i * 0.6f);
                GameObject go = Instantiate(coinPrefab, coinDropPoint.position + offset, Quaternion.identity, coinDropPoint);

                BenzinIstasyonu.CoinPickup coin = go.GetComponent<BenzinIstasyonu.CoinPickup>();
                if (coin != null) coin.Value = coinValue;
            }
        }

        static bool InteractPressed()
        {
            foreach (InputDevice device in InputSystem.devices)
            {
                if (device is Keyboard k && k.eKey.wasPressedThisFrame) return true;
                if (device is Mouse m && m.leftButton.wasPressedThisFrame) return true;
                if (device is Touchscreen t && t.primaryTouch.press.wasPressedThisFrame) return true;
            }
            return false;
        }
    }
}