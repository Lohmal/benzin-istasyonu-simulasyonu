using UnityEngine;

// "[E] Al" gibi etkileşim yazıları. benzinlikteki "[E] Doldur" ile aynı dursun diye onun gibi OnGUI ile çiziyorum
// kutu da düğme de bunu kullanıyor, her yerde aynı kodu yazmayayım diye ayırdım
public static class InteractHint
{
    private static GUIStyle style;

    // sadece OnGUI'nin içinden çağrılmalı
    public static void Draw(Vector3 worldPos, string text)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
        if (screenPos.z <= 0f) return;   // kameranın arkasındaysa çizme

        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 13;
            style.fontStyle = FontStyle.Bold;
        }

        float w = 160f;
        float h = 22f;
        float x = screenPos.x - w / 2f;
        float y = Screen.height - screenPos.y - h / 2f;   // GUI'de y yukarıdan başlıyor, ters çeviriyorum

        // arkasına siyah gölge, açık zeminde de okunsun
        style.normal.textColor = Color.black;
        GUI.Label(new Rect(x + 1, y + 1, w, h), text, style);

        style.normal.textColor = Color.white;
        GUI.Label(new Rect(x, y, w, h), text, style);
    }
    // ekranın sol altına sabit yazı, arkasında yarı saydam koyu kutu var
    // elde bir şey varken ne yapabileceğini göstermek için
    public static void DrawCorner(string text)
    {
        float w = 100f;
        float h = 26f;
        Rect rect = new Rect(16f, Screen.height - h - 16f, w, h);

        GUI.color = new Color(0f, 0f, 0f, 0.55f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle cornerStyle = new GUIStyle(GUI.skin.label);
        cornerStyle.alignment = TextAnchor.MiddleCenter;
        cornerStyle.fontSize = 12;
        cornerStyle.fontStyle = FontStyle.Bold;
        cornerStyle.normal.textColor = Color.white;

        GUI.Label(rect, text, cornerStyle);
    }
}