using UnityEngine;

public class OdemeAlani : MonoBehaviour
{
    [Header("Görsel")]
    [SerializeField] Renderer gorsel;      // Yeşil zeminin Renderer'ı
    [SerializeField] Material yesilMat;    // Normal renk buradan okunur
    [SerializeField] Material sariMat;     // Player alandayken renk buradan okunur

    [Header("Yanıp sönme")]
    [SerializeField] Color blinkColor = new Color(0.75f, 1f, 0.7f);
    [SerializeField] float blinksPerSecond = 2f;

    [Header("Ayarlar")]
    [SerializeField] string oyuncuTag = "Player";

    int colorId;
    MaterialPropertyBlock propertyBlock;
    Color normalColor;
    Color sariRenk;
    Collider alan;
    Transform oyuncu;

    bool musteriKasada;

    public void YanipSonmeyiBaslat() { musteriKasada = true; }
    public void YanipSonmeyiDurdur() { musteriKasada = false; }

    void Start()
    {
        if (gorsel == null) gorsel = GetComponent<Renderer>();
        alan = GetComponent<Collider>();
        propertyBlock = new MaterialPropertyBlock();

        if (yesilMat != null) gorsel.sharedMaterial = yesilMat;
        gorsel.enabled = true;

        Material m = gorsel.sharedMaterial;
        colorId = Shader.PropertyToID(m.HasProperty("_BaseColor") ? "_BaseColor" : "_Color");

        normalColor = m.GetColor(colorId);
        sariRenk = (sariMat != null && sariMat.HasProperty(colorId))
            ? sariMat.GetColor(colorId)
            : Color.yellow;

        SetColor(normalColor);
    }

    public bool OyuncuAlandaMi()
    {
        if (oyuncu == null)
        {
            GameObject go = GameObject.FindWithTag(oyuncuTag);
            if (go == null) return false;
            oyuncu = go.transform;
        }

        // Yüksekliği yok say: sadece yatayda (X, Z) kutunun içinde mi?
        Bounds b = alan.bounds;
        Vector3 p = oyuncu.position;
        p.y = b.center.y;
        return b.Contains(p);
    }

    void Update()
    {
        // 1) Player alandaysa: NPC'den bağımsız, sarı ve sabit
        if (OyuncuAlandaMi())
        {
            SetColor(sariRenk);
            return;
        }

        // 2) Player dışarıda, kasada müşteri varsa: yanıp sön
        if (musteriKasada)
        {
            float t = Mathf.PingPong(Time.time * blinksPerSecond * 2f, 1f);
            SetColor(Color.Lerp(normalColor * 0.6f, blinkColor, t));
            return;
        }

        // 3) Normal yeşil
        SetColor(normalColor);
    }

    void SetColor(Color color)
    {
        gorsel.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(colorId, color);
        gorsel.SetPropertyBlock(propertyBlock);
    }
}