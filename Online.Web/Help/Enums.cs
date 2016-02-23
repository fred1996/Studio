using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Online.Web.Help
{
    public enum UserRoleEnum
    {
        [Description("其他")]
        OTHER = 0,

        [Description("会员")]
        MEMBER = 1,

        [Description("子爵")]
        ZIJUE = 10,

        [Description("伯爵")]
        BOJUE = 20,

        [Description("白银VIP")]
        SILVER_MEMBER = 30,

        [Description("黄金VIp")]
        GOLD_MEMBER = 40,

        [Description("钻石VIP")]
        DIAMOND_MEMBER = 60,

        [Description("至尊VIP")]
        EXTERME_MEMBER = 70,

        [Description("大亨VIP")]
        DAHENG_MEMBER = 80,

        [Description("巡管")]
        XUGUAN = 100,

        [Description("频道巡管")]
        CHANNEL_XUGUAN = 110,

        [Description("超管")]
        SUPER_ADMINISTRATOR = 120,

        [Description("管理员")]
        GENERAL_ADMINISTRATOR = 1000,

        [Description("客服")]
        KE_FU = 1010,

        [Description("推广")]
        TUI_GUANG = 1020,

        [Description("讲师")]
        JIANG_SHI = 90,
        [Description("铂金VIP")]
        PLATINUM_MEMBER = 50,
    }

    public enum UserOwnerEnum
    {
        [Description("所有")]
        ALL = 0,

        [Description("量子金融直播室")]
        LiangZiTV = 1,

        [Description("金十财经直播室")]
        JinShiTV = 2,

        [Description("汇金财经直播室")]
        HuiJinTV = 3,

        [Description("联储金融直播室")]
        LianChuTV = 4,

        [Description("东方财金直播室")]
        DongFangTV = 5,

        [Description("环球外汇子频道")]
        HuanQiuWaiHui_WEB = 6,

        [Description("中嘉财经(WeiXin)")]
        ZhongJia_WEIXIN = 7,

        [Description("中嘉财经行情软件")]
        ZhongJia_SOFT = 8,

        [Description("中嘉财经(APP)")]
        ZhongJia_APP = 9,

        [Description("中嘉网")]
        ZhongJia_WEB = 10,

        [Description("银帮直播室")]
        YinBangTV = 11,

        [Description("赢通直播室")]
        YingTongTV = 12,
    }
}