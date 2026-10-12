using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Ekrandaki coin sayacı. CoinWallet değişince sayıyı günceller ve kısa bir "büyüyüp küçülme" yapar.
    /// </summary>
    public class CoinCounterUI : MonoBehaviour
    {
        [Tooltip("Coin sayısını gösteren yazı")]
        [SerializeField] TMPro.TMP_Text label;

        [Tooltip("Coin gelince büyüyüp küçülecek kutu")]
        [SerializeField] RectTransform popTarget;

        [Tooltip("Coin gelince ne kadar büyüsün")]
        [SerializeField] float popScale = 1.2f;

        [Tooltip("Büyüme efektinin süresi (saniye)")]
        [SerializeField] float popDuration = 0.18f;

        float popTimer;

        void OnEnable()
        {
            CoinWallet.Changed += OnCoinsChanged;
            Refresh(CoinWallet.Coins);
        }

        void OnDisable()
        {
            CoinWallet.Changed -= OnCoinsChanged;
        }

        void OnCoinsChanged(int coins)
        {
            Refresh(coins);
            popTimer = popDuration;
        }

        void Refresh(int coins)
        {
            if (label != null) label.text = coins.ToString();
        }

        void Update()
        {
            if (popTarget == null) return;
            if (popTimer > 0f)
            {
                popTimer -= Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(popTimer / popDuration);
                popTarget.localScale = Vector3.one * Mathf.Lerp(1f, popScale, t);
            }
            else if (popTarget.localScale != Vector3.one)
            {
                popTarget.localScale = Vector3.one;
            }
        }
    }
}
