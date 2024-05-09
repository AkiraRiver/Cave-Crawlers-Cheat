using MelonLoader;
using UnityEngine;
using Il2Cpp;
using Il2CppSystem.Net.Configuration;
using System.Linq;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
namespace CaveDestroyer
{
    public class CaveDestroyer : MelonMod
    {
        private bool _isMenuOpen = false;
        private int _selectedCategory = 0;
        private Rect windowRect = new Rect(10, 10, 400, 800);
        private string newCriticalChance = "0";
        private string newCriticalDamage = "0";
        private string newDamage = "0";
        private string newIncreasedProjectileSpeed = "0";
        private string newKnockback = "0";
        private string newReloadSpeed = "0";
        private string newSwingSpeed = "0";
        private string newWeaponRange = "0";
        private static float timeESP = 0.0f;
        private static float updateIntervalESP = 1 / 240f;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg("CaveDestroyer by AKIRA loaded!");

            // Créez un nouvel objet GameObject
        }

        public override void OnUpdate()
        {
            // Si l'utilisateur appuie sur la touche Insert, ouvrez ou fermez le menu
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                _isMenuOpen = !_isMenuOpen;
            }
            Modules.RunModules();


        }
        private void CreateFieldWithApplyButton(string label, ref string textFieldValue, Action<float> applyAction)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            textFieldValue = GUILayout.TextField(textFieldValue);

            if (GUILayout.Button("Apply"))
            {
                if (float.TryParse(textFieldValue, out float parsedValue))
                {
                    applyAction(parsedValue);
                }
                else
                {
                    MelonLogger.Msg("Invalid input for " + label);
                }
            }
            GUILayout.EndHorizontal();
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

                windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)MenuFunction, "CaveDestroyer by Akira", customStyle);
            }
            timeESP += Time.deltaTime;
            if (timeESP >= updateIntervalESP)
            {
                ESP.RunESP();
                timeESP = 0f;
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

            if (GUILayout.Button("Item"))
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
                    if(GUILayout.Button("Regen 1 health") || Input.GetKeyDown(KeyCode.F1))
                    {
                        Modules.addOneHealth = true;
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
                    
                    Modules.teamESP = GUILayout.Toggle(Modules.teamESP, "Team ESP");
                    Modules.mobESP = GUILayout.Toggle(Modules.mobESP, "Mob ESP");
                    Modules.itemESP = GUILayout.Toggle(Modules.itemESP, "Item ESP");
                    
                    break;
                case 2:
                    Player targetPlayer = Modules.players.FirstOrDefault(player => player.name == Modules.playerName);
                    if (targetPlayer == null)
                    {
                        GUILayout.Label("No player found with the name " + Modules.playerName);
                        break;
                    }
                    if(targetPlayer.HeldItem == null)
                    {
                        GUILayout.Label("No weapon found in player's hand");
                        break;
                    }
                    if(targetPlayer.HeldItem.Weapon == null)
                    {
                        GUILayout.Label("No weapon component found in player's hand");
                        break;
                    }
                    GUILayout.Label("Current Held weapon : " + targetPlayer.HeldItem.name);
                    WeaponInfo weaponInfo = targetPlayer.HeldItem.Weapon.info;
                    // Critical Damage
                    // Critical Damage
                    CreateFieldWithApplyButton("Critical Chance: " + weaponInfo.criticalChance.ToString(), ref newCriticalChance, value => weaponInfo.criticalChance = value);
                    CreateFieldWithApplyButton("Critical Damage: " + weaponInfo.criticalDamage.ToString(), ref newCriticalDamage, value => weaponInfo.criticalDamage = value);

                    // Damage
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Damage: " + weaponInfo.damage.ToString());
                    newDamage = GUILayout.TextField(newDamage);
                    if (GUILayout.Button("Apply"))
                    {
                        if (int.TryParse(newDamage, out int parsedValue))
                        {
                            weaponInfo.damage = parsedValue;
                        }
                        else
                        {
                            MelonLogger.Msg("Invalid input for damage");
                        }
                    }
                    GUILayout.EndHorizontal();
                    CreateFieldWithApplyButton("Increased Projectile Speed: " + weaponInfo.increasedProjectileSpeed.ToString(), ref newIncreasedProjectileSpeed, value => weaponInfo.increasedProjectileSpeed = value);
                    CreateFieldWithApplyButton("Knockback: " + weaponInfo.knockback.ToString(), ref newKnockback, value => weaponInfo.knockback = value);
                    CreateFieldWithApplyButton("Reload Speed: " + weaponInfo.reloadSpeed.ToString(), ref newReloadSpeed, value => weaponInfo.reloadSpeed = value);
                    CreateFieldWithApplyButton("Swing Speed: " + weaponInfo.swingSpeed.ToString(), ref newSwingSpeed, value => weaponInfo.swingSpeed = value);
                    CreateFieldWithApplyButton("Weapon Range: " + weaponInfo.weaponRange.ToString(), ref newWeaponRange, value => weaponInfo.weaponRange = value);



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
