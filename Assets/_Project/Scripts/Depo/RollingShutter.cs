using UnityEngine;

// Depo kepengini otomatik olarak açıp kapatır.
// Visual objesine takılır; kepengin üst kenarı sabit kalır, yüksekliği değişir.
public class RollingShutter : MonoBehaviour
{
    [SerializeField] private float openHeight = 0.1f;  // açıkken kalan yükseklik (metre)
    [SerializeField] private float speed = 1f;         // toplanma/inme hızı (metre/saniye)
    [SerializeField] private float waitTime = 2f;      // her uçta bekleme süresi (saniye)

    private float closedHeight;   // kapalıyken tam yükseklik
    private float topY;           // kepengin sabit kalan üst kenarı
    private bool opening = true;  // şu an açılıyor mu?
    private float waitTimer;      // kalan bekleme süresi

    void Start()
    {
        closedHeight = transform.localScale.y;                  // başlangıçtaki tam yükseklik (2.5)
        topY = transform.localPosition.y + closedHeight / 2f;   // üst kenarın konumunu hatırla
    }

    void Update()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;   // bekleme süresini azalt
            return;                        // beklerken Update'in geri kalanını atla
        }

        float target = opening ? openHeight : closedHeight;   // açılıyorsa hedef 0.1, kapanıyorsa 2.5

        Vector3 scale = transform.localScale;                                    // 1. kopyayı al
        scale.y = Mathf.MoveTowards(scale.y, target, speed * Time.deltaTime);    // 2. kopyayı değiştir
        transform.localScale = scale;                                            // 3. kopyayı geri ver

        Vector3 pos = transform.localPosition;
        pos.y = topY - scale.y / 2f;       // üst kenar sabit kalsın diye merkezi kaydır
        transform.localPosition = pos;

        if (scale.y == target)
        {
            opening = !opening;            // uca varınca yön değiştir
            waitTimer = waitTime;          // ve biraz bekle
        }
    }
}