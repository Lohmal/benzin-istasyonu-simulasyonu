using UnityEngine;

public class FircaDondur : MonoBehaviour
{
    public float donusHizi = 120f;
    public bool donuyor = false;

    void Update()
    {
        if (donuyor)
        {
            transform.Rotate(0f, donusHizi * Time.deltaTime, 0f);
        }
    }

    public void Baslat()
    {
        donuyor = true;
    }

    public void Durdur()
    {
        donuyor = false;
    }
}