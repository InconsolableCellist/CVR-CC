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
        private static readonly Text TextComponent;
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

        static UITextArea()
        {
            // Hippity Hoppity, your UI elements are now my property
            var baseUserInterface = GameObject.Find("SystemLoadingHolder").transform.parent.transform;
            MelonLogger.Msg( baseUserInterface == null ? "Could not find the base user interface" : "Found the base user interface");
            GameObject ui = new GameObject("CVR_CC_UITextArea");
            ui.transform.parent = baseUserInterface;
            ui.transform.localScale = Vector3.one;
            ui.transform.localRotation = Quaternion.identity;
            ui.name = "CVR-CC Text";
            ui.transform.localPosition = new Vector3(0, -350, 0);
            TextParent = ui.transform;
            
            // textMeshPro.fontSize = 25; //TODO: Perhaps scale the font size depending on how much text is being rendered
            /*
            TextMeshProUGUI textMeshPro = null;
           
            foreach (var obj in (TextMeshProUGUI[]) Resources.FindObjectsOfTypeAll(typeof(TextMeshProUGUI))) {
                if (obj.name == "Now loading text") {
                    MelonLogger.Msg("Found directly");
                    textMeshPro = Object.Instantiate(obj, ui.transform);
                    textMeshPro.name = "CVR-CC Text";
                    break;
                } else if (obj.transform.parent.name == "Now loading text") { 
                    MelonLogger.Msg("Found in parent");
                    textMeshPro = Object.Instantiate(obj, ui.transform);
                    textMeshPro.name = "CVR-CC Text";
                    break;
                }
            }
            
            if (textMeshPro == null) {
                MelonLogger.Error("Could not find the subtitle template object");
                return;
            }
            MelonLogger.Msg("Successfully set the TextMeshPro text output object");
            TextComponent = textMeshPro;
            */
            
            TextComponent = ui.AddComponent<Text>(); 
            TextComponent.name = "CVR-CC Text";
            TextComponent.fontSize = 25;
            
            
            
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
        
        public static void ToggleUI(bool newShownState) => TextParent.gameObject.SetActive(newShownState);
    }
}
