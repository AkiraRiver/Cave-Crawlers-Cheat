using System;
using UnityEngine;
using MelonLoader;
using UnityEngine.Rendering;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;
using Il2Cpp;
using UnityEngine.UIElements;

namespace CaveDestroyer
{
    public class Render : MonoBehaviour
    {
        public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);


        public static Color Color
        {
            get { return GUI.color; }
            set { GUI.color = value; }
        }

        public static void DrawString(Vector2 position, string label, bool centered = true)
        {
            var content = new GUIContent(label);
            var size = StringStyle.CalcSize(content);
            var upperLeft = centered ? position - size / 2f : position;
            GUI.Label(new Rect(upperLeft, size), content);
        }
        public static void DrawColorString(Vector2 position, string label, Color color, float size, bool centered = true)
        {
            var content = new GUIContent(label);
            var style = new GUIStyle();
            style.fontSize = Mathf.RoundToInt(size);
            style.normal.textColor = color;

            var sizeVec = style.CalcSize(content);
            var upperLeft = centered ? position - sizeVec / 2f : position;

            GUI.Label(new Rect(upperLeft, sizeVec), content, style);
        }


        public static Texture2D lineTex;
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            Matrix4x4 matrix = GUI.matrix;
            if (!lineTex)
                lineTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);

            Color color2 = GUI.color;
            GUI.color = color;
            float num = Vector3.Angle(pointB - pointA, Vector2.right);

            if (pointA.y > pointB.y)
                num = -num;

            GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
            GUIUtility.RotateAroundPivot(num, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), lineTex);
            GUI.matrix = matrix;
            GUI.color = color2;
        }

        public static void DrawBox(float x, float y, float w, float h, Color color, float thickness)
        {
            DrawLine(new Vector2(x, y), new Vector2(x + w, y), color, thickness);
            DrawLine(new Vector2(x, y), new Vector2(x, y + h), color, thickness);
            DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + h), color, thickness);
            DrawLine(new Vector2(x, y + h), new Vector2(x + w, y + h), color, thickness);
        }
    }
    public class ESP
    {
        static Color lightBlue = new Color(0.0f, 1.0f, 1.0f); // R = 0, G = 1, B = 1
        public static Color limeColor = new Color(0.0f, 1.0f, 0.0f); // R = 0, G = 1, B = 0

        private static float timeSinceLastUpdate = 0.0f;
        private static float updateInterval = 2f; // Update 60 times per second
        
        private static Item[] items;
        private static Player[] players;
        private static Mob[] bots;
        private static Camera cam = Camera.main;
        public static void Update()
        {
            timeSinceLastUpdate += Time.deltaTime;

            if (timeSinceLastUpdate >= updateInterval)
            {
                players = GameObject.FindObjectsOfType<Player>();
                //MelonLogger.Msg($"Found {players.Length} players");

                bots = GameObject.FindObjectsOfType<Mob>();
                //MelonLogger.Msg($"Found {bots.Length} mobs");

                items = GameObject.FindObjectsOfType<Item>();
                //MelonLogger.Msg($"Found {items.Length} items");

                timeSinceLastUpdate = 0f;
            }
            
            
        }


        public static void RunESP()
        {
            if (cam == null)
            {
                cam = Camera.main;
                if (cam == null)
                {
                    return;
                }
            }
            if (Modules.teamESP)
            {
                //Weirdly gets the spiders pos too....
                foreach (Player player in players)
                {
                    if (player != null && player.transform != null)
                    {
                        Vector3? enemyBottom = null;
                        try
                        {
                            enemyBottom = player.transform.position;
                            if (enemyBottom == null)
                            {
                                //MelonLogger.Msg("Player position is null");
                                continue;
                            }
                        }
                        catch (NullReferenceException)
                        {
                            //MelonLogger.Msg("Exception when trying to get player position");
                            continue;
                        }

                        Vector3 w2s = cam.WorldToScreenPoint(enemyBottom.Value);
                        Vector3 enemyTop = enemyBottom.Value;
                        enemyTop.y += 2f;
                        Vector3 worldToScreenBottom = cam.WorldToScreenPoint(enemyBottom.Value);
                        Vector3 worldToScreenTop = cam.WorldToScreenPoint(enemyTop);

                        if (ESPUtils.IsOnScreen(w2s))
                        {
                            float height = Mathf.Abs(worldToScreenTop.y - worldToScreenBottom.y);
                            float x = w2s.x - height * 0.3f;
                            float y = Screen.height - worldToScreenTop.y;

                            Vector2 namePosition = new Vector2(w2s.x, UnityEngine.Screen.height - w2s.y + 8f);
                            Vector2 hpPosition = new Vector2(x + (height / 2f) + 3f, y + 1f);
                            Vector3 enemyBottom2 = player.headObj.position;

                            namePosition -= new Vector2(enemyBottom2.x - enemyBottom2.x, 0f);
                            hpPosition -= new Vector2(enemyBottom2.x - enemyBottom2.x, 0f);

                            float distance = Vector3.Distance(UnityEngine.Camera.main.transform.position, enemyBottom2);
                            int fontSize = Mathf.Clamp(Mathf.RoundToInt(12f / distance), 10, 20);

                            ESPUtils.DrawString(namePosition, player.name + "\n" + "HP: " + player.hitPoints, Color.green, true, fontSize, FontStyle.Bold);
                            ESPUtils.DrawHealth(new Vector2(w2s.x, UnityEngine.Screen.height - w2s.y + 22f), player.hitPoints, 100f, 0.5f, true);
                        }
                    }
                }
            }

            if (Modules.mobESP)
            {
                ////MelonLogger.Msg("Mob ESP module is enabled");
                foreach (Mob enemy in bots)
                {
                    if (enemy != null && enemy.transform != null)
                    {
                        Vector3? pivotPos = enemy.transform.position;
                        if (pivotPos == null)
                        {
                            //MelonLogger.Msg("Mob position is null");
                            continue;
                        }

                        Vector3 playerHeadPos = (Vector3)pivotPos;

                        Vector3 w2s_headpos = Camera.main.WorldToScreenPoint(playerHeadPos);
                        if (w2s_headpos == null)
                        {
                            //MelonLogger.Msg("Screen position is null");
                            continue;
                        }

                        if (w2s_headpos.z > 0f)
                        {
                            Render.DrawColorString(new Vector2(w2s_headpos.x, (float)Screen.height - w2s_headpos.y + 0.3f), enemy.name, Color.red, 12f);
                        }
                        else
                        {
                            ////MelonLogger.Msg("Mob is not in the camera view");
                        }
                    }
                    else
                    {
                        //MelonLogger.Msg("Mob object or its transform is null");
                    }
                }
            }
            else
            {
                ////MelonLogger.Msg("Mob ESP module is not enabled");
            }

            if (Modules.itemESP)
            {
                ////MelonLogger.Msg("Item ESP module is enabled");
                foreach (Item itemInstance in items)
                {
                    if (itemInstance != null && itemInstance.transform != null)
                    {
                        Item item = itemInstance;
                        Vector3? pivotPos = itemInstance.transform.position;
                        if (pivotPos == null)
                        {
                            //MelonLogger.Msg("Item position is null");
                            continue;
                        }

                        Vector3 itemPos = (Vector3)pivotPos;

                        Vector3 w2s_itempos = Camera.main.WorldToScreenPoint(itemPos);
                        if (w2s_itempos == null)
                        {
                            //MelonLogger.Msg("Screen position is null");
                            continue;
                        }

                        if (w2s_itempos.z > 0f)
                        {
                            Render.DrawColorString(new Vector2(w2s_itempos.x, (float)Screen.height - w2s_itempos.y - 20f), item.name, Color.yellow, 12f);
                            Render.DrawBox(w2s_itempos.x - 10f, (float)Screen.height - w2s_itempos.y - 10f, 20f, 20f, Color.yellow, 2f);
                        }
                        else
                        {
                            ////MelonLogger.Msg("Item is not in the camera view");
                        }
                    }
                    else
                    {
                        //MelonLogger.Msg("Item object is null");
                    }
                }
            }
            else
            {
                //MelonLogger.Msg("Item ESP module is not enabled");
            }


        }
    }
}
