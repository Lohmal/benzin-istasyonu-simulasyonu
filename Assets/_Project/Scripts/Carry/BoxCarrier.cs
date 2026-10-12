using UnityEngine;

// karakterin kutu taşıması. karaktere takılıyor
public class BoxCarrier : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;   // kutunun duracağı yer, kafanın üstü

    private ProductBox carried;   // elimdeki kutu, null ise elim boş

    public bool IsCarrying()
    {
        return carried != null;
    }

    // karakter bir trigger'a girince çalışıyor
    void OnTriggerEnter(Collider other)
    {
        ProductBox box = other.GetComponent<ProductBox>();
        if (box == null) return;              // kutu değilse boşver
        if (IsCarrying()) return;             // zaten elim dolu
        if (!box.CanBePickedUp()) return;     // yeni düştü, biraz beklesin

        carried = box;
        box.PickUp(carryPoint);
    }

    // yağ, forklift gibi şeyler bunu çağırıyor
    public void DropBox()
    {
        if (!IsCarrying()) return;

        carried.Drop();
        carried = null;
    }

    // raf teslim alanı bunu çağırıyor, kutuyu verip elimi boşaltıyor
    public ProductBox HandOver()
    {
        ProductBox box = carried;
        carried = null;
        return box;
    }
}