using UnityEngine;
using UnityEngine.InputSystem;

// karakterin kutu taşıması. E ile alıyor, G ile bırakıyor
public class BoxCarrier : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;  // kutunun duracağı yer, kafanın üstü

    [SerializeField] private float putDownDistance = 1.1f;  // kutuyu ne kadar önüme koyayım

    private ProductBox carried;      // elimdeki kutu, null ise elim boş
    private ProductBox nearbyBox;    // yanımda duran kutu, E'ye basınca bu alınıyor

    private InputAction pickUpAction;
    private InputAction dropAction;

    void Awake()
    {
        // E zaten projedeki Interact action'ında var, onu kullanıyorum
        pickUpAction = InputSystem.actions.FindAction("Interact");

        // G'yi ortak input dosyasına eklemedim, herkes o dosyayı değiştiriyor çakışma çıkmasın diye
        // gamepad'de de sol tuş bırakıyor
        dropAction = new InputAction("DropBox", InputActionType.Button);
        dropAction.AddBinding("<Keyboard>/g");
        dropAction.AddBinding("<Gamepad>/buttonWest");
    }

    void OnEnable()
    {
        dropAction.Enable();
    }

    void OnDisable()
    {
        dropAction.Disable();
    }

    void Update()
    {
        // G: elimde kutu varsa bırak
        if (dropAction.WasPressedThisFrame())
        {
            PutDownBox();
            return;
        }

        // E: elim boşsa ve yanımda kutu varsa al
        // elim doluyken E'ye bir şey yapmıyorum, benzin doldurma vs. E'yi kullanabilsin
        if (pickUpAction == null || !pickUpAction.WasPressedThisFrame()) return;
        if (IsCarrying()) return;
        if (nearbyBox == null) return;
        if (!nearbyBox.CanBePickedUp()) return;

        carried = nearbyBox;
        nearbyBox = null;
        carried.PickUp(carryPoint);
    }

    public bool IsCarrying()
    {
        return carried != null;
    }

    // bir kutunun alma alanına girince onu aklımda tutuyorum
    void OnTriggerEnter(Collider other)
    {
        ProductBox box = other.GetComponent<ProductBox>();
        if (box != null) nearbyBox = box;
    }

    // alandan çıkınca unutuyorum
    void OnTriggerExit(Collider other)
    {
        ProductBox box = other.GetComponent<ProductBox>();
        if (box != null && box == nearbyBox) nearbyBox = null;
    }

    // G ile bırakma. kutuyu önüme, biraz yukarıdan koyuyorum, kendi düşüp yere oturuyor
    public void PutDownBox()
    {
        if (!IsCarrying()) return;

        Vector3 spot = transform.position + transform.forward * putDownDistance + Vector3.up * 0.5f;
        Quaternion facing = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);  // kutu benim baktığım yöne dönük dursun

        carried.PutDown(spot, facing);
        carried = null;
    }

    // yağ gibi şeyler de bunu çağırıyor
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