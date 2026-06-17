using MaderLand.Systems.Trample.Network;
using Vintagestory.API.Client;

namespace MaderLand.Systems.Trample.Gui;

public class GuiDebugPanel : GuiDialog
{
    public override string ToggleKeyCombinationCode => ""; // or a hotkey code
    public override EnumDialogType DialogType => EnumDialogType.HUD;

    public GuiDebugPanel(ICoreClientAPI capi) : base(capi)
    {
        Compose();
    }

    /// <summary>
    /// Compose dialog.
    /// </summary>
    private void Compose()
    {
        ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.LeftMiddle);

        ElementBounds posBounds = ElementBounds.Fixed(0, 25, 300, 20);
        ElementBounds durBounds = posBounds.BelowCopy(0, 0);
        ElementBounds maxDurBounds = durBounds.BelowCopy(0, 0);
        ElementBounds regenBounds = maxDurBounds.BelowCopy(0, 0);
        ElementBounds updatedAtBounds = regenBounds.BelowCopy(0, 0);

        ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.DialogToScreenPadding);
        bgBounds.BothSizing = ElementSizing.FitToChildren;
        bgBounds.WithChildren(posBounds, durBounds, maxDurBounds, regenBounds, updatedAtBounds);

        SingleComposer = capi.Gui.CreateCompo("trampleDebugDialog", dialogBounds)
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar("Trample Debug", OnTitleBarCloseClicked)
            .BeginChildElements(bgBounds)
              .AddDynamicText("Position: -", CairoFont.WhiteSmallText(), posBounds, "position")
              .AddDynamicText("Durability: -", CairoFont.WhiteSmallText(), durBounds, "durability")
              .AddDynamicText("MaxDurability: -", CairoFont.WhiteSmallText(), maxDurBounds, "maxDurability")
              .AddDynamicText("Regen: -", CairoFont.WhiteSmallText(), regenBounds, "regen")
              .AddDynamicText("UpdatedAt: -", CairoFont.WhiteSmallText(), updatedAtBounds, "updatedAt")
            .EndChildElements()
            .Compose();
    }

    private void OnTitleBarCloseClicked() => TryClose();

    //

    /// <summary>
    /// Update GUI.
    /// </summary>
    public void UpdateGui(TrampleDataResp packet)
    {
        string positionVal = packet.Pos != null ? packet.Pos.ToString() : "-";
        string durabilityVal = "-";
        string maxDurabilityVal = "-";
        string regenVal = "-";
        string updatedAtVal = "-";
        if (packet.Data != null)
        {
            durabilityVal = packet.Data.Durability.ToString();
            maxDurabilityVal = packet.Data.MaxDurability.ToString();
            regenVal = packet.Data.Regen.ToString();
            updatedAtVal = packet.Data.UpdatedAt.ToString();
        }

        SingleComposer.GetDynamicText("position").SetNewTextAsync("Position: " + positionVal, true);
        SingleComposer.GetDynamicText("durability").SetNewTextAsync("Durability: " + durabilityVal, true);
        SingleComposer.GetDynamicText("maxDurability").SetNewTextAsync("MaxDurability: " + maxDurabilityVal, true);
        SingleComposer.GetDynamicText("regen").SetNewTextAsync("Regen: " + regenVal, true);
        SingleComposer.GetDynamicText("updatedAt").SetNewTextAsync("UpdatedAt: " + updatedAtVal, true);
    }
}
