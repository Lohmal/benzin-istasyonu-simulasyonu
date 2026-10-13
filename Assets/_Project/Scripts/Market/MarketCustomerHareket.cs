using UnityEngine;

namespace Market
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class MarketCustomerHareket : MonoBehaviour
    {
        [System.Serializable]
        public class Nokta
        {
            public Transform hedef;
            [Tooltip("Bu noktaya varınca kaç saniye beklesin")]
            public float beklemeSuresi = 0f;
            [Tooltip("Bu noktaya doğru yürümeye başlarken ödeme alanı yanıp sönmeye başlasın")]
            public bool yurumeyeBaslarkenYanipSondur = false;
        }

        enum Durum { Yuruyor, Bekliyor, Kasada }

        [Header("Rota (ilki dışarıdaki başlangıç, sonuncusu kasanın önü)")]
        [SerializeField] Nokta[] path;

        [Header("Kasa")]
        [SerializeField] Transform faceTarget;      // Kasada dururken bakacağı obje
        [SerializeField] OdemeAlani odemeAlani;

        [Header("Ayarlar")]
        [SerializeField] float walkSpeed = 1.8f;
        [SerializeField] float turnSpeed = 540f;
        [SerializeField] float baslangicBekleme = 1f;

        Durum durum;
        int hedefIndex;
        float bekleSayaci;
        float baslangicSayaci;

        void Awake()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        void Start()
        {
            transform.position = path[0].hedef.position;
            transform.rotation = path[0].hedef.rotation;
            baslangicSayaci = baslangicBekleme;
            hedefIndex = 0;
            SonrakiNokta();
        }

        void Update()
        {
            if (baslangicSayaci > 0f)
            {
                baslangicSayaci -= Time.deltaTime;
                return;
            }

            switch (durum)
            {
                case Durum.Yuruyor:
                    if (MoveTo(path[hedefIndex].hedef.position))
                    {
                        float bekleme = path[hedefIndex].beklemeSuresi;
                        if (bekleme > 0f)
                        {
                            bekleSayaci = bekleme;
                            durum = Durum.Bekliyor;
                        }
                        else SonrakiNokta();
                    }
                    break;

                case Durum.Bekliyor:
                    // Noktanın baktığı yöne dön (rafa bak)
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation, path[hedefIndex].hedef.rotation, turnSpeed * Time.deltaTime);
                    bekleSayaci -= Time.deltaTime;
                    if (bekleSayaci <= 0f) SonrakiNokta();
                    break;

                case Durum.Kasada:
                    if (faceTarget != null) TurnTowards(faceTarget.position);
                    break;
            }
        }

        void SonrakiNokta()
        {
            hedefIndex++;
            if (hedefIndex >= path.Length)
            {
                durum = Durum.Kasada;
                return;
            }

            if (path[hedefIndex].yurumeyeBaslarkenYanipSondur && odemeAlani != null)
                odemeAlani.YanipSonmeyiBaslat();

            durum = Durum.Yuruyor;
        }

        bool MoveTo(Vector3 target)
        {
            TurnTowards(target);
            transform.position = Vector3.MoveTowards(transform.position, target, walkSpeed * Time.deltaTime);
            return (transform.position - target).sqrMagnitude < 0.0004f;
        }

        void TurnTowards(Vector3 point)
        {
            Vector3 direction = point - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) return;
            Quaternion look = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
        }
    }
}