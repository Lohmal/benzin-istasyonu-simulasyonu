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

        [Header("Coin (opsiyonel, şimdilik boş bırakabilirsin)")]
        [SerializeField] GameObject coinPrefab;
        [SerializeField] Transform coinDropPoint;
        [SerializeField] int coinsPerOrder = 2;
        [SerializeField] int coinValue = 10;

        bool serving;
        float progress;

        bool PlayerHere => odemeAlani != null && odemeAlani.OyuncuAlandaMi();

        public bool CanStartOrder => PlayerHere && !serving && customer != null && customer.IsWaiting;

        void Update()
        {
            if (customer != null) customer.ShowHint(CanStartOrder);

            if (InteractPressed()) TryStartOrder();

            if (!serving) return;

            // Oyuncu kasadan ayrılırsa hazırlık bekler, dönünce devam eder.
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
            customer.BeginService();
            return true;
        }

        void DropCoins()
        {
            Debug.Log("DropCoins çağrıldı. coinPrefab: " + coinPrefab + " | dropPoint: " + coinDropPoint);

            if (coinPrefab == null) Debug.LogWarning("Coin Prefab BOŞ");
            if (coinDropPoint == null) Debug.LogWarning("Coin Drop Point BOŞ");
            if (coinPrefab == null || coinDropPoint == null) return;

            for (int i = 0; i < coinsPerOrder; i++)
            {
                Vector3 offset = coinDropPoint.right * (i * 0.6f);
                GameObject go = Instantiate(coinPrefab, coinDropPoint.position + offset, Quaternion.identity, coinDropPoint);
                Debug.Log("Coin üretildi: " + go.name + " konum: " + go.transform.position);

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