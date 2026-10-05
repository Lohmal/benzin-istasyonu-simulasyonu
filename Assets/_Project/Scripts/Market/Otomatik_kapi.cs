using UnityEngine;

public class OtomatikKapi : MonoBehaviour
{
    [Header("Kapılar")]
    [SerializeField] Transform solKapi;
    [SerializeField] Transform sagKapi;

    [Header("Ayarlar")]
    [SerializeField] float acilmaMesafesi = 1f;   // Her kapının kayacağı mesafe
    [SerializeField] float hiz = 2f;              // Saniyedeki hareket hızı
    [SerializeField] string oyuncuTag = "Player";

    Vector3 solKapali, sagKapali, solAcik, sagAcik;
    int icerideKisiSayisi = 0;

    void Start()
    {
        solKapali = solKapi.localPosition;
        sagKapali = sagKapi.localPosition;

        // Sol kapı sola (-X), sağ kapı sağa (+X) kayar
        solAcik = solKapali + Vector3.left * acilmaMesafesi;
        sagAcik = sagKapali + Vector3.right * acilmaMesafesi;
    }

    void Update()
    {
        bool acik = icerideKisiSayisi > 0;

        Vector3 solHedef = acik ? solAcik : solKapali;
        Vector3 sagHedef = acik ? sagAcik : sagKapali;

        float adim = hiz * Time.deltaTime;
        solKapi.localPosition = Vector3.MoveTowards(solKapi.localPosition, solHedef, adim);
        sagKapi.localPosition = Vector3.MoveTowards(sagKapi.localPosition, sagHedef, adim);
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
}