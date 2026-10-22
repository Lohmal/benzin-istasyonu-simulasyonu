using UnityEngine;

// depo kepengi. eskiden kendi kendine açılıp kapanıyordu, artık düğmeyle çalışıyor
// Visual objesine takılı, üst kenar sabit kalıyor, sadece yüksekliği değişiyor
public class RollingShutter : MonoBehaviour
{
    [SerializeField] private float openHeight = 0.1f;   // açıkken yukarıda kalan kısım
    [SerializeField] private float speed = 1.5f;        // inip kalkma hızı (metre/saniye)
    [SerializeField] private bool startOpen = false;    // oyun başında açık mı olsun

    [SerializeField] private AudioClip moveSound;       // hareket ederken dönen tıkırtı
    [SerializeField] private AudioClip stopSound;       // yerine oturunca

    private float closedHeight;   // kapalıyken tam boy
    private float topY;           // üst kenarın yeri, bu hiç değişmiyor
    private bool isOpen;
    private bool moving;          // dururken ses kesilsin diye hareket ediyor mu bakıyorum
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        closedHeight = transform.localScale.y;
        topY = transform.localPosition.y + closedHeight / 2f;

        isOpen = startOpen;
        if (isOpen) SetHeight(openHeight);   // açık başlayacaksa direkt açık koy, animasyon olmasın
    }

    void Update()
    {
        float target = isOpen ? openHeight : closedHeight;
        if (transform.localScale.y == target) return;   // zaten yerindeyse bir şey yapma

        float height = Mathf.MoveTowards(transform.localScale.y, target, speed * Time.deltaTime);
        SetHeight(height);

        // tam yerine vardığı kare
        if (height == target && moving)
        {
            moving = false;
            if (audioSource != null) audioSource.Stop();   // tıkırtıyı kes
            PlaySound(stopSound);                           // yerine oturma sesi
        }
    }

    // düğme bunu çağırıyor. kepenk hareket ederken basılırsa da geri dönüyor
    public void Toggle()
    {
        isOpen = !isOpen;
        moving = true;

        // hareket sesi kepenk durana kadar dönsün
        if (moveSound != null && audioSource != null)
        {
            audioSource.clip = moveSound;
            audioSource.loop = true;
            if (!audioSource.isPlaying) audioSource.Play();
        }
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    // boyu değiştirip üst kenarı yerinde tutuyor
    private void SetHeight(float height)
    {
        Vector3 scale = transform.localScale;
        scale.y = height;
        transform.localScale = scale;

        Vector3 pos = transform.localPosition;
        pos.y = topY - height / 2f;
        transform.localPosition = pos;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null) audioSource.PlayOneShot(clip);
    }
}