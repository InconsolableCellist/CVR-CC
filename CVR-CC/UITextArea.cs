using System.Collections;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CVR_CC {
    /*
     * Main interface for displaying text on the user's UI.
     * Note that this class is static and is constructed only when needed
     * So uh, please don't need this before the UI Manager has finished initializing (though this should be impossible)
     */
    public static class UITextArea {
        private static readonly TextMeshProUGUI TextComponent;
        private static readonly Transform TextParent;

        public static string Text
        {
            get => TextComponent?.text ?? "";
            set
            {
                if (TextComponent != null)
                    TextComponent.text = value;
            }
        }

        static UITextArea() {
            // Hippity Hoppity, your UI elements are now my property
            var baseUserInterface = GameObject.Find("CohtmlHud").transform.parent.transform; 
            MelonLogger.Msg( baseUserInterface == null ? "Could not find the base user interface" : "Found the base user interface");
            GameObject ui = new GameObject("CVR_CC Text");
            ui.transform.parent = baseUserInterface;
            ui.transform.localScale = Vector3.one;
            ui.transform.localRotation = Quaternion.identity;
            ui.transform.position = Vector3.zero;
            ui.transform.localPosition = Vector3.zero;
            // ui.transform.localPosition = new Vector3(0, -350, 0);
            TextParent = ui.transform;
            
            ui.AddComponent<CanvasRenderer>();
            Canvas canvas = ui.AddComponent<Canvas>();
            // TODO: can use ScreenSpaceCamera to reduce draw impact when hidden?
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var text = ui.AddComponent<TextMeshProUGUI>();
            text.fontSize = 30;
            // text.color = new Color32(255, 255, 255, 255);
            // text.faceColor = new Color32(255, 255, 255, 255);
            text.alignment = TextAlignmentOptions.Bottom;
            text.margin += new Vector4(0, 0, 0, 200);
            text.faceColor = new Color32(255, 255,0, 255);
            text.outlineColor = new Color32(0, 0, 0, 255);
            text.outlineWidth = 0.1f;
            TextComponent = text;
            
            
            
        }

        public static IEnumerator DisplayAlert(string text, float timeInSeconds)
        {
            MelonLogger.Msg(text);
            Text = text;
            ToggleUI(true);
            yield return new WaitForSeconds(timeInSeconds);
            ToggleUI(false);
            Text = "";
        }
        
        public static void ToggleUI(bool newShownState) { 
            if (TextParent == null) { 
                // TODO: Whyyy
                MelonLogger.Msg("somehow textparent is null");
                return;
            }
            TextParent.gameObject.SetActive(newShownState);
        }
    }
}
