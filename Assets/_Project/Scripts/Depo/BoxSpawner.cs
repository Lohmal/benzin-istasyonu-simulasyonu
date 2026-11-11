using UnityEngine;

// depodaki koli noktası. üstündeki koli alınınca biraz bekleyip yenisini koyuyor
public class BoxSpawner : MonoBehaviour
{
    [SerializeField] private GameObject lightBoxPrefab;   // normal koli
    [SerializeField] private GameObject heavyBoxPrefab;   // ağır koli (B kısmında dolduracağız)
    [SerializeField] [Range(0f, 1f)] private float heavyChance = 0.3f;  // ağır gelme ihtimali
    [SerializeField] private float delay = 5f;            // koli alındıktan sonra kaç saniye boş kalsın

    private float timer;   // yeni koliye kalan süre. 0'dan başlıyor, yani ilk koli hemen geliyor

    void Update()
    {
        // üstümde hâlâ koli duruyorsa bekle
        if (GetComponentInChildren<ProductBox>() != null) return;

        // geri say
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        // süre doldu. ağır prefab varsa zar at, yoksa hep hafif
        GameObject prefab = lightBoxPrefab;
        if (heavyBoxPrefab != null && Random.value < heavyChance) prefab = heavyBoxPrefab;

        // kopyayı benim yerime, benim çocuğum olarak koy
        Instantiate(prefab, transform.position, transform.rotation, transform);
        timer = delay;
    }
}