using UnityEngine;

public class YikamaKontrol : MonoBehaviour
{
    [Header("Alan")]
    public Renderer yikamaAlani;
    public Material bosMaterial;
    public Material doluMaterial;

    [Header("Fircalar")]
    public FircaDondur sagFirca;
    public FircaDondur solFirca;

    [Header("Karakter")]
    public GameObject player;

    private bool yikamaAktif = false;
    private bool playerIceride = false;

    public bool PlayerIceride
    {
        get { return playerIceride; }
    }

    private void Start()
    {
        yikamaAlani.material = bosMaterial;
        sagFirca.Durdur();
        solFirca.Durdur();
    }

    public void YikamayiBaslat()
    {
        yikamaAktif = true;
        yikamaAlani.material = doluMaterial;

        // Player içeride değilse fırçaları çalıştır
        if (!playerIceride)
        {
            sagFirca.Baslat();
            solFirca.Baslat();
        }
    }

    public void YikamayiDurdur()
    {
        yikamaAktif = false;

        sagFirca.Durdur();
        solFirca.Durdur();

        yikamaAlani.material = bosMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player != null && other.transform.root.gameObject == player)
        {
            playerIceride = true;

            // Karakter alana girince güvenlik için fırçaları durdur
            sagFirca.Durdur();
            solFirca.Durdur();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (player != null && other.transform.root.gameObject == player)
        {
            playerIceride = false;

            // Yıkama hâlâ devam ediyorsa fırçaları tekrar çalıştır
            if (yikamaAktif)
            {
                sagFirca.Baslat();
                solFirca.Baslat();
            }
        }
    }
}