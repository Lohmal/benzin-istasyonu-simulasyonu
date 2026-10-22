using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OtoAraba : MonoBehaviour
{
    [HideInInspector] public Transform[] yol;

    public float hiz = 5f;
    public float donusHizi = 120f;
    public int kapiOnuIndex = 0;       // bariyer trigger'ının içindeki nokta
    public float kapiBeklemeSuresi = 1.5f;
    public int parkIndex = 2;          // park yerindeki nokta
    public float parkSuresi = 10f;

    void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Start()
    {
        StartCoroutine(Git());
    }

    IEnumerator Git()
    {
        for (int i = 0; i < yol.Length; i++)
        {
            Vector3 hedef = yol[i].position;
            hedef.y = transform.position.y;

            while (Vector3.Distance(transform.position, hedef) > 0.3f)
            {
                Vector3 yon = hedef - transform.position;
                Quaternion bakis = Quaternion.LookRotation(yon);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, bakis, donusHizi * Time.deltaTime);
                transform.position = Vector3.MoveTowards(
                    transform.position, hedef, hiz * Time.deltaTime);
                yield return null;
            }

            if (i == kapiOnuIndex) yield return new WaitForSeconds(kapiBeklemeSuresi);
            if (i == parkIndex) yield return new WaitForSeconds(parkSuresi);
        }

        Destroy(gameObject);
    }
}