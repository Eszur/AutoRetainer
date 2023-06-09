using Dalamud.Interface.Components;
using ECommons.Configuration;
using PunishLib.ImGuiMethods;
using AutoRetainer.UI.Settings;
using Dalamud.Interface.Style;

namespace AutoRetainer.UI;

unsafe internal class ConfigGui : Window
{
    public ConfigGui() : base($"{P.Name} {P.GetType().Assembly.GetName().Version}###AutoRetainer")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new(250, 100),
            MaximumSize = new(9999,9999)
        };
        P.ws.AddWindow(this);
    }

    public override void PreDraw()
    {
        if (!P.config.NoTheme)
        {
            P.Style.Push();
            P.StylePushed = true;
        }
    }

    public override void Draw()
    {
        var e = SchedulerMain.PluginEnabledInternal;
        var disabled = MultiMode.Active && !ImGui.GetIO().KeyCtrl;
        if (disabled)
        {
            ImGui.BeginDisabled();
        }
        // Enable {P.Name} (automatic mode)
        if (ImGui.Checkbox($"启用 {P.Name} (自动模式)", ref e))
        {
            P.WasEnabled = false;
            if(e)
            {
                SchedulerMain.EnablePlugin(PluginEnableReason.Auto);
            }
            else
            {
                SchedulerMain.DisablePlugin();
            }
        }
        if (disabled)
        {
            ImGui.EndDisabled();
            ImGuiComponents.HelpMarker($"MultiMode controls this option. Hold CTRL to override.");
        }

        if (P.WasEnabled)
        {
            ImGui.SameLine();
            // Paused
            ImGuiEx.Text(GradientColor.Get(ImGuiColors.DalamudGrey, ImGuiColors.DalamudGrey3, 500), $"暂停");
        }
        ImGui.SameLine();
        // Multi
        if (ImGui.Checkbox("多角色模式", ref MultiMode.Enabled))
        {
            MultiMode.OnMultiModeEnabled();
        }
        if(P.config.CharEqualize && MultiMode.Enabled)
        {
            ImGui.SameLine();
            // Reset counters
            if (ImGui.Button("重置计数器"))
            {
                MultiMode.CharaCnt.Clear();
            }
        }

        if(IPC.Suppressed)
        {
            ImGuiEx.Text(ImGuiColors.DalamudRed, $"Plugin operation is suppressed by other plugin.");
            ImGui.SameLine();
            // Cancel
            if (ImGui.SmallButton("取消"))
            {
                IPC.Suppressed = false;
            }
        }

        if (P.TaskManager.IsBusy)
        {
            ImGui.SameLine();
            // Abort {P.TaskManager.NumQueuedTasks} tasks
            if (ImGui.Button($"终止 {P.TaskManager.NumQueuedTasks} 任务"))
            {
                P.TaskManager.Abort();
            }
        }


        ImGuiEx.EzTabBar("tabbar",
                // Retainers
                ("雇员", MultiModeUI.Draw, null, true),
                // Statistics
                (P.config.RecordStats ? "统计" : null, StatisticsUI.Draw, null, true),
                // Settings
                ("设置", SettingsMain.Draw, null, true),
                // Expert
                (P.config.Expert?"专业":null, Expert.Draw, null, true),
                //("Beta", Beta.Draw, null, true),
                // About
                ("关于", delegate { AboutTab.Draw(P); }, null, true),
                (P.config.Verbose ? "Dev" : null, delegate
                {
                    ImGuiEx.EzTabBar("DebugBar",
                        ("Log", InternalLog.PrintImgui, null, false),
                        // Retainers (old)
                        ("雇员(旧)", Retainers.Draw, null, true),
                        ("Debug", Debug.Draw, null, true),
                        ("WIP", SuperSecret.Draw, null, true)
                    );
                }, null, true)
                );
    }

    public override void PostDraw()
    {
        if (P.StylePushed)
        {
            P.Style.Pop();
            P.StylePushed = false; 
        }
    }

    public override void OnClose()
    {
        EzConfig.Save();
        StatisticsUI.Data.Clear();
        MultiModeUI.JustRelogged = false;
    }

    public override void OnOpen()
    {
        MultiModeUI.JustRelogged = true;
    }
}
