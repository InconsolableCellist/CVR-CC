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
            // var baseUserInterface = GameObject.Find("CohtmlHud").transform.parent.transform; 
            var baseUserInterface = GameObject.Find("[CameraRigDesktop]").transform.parent.transform;
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
            // canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.renderMode = RenderMode.WorldSpace;
            
            canvas.GetComponent<Canvas>().worldCamera = Camera.main;
            canvas.GetComponent<Canvas>().sortingOrder = 1;
            canvas.GetComponent<Canvas>().sortingLayerName = "UI";
            canvas.GetComponent<Canvas>().pixelPerfect = true;
            canvas.GetComponent<Canvas>().planeDistance = 1;
            canvas.GetComponent<Canvas>().overrideSorting = true;
            
            
            var text = ui.AddComponent<TextMeshProUGUI>();
            text.fontSize = 30;
            // text.color = new Color32(255, 255, 255, 255);
            // text.faceColor = new Color32(255, 255, 255, 255);
            text.alignment = TextAlignmentOptions.Bottom;
            text.margin += new Vector4(0, 0, 0, 200);
            text.faceColor = new Color32(255, 255,0, 255);
            text.outlineColor = new Color32(0, 0, 0, 255);
            text.outlineWidth = 0.1f;
            
            text.fontSharedMaterial.SetFloat("_Emissive_Color_G", 1);
            text.fontSharedMaterial.SetFloat("_Emissive_Color_B", 1);
            text.fontSharedMaterial.SetFloat("_Emissive_Color_R", 1);
            text.fontSharedMaterial.SetFloat("_Emissive_Color_A", 1);
            text.fontSharedMaterial.SetFloat("_Emissive_Color_Power", 1);
            text.fontSharedMaterial.EnableKeyword("_EMISSION");
            text.fontSharedMaterial.EnableKeyword("_EMISSION_COLOR");
            text.fontSharedMaterial.EnableKeyword("_EMISSION_TEXTURE");
            text.fontSharedMaterial.EnableKeyword("_EMISSION_TEXTURE_MASK");
            text.fontSharedMaterial.EnableKeyword("_EMISSION_TEXTURE_MASK_SOFT");
            
            TextComponent = text;
            
            // add a debug line 1 unit length to the baseUserInterface
            var debugLine = new GameObject("Debug Line"); 
            debugLine.transform.parent = baseUserInterface;
            debugLine.transform.localScale = Vector3.one;
            debugLine.transform.localRotation = Quaternion.identity;
            debugLine.transform.position = Vector3.zero;
            debugLine.transform.localPosition = Vector3.zero;
            debugLine.AddComponent<CanvasRenderer>();
            var line = debugLine.AddComponent<LineRenderer>();
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.startColor = Color.red;
            line.endColor = Color.red;
            line.positionCount = 2;
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.forward * 1);
            
            // add a GameObject cube 1 unit length to the center of the baseUserInterface
            var cube = new GameObject("Cube");
            cube.transform.parent = baseUserInterface;
            cube.transform.localScale = Vector3.one;
            cube.transform.localRotation = Quaternion.identity;
            cube.transform.position = Vector3.zero;
            cube.transform.localPosition = Vector3.zero;
            cube.AddComponent<CanvasRenderer>();
            var mesh = cube.AddComponent<MeshFilter>();
            mesh.mesh = new Mesh();
            mesh.mesh.vertices = new Vector3[] {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, 0.5f)
            };
            mesh.mesh.triangles = new int[] {
                0, 1, 2,
                1, 3, 2,
                4, 6, 5,
                5, 6, 7,
                0, 2, 4,
                4, 2, 6,
                1, 5, 3,
                3, 5, 7,
                0, 4, 1,
                1, 4, 5,
                3, 7, 2,
                2, 7, 6
            };
            mesh.mesh.RecalculateNormals();
            mesh.mesh.RecalculateBounds();
            mesh.mesh.RecalculateTangents();
            // set color of mesh to red
            var color = new Color32(255, 0, 0, 255);
            var colors = new Color32[mesh.mesh.vertexCount];
            for (int i = 0; i < mesh.mesh.vertexCount; i++)
                colors[i] = color;
           
            
            
            
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
