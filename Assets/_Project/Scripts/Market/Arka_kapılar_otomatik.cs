using UnityEngine;

public class OtomatikTekKapi : MonoBehaviour
{
    public enum Yon { Sag, Sol }

    [Header("Kapı")]
    [SerializeField] Transform kapi;

    [Header("Ayarlar")]
    [SerializeField] Yon acilmaYonu = Yon.Sag;
    [SerializeField] float acilmaMesafesi = 1f;   // Kapının kayacağı mesafe
    [SerializeField] float hiz = 2f;              // Saniyedeki hareket hızı
    [SerializeField] string oyuncuTag = "Player";

    Vector3 kapaliPozisyon, acikPozisyon;
    int icerideKisiSayisi = 0;

    void Start()
    {
        kapaliPozisyon = kapi.localPosition;
        HesaplaAcikPozisyon();
    }

    void HesaplaAcikPozisyon()
    {
        Vector3 yon = acilmaYonu == Yon.Sag ? Vector3.forward : Vector3.back;
        acikPozisyon = kapaliPozisyon + yon * acilmaMesafesi;
    }

    void Update()
    {
        Vector3 hedef = icerideKisiSayisi > 0 ? acikPozisyon : kapaliPozisyon;
        kapi.localPosition = Vector3.MoveTowards(kapi.localPosition, hedef, hiz * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(oyuncuTag))
            icerideKisiSayisi++;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(oyuncuTag))
            icerideKisiSayisi = Mathf.Max(0, icerideKisiSayisi - 1);
    }

    // Oyun çalışırken Inspector'dan yön/mesafe değiştirince anında uygulanır
    void OnValidate()
    {
        if (Application.isPlaying && kapi != null)
            HesaplaAcikPozisyon();
    }
}