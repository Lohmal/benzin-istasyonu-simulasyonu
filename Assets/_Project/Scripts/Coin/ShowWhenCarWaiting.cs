using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Alt objelerini (ör. havada dönen coin) sadece en yakındaki yakıt alanında benzin bekleyen bir araba varken gösterir.
    /// Yakında bir FuelStation yoksa alt objeler hep gizli kalır.
    /// </summary>
    public class ShowWhenCarWaiting : MonoBehaviour
    {
        [Tooltip("Bağlı yakıt alanı. Boşsa oyun başında en yakın FuelStation aranır.")]
        [SerializeField] FuelStation station;

        [Tooltip("Otomatik aramada FuelStation'ın en fazla bu kadar yatay uzaklıkta olması gerekir (metre).")]
        [SerializeField] float searchRadius = 3f;

        bool shown = true;

        void Start()
        {
            if (station == null) station = FindNearestStation();
            Apply(false);
        }

        void Update()
        {
            Apply(station != null && station.IsCarWaiting);
        }

        void Apply(bool show)
        {
            if (show == shown) return;
            shown = show;
            foreach (Transform child in transform) child.gameObject.SetActive(show);
        }

        FuelStation FindNearestStation()
        {
            FuelStation best = null;
            float bestDist = searchRadius * searchRadius;
            foreach (var s in FindObjectsByType<FuelStation>(FindObjectsSortMode.None))
            {
                Vector3 d = s.transform.position - transform.position;
                d.y = 0f;
                if (d.sqrMagnitude <= bestDist) { bestDist = d.sqrMagnitude; best = s; }
            }
            return best;
        }
    }
}
