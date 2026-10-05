using System.Collections.Generic;
using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Kasanın önündeki algılama alanına bir müşteri (ya da oyuncu) girince
    /// kasanın arkasındaki yeşil alanı yanıp söndürür: "Kasada müşteri var, gel!"
    /// Bu script, kasanın önündeki görünmez trigger objesine eklenir.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CashRegisterIndicator : MonoBehaviour
    {
        [Tooltip("Yanıp sönecek obje: kasanın arkasındaki yeşil alan (Order_Point)")]
        [SerializeField] Renderer indicator;

        [Tooltip("Bu etiketlere (tag) sahip objeler müşteri sayılır")]
        [SerializeField] string[] customerTags = { "Customer", "Player" };

        [Tooltip("Yanıp sönerken ulaşılan parlak renk")]
        [SerializeField] Color blinkColor = new Color(0.75f, 1f, 0.7f);

        [Tooltip("Saniyede kaç kez yanıp söneceği")]
        [SerializeField] float blinksPerSecond = 2f;

        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        // Alanın içindeki müşteriler. Sayaç yerine liste tutulur; çünkü alanın içindeyken
        // yok edilen ya da kapatılan bir obje için Unity OnTriggerExit göndermez.
        readonly HashSet<Collider> customersInside = new HashSet<Collider>();
        Collider zone;
        MaterialPropertyBlock propertyBlock;
        Color normalColor;
        bool wasBlinking;

        /// <summary>Şu an kasanın önünde müşteri var mı?</summary>
        public bool HasCustomer => customersInside.Count > 0;

        void Awake()
        {
            zone = GetComponent<Collider>();
            zone.isTrigger = true;
            propertyBlock = new MaterialPropertyBlock();
            normalColor = indicator.sharedMaterial.GetColor(BaseColorId);
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsCustomer(other)) customersInside.Add(other);
        }

        void OnTriggerExit(Collider other)
        {
            customersInside.Remove(other);
        }

        void Update()
        {
            // Yok edilen, kapatılan ya da alanın dışına ışınlanan müşterileri listeden çıkar.
            customersInside.RemoveWhere(c => c == null || !c.enabled || !c.gameObject.activeInHierarchy
                                             || !zone.bounds.Intersects(c.bounds));

            if (!HasCustomer)
            {
                if (wasBlinking) SetColor(normalColor);
                wasBlinking = false;
                return;
            }

            wasBlinking = true;
            // 0 ile 1 arasında gidip gelen değer: koyu yeşil <-> parlak yeşil
            float t = Mathf.PingPong(Time.time * blinksPerSecond * 2f, 1f);
            SetColor(Color.Lerp(normalColor * 0.6f, blinkColor, t));
        }

        void SetColor(Color color)
        {
            indicator.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorId, color);
            indicator.SetPropertyBlock(propertyBlock);
        }

        bool IsCustomer(Collider other)
        {
            foreach (string tag in customerTags)
                if (other.CompareTag(tag)) return true;
            return false;
        }
    }
}
