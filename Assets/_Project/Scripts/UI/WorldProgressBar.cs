using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Dünyada duran basit ilerleme çubuğu: dolgu objesi soldan sağa doğru uzar.
    /// Dolgu (fill) tam doluyken ortalanmış olmalı; script genişliğini ve konumunu ayarlar.
    /// </summary>
    public class WorldProgressBar : MonoBehaviour
    {
        [Tooltip("Uzayıp kısalan dolgu objesi")]
        [SerializeField] Transform fill;

        float fullWidth;
        Vector3 fullPosition;
        bool initialized;

        void Awake()
        {
            Init();
        }

        void Init()
        {
            if (initialized || fill == null) return;
            fullWidth = fill.localScale.x;
            fullPosition = fill.localPosition;
            initialized = true;
        }

        /// <summary>0 = boş, 1 = dolu.</summary>
        public void SetProgress(float t)
        {
            Init();
            if (fill == null) return;
            t = Mathf.Clamp01(t);
            Vector3 scale = fill.localScale;
            scale.x = fullWidth * t;
            fill.localScale = scale;
            // Sol kenar sabit kalsın: merkez, boş kalan kısmın yarısı kadar sola kayar.
            Vector3 position = fullPosition;
            position.x = fullPosition.x - fullWidth * (1f - t) * 0.5f;
            fill.localPosition = position;
            fill.gameObject.SetActive(t > 0.001f);
        }
    }
}
