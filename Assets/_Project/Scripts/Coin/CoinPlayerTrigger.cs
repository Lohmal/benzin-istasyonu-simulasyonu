using UnityEngine;

public class CoinPlayerTrigger : MonoBehaviour
{
    [SerializeField] private string message = "Coin alýndý!";
    [SerializeField] private GameObject vehicle;

    private void OnTriggerEnter(Collider other)
    {
        // Sadece oyuncu deðdiðinde çalýþýr
        if (!other.CompareTag("Player")) return;

        // Consola mesaj yazdýr
        Debug.Log(message);

        // Araba hareketini tekrar aç
        if (vehicle != null)
        {
            MonoBehaviour follower = vehicle.GetComponent("CarPathFollower") as MonoBehaviour;
            if (follower != null)
            {
                follower.enabled = true;
            }
        }

        //Destroy etmek yerine SetActive(false) ile gizliyoruz ki sonra tekrar açýlabilsin
        gameObject.SetActive(false);
    }
}