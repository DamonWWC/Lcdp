using System.ComponentModel;

namespace Hjmos.Lcdp.Plugins.NccControls.Enums
{
    /// <summary>
    /// 110联络状态枚举
    /// </summary>
    public enum PoliceContactState
    {
        [Description("未与110联络")]
        未联络 = 830401,
        [Description("已与110联络，到达中")]
        已联络 = 830402,
        [Description("110已到达")]
        已到达 = 830403
    }

    /// <summary>
    /// 120联络状态枚举
    /// </summary>
    public enum FirstAidContactState
    {
        [Description("未与120联络")]
        未联络 = 830401,
        [Description("已与120联络，到达中")]
        已联络 = 830402,
        [Description("120已到达")]
        已到达 = 830403,
    }

    /// <summary>
    /// 119联络状态枚举
    /// </summary>
    public enum FirePoliceeContactState
    {
        [Description("未与119联络")]
        未联络 = 830401,
        [Description("已与119联络，到达中")]
        已联络 = 830402,
        [Description("119已到达")]
        已到达 = 830403
    }

    /// <summary>
    /// 专业救援联络状态枚举
    /// </summary>
    public enum ProfessionalRescueContactState
    {
        [Description("未与专业救援联络")]
        未联络 = 830401,
        [Description("已与专业救援联络，到达中")]
        已联络 = 830402,
        [Description("专业救援已到达")]
        已到达 = 830403
    }

    /// <summary>
    /// 应急救援单位枚举
    /// </summary>
    public enum RescueUnits
    {
        [Description("42D00")]
        公安警察,
        [Description("46000")]
        医疗卫生资源,
        [Description("42D07")]
        公安消防部队,
        [Description("42E00")]
        专业求援队伍
    }
}
