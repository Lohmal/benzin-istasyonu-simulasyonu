using UnityEngine;

public class OtoYikamaCoin : MonoBehaviour
{
    [SerializeField] private int arabaBasinaUcret = 10;

    private int birikenPara = 0;

    public void ParaEkle()
    {
        birikenPara += arabaBasinaUcret;

        // Bekleyen para olduğu için coin görünür
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (birikenPara <= 0)
            return;

        // Biriken paranın tamamını oyuncuya ver
        BenzinIstasyonu.CoinWallet.Add(birikenPara);

        Debug.Log("Oto yıkamadan " + birikenPara + " coin alındı.");

        // Para toplandı
        birikenPara = 0;

        // Coin'i yok etme, sonraki ödeme için gizle
        gameObject.SetActive(false);
    }
}