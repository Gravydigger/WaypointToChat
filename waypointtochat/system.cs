using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using HarmonyLib;
using System.Reflection;
using Vintagestory.API.MathTools;

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

    // Postfix patching the following method:
    // https://github.com/anegostudios/vsessentialsmod/blob/master/Systems/WorldMap/WaypointLayer/WaypointMapComponent.cs#L135-L186
    public static void MouseUp(WaypointMapComponent __instance, MouseEvent args)
    {
        ICoreClientAPI capi = __instance.capi;
        if (args.Button == EnumMouseButton.Right && capi.World.Player.Entity.Controls.CtrlKey)
        {
            Waypoint waypoint = Traverse.Create(__instance).Field("waypoint").GetValue() as Waypoint;

            var pos = waypoint.Position.AsBlockPos;
            pos.X -= (int)capi.World.DefaultSpawnPosition.X;
            pos.Z -= (int)capi.World.DefaultSpawnPosition.Z;

            capi.SendChatMessage(string.Format("<a href=\"chattype:///waypoint addati {0} {1} {2} {3} {4} {5} {6}\">[{6}]</a>", waypoint.Icon, pos.X, pos.Y, pos.Z, waypoint.Pinned, ColorUtil.Int2Hex(waypoint.Color), waypoint.Title));
        }
    }

    public override void Dispose()
    {
        harmony.UnpatchAll(harmonyId);
    }
}