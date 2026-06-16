using Vintagestory.API.Client;

namespace MaderLand.Systems.Trample.Gui;

public class GuiDebugPanel : GuiDialog
{
    public override string ToggleKeyCombinationCode => ""; // or a hotkey code
    public override EnumDialogType DialogType => EnumDialogType.HUD;

    public GuiDebugPanel(ICoreClientAPI capi) : base(capi)
    {
        SetupDialog();
    }

    /// <summary>
    /// Setup dialog.
    /// </summary>
    private void SetupDialog()
    {
        ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.LeftMiddle);

        ElementBounds textBounds = ElementBounds.Fixed(0, 25, 300, 200);

        ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.DialogToScreenPadding);
        bgBounds.BothSizing = ElementSizing.FitToChildren;
        bgBounds.WithChildren(textBounds);

        SingleComposer = capi.Gui.CreateCompo("trampleDebugDialog", dialogBounds)
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar("Trample Debug", OnTitleBarCloseClicked)
            .AddStaticText("Here will be debug info.", CairoFont.WhiteSmallText(), textBounds)
            .Compose();
    }

    private void OnTitleBarCloseClicked() => TryClose();

}
