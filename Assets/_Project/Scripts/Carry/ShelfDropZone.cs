using UnityEngine;

// market rafındaki teslim noktası. kutuyu getirince alıp siliyor
public class ShelfDropZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // gelen kutu taşıyan karakter mi
        BoxCarrier carrier = other.GetComponent<BoxCarrier>();
        if (carrier == null) return;          // karakter değil
        if (!carrier.IsCarrying()) return;    // eli boş gelmiş

        // kutuyu al, sahneden kaldır
        ProductBox box = carrier.HandOver();
        Destroy(box.gameObject);
        Debug.Log("Ürün rafa kondu!");
    }
}