using System.Collections.Generic;
using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Oyuncu ile kamera arasında "BenzinIstasyonu/SeeThrough Lit" shader'lı bir obje (duvar vb.) kaldığında,
    /// o objelerde oyuncunun etrafında yuvarlak, kenarı noktalı bir delik açar.
    /// Kameraya eklenir. Shader'a global değerler gönderir; duvarlara tek tek bir şey eklemek gerekmez.
    /// </summary>
    [DisallowMultipleComponent]
    public class SeeThroughController : MonoBehaviour
    {
        [Header("Hedef")]
        [Tooltip("Görünür tutulacak obje. Boşsa 'Player' tag'li obje aranır.")]
        [SerializeField] Transform target;

        [Tooltip("Hedefin pivotundan itibaren kontrol noktalarının yükseklikleri (ayak, gövde, kafa).")]
        [SerializeField] float[] checkHeights = { 0.3f, 1f, 1.7f };

        [Tooltip("Deliğin merkezi: pivot + bu yükseklik.")]
        [SerializeField] float centerHeight = 1f;

        [Header("Engel tespiti")]
        [Tooltip("Hangi layer'lardaki collider'lar engel sayılır.")]
        [SerializeField] LayerMask occluderMask = ~0;

        [Tooltip("Oyuncudan kameraya atılan SphereCast'in yarıçapı.")]
        [SerializeField] float castRadius = 0.25f;

        [Header("Delik görünümü")]
        [Tooltip("Deliğin varsayılan yarıçapı, ekran yüksekliğinin oranı olarak. " +
                 "Kapatan objede SeeThroughRadiusOverride varsa onun değeri kullanılır.")]
        [Range(0.02f, 0.5f)] [SerializeField] float radius = 0.13f;

        [Tooltip("Yarıçap değişirken (ör. duvardan çatıya geçince) geçiş hızı.")]
        [SerializeField] float radiusLerpSpeed = 6f;

        [Tooltip("Kenar yumuşaklığı. 0 = keskin, 1 = merkeze kadar geçiş.")]
        [Range(0f, 1f)] [SerializeField] float softness = 0.45f;

        [Tooltip("Açıkken kenar noktalı (dither) geçişli olur, kapalıyken düz yumuşak kesim.")]
        [SerializeField] bool dither = true;

        [Tooltip("Ayaktan itibaren bu yüksekliğin altı kesilmez (zemin, eşik vb. delinmesin).")]
        [SerializeField] float minCutHeight = 0.15f;

        [Tooltip("Sadece oyuncudan en az bu kadar kameraya yakın pikseller kesilir.")]
        [SerializeField] float depthMargin = 0.3f;

        [Header("Animasyon")]
        [Tooltip("Deliğin açılma hızı (saniyede 0-1).")]
        [SerializeField] float openSpeed = 5f;

        [Tooltip("Deliğin kapanma hızı (saniyede 0-1).")]
        [SerializeField] float closeSpeed = 3f;

        const string ShaderName = "BenzinIstasyonu/SeeThrough Lit";

        static readonly int TargetId = Shader.PropertyToID("_SeeThroughTarget");
        static readonly int ParamsId = Shader.PropertyToID("_SeeThroughParams");
        static readonly int Params2Id = Shader.PropertyToID("_SeeThroughParams2");

        // Collider başına önbellek: see-through mu, varsa özel yarıçapı (yoksa -1)
        struct OccluderInfo
        {
            public bool seeThrough;
            public float radius;
        }

        readonly RaycastHit[] hits = new RaycastHit[16];
        readonly Dictionary<Collider, OccluderInfo> occluderCache = new Dictionary<Collider, OccluderInfo>();
        Shader seeThroughShader;
        float amount;
        float currentRadius;
        float targetRadius;

        /// <summary>Deliğin şu anki açıklığı (0 kapalı, 1 tam açık).</summary>
        public float Amount => amount;

        void Awake()
        {
            seeThroughShader = Shader.Find(ShaderName);
            currentRadius = targetRadius = radius;
            if (target == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) target = player.transform;
            }
        }

        void OnDisable()
        {
            amount = 0f;
            Shader.SetGlobalVector(ParamsId, Vector4.zero);
        }

        void LateUpdate()
        {
            if (target == null)
            {
                Shader.SetGlobalVector(ParamsId, Vector4.zero);
                return;
            }

            bool occluded = IsOccluded(out float occluderRadius);
            float speed = occluded ? openSpeed : closeSpeed;
            amount = Mathf.MoveTowards(amount, occluded ? 1f : 0f, speed * Time.deltaTime);
            float eased = amount * amount * (3f - 2f * amount); // smoothstep

            // Kapanırken son yarıçap korunur, böylece delik büyüklüğü değişmeden kapanır
            if (occluded) targetRadius = occluderRadius;
            if (amount <= 0f) currentRadius = targetRadius = radius;
            currentRadius = Mathf.Lerp(currentRadius, targetRadius, 1f - Mathf.Exp(-radiusLerpSpeed * Time.deltaTime));

            Vector3 basePos = target.position;
            Vector3 center = basePos + Vector3.up * centerHeight;
            Shader.SetGlobalVector(TargetId, new Vector4(center.x, center.y, center.z, basePos.y));
            Shader.SetGlobalVector(ParamsId, new Vector4(eased, currentRadius, softness, dither ? 1f : 0f));
            Shader.SetGlobalVector(Params2Id, new Vector4(minCutHeight, depthMargin, 0f, 0f));
        }

        // Oyuncuyu see-through bir obje kapatıyor mu? Kapatıyorsa, kapatanlar arasındaki en büyük yarıçapı döndürür.
        bool IsOccluded(out float occluderRadius)
        {
            bool occluded = false;
            occluderRadius = radius;
            Vector3 camPos = transform.position;
            foreach (float h in checkHeights)
            {
                Vector3 from = target.position + Vector3.up * h;
                Vector3 toCam = camPos - from;
                float dist = toCam.magnitude;
                if (dist < 0.01f) continue;

                int count = Physics.SphereCastNonAlloc(from, castRadius, toCam / dist, hits, dist,
                    occluderMask, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < count; i++)
                {
                    Collider col = hits[i].collider;
                    if (col.transform.IsChildOf(target)) continue;
                    OccluderInfo info = GetOccluderInfo(col);
                    if (!info.seeThrough) continue;

                    float r = info.radius > 0f ? info.radius : radius;
                    occluderRadius = occluded ? Mathf.Max(occluderRadius, r) : r;
                    occluded = true;
                }
            }
            return occluded;
        }

        // Collider'ın görseli see-through shader kullanıyor mu, özel yarıçapı var mı? (Collider başına önbelleğe alınır.)
        OccluderInfo GetOccluderInfo(Collider col)
        {
            if (occluderCache.TryGetValue(col, out OccluderInfo cached)) return cached;

            var info = new OccluderInfo { seeThrough = false, radius = -1f };
            Renderer rend = col.GetComponent<Renderer>();
            if (rend == null) rend = col.GetComponentInParent<Renderer>();
            if (rend != null && seeThroughShader != null)
            {
                foreach (Material m in rend.sharedMaterials)
                {
                    if (m != null && m.shader == seeThroughShader) { info.seeThrough = true; break; }
                }
            }
            var radiusOverride = col.GetComponentInParent<SeeThroughRadiusOverride>();
            if (radiusOverride != null) info.radius = radiusOverride.Radius;

            occluderCache[col] = info;
            return info;
        }
    }
}
