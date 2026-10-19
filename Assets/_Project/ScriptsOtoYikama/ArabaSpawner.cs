using System.Collections;
using UnityEngine;

public class ArabaSpawner : MonoBehaviour
{
    [Header("Arabalar")]
    public GameObject[] arabaPrefablar;

    [Header("Noktalar")]
    public Transform spawnNoktasi;
    public Transform yikamaHedefi;
    public Transform cikisHedefi;

    [Header("Yikama")]
    public YikamaKontrol yikamaKontrol;

    [Header("Ayarlar")]
    public float yeniArabaBeklemeSuresi = 3f;
    public float arabaHizi = 2f;
    public float yikamaSuresi = 8f;

    [Header("Odul")]
    public OtoYikamaCoin yikamaCoin;

    private bool yeniArabaBekleniyor = false;

    public void YeniArabaIste()
    {
        if (!yeniArabaBekleniyor)
        {
            StartCoroutine(YeniArabaOlustur());
        }
    }

    IEnumerator YeniArabaOlustur()
    {
        yeniArabaBekleniyor = true;

        yield return new WaitForSeconds(yeniArabaBeklemeSuresi);

        // 5 arabadan rastgele birini seç
        int rastgeleIndex = Random.Range(0, arabaPrefablar.Length);
        GameObject secilenAraba = arabaPrefablar[rastgeleIndex];

        // Arabayı spawn noktasında oluştur
        GameObject yeniAraba = Instantiate(
            secilenAraba,
            spawnNoktasi.position,
            spawnNoktasi.rotation
        );

        // Trigger'ın arabayı tanıması için
        yeniAraba.tag = "Car";

        // Rigidbody yoksa ekle
        Rigidbody rb = yeniAraba.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = yeniAraba.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        rb.isKinematic = true;

        // ArabaHareket yoksa ekle
        ArabaHareket hareket = yeniAraba.GetComponent<ArabaHareket>();

        if (hareket == null)
        {
            hareket = yeniAraba.AddComponent<ArabaHareket>();
        }

        // Gerekli bilgileri yeni arabaya ver
        hareket.yikamaHedefi = yikamaHedefi;
        hareket.cikisHedefi = cikisHedefi;
        hareket.yikamaKontrol = yikamaKontrol;

        hareket.hiz = arabaHizi;
        hareket.yikamaSuresi = yikamaSuresi;
        hareket.spawner = this;

        yeniArabaBekleniyor = false;
    }

    public void CoinGoster()
    {
        if (yikamaCoin != null)
        {
            yikamaCoin.ParaEkle();
        }
    }
}