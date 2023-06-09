using Dalamud.Interface.Components;
using PunishLib.ImGuiMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.Settings
{
    internal static class SuperSecret
    {
        internal static void Draw()
        {
            ImGuiEx.TextWrapped(ImGuiColors.ParsedOrange, "Anything can happen here.");
            // Notification settings
            InfoBox.DrawBox("通知设置", NotifyGui.Draw);
            // Old RetainerSense
            ImGui.Checkbox("旧的RetainerSense", ref P.config.OldRetainerSense);
            // Detect and use the closest Summoning Bell within valid distance of the player.
            ImGuiComponents.HelpMarker("检测并使用玩家有效距离内最近的雇员铃。");
            // RetainerSense is enforced to be active during MultiMode operation.
            ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey, "在多模式运行期间，RetainerSense被强制激活。");
            ImGui.Separator();
            // Unsafe options protection
            ImGui.Checkbox($"不安全的选项保护", ref P.config.UnsafeProtection);
            ImGui.SameLine();
            // Write to registry
            if (ImGui.Button($"Write to registry"))
            {
                Safety.Set(P.config.UnsafeProtection);
            }
            var g = Safety.Get();
            // Safety flag: {(g ? "Present" : "Absent")}
            ImGuiEx.Text(g?ImGuiColors.ParsedGreen:ImGuiColors.DalamudRed, $"安全标志: {(g ? "存在" : "缺失")}");
        }
    }
}
