using System.Collections.Generic;
using UnityEngine;
using BenzinIstasyonu;

public class GateTrigger : MonoBehaviour
{
    public BoomGateController gate;

    [Header("Coin")]
    public GameObject coinPrefab;       // içinde CoinPickup olan prefab
    public Transform coinNoktasi;       // yeşil alan objesi
    public int aracBasinaCoin = 10;
    public float dagilma = 1f;          // coinler yeşil alanda ne kadar yayılsın

    private readonly HashSet<GameObject> icerdekiler = new HashSet<GameObject>();

    private static GameObject KokBul(Collider c)
    {
        return c.attachedRigidbody ? c.attachedRigidbody.gameObject : c.gameObject;
    }

    private static bool Gecerli(GameObject o)
    {
        return o.CompareTag("Player") || o.CompareTag("Car");
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject kok = KokBul(other);
        if (!Gecerli(kok)) return;

        // Add true dönerse obje listeye yeni girdi
        if (icerdekiler.Add(kok) && kok.CompareTag("Car"))
            CoinBirak();
    }

    private void OnTriggerExit(Collider other)
    {
        icerdekiler.Remove(KokBul(other));
    }

    private void CoinBirak()
    {
        if (coinPrefab == null || coinNoktasi == null) return;

        Vector3 konum = coinNoktasi.position + new Vector3(
            Random.Range(-dagilma, dagilma), 0.5f, Random.Range(-dagilma, dagilma));

        GameObject coin = Instantiate(coinPrefab, konum, coinPrefab.transform.rotation);

        CoinPickup pickup = coin.GetComponent<CoinPickup>();
        if (pickup != null) pickup.Value = aracBasinaCoin;
    }

    private void Update()
    {
        icerdekiler.RemoveWhere(o => o == null || !o.activeInHierarchy);

        if (gate == null) return;

        if (icerdekiler.Count > 0) gate.OpenGate();
        else gate.CloseGate();
    }
}