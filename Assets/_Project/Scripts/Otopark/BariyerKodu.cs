using UnityEngine;

public class BariyerKodu : MonoBehaviour
{
    [Header("Bariyer Bağlantısı")]
    public Transform pivotObjesi; // PivotNoktası objesi

    [Header("Açı Ayarları")]
    public Vector3 kapaliAci = new Vector3(0, 0, 0);   // Kapalı duruş
    public Vector3 acikAci = new Vector3(0, 0, 90);    // Açık duruş
    public float acilmaHizi = 3f;                      // Hareket hızı

    private bool acilsinMi = false;
    private float sayaç = 0f;

    void Update()
    {
        // Zamanı say
        sayaç += Time.deltaTime;

        // 5 saniye dolduğunda durumu tersine çevir
        if (sayaç >= 5f)
        {
            acilsinMi = !acilsinMi; // Açıksa kapatır, kapalıysa açar
            sayaç = 0f;             // Sayacı sıfırla
        }

        // Hedef açıya yumuşakça dön
        Vector3 hedefAci = acilsinMi ? acikAci : kapaliAci;
        pivotObjesi.localRotation = Quaternion.Lerp(
            pivotObjesi.localRotation,
            Quaternion.Euler(hedefAci),
            Time.deltaTime * acilmaHizi
        );
    }
}