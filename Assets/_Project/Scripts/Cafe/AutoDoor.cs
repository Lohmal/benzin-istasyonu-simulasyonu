using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kapının önündeki algılama alanına biri girince iki kanadı yana kaydırarak açar,
/// alanda kimse kalmayınca kapatır. Bu script, kapının görünmez trigger objesine eklenir.
/// </summary>
[RequireComponent(typeof(Collider))]
public class AutoDoor : MonoBehaviour
{
    [Tooltip("Sola açılan kanat")]
    [SerializeField] Transform leftPanel;

    [Tooltip("Sağa açılan kanat")]
    [SerializeField] Transform rightPanel;

    [Tooltip("Her kanadın ne kadar kayacağı (metre)")]
    [SerializeField] float openDistance = 1.9f;

    [Tooltip("Açılma/kapanma süresi (saniye)")]
    [SerializeField] float openDuration = 0.4f;

    [Tooltip("Bu etiketlere (tag) sahip objeler kapıyı açar")]
    [SerializeField] string[] openerTags = { "Player", "Customer" };

    // Algılama alanındaki kişiler. Sayaç yerine liste tutulur; çünkü alanın içindeyken
    // yok edilen ya da kapatılan bir obje için Unity OnTriggerExit göndermez.
    readonly HashSet<Collider> peopleInside = new HashSet<Collider>();
    Collider sensor;
    Vector3 leftClosedPosition;
    Vector3 rightClosedPosition;
    float openAmount;   // 0 = kapalı, 1 = tam açık

    /// <summary>Kapı şu an açık mı (ya da açılıyor mu)?</summary>
    public bool IsOpen => peopleInside.Count > 0;

    void Awake()
    {
        sensor = GetComponent<Collider>();
        sensor.isTrigger = true;
        leftClosedPosition = leftPanel.localPosition;
        rightClosedPosition = rightPanel.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsOpener(other)) peopleInside.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        peopleInside.Remove(other);
    }

    void Update()
    {
        // Yok edilen, kapatılan ya da alanın dışına ışınlanan kişileri listeden çıkar.
        peopleInside.RemoveWhere(c => c == null || !c.enabled || !c.gameObject.activeInHierarchy
                                      || !sensor.bounds.Intersects(c.bounds));

        float target = IsOpen ? 1f : 0f;
        openAmount = Mathf.MoveTowards(openAmount, target, Time.deltaTime / openDuration);

        // SmoothStep ile hareket başta ve sonda yavaşlar, daha doğal görünür.
        float slide = Mathf.SmoothStep(0f, 1f, openAmount) * openDistance;
        leftPanel.localPosition = leftClosedPosition + Vector3.left * slide;
        rightPanel.localPosition = rightClosedPosition + Vector3.right * slide;
    }

    bool IsOpener(Collider other)
    {
        foreach (string tag in openerTags)
            if (other.CompareTag(tag)) return true;
        return false;
    }
}
