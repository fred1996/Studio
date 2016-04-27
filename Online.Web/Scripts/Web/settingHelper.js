var setting = new Object();
setting.oldImg = "";
setting.init = function () {
    $(".kechengbiao").fancybox({
        maxWidth: 799,
        maxHeight: 432,
        fitToView: false,
        width: '100%',
        height: '100%',
        autoSize: false,
        closeClick: false,
        openEffect: 'none',
        closeEffect: 'none'
    });
    $(".various").fancybox({
        maxWidth: 680,
        fitToView: false,
        width: '70%',
        height: '50%',
        autoSize: false,
        closeClick: false,
        openEffect: 'none',
        closeEffect: 'none',
        overlayShow: false
    });
    setting.methods.initTheme();
    setting.methods.settingevent();
    //setting.methods.initVote();
    setTimeout("setting.methods.initFly()", 3000);
}
setting.methods = {
    settingevent: function () {
        var html = new StringBuilder();
        var wd = "190px";
        html.Append('<div id="btnsetting" style=\'width:100px;height:' + wd + ';\'>');
        html.Append("<a href=\"javascript:;\" onclick=\"setting.methods.openThemeevent()\">主题</a>");
        html.Append("<a class='fancybox fancybox.iframe various'  href='/Vote/Index'>投票</a>");
        html.Append("<a class='fancybox fancybox.iframe various'  href='/Home/Announcement'>公告</a>");
        html.Append("<a class='fancybox fancybox.iframe various'  href='/Home/ImageManage'>图片</a>");
        html.Append("<a href=\"javascript:;\" onclick=\"setting.methods.openFly()\">飞屏</a>");
        html.Append("<a class='fancybox fancybox.iframe various'  href='/Home/Playfigure'>弹图</a>");
        html.Append("<a href=\"javascript:;\" onclick=\"setting.methods.openServerQQ()\">QQ</a>");
        html.Append("<a href=\"javascript:;\" onclick=\"setting.methods.ClearIp()\">清除IP名单</a>");
        html.Append("<div id='content'>");
        html.Append("<div>3</div>");
        html.Append("</div>");
        html.Append("<div class='clear'></div>");
        html.Append('</div>');
        $('.tooltip').tooltipster({
            animation: 'fade',
            delay: 200,
            theme: 'tooltipster-light',
            touchDevices: true,
            trigger: 'hover',
            contentAsHTML: true,
            multiple: true,
            position: 'bottom',
            interactive: true,
            content: html.toString()
        });
    },
    openServerQQ: function () {
        $.fancybox.open($("#QQBox"), {
            width: 400,
            height: 400,
            fitToView: false,
            padding: 0,
            margin: 0,
            scrolling: 'no',
            autoSize: false,
            closeClick: false,
            closeBtn: true,
            openEffect: 'none',
            closeEffect: 'none',
            type: 'inline'
            //modal: true,
            //hideOnOverlayClick: false,
            //hideOnContentClick: false,
            //overlayShow: true
        });

        $("#btnserverQQ").unbind("click").bind("click", function () {
            var QQ = $("#Qcarid").val();
            //  var name = $("#QNAME").val();
            if (QQ.length > 0) {
                var strq = QQ;
                $.ajax({ url: '/Home/UpdateServerConfigQQ', data: { serviceQQs: strq } }).done(function (result) {
                    $("#Qcarid").val('');
                    $("#QNAME").val('');
                    $("#tipqq").empty();
                    $("#btnserverQQ").after("<span id='tipqq' class='mLeft20'>添加成功</span>");
                    $("#QQList").val($("#QQList").val() + ";" + strq);
                });
            } else {
                alert("QQ不能为空！");
                return;
            }
        });
        //绑定现有QQ到界面显示，提供删除
        $.post("/Home/ServerConfigQQ").done(function (ret) {
            $("#QQList").val(ret);
        });
        $("#btnSaveQQ").unbind("click").bind("click", function () {
            var qq = $("#QQList").val();
            $.post("/Home/UpdateConfigQQ", { serviceQQs: qq }).done(function (ret) {
                if (ret) $("#btnSaveQQ").after("<span id='tipqq' class='mLeft20'>更新成功</span>");
            });
        });
    },
    openThemeevent: function () {
        $.get("/config/theme.xml", {}, function (data) {
            var html = new StringBuilder();
            html.Append("<tr>");
            html.Append("<td>");
            $(data).find("name").each(function (i) {
                var url = $(this).attr("url");
                var val = $(this).text();
                var title = $(this).attr("title");
                html.Append("<div class='left mLeft10'><span class='theme_radio'></span>" + val + "<span class='" + title + "' url='" + url + "'></span></div>");
            });
            html.Append("</td>");
            html.Append("</tr>");
            $(".setting_theme").empty().append(html.toString());
            $(".setting_theme div").click(function () {
                var obj = $(this).find("span:eq(0)");
                $(this).removeClass("theme_radio");
                $(".theme_radio").removeClass("theme_radiock");
                if ($(obj).hasClass("theme_radio")) {
                    $(obj).addClass("theme_radiock");
                    $("#btnSavetheme", "#SettingTheme").attr("theme", $(this).find("span:eq(1)").attr("url"));
                } else {
                    $(obj).removeClass("theme_radio");
                }
            });
        });
        $.fancybox.open($("#SettingTheme"), {
            width: 300,
            height: 200,
            fitToView: false,
            padding: 0,
            margin: 0,
            scrolling: 'no',
            autoSize: false,
            closeClick: false,
            closeBtn: true,
            openEffect: 'none',
            closeEffect: 'none',
            type: 'inline'
        });

        $("#btnSavetheme", "#SettingTheme").click(function () {
            if ($(this).attr("theme") != undefined && $(this).attr("theme") != '') {
                $.post("/Home/SetTheme", { style: $(this).attr('theme') }, function (data) {
                    if (data.start > 0) {
                        window.location.reload();
                    } else {
                        alert("已是当前选择的主题了，无需重复切换！");
                    }
                });
            }
        });
    },
    initVote: function () {
        $(".various").fancybox({
            maxWidth: 680,
            fitToView: false,
            width: '70%',
            height: '50%',
            autoSize: false,
            closeClick: false,
            openEffect: 'none',
            closeEffect: 'none',
            overlayShow: false
        });
        $.ajax({ url: '/Home/GetIsUserVotes' }).done(function (data) {
            if (data) {
                $("#uservote").click();                
            }
        });
    },
    initTheme: function () {
        $.ajax({ url: '/Home/GetUserSexTheme' }).done(function (data) {
            if (data.userAdmin) {
                $(".admin-setting").removeClass("hidden");
            }
            if (data.userstarlo) {
                $("#usersextheme").show();
                $("#usersextheme").prev("span").show();
            }
            if (data.theme != "" && data.theme != null) {
                $("#theme_css").attr("href", "/content/theme/" + data.theme + "");
            } else {
                $("#theme_css").attr("href", "/content/theme/theme_blue.css");
            }
        });
    },
    initSettingVis: function () {
        $.ajaxSetup({
            async: false
        });
        //$("#usersextheme").show();
        //$("#usersextheme").prev("span").show();
        $.ajax({ url: '/Home/GetSettingVis' }).done(function (data) {
            if (data.userAdmin) {
                $(".admin-setting").removeClass("hidden");
            }
            if (data.theme != "" && data.theme != null) {
                $("#theme_css").attr("href", "/content/theme/" + data.theme + "");
            } else {
                $("#theme_css").attr("href", "/content/theme/theme_blue.css");
            }
        });
        $.ajaxSetup({
            async: true
        });
    },
    initFly: function () {
        $("#dofly").danmu({
            left: 0,
            top: 0,
            width: "100%",
            height: 500,
            zindex: 1100,
            speed: 40000,
            opacity: 1,
            font_size_small: 38,
            danmuLoop: false,
            font_size_big: 38
        });
        $('#dofly').danmu('danmu_resume');
    },
    openFly: function () {
        $.fancybox.open($("#messagefly"), {
            width: 400,
            height: 200,
            fitToView: false,
            padding: 0,
            margin: 0,
            scrolling: 'no',
            autoSize: false,
            closeClick: false,
            closeBtn: true,
            openEffect: 'none',
            closeEffect: 'none',
            type: 'inline',
            //modal: true,
            //hideOnOverlayClick: false,
            //hideOnContentClick: false,
            //overlayShow: true
        });
        $(".btnsendfly").unbind("click").click(function () {
            if ($("#txtflyval").val().length <= 0) {
                alert("请输入飞屏内容！");
                return;
            }
            var context = homeMain.OnlineData.UserRoleName() + "提示：" + $("#txtflyval").val();
            $.ajax({ url: '/Home/SaveSystemInfo', data: { content: context } }).done(function (data) {
                client.events.sendDanmuClick(context);
                $(".fancybox-close").click();
                var curdanmu = { "text": context, "color": "#fff", "size": "1", "position": "0", "time": $('#dofly').data("nowtime") + 5 };
                $('#dofly').danmu("add_danmu", curdanmu);
                $("#txtflyval").val('');
            });
        });
    },
    initFlyImage: function () {
        $.ajax({ url: '/Home/GetSysteminfoFlyImage' }).done(function (data) {
            if (data) {
                $("#heomefiyimg").attr("src", data);
                setting.oldImg = data;
                var strhtml = new StringBuilder();
                strhtml.Append("<div id='homeflyimgbox' style=\"background: url('/Image/images/Vote_Bg.png') repeat scroll 0 0 transparent;width: 800px;height: 300px;z-index:1112; position: fixed; margin:auto; left:0; right:0; top:0; bottom:0; opacity: 1; overflow: visible;\"><div class='closefly' onclick=\"hideflyimg()\"></div>");
                strhtml.Append("<div>");
                strhtml.Append("<div style='text-align: center;padding: 10px;font-size:16px;color:#D0A644;'>给自己取个好听的名字，就能与专业分析师及投资人<span style='color:white;'>在线交流</span>哦</div>");
                strhtml.Append("<div style='text-align:center;'><input id=\"usernamether\" type='text' class='form-control' style='margin-right:20px;margin-bottom: 10px;'><button class='btn btn-primary' style='padding: 3px 25px;' id='btnconsave'>确认</button></div>");
                strhtml.Append("</div>");
                strhtml.Append("<div  id=\"heomefiyimg\" style=\"width:100%;height:300px;z-index:911200;\">");
                strhtml.Append("<img src=" + data + " width=\"100%\" height='100%'>");
                strhtml.Append("<a onclick='homeMain.ShowRegister();' style='position:absolute;top:231px;left:430px;width:115px;height:40px;display:inline-block;cursor: pointer;display: none;' class='modal_yk'></a>");
                strhtml.Append("<a onclick='homeMain.ShowLogin();' style='position:absolute;top:231px;left:554px;width:115px;height:40px;display:inline-block; cursor: pointer;display: none;' class='modal_yk'></a>");
                strhtml.Append("</div>");
                strhtml.Append('<div class="qqs">' + $(".serviceqq-list").html() + '</div>');
                strhtml.Append("</div>");
                $("body").append(strhtml.toString());
                $("#btnconsave").click(function () {
                    if ($("#usernamether").val().trim().length>0) {
                        homeMain.ShowRegister();
                        $("#UserNickname").val($("#usernamether").val());
                    } else {
                        alert("请输入用户名~.~");
                        $("#usernamether").focus();
                    }
                });
            }
        });
    },
    OpenSchedule: function () {
        $.fancybox.open({
            href: '/Home/UploadSchedule',
            type: 'iframe',
            padding: 5,
            scrolling: 'no',
            fitToView: true,
            width: 610,
            height: 300,
            autoSize: false,
            closeClick: false
        });
    },
    ShowScheduleImg: function () {
        $.fancybox.open({
            href: '/Home/ShowScheduleImg',
            type: 'iframe',
            scrolling: 'no',
            leftRatio: 0.5,
            autoSize: true,
            closeClick: false,
            closeBtn: true,
            autoDimensions: true,
            autoScale: false,
            openEffect: 'none',
            closeEffect: 'none',
            minHeight: 0,
            width: 820,
            minWidth: 0,
            maxWidth: 820,
            maxHeight: 600,
            fitToView: false
        });
    },
    ClearIp: function () {
        $.fancybox.open($("#ClearIp"), {
            width: 400,
            height: 150,
            fitToView: false,
            padding: 0,
            margin: 0,
            scrolling: 'no',
            autoSize: false,
            closeClick: false,
            closeBtn: true,
            openEffect: 'none',
            closeEffect: 'none',
            type: 'inline'
        });
        $("#btnClearIp").unbind("click").bind("click", function () {
            var qq = $("#IpOrName").val();
            if (qq)
                $.post("/Home/ClearIpOrName", { name: qq }).done(function (ret) {
                    if (ret) {
                        $("#IpOrName").val();
                        $("#clearips").empty();
                        $("#btnClearIp").after("<span id='clearips' class='mLeft20'>清除成功</span>");
                    }
                });
        });
    }
};
var timeID;
var refreshRate = 600000;
$(function () {
    $("#MainContent").click(function () {
        $("#homeflyimgbox").hide();
        $("#MainContent").unbind("click");
    });
    setting.oldImg = "";
    setting.init();
    setTimeout("ShowRegLogin()", refreshRate);
});
function ShowRegLogin() {
    var roleid = $.cookie("UserRoleID");
    if (setting.oldImg == "" || typeof (setting.oldImg) == "undefined") setting.oldImg = $("#heomefiyimg img").attr('src');
    if (roleid == 0) {
        refreshRate = 180000;
        $("#heomefiyimg img").attr('src', "../../Image/images/modal_bg.png");
        $(".modal_yk").show();
        $('#homeflyimgbox').show();
        timeID = setTimeout("ShowRegLogin()", refreshRate);
    }
}

function hideflyimg() {
    $("#homeflyimgbox").hide();
    $("#MainContent").unbind("click");
}
function openFlyImg() {
    $("#heomefiyimg img").attr('src', setting.oldImg);
    $(".modal_yk").hide();
    $('#homeflyimgbox').show();
}


