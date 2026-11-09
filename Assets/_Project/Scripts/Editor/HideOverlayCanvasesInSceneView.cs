using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BenzinIstasyonu.EditorTools
{
    /// <summary>
    /// Screen Space - Overlay canvas'ları (HUD gibi) Scene görünümünde otomatik gizler.
    /// Overlay canvas Scene'de 1 piksel = 1 birim çizildiği için sahnenin üstünde dev bir çerçeve olarak durur.
    /// Sadece Scene görünümünü etkiler: Game görünümünde ve oyunda HUD normal görünür.
    /// Kapatıp açmak için: Tools > Benzin İstasyonu > Scene'de HUD'u Gizle
    /// </summary>
    [InitializeOnLoad]
    static class HideOverlayCanvasesInSceneView
    {
        const string PrefKey = "BenzinIstasyonu.HideOverlayCanvasesInSceneView";
        const string MenuPath = "Tools/Benzin İstasyonu/Scene'de HUD'u Gizle";

        static HideOverlayCanvasesInSceneView()
        {
            EditorSceneManager.sceneOpened += (scene, mode) => Apply();
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.EnteredPlayMode)
                    Apply();
            };
            // Editor açılışı / script derlemesi sonrası: sahne ve görünürlük durumu yüklendikten sonra uygula
            ApplyAfterFrames(10);
        }

        static void ApplyAfterFrames(int frames)
        {
            EditorApplication.CallbackFunction tick = null;
            tick = () =>
            {
                if (--frames > 0) return;
                EditorApplication.update -= tick;
                Apply();
            };
            EditorApplication.update += tick;
        }

        static bool Enabled
        {
            get => EditorPrefs.GetBool(PrefKey, true);
            set => EditorPrefs.SetBool(PrefKey, value);
        }

        [MenuItem(MenuPath)]
        static void Toggle()
        {
            Enabled = !Enabled;
            Apply();
        }

        [MenuItem(MenuPath, true)]
        static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, Enabled);
            return true;
        }

        static void Apply()
        {
            var visibility = SceneVisibilityManager.instance;
            foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (!canvas.isRootCanvas || canvas.renderMode != RenderMode.ScreenSpaceOverlay) continue;
                if (Enabled) visibility.Hide(canvas.gameObject, true);
                else visibility.Show(canvas.gameObject, true);
            }
        }
    }
}
