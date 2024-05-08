using MelonLoader;
using UnityEngine;
using Il2Cpp;
using Il2CppPhoton.Pun.Demo.PunBasics;
namespace CaveDestroyer
{
    public static class Modules
    {
        public static bool infHeal = false, infJump = false;
        public static bool reach = false;
        public static float speed = 1.5f;
        public static float jumpHeight = 1f;
        private static float timeSinceLastUpdate = 0.0f;
        private static float updateInterval2 = 1f;
        public static bool firsttime = true;
        public static bool firsttimejump = true;
        public static bool addTalentPoints = false;
        public static bool addCharacterPoints = false;
        public static Player[] players;
        public static TalentTree[] talentTree;
        public static string playerName = "None";
        public static void RunModules()
        {
            if (timeSinceLastUpdate >= updateInterval2)
            {
                players = GameObject.FindObjectsOfType<Player>();
                talentTree = GameObject.FindObjectsOfType<TalentTree>();
                timeSinceLastUpdate = 0f;
            }
            timeSinceLastUpdate += Time.deltaTime;
            if (infHeal)
            {
                
                if(players == null)
                {
                    MelonLogger.Log("No players found!");
                    return;
                }
                // Parcourir chaque Player
                foreach (Player player in players)
                {
                    if(playerName == "None" || playerName == player.name)
                    {
                        // Vérifier si le Player a une PlayerHealthBar
                        player.hitPoints = player.maxHitPoints;
                    }
                }
            }
            if(infJump)
            {
                foreach (Player player in players)
                {
                    if (playerName == "None" || playerName == player.name)
                        player.jumpCoolDown = 0f;
                }
                firsttimejump = true;
            }
            else
            {
                if (firsttimejump)
                {
                    foreach (Player player in players)
                    {
                        if (playerName == "None" || playerName == player.name)
                            player.jumpCoolDown = 0.5f;
                    }
                    firsttimejump = false;
                }
            }
            if (jumpHeight != 1f)
            {
                foreach (Player player in players)
                {
                    // Vérifier si le Player a une PlayerHealthBar
                    if (playerName == "None" || playerName == player.name)
                        player.jumpHeight = jumpHeight;
                }
            }
            if(reach)
            {
                foreach (Player player in players)
                {
                    // Vérifier si le Player a une PlayerHealthBar
                    if (playerName == "None" || playerName == player.name)
                        player.interactRange = 100f;
                }
                firsttime = true;
            }
            else
            {
                if(firsttime)
                {
                    foreach (Player player in players)
                    {
                        // Vérifier si le Player a une PlayerHealthBar
                        if (playerName == "None" || playerName == player.name)
                            player.interactRange = 5f;
                    }
                    firsttime = false;
                }
            }
            if(speed != 1.5f)
            {
                if(players == null)
                {
                    MelonLogger.Log("No players found!");
                    return;
                }
                // Parcourir chaque Player
                foreach (Player player in players)
                {
                    // Vérifier si le Player a une PlayerHealthBar
                    if (playerName == "None" || playerName == player.name)
                        player.runMultiplier = speed;
                }
            }
            if(addTalentPoints)
            {
                foreach(TalentTree tree in talentTree)
                {
                    tree.FreeTalentPoints += 1;
                }
            }
            if(addCharacterPoints)
            {
                foreach (TalentTree p in talentTree)
                {
                    p.FreeTalentPoints += 1;
                }
            }
        }
    }
}