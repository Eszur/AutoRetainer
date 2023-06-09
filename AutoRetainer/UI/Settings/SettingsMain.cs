using Dalamud.Interface.Components;
using ECommons.Interop;
using ECommons.MathHelpers;
using PInvoke;
using PunishLib.ImGuiMethods;
using System.Windows.Forms;

namespace AutoRetainer.UI.Settings;

internal static class SettingsMain
{
    internal static void Draw()
    {
        ImGuiEx.EzTabBar("GeneralSettings",
            // General
            ("通常设置", TabGeneral, null, true),
            // Multi Mode
            ("多角色模式", TabMulti, null, true),
            // Other
            ("其他设置", TabOther, null, true)
            );
    }

    static void TabGeneral()
    {
        ImGuiHelpers.ScaledDummy(5f);
        // Settings
        InfoBox.DrawBox("设置", delegate
        {
            ImGui.SetNextItemWidth(100f);
            // Time Desynchronization Compensation
            ImGui.SliderInt("同步补偿时间", ref P.config.UnsyncCompensation.ValidateRange(-60, 0), -10, 0);
            // Additional amount of seconds that will be subtracted from venture ending time to help mitigate possible issues of time desynchronization between the game and your PC.
            ImGuiComponents.HelpMarker("从任务结束时间中减去的额外秒数，有助于缓解游戏和PC之间可能出现的时间不同步问题。");
            // Anonymise Retainers
            ImGui.Checkbox("雇员匿名", ref P.config.NoNames);
            // Retainer names will be redacted from general UI elements. They will not be hidden in debug menus and plugin logs however. While this option is on, character and retainer numbers are not guaranteed to be equal in different sections of a plugin (for example, retainer 1 in retainers view is not guaranteed to be the same retainer as in statistics view).
            ImGuiComponents.HelpMarker("插件ui中匿名，和游戏内、log、debug无关");
            // Display Quick Menu in Retainer UI
            ImGui.Checkbox($"在雇员UI中显示快捷菜单", ref P.config.UIBar);
            // Opt out of custom Dalamud theme
            ImGui.Checkbox($"不使用定制的Dalamud主题", ref P.config.NoTheme);
            ImGui.SetNextItemWidth(100f);
            // Interaction Delay, seconds
            ImGuiEx.SliderIntAsFloat("交互延迟，秒", ref P.config.Delay.ValidateRange(10, 1000), 20, 1000);
            // The lower this value is the faster plugin will use actions. When dealing with low FPS or high latency you may want to increase this value. If you want the plugin to operate faster you may decrease it.
            ImGuiComponents.HelpMarker("这个值越低，插件操作动作就越快。当FPS过低或延迟过高时，你可能想增加这个值。如果你想让插件运行得更快，你可以减少它。");
            //Display Extended Retainer Info
            ImGui.Checkbox($"显示雇员的扩展信息", ref P.config.ShowAdditionalInfo);
            //Displays retainer item level/gathering/perception and the name of their current venture in the main UI.
            ImGuiComponents.HelpMarker("在主界面中显示雇员的等级/获得力/鉴别力以及他们当前的探险名称。");
        });
        // Operation
        InfoBox.DrawBox("操作", delegate
        {
            // Assign + Reassign
            if (ImGui.RadioButton("指派+重新指派", P.config.EnableAssigningQuickExploration && !P.config._dontReassign))
            {
                P.config.EnableAssigningQuickExploration = true;
                P.config.DontReassign = false;
            }
            // Automatically assigns enabled retainers to a Quick Venture if they have none already in progress and reassigns current venture.
            ImGuiComponents.HelpMarker("如果已启用的雇员没有正在进行的探险，则自动将其分配给自由探索委托，并重新委托当前探险。");
            // Collect
            if (ImGui.RadioButton("收集", !P.config.EnableAssigningQuickExploration && P.config._dontReassign))
            {
                P.config.EnableAssigningQuickExploration = false;
                P.config.DontReassign = true;
            }
            // Only collect venture rewards from the retainer, and will not reassign them.\nHold CTRL when interacting with the Summoning Bell to apply this mode temporarily.
            ImGuiComponents.HelpMarker("只从雇员那里收集探险奖励，不会重新委托。在与雇员铃互动时按住CTRL键可以暂时启用该模式。");
            // Reassign
            if (ImGui.RadioButton("重新指派", !P.config.EnableAssigningQuickExploration && !P.config._dontReassign))
            {
                P.config.EnableAssigningQuickExploration = false;
                P.config.DontReassign = false;
            }
            // Only reassign ventures that retainers are undertaking.
            ImGuiComponents.HelpMarker("只重新指派雇员正在进行的探险。");

            var d = MultiMode.GetAutoAfkOpt() != 0;
            if (d) ImGui.BeginDisabled();
            // RetainerSense
            ImGui.Checkbox("RetainerSense", ref P.config.RetainerSense);
            // AutoRetainer will automatically enable itself when the player is within interaction range of a Summoning Bell. You must remain stationary or the activation will be cancelled.
            ImGuiComponents.HelpMarker($"当玩家处于雇员铃的互动范围内时，AutoRetainer会自动启用。你必须保持静止，否则激活将被取消。");
            if (d)
            {
                ImGui.EndDisabled();
                // Using RetainerSense requires Auto-afk option to be turned off.
                ImGuiComponents.HelpMarker("使用RetainerSense 必须关闭Auto-afk.");
            }
            ImGui.SetNextItemWidth(200f);
            // Activation Time
            ImGuiEx.SliderIntAsFloat("启动时间", ref P.config.RetainerSenseThreshold, 1000, 100000);
        });
    }

    static void TabMulti()
    {
        ImGuiHelpers.ScaledDummy(5f);
        // Wait For Venture Completion
        ImGui.Checkbox("等待探险完成", ref P.config.MultiWaitForAll);
        // AutoRetainer will wait for all ventures to return before cycling to the next character in multi mode operation.
        ImGuiComponents.HelpMarker("在多模式操作中，AutoRetainer将等待所有探险完成后再循环到下一个角色。");
        ImGui.SetNextItemWidth(60);
        // Advance Relog Threshold
        ImGui.DragInt("提前切换角色阈值", ref P.config.AdvanceTimer.ValidateRange(0, 300), 0.1f, 0, 300);
        // Housing Bell Support
        ImGui.Checkbox($"房屋雇员铃支持", ref P.config.MultiAllowHET);
        // A Summoning Bell must be within range of the spawn point once the home is entered.
        ImGuiEx.TextWrapped(ImGuiColors.DalamudOrange, $"一旦进入房屋，必须有一个雇员铃在进入点的范围内。");
        // Upon activating Multi Mode, attempt to enter nearby house
        ImGui.Checkbox($"激活多角色模式后，尝试进入附近的房屋", ref P.config.MultiHETOnEnable);
        // Display Login Overlay
        ImGui.Checkbox($"显示登录Overlay", ref P.config.LoginOverlay);
        // Enforce Full Character Rotation
        ImGui.Checkbox($"强制执行完整的角色轮换", ref P.config.CharEqualize);
        // Recommended for users with > 15 characters, forces multi mode to make sure ventures are processed on all characters in order before returning to the beginning of the cycle.
        ImGuiComponents.HelpMarker("建议有>15个角色的用户使用，强制使用多角色模式，以确保在返回循环开始之前按顺序对所有的角色依次进行探险。");
        // Wait on login screen
        ImGui.Checkbox($"在登录屏幕上等待", ref P.config.MultiWaitOnLoginScreen);
        // If no character is available for ventures, you will be logged off until any character is available again. Title screen movie will be disabled while this option and MultiMode are enabled.
        ImGuiComponents.HelpMarker($"如果没有可供雇员探险的角色，你将被注销，直到任何角色可以再次雇员探险。当这个选项和多角色模式启用时，标题页面的动画将被禁用。");
        // Synchronise Retainers (one time)
        ImGui.Checkbox("同步雇员（一次）", ref MultiMode.Synchronize);
        // AutoRetainer will wait until all enabled retainers have completed their ventures. After that this setting will be disabled automatically and all characters will be processed.
        ImGuiComponents.HelpMarker("AutoRetainer将等待直到所有启用的雇员完成其任务。之后，将自动禁用此设置，并处理所有角色。");
        ImGui.Separator();
        // Character Order:
        ImGuiEx.Text($"C角色顺序:");
        for (int index = 0; index < P.config.OfflineData.Count; index++)
        {
            if (P.config.OfflineData[index].World.IsNullOrEmpty()) continue;
            ImGui.PushID($"c{index}");
            if (ImGui.ArrowButton("##up", ImGuiDir.Up) && index > 0)
            {
                try
                {
                    (P.config.OfflineData[index - 1], P.config.OfflineData[index]) = (P.config.OfflineData[index], P.config.OfflineData[index - 1]);
                }
                catch (Exception e)
                {
                    e.Log();
                }
            }
            ImGui.SameLine();
            if (ImGui.ArrowButton("##down", ImGuiDir.Down) && index < P.config.OfflineData.Count - 1)
            {
                try
                {
                    (P.config.OfflineData[index + 1], P.config.OfflineData[index]) = (P.config.OfflineData[index], P.config.OfflineData[index + 1]);
                }
                catch (Exception e)
                {
                    e.Log();
                }
            }
            ImGui.SameLine();
            ImGuiEx.TextV(Censor.Character(P.config.OfflineData[index].Name));
            ImGui.PopID();
        }

        if (P.config.Blacklist.Any())
        {
            // Excluded Characters
            InfoBox.DrawBox("排除的角色", delegate
            {
                for (int i = 0; i < P.config.Blacklist.Count; i++)
                {
                    var d = P.config.Blacklist[i];
                    ImGuiEx.TextV($"{d.Name} ({d.CID:X16})");
                    ImGui.SameLine();
                    if (ImGui.Button($"Delete##bl{i}"))
                    {
                        P.config.Blacklist.RemoveAt(i);
                        P.config.SelectedRetainers.Remove(d.CID);
                        break;
                    }
                }
            });
        }
    }

    static void TabOther()
    {
        ImGuiHelpers.ScaledDummy(5f);
        // Keybinds
        InfoBox.DrawBox("按键绑定", delegate
        {
            // Temporarily prevents AutoRetainer from being automatically enabled when using a Summoning Bell
            DrawKeybind("在使用雇员铃时，暂时阻止AutoRetainer自动启用。", ref P.config.Suppress);
            // Temporarily set the Collect Operation mode, preventing ventures from being assigned for the current cycle
            DrawKeybind("临时设置收集操作模式，防止为当前周期分配探险。", ref P.config.TempCollectB);
        });

        InfoBox.DrawBox("Quick Retainer Action", delegate
        {
            // Sell Item
            QRA("出售物品", ref P.config.SellKey);
            // Entrust Item
            QRA("交给雇员保管", ref P.config.EntrustKey);
            // Retrieve Item
            QRA("从雇员处取回", ref P.config.RetrieveKey);
            // Put up For Sale
            QRA("到市场出售", ref P.config.SellMarketKey);
        });
        // Statistics
        InfoBox.DrawBox("统计", delegate
        {
            // Record Venture Statistics
            ImGui.Checkbox($"记录探险统计", ref P.config.RecordStats);
        });
        // Automatic Grand Company Expert Delivery
        InfoBox.DrawBox("自动军队筹备稀有品", AutoGCHandinUI.Draw);
    }

    static void QRA(string text, ref LimitedKeys key)
    {
        if(DrawKeybind(text, ref key))
        {
            P.quickSellItems.Toggle();
        }
        ImGui.SameLine();
        // + right click
        ImGuiEx.Text("+ 右键点击");
    }

    static string KeyInputActive = null;
    static bool DrawKeybind(string text, ref LimitedKeys key)
    {
        bool ret = false;
        ImGui.PushID(text);
        ImGuiEx.Text($"{text}:");
        ImGui.Dummy(new(20, 1));
        ImGui.SameLine();
        ImGui.SetNextItemWidth(200f);
        if (ImGui.BeginCombo("##inputKey", $"{key}"))
        {
            if (text == KeyInputActive)
            {
                //Now press new key
                ImGuiEx.Text(ImGuiColors.DalamudYellow, $"现在按下新按键...");
                foreach (var x in Enum.GetValues<LimitedKeys>())
                {
                    if (IsKeyPressed(x))
                    {
                        KeyInputActive = null;
                        key = x;
                        ret = true;
                        break;
                    }
                }
            }
            else
            {
                // Auto-detect new key
                if (ImGui.Selectable("自动检测新按键", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    KeyInputActive = text;
                }
                // Select key manually
                ImGuiEx.Text($"手动选择按键:");
                ImGuiEx.SetNextItemFullWidth();
                ImGuiEx.EnumCombo("##selkeyman", ref key);
            }
            ImGui.EndCombo();
        }
        else
        {
            if(text == KeyInputActive)
            {
                KeyInputActive = null;
            }
        }
        if (key != LimitedKeys.None)
        {
            ImGui.SameLine();
            if (ImGuiEx.IconButton(FontAwesomeIcon.Trash))
            {
                key = LimitedKeys.None;
                ret = true;
            }
        }
        ImGui.PopID();
        return ret;
    }
}
