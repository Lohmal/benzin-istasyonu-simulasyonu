using UnityEngine;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Cafe müşterisi: dışarıdan kapıya, oradan kasanın önüne yürür ve servis edilmeyi bekler.
    /// Servis bitince aynı yoldan çıkar, bir süre görünmez kalır ve tekrar gelir.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class CafeCustomer : MonoBehaviour
    {
        public enum State { Away, WalkingIn, Waiting, BeingServed, WalkingOut }

        [Tooltip("Yürüyeceği noktalar: ilki dışarıdaki başlangıç, sonuncusu kasanın önü")]
        [SerializeField] Transform[] path;

        [Tooltip("Kasanın önünde dururken yüzünü döneceği obje (kasa)")]
        [SerializeField] Transform faceTarget;

        [Tooltip("Yürüme hızı (metre/saniye)")]
        [SerializeField] float walkSpeed = 2.5f;

        [Tooltip("Dönüş hızı (derece/saniye)")]
        [SerializeField] float turnSpeed = 540f;

        [Tooltip("Gittikten kaç saniye sonra tekrar gelsin")]
        [SerializeField] float returnDelay = 5f;

        [Tooltip("Müşterinin görünen kısmı (gidince gizlenir)")]
        [SerializeField] GameObject visual;

        [Tooltip("Başının üstündeki ilerleme çubuğu")]
        [SerializeField] WorldProgressBar progressBar;

        [Tooltip("Başının üstündeki \"Tıkla\" ipucu")]
        [SerializeField] GameObject hint;

        public State Current { get; private set; }

        /// <summary>Kasanın önünde servis bekliyor mu?</summary>
        public bool IsWaiting => Current == State.Waiting;

        Collider body;
        int targetIndex;
        float awayTimer;

        void Awake()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;   // hareketi bu script yapar; fizik itip düşürmesin
            rb.useGravity = false;
            body = GetComponent<Collider>();
            ShowProgress(false);
            ShowHint(false);
        }

        void Start()
        {
            WalkIn();
        }

        void Update()
        {
            switch (Current)
            {
                case State.WalkingIn:
                    if (MoveTo(path[targetIndex].position))
                    {
                        targetIndex++;
                        if (targetIndex >= path.Length) Current = State.Waiting;
                    }
                    break;

                case State.Waiting:
                case State.BeingServed:
                    if (faceTarget != null) TurnTowards(faceTarget.position);
                    break;

                case State.WalkingOut:
                    if (MoveTo(path[targetIndex].position))
                    {
                        targetIndex--;
                        if (targetIndex < 0) GoAway();
                    }
                    break;

                case State.Away:
                    awayTimer -= Time.deltaTime;
                    if (awayTimer <= 0f) WalkIn();
                    break;
            }
        }

        /// <summary>Kasiyer siparişi almaya başladı.</summary>
        public void BeginService()
        {
            if (!IsWaiting) return;
            Current = State.BeingServed;
            ShowHint(false);
            ShowProgress(true);
            progressBar.SetProgress(0f);
        }

        /// <summary>Siparişin ne kadarının hazır olduğu (0-1).</summary>
        public void SetServiceProgress(float t)
        {
            if (progressBar != null) progressBar.SetProgress(t);
        }

        /// <summary>Sipariş bitti: müşteri geldiği yoldan çıkar.</summary>
        public void FinishService()
        {
            ShowProgress(false);
            Current = State.WalkingOut;
            targetIndex = path.Length - 2;
        }

        public void ShowHint(bool on)
        {
            if (hint != null && hint.activeSelf != on) hint.SetActive(on);
        }

        void WalkIn()
        {
            transform.position = path[0].position;
            SetVisible(true);
            targetIndex = 1;
            Current = State.WalkingIn;
        }

        void GoAway()
        {
            SetVisible(false);
            awayTimer = returnDelay;
            Current = State.Away;
        }

        // Hedefe doğru döner ve yürür; vardığında true döner.
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

        void SetVisible(bool on)
        {
            if (visual != null) visual.SetActive(on);
            body.enabled = on;   // görünmezken kapıyı açmasın, kasada sayılmasın
            if (!on)
            {
                ShowProgress(false);
                ShowHint(false);
            }
        }

        void ShowProgress(bool on)
        {
            if (progressBar != null && progressBar.gameObject.activeSelf != on) progressBar.gameObject.SetActive(on);
        }
    }
}
