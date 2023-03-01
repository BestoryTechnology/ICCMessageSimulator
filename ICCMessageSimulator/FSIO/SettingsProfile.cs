using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 本地设置配置
/// </summary>
[Serializable()]
public class SettingsProfile
{
    public string CenterIP = "127.0.0.1";
    public int CenterPort = 8725;
    public string CenterName = "中控";
    public string oscAddress = "通用";
    public string projectName = "指令中心测试工具";
    public string data = "string@start";
}
