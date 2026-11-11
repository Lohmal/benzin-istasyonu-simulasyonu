using UnityEngine;

// koli tipi: hafif mi ağır mı. sayılar kodda değil bu dosyada duruyor
// Create menüsüne "Benzin İstasyonu > Koli Tipi" ekliyor
[CreateAssetMenu(menuName = "Benzin İstasyonu/Koli Tipi")]
public class BoxType : ScriptableObject
{
    [SerializeField] private string label = "";             // ipucunda yazsın diye. hafifte boş
    [SerializeField] private float speedMultiplier = 1f;    // taşırken yürüme hızı çarpanı
    [SerializeField] private float jumpMultiplier = 1f;     // taşırken zıplama çarpanı

    // başka scriptler okuyabilir ama değiştiremez
    public string Label()
    {
        return label;
    }

    public float SpeedMultiplier()
    {
        return speedMultiplier;
    }

    public float JumpMultiplier()
    {
        return jumpMultiplier;
    }
}