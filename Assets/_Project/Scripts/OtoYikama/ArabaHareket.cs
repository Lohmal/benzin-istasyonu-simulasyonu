using System.Collections;
using UnityEngine;

public class ArabaHareket : MonoBehaviour
{
    [Header("Noktalar")]
    public Transform yikamaHedefi;
    public Transform cikisHedefi;

    [Header("Yikama")]
    public YikamaKontrol yikamaKontrol;

    [Header("Ayarlar")]
    public float hiz = 2f;
    public float yikamaSuresi = 8f;

    [HideInInspector]
    public ArabaSpawner spawner;

    private Transform aktifHedef;
    private bool hareketEdiyor = true;
    private bool yikamaBasladi = false;

    void Start()
    {
        aktifHedef = yikamaHedefi;
    }

    void Update()
    {
        if (!hareketEdiyor || aktifHedef == null)
            return;

        Vector3 hedefKonum = new Vector3(
            aktifHedef.position.x,
            transform.position.y,
            aktifHedef.position.z
        );

        Vector3 yon = hedefKonum - transform.position;

        if (yon.sqrMagnitude > 0.001f)
        {
            Quaternion hedefRotasyon = Quaternion.LookRotation(yon);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                hedefRotasyon,
                5f * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            hedefKonum,
            hiz * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, hedefKonum) < 0.05f)
        {
            if (!yikamaBasladi && aktifHedef == yikamaHedefi)
            {
                StartCoroutine(YikamaBekle());
            }
            else if (aktifHedef == cikisHedefi)
            {
                hareketEdiyor = false;

                if (spawner != null)
                {
                    // Araba çıkınca parayı göster
                    spawner.CoinGoster();

                    // Yeni araba döngüsünü başlat
                    spawner.YeniArabaIste();
                }

                Destroy(gameObject);
            }
        }
    }

    IEnumerator YikamaBekle()
    {
        yikamaBasladi = true;
        hareketEdiyor = false;

        if (yikamaKontrol != null)
        {
            yikamaKontrol.YikamayiBaslat();
        }

        float kalanSure = yikamaSuresi;

        while (kalanSure > 0f)
        {
            // Player içeride değilken yıkama süresi ilerler
            if (yikamaKontrol == null || !yikamaKontrol.PlayerIceride)
            {
                kalanSure -= Time.deltaTime;
            }

            yield return null;
        }

        if (yikamaKontrol != null)
        {
            yikamaKontrol.YikamayiDurdur();
        }

        aktifHedef = cikisHedefi;
        hareketEdiyor = true;
    }
}