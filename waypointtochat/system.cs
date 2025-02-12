using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using HarmonyLib;
using System.Reflection;

public class WaypointToChat : ModSystem
{
    public ICoreClientAPI capi;
    private Harmony harmony;
    protected const string harmonyId = "waypointtochat";

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return forSide == EnumAppSide.Client;
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);

        capi = api;

        harmony = new Harmony(harmonyId);
        harmony.Patch(typeof(WaypointMapComponent).GetMethod("OnMouseUpOnElement", BindingFlags.Instance | BindingFlags.Public),
            postfix: new HarmonyMethod(typeof(WaypointToChat).GetMethod(nameof(MouseUp))));
    }

    public static void MouseUp(WaypointMapComponent __instance, MouseEvent args, GuiElementMap mapElem)
    {
        Waypoint myvalue = Traverse.Create(__instance).Field("waypoint").GetValue() as Waypoint;

        if (args.Button == EnumMouseButton.Right && __instance.capi.World.Player.Entity.Controls.CtrlKey)
        {
            __instance.capi.Logger.Debug("Yippee!!");
        }
    }

    public override void Dispose()
    {
        harmony.UnpatchAll(harmonyId);
    }
}