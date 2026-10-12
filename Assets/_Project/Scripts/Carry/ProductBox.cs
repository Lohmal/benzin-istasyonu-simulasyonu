using UnityEngine;

// ürün kutusu. karakter alınca kafasına çıkıyor, düşünce yere geri dönüyor
public class ProductBox : MonoBehaviour
{
    private Rigidbody rb;
    private Collider box;        // kutunun gövdesi
    private Collider range;      // alma alanı (sphere, trigger)
    private float canPickUpAt;   // düştükten sonra hemen alınmasın diye

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();
    }

    // düşeli yarım saniye geçtiyse tekrar alınabilir
    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    public void PickUp(Transform carryPoint)
    {
        rb.isKinematic = true;   // bunu yapmazsam kutu yerden düşüp gidiyor
        box.enabled = false;     // taşırken karaktere çarpmasın
        range.enabled = false;   // elimdeyken tekrar alınmasın

        // carry point'in altına girsin ki karakterle beraber gelsin
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        transform.SetParent(null);
        box.enabled = true;
        range.enabled = true;
        rb.isKinematic = false;  // fizik geri gelsin

        // biraz geriye ve yukarı fırlasın
        Vector3 push = -transform.forward + Vector3.up;
        rb.AddForce(push * 3f, ForceMode.Impulse);

        canPickUpAt = Time.time + 0.5f;
    }
}