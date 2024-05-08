using MelonLoader;
using UnityEngine;
using Il2Cpp;
using Il2CppSystem.Net.Configuration;
namespace CaveDestroyer
{
    public class CaveDestroyer : MelonMod
    {
        private bool _isMenuOpen = false;
        private int _selectedCategory = 0;
        private Rect windowRect = new Rect(10, 10, 400, 800);

        public override void OnUpdate()
        {
            // Si l'utilisateur appuie sur la touche Insert, ouvrez ou fermez le menu
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                _isMenuOpen = !_isMenuOpen;
            }
            Modules.RunModules();


        }

        public override void OnGUI()
        {
            // Si le menu est ouvert, affichez le contenu du menu
            if (_isMenuOpen)
            {
                var customStyle = new GUIStyle(GUI.skin.window);
                customStyle.normal.background = MakeTex(1, 1, new Color(0.1f, 0.1f, 0.1f, 1.0f));
                customStyle.focused.background = MakeTex(1, 1, new Color(0.1f, 0.1f, 0.1f, 1.0f));
                customStyle.onNormal.background = MakeTex(1, 1, new Color(0.1f, 0.1f, 0.1f, 1.0f));
                customStyle.hover.background = MakeTex(1, 1, new Color(0.1f, 0.1f, 0.1f, 1.0f));
                customStyle.normal.textColor = Color.white;
                customStyle.focused.textColor = Color.white;
                customStyle.onNormal.textColor = Color.white;
                customStyle.hover.textColor = Color.white;

                windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)MenuFunction, "CaveDestroyer", customStyle);
            }
        }

        void MenuFunction(int windowID)
        {
            // Dessinez les options de sélection de catégorie
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Self"))
                _selectedCategory = 0;

            if (GUILayout.Button("World"))
                _selectedCategory = 1;

            if (GUILayout.Button("Misc"))
                _selectedCategory = 2;
            GUILayout.EndHorizontal();
            // Dessinez les boutons de test en fonction de la catégorie sélectionnée
            switch (_selectedCategory)
            {
                case 0:
                    GUILayout.Label("If you want to only apply to yourself, enter your character name : " + Modules.playerName);
                    Modules.playerName = GUILayout.TextField(Modules.playerName);
                    if (GUILayout.Button("InfHeal"))
                    {
                        // Trouver tous les objets Player dans la scène
                        Modules.infHeal = !Modules.infHeal;
                    }
                    Modules.infJump = GUILayout.Toggle(Modules.infJump, "InfJump");
                    GUILayout.Label("Custom Speed Modulation: " + Modules.speed);
                    Modules.speed = GUILayout.HorizontalSlider(Modules.speed, 1.5f, 10f);
                    GUILayout.Label("Custom Jump Height Modulation: " + Modules.jumpHeight);
                    Modules.jumpHeight = GUILayout.HorizontalSlider(Modules.jumpHeight, 1f, 20f);
                    Modules.reach = GUILayout.Toggle(Modules.reach, "Reach");
                    Modules.addTalentPoints = GUILayout.Button("Add Talent Points");
                    Modules.addCharacterPoints = GUILayout.Button("Add Character Points");
                    break;
                case 1:
                    if (GUILayout.Button("Test 2"))
                    {
                        // Faire quelque chose
                    }
                    break;
                case 2:
                    if (GUILayout.Button("Test 3"))
                    {
                        // Faire quelque chose
                    }
                    break;
            }
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
