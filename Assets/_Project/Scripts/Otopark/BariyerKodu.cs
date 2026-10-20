using UnityEngine;

public class BariyerKodu : MonoBehaviour
{
    [Header("Bariyer Bağlantısı")]
    public Transform pivotObjesi;

    [Header("Açı Ayarları")]
    public Vector3 kapaliAci = new Vector3(0, 0, 0);
    public Vector3 acikAci = new Vector3(0, 0, 90);
    public float acilmaHizi = 3f;

    private bool acilsinMi = false;

    private int iceridekiObjeSayisi = 0;

    void Update()
    {
        // Pivot objesi atanmadıysa hata vermesini önle
        if (pivotObjesi == null) return;

        // Hedef açıya yumuşakça dön
        Vector3 hedefAci = acilsinMi ? acikAci : kapaliAci;
        pivotObjesi.localRotation = Quaternion.Slerp(
            pivotObjesi.localRotation,
            Quaternion.Euler(hedefAci),
            Time.deltaTime * acilmaHizi
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bariyere temas eden: " + other.name + " | Etiketi: " + other.tag);

        if (other.CompareTag("Player") || other.CompareTag("Car"))
        {
            iceridekiObjeSayisi++;
            acilsinMi = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Car"))
        {
            iceridekiObjeSayisi--;
            if (iceridekiObjeSayisi <= 0)
            {
                iceridekiObjeSayisi = 0;
                acilsinMi = false;
            }
        }
    }
}