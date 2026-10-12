using UnityEngine;

public class YikamaKontrol : MonoBehaviour
{
    public Renderer yikamaAlani;

    public Material bosMaterial;
    public Material doluMaterial;

    public FircaDondur sagFirca;
    public FircaDondur solFirca;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            // Yeşil alanı kırmızı yap
            yikamaAlani.material = doluMaterial;

            // Fırçaları çalıştır
            sagFirca.Baslat();
            solFirca.Baslat();
        }
    }
}