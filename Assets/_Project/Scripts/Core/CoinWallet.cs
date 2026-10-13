using System;
using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Oyuncunun coin bakiyesi. Para kazandıran her şey CoinWallet.Add(...) çağırır,
    /// satın alma noktaları CoinWallet.TrySpend(...) ile harcar. Ekrandaki sayaç Changed olayını dinler.
    /// </summary>
    public static class CoinWallet
    {
        /// <summary>Şu anki coin miktarı.</summary>
        public static int Coins { get; private set; }

        /// <summary>Bakiye her değiştiğinde yeni değerle çağrılır.</summary>
        public static event Action<int> Changed;

        public static void Add(int amount)
        {
            if (amount <= 0) return;
            Coins += amount;
            Changed?.Invoke(Coins);
        }

        /// <summary>Yeterli coin varsa harcar ve true döner; yoksa hiçbir şey yapmaz.</summary>
        public static bool TrySpend(int amount)
        {
            if (amount < 0 || Coins < amount) return false;
            Coins -= amount;
            Changed?.Invoke(Coins);
            return true;
        }

        // Play'e her girişte bakiye sıfırdan başlasın (kayıt sistemi gelene kadar).
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetOnPlay()
        {
            Coins = 0;
            Changed = null;
        }
    }
}
