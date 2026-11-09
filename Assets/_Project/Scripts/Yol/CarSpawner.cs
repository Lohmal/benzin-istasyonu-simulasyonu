using UnityEngine;

namespace BenzinIstasyonu
{
    public class CarSpawner : MonoBehaviour
    {
        // Slayt 14: Kopyalanacak araba prefabı ve tekrar çıkma süresi
        [Tooltip("Üretilecek araba Prefab'ı (Prometheus veya BlueCar)")]
        [SerializeField] private GameObject carPrefab;

        [Tooltip("Araba yok olduktan kaç saniye sonra yenisi üretilsin?")]
        [SerializeField] private float delay = 2f;

        [Tooltip("Arabanın gideceği noktalar (Örn: Sadece WP_1 veya Sadece WP_3 atabilirsin)")]
        [SerializeField] private Transform[] pathPoints;

        private float timer;

        void Update()
        {
            // Slayt 14 Satır 14: Bu spawner'ın ürettiği araba hala yoldaysa (yok olmadıysa) bekle
            if (GetComponentInChildren<CarPathFollower>() != null) return;

            // Slayt 14 Satır 17-18: Süreyi geri say
            timer -= Time.deltaTime;
            if (timer > 0f) return;

            // Slayt 14 Satır 21-22: Arabayı bu noktanın pozisyonunda ve child'ı olarak üret
            GameObject newCar = Instantiate(carPrefab, transform.position, transform.rotation, transform);

            // Üretilen arabaya gideceği hedef noktayı (WP_1 veya WP_3) ver
            if (newCar.TryGetComponent(out CarPathFollower follower))
            {
                follower.SetWaypoints(pathPoints);
            }

            // Slayt 14 Satır 23: Sayacı sıfırla
            timer = delay;
        }
    }
}