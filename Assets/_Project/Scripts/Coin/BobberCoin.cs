using UnityEngine;

public class BobberCoin : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.25f; // Yukarý aþaðý kaç metre yüzeceði
    [SerializeField] private float bobSpeed = 2f;      // Ne kadar hýzlý salýnacaðý

    private Vector3 startLocalPosition;

    void Start()
    {
        // Baþlangýç yerel konumunu kaydet
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Slayt kuralý: Mathf.Sin ile saat gibi periyodik yüzme (deltaTime gerekmez)
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startLocalPosition + Vector3.up * offset;
    }
}