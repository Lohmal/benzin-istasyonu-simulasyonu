using UnityEngine;

public class OdemeAlani : MonoBehaviour
{
    [Header("Görsel")]
    [SerializeField] Renderer gorsel;      // Yeşil zeminin Renderer'ı
    [SerializeField] Material yesilMat;
    [SerializeField] Material sariMat;

    [Header("Ayarlar")]
    [SerializeField] float yanipSonmeAraligi = 0.5f;
    [SerializeField] string oyuncuTag = "Player";

    bool yanipSonsun;
    int icerideOyuncuSayisi = 0;
    float sayac;

    void Start()
    {
        if (gorsel == null) gorsel = GetComponent<Renderer>();
        gorsel.sharedMaterial = yesilMat;
        gorsel.enabled = true;
    }

    public void YanipSonmeyiBaslat()
    {
        yanipSonsun = true;
        sayac = 0f;
    }

    void Update()
    {
        // Oyuncu alandaysa sarı ve sabit
        if (icerideOyuncuSayisi > 0)
        {
            gorsel.sharedMaterial = sariMat;
            gorsel.enabled = true;
            return;
        }

        gorsel.sharedMaterial = yesilMat;

        if (yanipSonsun)
        {
            sayac += Time.deltaTime;
            if (sayac >= yanipSonmeAraligi)
            {
                sayac = 0f;
                gorsel.enabled = !gorsel.enabled;   // Aç/kapa = yanıp sönme
            }
        }
        else
        {
            gorsel.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(oyuncuTag)) return;
        icerideOyuncuSayisi++;
        yanipSonsun = false;   // Oyuncu geldi, yanıp sönme biter
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(oyuncuTag)) return;
        icerideOyuncuSayisi = Mathf.Max(0, icerideOyuncuSayisi - 1);
    }
}