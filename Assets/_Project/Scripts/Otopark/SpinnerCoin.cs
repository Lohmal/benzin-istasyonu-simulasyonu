using UnityEngine;

public class SpinnerCoin : MonoBehaviour
{
    // degrees per SECOND
    [SerializeField] private float rotationSpeed = 90f;

    void Update()
    {
        // Slayt kuralý: Kendi etrafýnda Y ekseninde deltaTime ile dön
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
