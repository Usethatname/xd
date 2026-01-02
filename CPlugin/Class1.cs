using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using AmongUs.GameOptions;
using Hazel;
using UnityEngine;

namespace CPlugin
{
    [BepInPlugin(id, plugin_name, versionString)]
    public class Plugin : BasePlugin
    {
        public const string id = "com.xd.sah_mod";
        public const string plugin_name = "shittyasshole_mod";
        public const string versionString = "1.0.0";

        private readonly Harmony harmony = new Harmony(id);

        public override void Load()
        {
            Log.LogInfo($"Плагин {plugin_name} загружен!");

            harmony.PatchAll();
        }
// реализация пачта на нажатие клавиши
    [HarmonyPatch(typeof(PlayerControl), "FixedUpdate")]
    public static class PlayerControl_FixedUpdatePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            PlayerControl local = PlayerControl.LocalPlayer;
            if (local == null) return;

            if (Input.GetKey(KeyCode.F1))
            {

                PlayerControl.LocalPlayer.RpcSetName("sosun");
                
            }
        }
    }
}
