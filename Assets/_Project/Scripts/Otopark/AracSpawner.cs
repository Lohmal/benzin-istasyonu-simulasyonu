using System.Collections;
using UnityEngine;

public class AracSpawner : MonoBehaviour
{
    public GameObject aracPrefab;
    public Transform[] yol;            // sırayla: kapı önü, kapıdan sonra, park yeri, çıkış
    public Transform dogmaNoktasi;
    public float araliksn = 6f;
    public int maxAraba = 5;

    private int aktif;

    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(araliksn);
            if (aktif >= maxAraba) continue;

            GameObject arac = Instantiate(aracPrefab, dogmaNoktasi.position, dogmaNoktasi.rotation);
            var oto = arac.GetComponent<OtoAraba>();
            oto.yol = yol;
            aktif++;
            StartCoroutine(Takip(arac));
        }
    }

    IEnumerator Takip(GameObject arac)
    {
        while (arac != null) yield return null;
        aktif--;
    }
}