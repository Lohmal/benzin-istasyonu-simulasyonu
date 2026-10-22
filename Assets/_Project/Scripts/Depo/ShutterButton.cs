using UnityEngine;
using UnityEngine.InputSystem;

// kepenk düğmesi. yanına gelip E'ye basınca kepengi açıyor/kapatıyor
public class ShutterButton : MonoBehaviour
{
    [SerializeField] private RollingShutter shutter;   // hangi kepengi açacak
    [SerializeField] private Transform buttonCap;      // kırmızı kısım, basınca içeri giriyor
    [SerializeField] private AudioClip clickSound;     // basınca tık

    private InputAction interactAction;
    private AudioSource audioSource;
    private bool playerNear;
    private Vector3 capStartPos;
    private float pressTimer;

    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
        audioSource = GetComponent<AudioSource>();
        if (buttonCap != null) capStartPos = buttonCap.localPosition;
    }

    void Update()
    {
        // basılı görünme süresi bitince düğme geri çıksın
        if (pressTimer > 0f)
        {
            pressTimer -= Time.deltaTime;
            if (pressTimer <= 0f && buttonCap != null) buttonCap.localPosition = capStartPos;
        }

        if (!playerNear) return;
        if (interactAction == null || !interactAction.WasPressedThisFrame()) return;

        shutter.Toggle();

        if (clickSound != null && audioSource != null) audioSource.PlayOneShot(clickSound);

        // düğme biraz içeri girsin, basıldığı belli olsun
        if (buttonCap != null)
        {
            buttonCap.localPosition = capStartPos + new Vector3(0f, 0f, 0.025f);
            pressTimer = 0.15f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerNear = false;
    }

    // yanındayken ne yapacağını yazsın, kepenk açıksa kapat, kapalıysa aç
    void OnGUI()
    {
        if (!playerNear) return;

        string text = shutter.IsOpen() ? "[E] Kepengi Kapat" : "[E] Kepengi Aç";
        InteractHint.Draw(transform.position + Vector3.up * 0.5f, text);
    }
}