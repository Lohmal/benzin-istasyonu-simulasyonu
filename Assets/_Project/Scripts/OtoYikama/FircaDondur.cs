using UnityEngine;

public class FircaDondur : MonoBehaviour
{
    public float donusHizi = 120f;

    void Update()
    {
        transform.Rotate(0f, donusHizi * Time.deltaTime, 0f);
    }
}