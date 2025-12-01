using BepInEx; // нужна ссылка на BepInEx.Core.dll
using BepInEx.Unity.IL2CPP; // нужна ссылка на BepInEx.Unity.IL2CPP.dll
using HarmonyLib; // нужна ссылка на 0Harmony.dll
using Hazel; // нужна ссылка на Hazel.dll
using InnerNet; // нужна ссылка на Assembly-CSharp.dll
using UnityEngine; // нужна ссылка на UnityEngine.dll
using Il2CppInterop.Runtime; // нужна ссылка на Il2CppInterop.Runtime.dll

[BepInPlugin("com.example.directrpc", "Plugin", "1.0.0")]
public class Plugin : BasePlugin
{
    public new static BepInEx.Logging.ManualLogSource Log;

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo("Plugin загружен!");

        Harmony harmony = new Harmony("com.example.directrpc");
        harmony.PatchAll();
    }


    public void SendOrDisconnect(MessageWriter msg)
    {
     
    }
}
