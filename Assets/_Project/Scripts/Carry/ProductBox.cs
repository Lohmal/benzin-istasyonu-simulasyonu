using UnityEngine;

// ürün kutusu. karakter alınca kafasına çıkıyor, düşünce yere geri dönüyor
public class ProductBox : MonoBehaviour
{
    [SerializeField] private BoxType type;   // hafif mi ağır mı, veri dosyasından geliyor

    private Rigidbody rb;
    private Collider box;        // kutunun gövdesi
    private Collider range;      // alma alanı (sphere, trigger)
    private float canPickUpAt;   // düştükten sonra hemen alınmasın diye

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();

        // tip seçmeyi unuttuysam kırmızıyla söylesin
        if (type == null) Debug.LogError(name + ": koli tipi seçilmemiş");
    }

    // taşıyan karakter bunlara bakıp yavaşlıyor. tip yoksa normal say
    public float SpeedMultiplier()
    {
        return type != null ? type.SpeedMultiplier() : 1f;
    }

    public float JumpMultiplier()
    {
        return type != null ? type.JumpMultiplier() : 1f;
    }

    public string Label()
    {
        return type != null ? type.Label() : "";
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

    // G ile bırakınca fırlatmıyorum, verilen yere koyuyorum
    public void PutDown(Vector3 position, Quaternion rotation)
    {
        transform.SetParent(null);
        transform.position = position;
        transform.rotation = rotation;

        box.enabled = true;
        range.enabled = true;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;     // önceden kalan hız varsa sıfırla
        rb.angularVelocity = Vector3.zero;

        canPickUpAt = Time.time + 0.5f;
    }
}