using UnityEngine;

public class UrunSpawner : MonoBehaviour
{
    [SerializeField] private GameObject urunPrefab;

    private float timer;           // doğmasına kalan süre
    private bool dogabilir = true; // false iken yeni ürün üretilmez

    void Update()
    {
        // Pad'de ürün duruyorsa bir şey yapma
        if (GetComponentInChildren<Urun>() != null) return;

        // Müşteri henüz kasaya varmadıysa bekle
        if (!dogabilir) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Instantiate(urunPrefab, transform.position, transform.rotation, transform);
    }

    // Müşteri rafa vardı: ürünü al, yenisi şimdilik doğmasın
    public void UrunuAl()
    {
        Urun urun = GetComponentInChildren<Urun>();
        if (urun != null) Destroy(urun.gameObject);
        dogabilir = false;
    }

    // Müşteri kasaya vardı: verilen saniye sonra yeni ürün doğsun
    public void GeriGetir(float saniye)
    {
        timer = saniye;
        dogabilir = true;
    }
}