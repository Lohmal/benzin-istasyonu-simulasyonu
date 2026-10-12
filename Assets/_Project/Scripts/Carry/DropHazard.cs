using UnityEngine;

// yağ birikintisi gibi şeyler. kutu taşırken basarsan kutu düşüyor
public class DropHazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // gelen kutu taşıyan karakter mi
        BoxCarrier carrier = other.GetComponent<BoxCarrier>();
        if (carrier == null) return;
        if (!carrier.IsCarrying()) return;

        // kutuyu düşür
        carrier.DropBox();
        Debug.Log(name + " yüzünden kutu düştü!");
    }
}