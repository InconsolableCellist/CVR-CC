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
            Transform baseUserInterface = null;
            try { 
                GameObject cameraRig = GameObject.Find("[CameraRigDesktop]");
                if (cameraRig != null && cameraRig.activeSelf)  { 
                    MelonLogger.Msg("CVR-CC initializing for desktop mode");
                    baseUserInterface = cameraRig.transform;
                } else { 
                    MelonLogger.Msg("CVR-CC initializing in VR mode");
                    baseUserInterface = GameObject.Find("[CameraRigVR]").transform;
                }
                baseUserInterface = baseUserInterface.Find("Camera").Find("Canvas").transform;
            } catch (System.Exception e) {
                MelonLogger.Error("Could not find base user interface. " + e);
                return;
            }
            
            TextMeshProUGUI text = baseUserInterface.gameObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = 32;
            text.alignment = TextAlignmentOptions.Bottom;
            text.margin += new Vector4(0, 0, 0, 200);
            text.faceColor = new Color32(255, 255,0, 255);
            text.outlineColor = new Color32(0, 0, 0, 255);
            text.outlineWidth = 0.1f;
            
            TextComponent = text;
            TextParent = baseUserInterface;
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
                // TODO: Whyyy. Gets called each update
                // MelonLogger.Msg("somehow textparent is null");
                return;
            }
            TextParent.gameObject.SetActive(newShownState);
        }
    }
}
