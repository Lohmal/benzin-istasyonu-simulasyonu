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

        // Arabanin onune gorunmez guvenlik alani ekle
        GameObject guvenlikKupu = new GameObject("OnGuvenlikTrigger");

        guvenlikKupu.transform.SetParent(yeniAraba.transform, false);

        guvenlikKupu.transform.localPosition = new Vector3(0f, 0.8f, 2.5f);
        guvenlikKupu.transform.localRotation = Quaternion.identity;
        guvenlikKupu.transform.localScale = Vector3.one;

        // Gorunmez trigger alani
        BoxCollider collider = guvenlikKupu.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(2.5f, 1.5f, 2f);

        // Player algilama scripti
        ArabaOnGuvenlik guvenlik = guvenlikKupu.AddComponent<ArabaOnGuvenlik>();

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
        hareket.onGuvenlik = guvenlik;

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