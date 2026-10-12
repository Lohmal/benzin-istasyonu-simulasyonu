using UnityEngine;

public class ArabaHareket : MonoBehaviour
{
    public Transform hedef;
    public float hiz = 2f;

    private bool hareketEdiyor = true;

    void Update()
    {
        if (!hareketEdiyor || hedef == null)
            return;

        // Arabanın yüksekliğini değiştirmeden hedefe git
        Vector3 hedefKonum = new Vector3(
            hedef.position.x,
            transform.position.y,
            hedef.position.z
        );

        // Hareket yönünü hesapla
        Vector3 yon = hedefKonum - transform.position;

        // Arabayı hedefe doğru çevir
        if (yon.sqrMagnitude > 0.001f)
        {
            Quaternion hedefRotasyon = Quaternion.LookRotation(yon);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                hedefRotasyon,
                5f * Time.deltaTime
            );
        }

        // Arabayı hareket ettir
        transform.position = Vector3.MoveTowards(
            transform.position,
            hedefKonum,
            hiz * Time.deltaTime
        );

        // Hedefe ulaştığında dur
        if (Vector3.Distance(transform.position, hedefKonum) < 0.05f)
        {
            hareketEdiyor = false;
        }
    }
}