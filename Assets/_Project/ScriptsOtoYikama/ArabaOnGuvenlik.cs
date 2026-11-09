
using UnityEngine;

public class ArabaOnGuvenlik : MonoBehaviour
{
    public bool playerOnde = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnde = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnde = false;
        }
    }

}
