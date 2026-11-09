using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Oyun başında sahnedeki her FuelStation için bir "araba bekliyor" ikonu oluşturur.
    /// Yeni bir yakıt alanına FuelStation eklendiğinde ikonu da otomatik gelir.
    /// </summary>
    public class FuelWaitingIndicatorSpawner : MonoBehaviour
    {
        [Tooltip("Her FuelStation için kopyalanacak ikon prefabı.")]
        [SerializeField] FuelWaitingIndicator indicatorPrefab;

        void Start()
        {
            if (indicatorPrefab == null) return;
            foreach (var station in FindObjectsByType<FuelStation>(FindObjectsSortMode.None))
            {
                var indicator = Instantiate(indicatorPrefab, transform);
                indicator.name = indicatorPrefab.name + "_" + station.name;
                indicator.Init(station);
            }
        }
    }
}
