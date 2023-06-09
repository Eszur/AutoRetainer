namespace AutoRetainer.UI.Settings;

internal static class NotifyGui
{
    internal static void Draw()
    {
        // Display overlay notification if one of retainers has completed a venture
        ImGui.Checkbox($"如果一个雇员完成了一项探险，则显示通知悬浮窗", ref P.config.NotifyEnableOverlay);
        // Do not display overlay in duty or combat
        ImGui.Checkbox($"在任务或战斗中不显示悬浮窗", ref P.config.NotifyCombatDutyNoDisplay);
        // Include other characters
        ImGui.Checkbox($"包括其他角色", ref P.config.NotifyIncludeAllChara);
        // Ignore other characters that have not been enabled in MultiMode
        ImGui.Checkbox($"忽略其他未在多角色模式中启用的角色", ref P.config.NotifyIgnoreNoMultiMode);
        // Display notification in game chat
        ImGui.Checkbox($"在游戏聊天中显示通知", ref P.config.NotifyDisplayInChatX);
        // If game is inactive: (requires NotificationMaster to be installed and enabled)
        ImGuiEx.Text($"如果游戏处于非活动状态：（需要安装并启用NotificationMaster）。");
        // Send desktop notification on retainers available
        ImGui.Checkbox($"当雇员可用时，发送桌面通知", ref P.config.NotifyDeskopToast);
        // Flash taskbar
        ImGui.Checkbox($"任务栏闪烁", ref P.config.NotifyFlashTaskbar);
        // Do not notify if AutoRetainer is enabled or MultiMode is runnin
        ImGui.Checkbox($"如果启用了AutoRetainer或运行了“多角色模式”，则不通知。", ref P.config.NotifyNoToastWhenRunning);
    }
}
