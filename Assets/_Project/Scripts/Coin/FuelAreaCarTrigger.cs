using UnityEngine;

public class FuelAreaCarTrigger : MonoBehaviour
{
    [SerializeField] private GameObject myCoin;

    private void OnTriggerEnter(Collider other)
    {
        // Slayt Mantýðý: Gelen obje araç ise
        if (other.CompareTag("Vehicle") || other.name.Contains("MusteriArabasi"))
        {
            // Arabanýn hareketini durdur
            MonoBehaviour follower = other.GetComponent("CarPathFollower") as MonoBehaviour;
            if (follower != null)
            {
                follower.enabled = false;
            }

            // Coin'i tekrar görünür/aktif yap (tur atýp geldiyse de çalýþýr)
            if (myCoin != null)
            {
                myCoin.SetActive(true);
            }
        }
    }
}