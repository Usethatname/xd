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
// реализация пачта(k diddy i hear yu,патча.) на нажатие клавиши
    [HarmonyPatch(typeof(PlayerControl), "FixedUpdate")]
    public static class PlayerControl_FixedUpdatePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            PlayerControl local = PlayerControl.LocalPlayer;
            if (local == null) return;

            if (Input.GetKey(KeyCode.F1))
            {
                // баг с object null reference, надо будет потом пофиксить
                foreach (var item in PlayerControl.AllPlayerControls)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.Exiled, SendOption.Reliable, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                
            }
        }
    }
}
