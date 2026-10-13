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

    void Update()
    {
        Vector3 hedefAci = acilsinMi ? acikAci : kapaliAci;
        pivotObjesi.localRotation = Quaternion.Slerp(
            pivotObjesi.localRotation,
            Quaternion.Euler(hedefAci),
            Time.deltaTime * acilmaHizi
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        // Temas eden HER OBJE bariyeri açar
        acilsinMi = true;
    }

    private void OnTriggerExit(Collider other)
    {
        acilsinMi = false;
    }
}