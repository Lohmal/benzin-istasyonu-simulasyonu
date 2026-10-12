using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Oyuncu üstünden geçince toplanan coin. Değerini CoinWallet'a ekler ve kaybolur.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CoinPickup : MonoBehaviour
    {
        [Tooltip("Bu coin toplanınca kaç para kazandırır")]
        [SerializeField] int value = 10;

        public int Value
        {
            get => value;
            set => this.value = Mathf.Max(0, value);
        }

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            CoinWallet.Add(value);
            Destroy(gameObject);
        }
    }
}
