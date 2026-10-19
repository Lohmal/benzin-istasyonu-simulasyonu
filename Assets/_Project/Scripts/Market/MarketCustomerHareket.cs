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
            [Tooltip("Bu noktaya VARINCA ödeme alanı yanıp sönmeye başlasın")]
            public bool varincaYanipSondur = false;
        }

        enum Durum { Yuruyor, Bekliyor, Kasada, Serviste, Cikiyor, Uzakta }

        [Header("Rota (ilki dışarıdaki başlangıç, sonuncusu kasanın önü)")]
        [SerializeField] Nokta[] path;

        [Header("Kasa")]
        [SerializeField] Transform faceTarget;
        [SerializeField] OdemeAlani odemeAlani;

        [Header("Görünüm")]
        [Tooltip("Karakterin Visual objesi (gidince gizlenir)")]
        [SerializeField] GameObject visual;
        [Tooltip("Başının üstündeki ilerleme çubuğu (opsiyonel)")]
        [SerializeField] BenzinIstasyonu.WorldProgressBar progressBar;
        [Tooltip("Başının üstündeki \"Tıkla\" ipucu (opsiyonel)")]
        [SerializeField] GameObject hint;

        [Header("Ayarlar")]
        [SerializeField] float walkSpeed = 1.8f;
        [SerializeField] float turnSpeed = 540f;
        [SerializeField] float baslangicBekleme = 1f;
        [SerializeField] bool tekrarGel = true;
        [SerializeField] float tekrarGelmeSuresi = 5f;

        Durum durum;
        int hedefIndex;
        float bekleSayaci;
        float baslangicSayaci;
        float uzaktaSayaci;
        Collider body;

        public bool IsWaiting => durum == Durum.Kasada;

        void Awake()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            body = GetComponent<Collider>();
            ShowProgress(false);
            ShowHint(false);
        }

        void Start()
        {
            baslangicSayaci = baslangicBekleme;
            GirisiBaslat();
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
                        if (path[hedefIndex].varincaYanipSondur && odemeAlani != null)
                            odemeAlani.YanipSonmeyiBaslat();

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
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation, path[hedefIndex].hedef.rotation, turnSpeed * Time.deltaTime);
                    bekleSayaci -= Time.deltaTime;
                    if (bekleSayaci <= 0f) SonrakiNokta();
                    break;

                case Durum.Kasada:
                case Durum.Serviste:
                    if (faceTarget != null) TurnTowards(faceTarget.position);
                    break;

                case Durum.Cikiyor:
                    // Ara noktalara uğramadan doğrudan dış noktaya yürü
                    if (MoveTo(path[0].hedef.position))
                        Uzaklas();
                    break;

                case Durum.Uzakta:
                    if (!tekrarGel) break;
                    uzaktaSayaci -= Time.deltaTime;
                    if (uzaktaSayaci <= 0f) GirisiBaslat();
                    break;
            }
        }

        // ---- Kasa istasyonunun çağırdıkları ----

        public void BeginService()
        {
            if (!IsWaiting) return;
            durum = Durum.Serviste;
            ShowHint(false);
            ShowProgress(true);
            if (progressBar != null) progressBar.SetProgress(0f);
        }

        public void SetServiceProgress(float t)
        {
            if (progressBar != null) progressBar.SetProgress(t);
        }

        public void FinishService()
        {
            ShowProgress(false);
            if (odemeAlani != null) odemeAlani.YanipSonmeyiDurdur();
            durum = Durum.Cikiyor;
        }

        public void ShowHint(bool on)
        {
            if (hint != null && hint.activeSelf != on) hint.SetActive(on);
        }

        // ---- İç işler ----

        void GirisiBaslat()
        {
            transform.position = path[0].hedef.position;
            transform.rotation = path[0].hedef.rotation;
            SetVisible(true);
            hedefIndex = 0;
            SonrakiNokta();
        }

        void SonrakiNokta()
        {
            hedefIndex++;
            if (hedefIndex >= path.Length)
            {
                hedefIndex = path.Length - 1;
                durum = Durum.Kasada;
                return;
            }
            durum = Durum.Yuruyor;
        }

        void Uzaklas()
        {
            SetVisible(false);
            uzaktaSayaci = tekrarGelmeSuresi;
            durum = Durum.Uzakta;
        }

        void SetVisible(bool on)
        {
            if (visual != null) visual.SetActive(on);
            body.enabled = on;   // görünmezken kapıyı açmasın
            if (!on)
            {
                ShowProgress(false);
                ShowHint(false);
            }
        }

        void ShowProgress(bool on)
        {
            if (progressBar != null && progressBar.gameObject.activeSelf != on)
                progressBar.gameObject.SetActive(on);
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