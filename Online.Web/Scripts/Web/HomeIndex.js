var homeMain = {
    isscroll: true,
    ibrowser: {
        iPhone: navigator.userAgent.indexOf('iPhone') > -1,
        iPad: navigator.userAgent.indexOf('iPad') > -1,
        mobile: !!navigator.userAgent.match(/AppleWebKit.*Mobile.*/)
    },
    OnlineData: {
        Title: ko.observable('九鼎财经直播室'),
        Count: ko.observable(),
        SaveDesktopUrl: ko.observable(),
        RoomId: ko.observable(0),
        RoomName: ko.observable(),
        randomStrLen: ko.observable(8),
        randUN: ko.observable(''),
        isShowAdminMsg: ko.observable(true),
        isKickRoom: null,
        isCheckMsg: ko.observable(true),
        isFilterMsg: ko.observable(true),
        UserId: ko.observable(0),
        UserRoleID: ko.observable(0),
        UserRoleName: ko.observable('游客'),
        Disclaimer: ko.observable(),
        ToSay: ko.observable('所有人'),
        ToSayUserId: ko.observable(0),
        SayWord: ko.observable(''),
        PostedFilePath: ko.observable(),
        Token: ko.observable(),
    },
    LoginData: {
        UserName: ko.observable(),
        Password: ko.observable(),
        Code: ko.observable(),
        CodeError: ko.observable('请输入随机码'),
    },
    RegisterData: {
        Email: ko.observable(),
        UserName: ko.observable(),
        Phone: ko.observable(),
        Password: ko.observable(),
        ConfimPassword: ko.observable(),
        Code: ko.observable(),
        CodeError: ko.observable('请输入随机码'),
        RecommendCode: ko.observable(),
        QQ: ko.observable(),
    },
    dataTemplate: {
        'SEND_MSG': '<img src="../../Image/images/face.png" style="vertical-align: middle;width:25px;height:25px;" onclick="" class="face-set" /> <span class="to-user" touserid="0" data-bind="text:OnlineData.ToSay">所有人</span><input type="text" id="SayingInputVal"  data-bind="textInput: OnlineData.SayWord" /><a id="btnSayingTo" href="javascript:void(0);" onclick="client.events.sendMsgClick();">发送</a><div class="clear"></div>'
         , 'DISABLED_POST': '<div class="disabled-post">当前房间不允许任何人发言</div>'
         , 'PERMISSION_POPUP': '<div id="roomboxc" class="hidden permission-list roombox" > <ul style="color:#333;padding:0px;margin:0px;"> <li onclick="homeMain.toSaying(\'#UserName#\',\'#UserID#\')" class="Hand">对他说</li> <li onclick="homeMain.kickRoom(\'#UserName#\',\'#UserID#\',3600)" class="Hand">禁言1小时</li> <li onclick="homeMain.joinBalckList(\'#UserName#\',\'#UserID#\',\'\')" class="Hand">封它IP</li><li onclick="homeMain.joinBalckList(\'#UserName#\',\'#UserID#\',\'#UserName#\')" class="Hand">加入黑名单</li>  <li onclick="homeMain.kickRoom(\'#UserName#\',\'#UserID#\',300)" class="Hand">禁言5分钟</li> <li onclick="homeMain.recoveryPost (\'#UserName#\',\'#UserID#\')" class="Hand">恢复发言</li><li onclick="homeMain.DelBalckList (\'#UserID#\')" class="Hand">删除黑名单</li></ul></div>'
    },
    wordFilterStr: ko.observableArray(),
    AdvancedTechnologys: ko.observableArray(),
    BasicKnowledges: ko.observableArray(),
    IntelligentTradings: ko.observableArray(),
    Vips: ko.observableArray(),
    Activitys: ko.observableArray(),
    Messages: ko.observableArray(),
    NewsFlash: ko.observableArray(),
    Comments: ko.observableArray(),
    Query: function () {
        homeMain.OnlineData.SaveDesktopUrl('/Home/SaveDesktop?url=' + window.location.href + '&title=' + homeMain.OnlineData.Title());
    },
    QueryAdvancedTechnology: function () {
        $.ajax({ url: '/Home/QueryAdvancedTechnology' }).done(function (results) {
            homeMain.AdvancedTechnologys.removeAll();
            for (var i = 0; i < results.length; i++) {
                homeMain.AdvancedTechnologys.push(results[i]);
            }
        });
    },
    QueryBasicKnowledges: function () {
        $.ajax({ url: '/Home/QueryBasicKnowledges' }).done(function (results) {
            homeMain.BasicKnowledges.removeAll();
            for (var i = 0; i < results.length; i++) {
                homeMain.BasicKnowledges.push(results[i]);
            }
        });
    },
    QueryIntelligentTradings: function () {
        $.ajax({ url: '/Home/QueryIntelligentTradings' }).done(function (results) {
            homeMain.IntelligentTradings.removeAll();
            for (var i = 0; i < results.length; i++) {
                homeMain.IntelligentTradings.push(results[i]);
            }
        });
    },
    QueryNewsFlashs: function () {
        $.ajax({ url: '/Home/QueryNewsFlashs' }).done(function (results) {
            homeMain.NewsFlash.removeAll();
            for (var i = 0; i < results.length; i++) {
                homeMain.NewsFlash.push(results[i]);
            }
        });
    },
    QueryComments: function () {

        $.ajax({ url: "/Home/QueryComment" }).done(function (results) {
            homeMain.Comments.removeAll();
            for (var i = 0; i < results.length; i++) {
                results[i].CreateTime=homeMain.ChangeDateFormat(results[i].CreateTime)
                homeMain.Comments.push(results[i]);
            }
        });
    },
    QueryVips: function () {
        $.ajax({ url: '/Home/QueryVips' }).done(function (results) {
            homeMain.Vips.removeAll();
            for (var i = 0; i < results.length; i++) {
                homeMain.Vips.push(results[i]);
            }
        });
    },
    QueryActivitys: function () {
        $.ajax({ url: '/Home/QueryActivitys' }).done(function (results) {
            homeMain.Activitys.removeAll();
            for (var i = 0; i < results.length; i++) {
                homeMain.Activitys.push(results[i]);
            }
        });
    },
    QueryDisclaimer: function () {
        $.ajax({ url: '/Home/QueryDisclaimer' }).done(function (result) {
            homeMain.OnlineData.Disclaimer(result);
        });
    },
    QueryMessages: function () {
        $.ajax({ url: '/Home/QueryMsgs', async: false }).done(function (results) {
            if (results) {
                var msgHtml = new StringBuilder();
                for (var i = 0; i < results.length; i++) {
                    var item = results[i];
                    //if (item.from == homeMain.OnlineData.randUN()) {
                    //    item.msgtype = 5;
                    //}
                    if (homeMain.OnlineData.UserRoleID() >= 100) {
                        msgHtml.Append(client.methods.initMsgHtml(item.msgtype, item));
                    } else {
                        if (item.ischeck == 1) {
                            msgHtml.Append(client.methods.initMsgHtml(item.msgtype, item));
                        }
                    }
                }
                $("#Msg").empty();
                $("#Msg").append(msgHtml.toString());
                homeMain.ShowChatPopup();
                $("#MsgListWrapper .msgInfo .sayingMan,.toSayingMan").tooltipster("show");
            }
        });
    },
    ShowChatPopup: function () {
        $("#MsgListWrapper .msgInfo .sayingMan,.toSayingMan").tooltipster({
            delay: 0,
            theme: 'tooltipster-light',
            touchDevices: true,
            trigger: 'hover',
            contentAsHTML: true,
            multiple: true,
            position: 'bottom',
            interactive: true,
            functionBefore: function (origin, continueTooltip) {
                continueTooltip();
                $(".roombox").empty();
                $(".roombox").remove();
                var $this = $(this);
                var uname = $.trim($this.text().replace(':', ''));
                var uid = $this.attr("uid");
                var phtml = homeMain.dataTemplate.PERMISSION_POPUP;
                phtml = phtml.replace(/#UserName#/g, uname);
                phtml = phtml.replace(/#UserID#/g, uid);
                $("body").append(phtml);
                if (parseInt(client.config.roleID()) < 100) {
                    $(".permission-list li").each(function () {
                        $(this).hide();
                    });
                    $(".permission-list li").eq(0).show();
                }
                origin.tooltipster('content', $(".permission-list").html());
            }
        });
    },
    initEmojiHtml: function () {
        $("#ToolBarWapper .face-set").qqFace({
            id: "Facebox",
            assign: 'SayingInputVal',
            path: 'scripts/qqface/emoji/',
        });
    },
    LoadSystemInfos: function () {
        $.ajax({ url: '/Home/LoadSystemInfos' }).done(function (results) {
            homeMain.initRoomPosts(results);
        });
    },
    loadRoomInfo: function () {
        $.ajax({ url: '/Home/LoadRoomInfo' }).done(function (result) {
            if (result) {
                var $data = result;
                homeMain.OnlineData.RoomId($data.Entity.RoomId);
                homeMain.OnlineData.RoomName($data.Entity.RoomName);
                if ($data.User) {
                    homeMain.initUserLoginedHtml($data.User);
                } else {
                    var un = $.cookie("UserName");
                    if (un != null && un != "undefined") {
                        if (un.length != 8) {
                            homeMain.OnlineData.randUN(homeMain.RandomWord(8));
                            $.cookie("UserName", homeMain.OnlineData.randUN(), { expires: 1 });
                            $.cookie("UserRoleID", 0, { expires: 1 });
                        } else {
                            homeMain.OnlineData.randUN(un);
                        }
                        homeMain.OnlineData.UserRoleID(0);
                    } else {
                        homeMain.OnlineData.randUN(homeMain.RandomWord(8));
                        $.cookie("UserName", homeMain.OnlineData.randUN(), { expires: 1 });
                        $.cookie("UserRoleID", 0, { expires: 1 });
                        homeMain.OnlineData.UserRoleID(0);
                    }
                    $("#RoleList ul").empty();
                    $("#TopbarWapper .topbar-userlogin").show();
                    $("#TopbarWapper .logout").hide();
                    $(".permission-list li").eq(2).hide();
                    if (homeMain.ibrowser.mobile) {
                        if (homeMain.OnlineData.UserRoleID() <= 0) {
                            $(".topbar-userinfo").hide();
                        }
                    }
                }
                if ($data.Entity && $data.Entity.IsDeleted == "0" && $data.Entity.BizStatus == 1) {
                    if (!$data.Entity.IsPrivateChat) {
                        //不允许私聊，权限浮层去掉“对谁说”
                        if (!!$(".permission-list")) {
                            $(".permission-list li").eq(0).hide();
                        }
                        homeMain.OnlineData.isShowAdminMsg($data.Entity.IsShowAdminMsg);
                    } else {
                        if (!!$(".permission-list")) {
                            $(".permission-list li").eq(0).show();
                        }
                    }
                    if ($data.Conf) {
                        if (!!$data.Conf.IsAllowPost || homeMain.OnlineData.isKickRoom != null) {
                            $("#SayingInfoWrapper").empty();
                            $("#SayingInfoWrapper .disabled-post").text("当前房间不允许任何人发言");
                            $("#SayingInfoWrapper").append(homeMain.dataTemplate.DISABLED_POST);
                        } else {
                            homeMain.OnlineData.SayWord('');
                        }
                        if (!$data.Conf.IsAllowTouristPost && (homeMain.OnlineData.UserId() > 0)) {
                            $("#SayingInfoWrapper").empty();
                            $("#SayingInfoWrapper").append(homeMain.dataTemplate.DISABLED_POST);
                            $("#SayingInfoWrapper .disabled-post").text("不允许游客发言，请登录/注册");
                        }
                        if ($data.Conf.Token) {
                            homeMain.OnlineData.Token($data.Conf.Token);
                        }
                        if (!$data.Conf.IsUploadFile) {
                            $("#ToolBarWapper .upload-img").hide();
                            $("#txtPostFile").hide();
                        } else {
                            $("#ToolBarWapper .upload-img").show();
                            if (!!$data.Conf && !!$data.Conf.UploadFileSize) {
                                //上传附件体积限制
                            }
                        }
                        if (!$data.Conf.IsOpenReg) {
                            $("#btnUserReg").attr("disabled", "disabled");
                        } else {
                            //$("#btnUserReg").removeAttr("disabled");
                            if (!!$data.Conf.IsVerifyPhone) {
                                //不显示验证码，显示文字发短信“获取验证码”
                            } else {
                                //显示验证码
                            }
                        }
                        if (!$data.Conf.IsCheckMsg) {
                            homeMain.OnlineData.isCheckMsg(false);
                        }
                        if ($data.Conf.IsFilterMsg) {
                            homeMain.OnlineData.isFilterMsg(true);
                            for (var i = 0; i < $data.Conf.FilterWords.length; i++) {
                                if ($data.Conf.FilterWords[i]) {
                                    homeMain.wordFilterStr.push($data.Conf.FilterWords[i]);
                                }
                            }
                        }
                        if ($data.Conf.ServiceQQs) {
                            var qqs = $data.Conf.ServiceQQs; //eg:4524878-张老师;4524878-张老师;
                            var qqhtml = '';
                            var qqArrary = qqs.split(';').sort(homeMain.RandomSort);
                            for (var i = 0; i < qqArrary.length; i++) {
                                if (qqArrary[i])
                                    if (homeMain.ibrowser.mobile) {
                                        qqhtml += '<a class="mRight10 mLeft10" target="_blank" href="http://wpa.qq.com/msgrd?v=3&uin=' + qqArrary[i].split('-')[0] + '&site=qq&menu=yes"><div class="serverqqlist " qqnum=' + qqArrary[i].split('-')[0] + '><span style=\'padding-left:25px;\'>助理' + qqArrary[i].split('-')[1] + '</span></div></a>';
                                    } else {
                                        qqhtml += '<a class="mRight10 mLeft10" href="tencent://message/?uin=' + qqArrary[i].split('-')[0] + '&amp;Site=www.yyzhiboshi.com&amp;Menu=yes"><div class="serverqqlist " qqnum=' + qqArrary[i].split('-')[0] + '><span style=\'padding-left:25px;\'>助理' + qqArrary[i].split('-')[1] + '</span></div> </a>';
                                    }
                            }
                            $("#ServiceQQs .serviceqq-list").append(qqhtml);
                            var qqArr;
                            $(".serviceqq-list div").each(function () {
                                qqArr += $(this).attr("qqnum") + ",";
                            });
                            qqArr = qqArr.split(',');
                            var iNum = parseInt((qqArr.length - 1) * Math.random());
                            var qqtc = document.createElement('div');
                            qqtc.innerHTML = "<iframe src='tencent://message/?Menu=yes&uin=" + qqArr[iNum] + "&Site=&Service=201' frameborder='0'></iframe>";
                            document.body.appendChild(qqtc);
                            qqtc.style.display = "none";
                            if (homeMain.ibrowser.mobile != true) {
                                setting.methods.initFlyImage();
                            } else {
                                $(".qqmore").hide();
                                $("#ServiceQQs").css("width", "100%");
                                $(".serviceqq-list").css("width", "100%");
                                $("#SayingInputVal").css({ "width": "50%!important", "float": "left" });
                                $("#btnSayingTo").css("float", "left");
                                $("#SayingInfoWrapper").css("position", "relative");
                                $("#SayingInfoWrapper span:eq(1)").css({ "position": "absolute", "left": "100px", "top": "0" });
                                $("#UserLoginDiv,#UserRegDiv").addClass("width1");
                                $("#UserLoginDiv").find(".userlogin-title").next("div").hide();
                                $("#UserRegDiv").find(".userreg-title").next("div").hide();
                            }
                        }
                    }
                    if ($data.Conf && $data.Entity && $data.Conf.IsLock || $data.Entity.RType) {
                        //需要输入房间密码
                    }
                    //bind events
                    $('#SayingInputVal').keydown(function (e) {
                        if (e.keyCode == 13) {
                            client.events.sendMsgClick();
                        }
                    });
                } else {
                    alert("此房间不存在");
                    return false;
                }
            }
        });
    },
    RandomSort: function (a, b) {
        return Math.random() > .5 ? -1 : 1;
        //用Math.random()函数生成0~1之间的随机数与0.5比较，返回-1或1  
    },
    RefreshValidateCode: function () {
        $("#UserCodeimg").attr('src', '/Home/GetValidateCode?time=' + (new Date()).getTime());
        $("#UserCodeimg").show();
    },
    RefreshLoginValidateCode: function () {
        $("#LoginUserCodeimg").attr('src', '/Home/GetValidateCode?time=' + (new Date()).getTime());
        $("#LoginUserCodeimg").show();
    },
    showCommits: function (type) {
        //$.fancybox.open($("#Commentsbox"), {
        //    fitToView: false,
        //    padding: 0,
        //    margin: 0,
        //    minHeight: 0,
        //    minWidth: 0,
        //    maxWidth: 800,
        //    maxHeight: 600,
        //    scrolling: 'no',
        //    autoSize: true,
        //    closeClick: false,
        //    closeBtn: true,
        //    autoDimensions: true,
        //    autoScale: false,
        //    openEffect: 'none',
        //    closeEffect: 'none',
        //    type: 'inline'
        //});
        //$("#Commentsbox").show();
        if (type=='1') {
            $("#commits").show();
            $("#DisclaimerInfo").hide();
        } else {
            $("#DisclaimerInfo").show();
            $("#commits").hide();
        }
    }
    ,
    showArticleInfo: function (id, type) {
        var title = '';
        if (type == 11)
            title = '早晚评';
        else if (type == 10)
            title = "新闻快讯";
        $.ajax({ url: "/Home/QueryArticleInfo", data: { id: id} }).done(function (result) {
            if (result) {
                $("#articleInfo").empty();
                $.fancybox.open($("#articleInfo"), {
                    fitToView: false,
                    padding: 0,
                    margin: 0,
                    width: "80%",
                    height: "80%",
                    scrolling: 'no',
                    autoSize: false,
                    closeClick: false,
                    closeBtn: true,
                    autoDimensions: true,
                    autoScale: false,
                    openEffect: 'none',
                    closeEffect: 'none',
                    type: 'inline'
                });
                var item = result;
                var html = '';
                html += '<div>';
                html += '<div class="_dialog_title">';
                html += title;
                html += '</div>';
                html += '<div class="content" style="text-align:center;">';
                html += '<div >';
                html += "<h1>" + item.ItemTitle + "</h1>";
                html += "<p><span>时间：" + homeMain.ChangeDateFormat(item.CreateTime) + "</span>&nbsp;&nbsp;<span>发布人：" + item.CreateUser + "</span></p>";
                html += '</div>';
                html += '<div>';
                html += "<p style=\"text-align:left;padding:0 20px;\">" + item.ISummary + "</p>";
                html += '</div>';
                html += '</div>';
                html += '</div>';
                $("#articleInfo").append(html);
                $("#articleInfo").show();
            }
        });
    },
    ChangeDateFormat(val) {
        if (val != null) {
            var date = new Date(parseInt(val.replace("/Date(", "").replace(")/", ""), 10));
            //月份为0-11，所以+1，月份小于10时补个0
            var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
            var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
            return date.getFullYear() + "-" + month + "-" + currentDate;
        }

        return "";
    }
    ,
    ShowRegister: function () {
        $.fancybox.close();
        homeMain.clearForminfo("reg");
        homeMain.RefreshValidateCode();
        $("#UserRegDiv label").empty();
        $("#UserRegDiv img").hide();
        $.fancybox.open($("#UserRegDiv"), {
            //width: 525,
            //height: 550,
            width: '90%',
            height: '70%',
            maxWidth: 525,
            maxHeight: 400,
            minHeight: 550,
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
            modal: true,
            hideOnOverlayClick: true,
            hideOnContentClick: true,
            overlayShow: true,
            autoScale: true
        });
        if (homeMain.ibrowser.mobile) {
            //$(".fancybox-inner").addClass("heightw");
            $(".userreg-form").parent("div").removeAttr("style");
            $("#UserRegDiv").find("span:eq(2)").remove();
            $(".userreg-form").find("input[type='text'],input[type='password']").addClass("ipwidth mLeft10");
            $("#btnUserReg").addClass("mLeft10");
        }
        $("#UserCodeimg").show();
    },
    userRegister: function () {
        if (homeMain.validateRegInputError()) {
            $.ajax({
                url: '/Home/Register', data: {
                    email: homeMain.RegisterData.Email(), nickName: $.trim(homeMain.RegisterData.UserName()),
                    phone: homeMain.RegisterData.Phone(), password: homeMain.RegisterData.Password(),
                    verifyCode: homeMain.RegisterData.Code(), qq: homeMain.RegisterData.QQ(),
                    fromUrl:window.location.href
                }
            }).done(function (result) {
                if (result) {
                    homeMain.initUserLoginedHtml(result);
                    $.fancybox.close();
                }
            }).fail(function (data) {
                //var htmlDoc = data.responseText;
                //var doms = $.parseHTML(htmlDoc);
                //var msg = doms[1].innerHTML;
                //alert(msg);
                alert("用户名或邮箱或手机号重复啦。");
                //$("#UserEmail").parents().next().find("img").attr("src", "../../Image/images/icon_reg_error.png");
                //$("#UserEmail").parents().next().find("img").show();
                //$("#UserEmailError").show();
                //$("#UserTel").parents().next().find("img").attr("src", "../../Image/images/icon_reg_error.png");
                //$("#UserTel").parents().next().find("img").show();
                //$("#UserTelError").show();
                //$("#UserNickname").parents().next().find("img").attr("src", "../../Image/images/icon_reg_error.png");
                //$("#UserNickname").parents().next().find("img").show();
                //$("#UserNMError").show();
            });
        }
    },
    DialogQQ: function () {
        var qqArr = "";
        $(".serviceqq-list div").each(function () {
            qqArr += $(this).attr("qqnum") + ",";
        });
        qqArr = qqArr.split(',')
        var iNum = parseInt((qqArr.length - 1) * Math.random());
        var qqtc = document.createElement('div');
        qqtc.innerHTML = "<iframe src='tencent://message/?Menu=yes&uin=" + qqArr[iNum] + "&Site=&Service=201' frameborder='0'></iframe>";
        document.body.appendChild(qqtc);
        qqtc.style.display = "none";
    },
    ShowLogin: function () {
        $("#UserLoginDiv input").val('');
        $("#UserLoginDiv label").empty();
        $("#UserLoginDiv img").hide();
        homeMain.RefreshLoginValidateCode();
        $.fancybox.open($("#UserLoginDiv"), {
            //width: 525,
            //height: 400,
            width: '90%',
            height: '70%',
            maxWidth: 525,
            maxHeight: 400,
            minHeight: 250,
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
            modal: true,
            hideOnOverlayClick: true,
            hideOnContentClick: true,
            overlayShow: true,
            autoScale: true
        });
        if (homeMain.ibrowser.mobile) {
            $(".fancybox-inner").addClass("heightw");
            $(".userlogin-form").find("input[type='text'],input[type='password']").addClass("ipwidth");
        }
    },
    ShowImage: function (href) {
        var wd = "";
        var hd = "";
        var theImage = new Image();
        theImage.src = href;
        if (theImage.width > 800) {
            wd = 800;
        }
        if (theImage.height > 600) {
            hd = 600;
        }
        if (homeMain.ibrowser.mobile) {
            if (theImage.width > 200 || theImage.height > 200) {
                wd = "200px";
                hd = "200px";
            }
        }
        var image = "<img src=" + href + " width=" + wd + " height=" + hd + " ><\img>";
        $.fancybox.open(image, {
            width: theImage.width,
            height: theImage.height,
            fitToView: false,
            padding: 0,
            margin: 0,
            minHeight: 0,
            minWidth: 0,
            maxWidth: 800,
            maxHeight: 600,
            scrolling: 'no',
            autoSize: true,
            closeClick: false,
            closeBtn: true,
            autoDimensions: true,
            autoScale: false,
            openEffect: 'none',
            closeEffect: 'none',
            type: 'inline'
        });
        theImage.src = "";
        theImage = null;
    },
    userLogin: function () {
        //if (!homeMain.LoginData.UserName()) {
        //    alert("请输入用户名/邮箱");
        //    $("#lblUserName").text('请输入用户名/邮箱');
        //    $("#lblUserName").show();
        //    return;
        //} else {
        //    $("#lblUserName").hide();
        //}
        //if (!homeMain.LoginData.Password()) {
        //    alert("请输入密码");
        //    $("#lblUPwd").text('请输入密码');
        //    $("#lblUPwd").show();
        //    return;
        //} else {
        //    $("#lblUPwd").hide();
        //}
        //if (!homeMain.LoginData.Code()) {
        //    alert("请输入验证码");
        //    return;
        //}
        var username = $.trim(homeMain.LoginData.UserName());
        var upwd = homeMain.LoginData.Password();
        var randcodeError = homeMain.LoginData.CodeError();
        //if (username === "" || upwd === "" || randcodeError) {
        //    return false;
        //}
        if (homeMain.validateLoginInputError()) {
            $.ajax({ url: '/Home/UserLogin', type: "POST", async: false, data: { userName: username, password: upwd } }).done(function (result) {
                if (result == 1) {
                    alert("用户名或密码不正确！");
                }
                else if (result == 2) {
                    window.location.href = '/Home/Invalid';
                } else if (result) {
                    homeMain.OnlineData.randUN(username);
                    $.cookie("UserName", result.UserName, { expires: 30 });
                    for (var i = 0; i < result.RoleList.length; i++) {
                        if (result.RoleList[i].RoleID >= 100) {
                            setting.methods.initSettingVis();
                        }
                    }
                    if ($(window).width() <= 400) {
                        $(".topbar-userinfo").show();
                    }
                    setting.methods.initTheme();
                    homeMain.initUserLoginedHtml(result);
                    setting.methods.initVote();
                    if (client.socket);
                    client.socket.emit('onlineEvent', {
                        roomid: homeMain.OnlineData.RoomId(),
                        uid: homeMain.OnlineData.UserId(),
                        from: homeMain.OnlineData.randUN(),
                        rid: homeMain.OnlineData.UserRoleID(),
                    });
                    homeMain.QueryMessages();
                    $.fancybox.close();
                    setting.methods.settingevent();
                    if (homeMain.isscroll) {
                        $(".nano").nanoScroller();
                        $(".nano").nanoScroller({ scroll: 'bottom' });
                    }
                }
            }).fail(function (data) {
                // var htmlDoc = data.responseText;
                // var doms = $.parseHTML(htmlDoc);
                //var msg = doms[1].innerHTML;
                alert("用户名或密码不正确！");
            });
        }

    },
    userLogout: function () {
        $.ajax({ url: '/Home/Logout', type: "POST", async: false }).done(function (result) {
            homeMain.OnlineData.randUN(homeMain.RandomWord(homeMain.OnlineData.randomStrLen()));
            $.cookie("UserName", homeMain.OnlineData.randUN(), { expires: 1 });
            $.cookie("UserRoleID", 0, { expires: 1 });
            homeMain.OnlineData.UserRoleID(0);
            window.location.reload();
        });
        if (client.socket)
            client.socket.emit('disconnect');
    },
    RandomWord: function (num) {
        var str = "",
            arr = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];
        for (var i = 0; i < num; i++) {
            var pos = Math.round(Math.random() * (arr.length - 1));
            str += arr[pos];
        }
        return str;
    },
    InitStartUserItem: function () {
        var un = $.cookie("UserName");
        if (un != null && un != "undefined") {
            homeMain.OnlineData.randUN(un);
            homeMain.OnlineData.UserRoleID($.cookie("UserRoleID"));
        } else {
            homeMain.OnlineData.randUN(homeMain.RandomWord(homeMain.OnlineData.randomStrLen()));
            $.cookie("UserName", homeMain.OnlineData.randUN(), { expires: 1 });
            $.cookie("UserRoleID", 0, { expires: 1 });
            homeMain.OnlineData.UserRoleID(0);
        }
    },
    initUserLoginedHtml: function (useritem) {
        if (useritem) {
            homeMain.OnlineData.UserId(useritem.UserID);
            homeMain.OnlineData.randUN(useritem.UserName);
            if (useritem.RoleList && useritem.RoleList.length > 0) {
                homeMain.OnlineData.UserRoleID(useritem.RoleList[0].RoleID);
                $.cookie("UserRoleID", useritem.RoleList[0].RoleID, { expires: 1 });
                homeMain.OnlineData.UserRoleName(useritem.RoleList[0].RoleName);
                if (useritem.RoleList.length > 1) {
                    var roleStr = '';
                    for (var i = 0; i < useritem.RoleList.length; i++) {
                        roleStr += '<li roleid="' + useritem.RoleList[i].RoleID + '" nickname="' + useritem.RoleList[i].NickName + '" onclick="homeMain.changeUserRole(this);">' + useritem.RoleList[i].RoleName + '</li>';
                    }
                    $("#RoleList ul").append(roleStr);
                }
                if (homeMain.OnlineData.UserRoleID() >= 100) {
                    $(".onlineWrapper").removeClass("hidden");
                }
            }
            $("#usersextheme").show();
            if (homeMain.ibrowser.mobile) {
                $("#TopbarWapper .topbar-userlogin").hide();
                $(".topbar-userinfo").find(".roles-name").hide();
                $(".user-id").parent("b").hide();
                $(".user-name").css("width", "50px");
            } else {
                $("#TopbarWapper .topbar-userlogin").hide();
            }
            $("#TopbarWapper .logout").show();
            $(".permission-list li").eq(2).show();
            setting.methods.settingevent();
        } else {
            $("#RoleList ul").empty();
            $("#TopbarWapper .topbar-userlogin").show();
            $("#TopbarWapper .logout").hide();
            $(".permission-list li").eq(2).hide();
        }
    },
    showUserRole: function () {
        $("#TopbarWapper .roles-name,#RoleList").mouseover(function () {
            if ($("#RoleList li").length > 0) {
                $("#RoleList").show();
            } else {
                $("#RoleList").hide();
            }
        }).mouseout(function () {
            $("#RoleList").hide();
        });
    },
    changeUserRole: function (jobj) {
        if ($(jobj).attr("roleid") == homeMain.OnlineData.UserRoleID() || !$(jobj).attr("nickname")) return;
        client.socket.emit('disconnect');
        homeMain.OnlineData.randUN($(jobj).attr("nickname"));
        homeMain.OnlineData.UserRoleName($(jobj).text());
        homeMain.OnlineData.UserRoleID($(jobj).attr("roleid"));
        $.ajax({ url: '/Home/ChangeUserRole', data: { userName: $("#TopbarWapper .user-name").text(), roleId: $("#hidRoleID").val() } }).done(function (result) {
            client.socket.emit('onlineEvent', {
                roomid: homeMain.OnlineData.RoomId(),
                uid: homeMain.OnlineData.UserId(),
                from: homeMain.OnlineData.randUN(),
                rid: homeMain.OnlineData.UserRoleID()
            });
            homeMain.QueryMessages();
        });
    },
    isValExists: function (value, vType, content) {
        $.ajax({ url: '/Home/HasValExitsts', data: { value: value, type: vType } }).done(function (result) {
            if (result && !result.IsSuccess) {
                $(content).text(result.Message);
                $(content).show();
            } else {
                $(content).empty();
            }
        });
        return false;
    },
    clearForminfo: function (ftype) {
        switch (ftype) {
            case "reg":
                $("#UserRegDiv input").val('');
                break;
            case "login":
                $("#UserLoginDiv input").val('');
                break;
        }
    },
    isValidateCode: function (code) {
        if (code && code.length > 0) {
            $.ajax({ url: '/Home/ValidateCode', type: "POST", async: false, data: { code: code } }).done(function (result) {
                if (result && !result.IsSuccess) {
                    homeMain.RegisterData.CodeError(result.Message);
                    return false;
                } else {
                    homeMain.RegisterData.CodeError('');
                    return true;
                }
            });
        }
    },
    isLoginValidateCode: function (code) {
        if (code && code.length > 0) {
            $.ajax({ url: '/Home/ValidateCode', type: "POST", async: false, data: { code: code } }).done(function (result) {
                if (result && !result.IsSuccess) {
                    homeMain.LoginData.CodeError(result.Message);
                    return false;
                } else {
                    homeMain.LoginData.CodeError('');
                    return true;
                }
            }).fail(function (data) {
                return false;
            });
        } else {
            return false;
        }
    },
    validateRegInputError: function () {
        var errorNum = 0;
        var tmpVal = $("#UserEmail").val();
        if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
            alert("注册邮箱不能空");
            $("#UserEmailError").text('注册邮箱不能空');
            errorNum++;
        } else if (!RegExp.isEmail(tmpVal)) {
            alert("请输入有效邮箱");
            $("#UserEmailError").text('请输入有效邮箱');
            errorNum++;
        } else if (homeMain.isValExists(tmpVal, 'Email', '#UserEmailError')) {
            alert("此邮箱已经被占用");
            $("#UserEmailError").text('此邮箱已经被占用');
            errorNum++;
        } else {
            errorNum = 0;
        }
        if (errorNum > 0) {
            $("#UserEmail").parents().next().find("img").attr("src", "../../Image/images/icon_reg_error.png");
            $("#UserEmail").parents().next().find("img").show();
            $("#UserEmailError").show();
            return false;
        } else {
            $("#UserEmail").parents().next().find("img").attr("src", "../../Image/images/icon_reg_right.png");
            $("#UserEmail").parents().next().find("img").show();
            $("#UserEmailError").empty();
        }
        var tmpVal = $("#UserNickname").val();
        if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
            $("#UserNMError").text('昵称不能空');
            alert("昵称不能空");
            errorNum++;
        } else if (RegExp.isContainSpecial(tmpVal)) {
            $("#UserNMError").text('昵称只支持数字，字母和_组合');
            alert("昵称只支持数字，字母和_组合");
            errorNum++;
        } else if (tmpVal.length > 64) {
            $("#UserNMError").text('昵称长度不允许超出64个字符');
            alert("昵称长度不允许超出64个字符");
            errorNum++;
        } else if (tmpVal.length < 4) {
            $("#UserNMError").text('昵称长度不允许少于4个字符');
            alert("昵称长度不允许少于4个字符");
            errorNum++;
        } else if (homeMain.isValExists(tmpVal, 'UserName', '#UserNMError')) {
            $("#UserNMError").text('此昵称太火，已被注册');
            alert("此昵称太火，已被注册");
            errorNum++;
        } else {
            errorNum = 0;
        }
        if (errorNum > 0) {
            $("#UserNickname").parents().next().find("img").attr("src", "../../Image/images/icon_reg_error.png");
            $("#UserNickname").parents().next().find("img").show();
            $("#UserNMError").show();
            return false;
        } else {
            $("#UserNickname").parents().next().find("img").attr("src", "../../Image/images/icon_reg_right.png");
            $("#UserNickname").parents().next().find("img").show();
            $("#UserNMError").empty();
        }

        var tmpVal = $("#UserTel").val();
        if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
            $("#UserTelError").text('手机号不能空');
            alert("手机号不能空");
            errorNum++;
        } else if (!RegExp.isMobile(tmpVal)) {
            $("#UserTelError").text('请输入有效手机号');
            alert("请输入有效手机号");
            errorNum++;
        } else if (homeMain.isValExists(tmpVal, 'Telephone', '#UserTelError')) {
            $("#UserTelError").text('此手机号已经注册');
            alert("此手机号已经注册");
            errorNum++;
        } else {
            errorNum = 0;
        }
        if (errorNum > 0) {
            $("#UserTel").parents().next().find("img").attr("src", "../../Image/images/icon_reg_error.png");
            $("#UserTel").parents().next().find("img").show();
            $("#UserTelError").show();
            return false;
        } else {
            $("#UserTel").parents().next().find("img").attr("src", "../../Image/images/icon_reg_right.png");
            $("#UserTel").parents().next().find("img").show();
            $("#UserTelError").empty();
        }

        var tmpVal = $("#UserPwd").val();
        if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
            alert("密码不能空");
            $("#UserPwdError").text('密码不能空');
            errorNum++;
        } else if (tmpVal.length < 6) {
            alert("密码长度不允许少于6位");
            $("#UserPwdError").text('密码长度不允许少于6位');
            errorNum++;
        } else if (tmpVal.length > 12) {
            alert("'密码长度不允许大于12位");
            $("#UserPwdError").text('密码长度不允许大于12位');
            errorNum++;
        } else {
            errorNum = 0;
        }
        if (errorNum > 0) {
            $("#UserPwd").parents().next().find("img").attr("src", "../../Image/images/icon_reg_error.png");
            $("#UserPwd").parents().next().find("img").show();
            $("#UserPwdError").show();
            return false;
        } else {
            $("#UserPwd").parents().next().find("img").attr("src", "../../Image/images/icon_reg_right.png");
            $("#UserPwd").parents().next().find("img").show();
            $("#UserPwdError").empty();
        }

        var tmpVal = $("#UserRepwd").val();
        if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
            alert("密码不能空");
            $("#UserRePwdError").text('密码不能空');
            errorNum++;
        } else if (tmpVal.length < 6) {
            $("#UserRePwdError").text('密码长度不允许少于6位');
            alert("密码长度不允许少于6位");
            errorNum++;
        } else if (tmpVal.length > 12) {
            $("#UserRePwdError").text('密码长度不允许大于12位');
            alert("密码长度不允许大于12位");
            errorNum++;
        } else if (tmpVal != $("#UserPwd").val()) {
            $("#UserRePwdError").text('两次输入的密码不一致');
            alert("两次输入的密码不一致");
            errorNum++;
        } else {
            errorNum = 0;
        }
        if (errorNum > 0) {
            $("#UserRepwd").parents().next().find("img").attr("src", "../../Image/images/icon_reg_error.png");
            $("#UserRepwd").parents().next().find("img").show();
            $("#UserRePwdError").show();
            return false;
        } else {
            $("#UserRepwd").parents().next().find("img").attr("src", "../../Image/images/icon_reg_right.png");
            $("#UserRepwd").parents().next().find("img").show();
            $("#UserRePwdError").empty();
        }
        var tmpVal = $("#UserRandcode").val();
        if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
            homeMain.RegisterData.CodeError('请输入短信验证码');
            alert("请输入短信验证码");
            return false;
        }
        if ($.cookie("RegisterCode") == null || $.cookie("RegisterCode") !== homeMain.RegisterData.Phone() + homeMain.RegisterData.Code()) {
            homeMain.RegisterData.CodeError('短信验证码不正确');
            alert("短信验证码不正确");
            return false;
        }

        if ($("#UserEmail").val() === "" || $("#UserNickname").val() === "" || $("#UserTel").val() === "" || $("#UserPwd").val() === "" || $("#UserRepwd").val() === "" || $("#UserRandcode").val() === "") {
            return false;
        } else {
            return true;
        }

    },
    validateLoginInputError: function () {
        var tmpVal = $("#UserName").val();
        if (!tmpVal || tmpVal == '' || tmpVal == 'undefined' || tmpVal == '昵称/邮箱') {
            $("#lblUserName").text('请输入用户名');
            alert("请输入用户名");
            $("#UserName").parents().next().find("img").attr("src", "../../Image/images/icon_reg_error.png");
            $("#UserName").parents().next().find("img").show();
            $("#lblUserName").show();
            return false;
        } else {
            $("#UserName").parents().next().find("img").attr("src", "../../Image/images/icon_reg_right.png");
            $("#UserName").parents().next().find("img").show();
            $("#lblUserName").empty();
        }
        var tmpVal = $("#UserPassword").val();
        if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
            $("#lblUPwd").text('请输入密码');
            alert("请输入密码");
            $(this).parents().next().find("img").attr("src", "../../Image/images/icon_reg_error.png");
            $(this).parents().next().find("img").show();
            $("#lblUPwd").show();
            return false;
        } else {
            $(this).parents().next().find("img").attr("src", "../../Image/images/icon_reg_right.png");
            $(this).parents().next().find("img").show();
            $("#lblUPwd").empty();
        }

        var tmpVal = $("#LoginUserRandcode").val();
        if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
            homeMain.LoginData.CodeError('请输入随机码');
            alert("请输入验证码");
            return false;
        }
        else {
            homeMain.isLoginValidateCode(tmpVal);
        }
        if (homeMain.LoginData.CodeError() || $("#UserName").val() === "" || $("#UserPassword").val() === "") {
            return false;
        } else {
            return true;
        }

    },
    toSaying: function (un, uid) {
        if (un && uid) {
            homeMain.OnlineData.ToSay(un);
            homeMain.OnlineData.ToSayUserId(uid);
        }
    },
    kickRoom: function (un, uid, sec) {
        if (un && !isNaN(sec) && sec > 0) {
            var data = {
                roomid: homeMain.OnlineData.RoomId(),
                uid: uid,
                from: un,
                time: sec * 1000
            };
            client.socket.emit('kickRoomEvent', data);
            homeMain.OnlineData.isKickRoom = setTimeout(function () {
                homeMain.OnlineData.SayWord('');
                homeMain.OnlineData.isKickRoom = null;
            }, (sec * 1000));
            alert("己禁言");
        }
    },
    joinBalckList: function (un, uid, userName) {
        if (un && un !== "undefined" && uid && uid != "undefined") {
            if (uid > 0 || userName) {
                $.ajax({ url: '/Home/JoinBlackList', data: { userId: uid, userName: userName } }).done(function (result) {
                });
            }
            var data = {
                roomid: homeMain.OnlineData.RoomId(),
                uid: uid,
                from: un,
            };
            client.socket.emit('forceLogOutEvent', data);
            alert("添加成功");
            return true;
        }
    },
    DelBalckList: function (uid) {
        if (uid && uid != "undefined") {
            $.ajax({ url: '/Home/DelBlackList', data: { userId: uid } }).done(function (result) {
                alert("删除成功！");
                return true;
            });
        } else {
            alert("删除失败");
            return false;
        }
    },
    recoveryPost: function (un, uid) {
        if (!!un && !!uid) {
            var data = {
                roomid: homeMain.OnlineData.RoomId(),
                uid: uid,
                from: un
            };
            client.socket.emit('recoveryPostEvent', data);
            alert("恢复发言成功");
        }
    },
    clearScreen: function () {
        $("#MsgListWrapper #Msg").empty();
    },
    scrollScreen: function (e) {
        this.isscroll = !this.isscroll;
        if (!this.isscroll) $("#isscroll").addClass("noscroll");
        if (this.isscroll) $("#isscroll").removeClass("noscroll");
    },

    initRoomPosts: function (postDat) {
        if (postDat) {
            var col1Html = new StringBuilder();
            var col2Html = new StringBuilder();
            for (var i = 0; i < postDat.length; i++) {
                var item = postDat[i];
                //0：其他，1：房间公告，2：服务通知，3：飞屏，4：首屏弹图
                //var time = item.SendTime? new Date(item.SendTime):null;
                switch (item.InfoType) {
                    case 1:
                        //if (item.SendTime) {
                        //   time = time.getMonth() + "." + time.getDate();
                        //}
                        col1Html.Append('<div class="notice-info overflow_ellipsis" postid="' + item.SysInfoID + '">' + item.InfoContent + '</div>');
                        break;
                    case 2:
                        //if (!!item.SendTime) {
                        //    time = time.getMinutes() + ":" + time.getSeconds();
                        //}
                        //col2Html.Append('<span class="serviceTime">' + time + '</span>');
                        col2Html.Append('<span id="ServiceMsg">' + item.InfoContent + '</span>');
                        //col2Html.Append('<span class="service-infoMore" onclick="homeMain.showCompletePost();" actualheight="0"></span>');
                        break;
                }
            }

            $("#RoomPosts").append(col1Html.toString());
            $(".service-info").append(col2Html.toString());
            $(".service-infoMore").attr("actualheight", $("#ServiceMsg").height());
            //if ($("#ServiceMsg").height() > 32) {
            //    $("#ServiceMsg").height(32);
            //    $(".service-infoMore").show();
            //} else {
            //    $(".service-infoMore").hide();
            //}
            var roomScroll = new Marquee("RoomPosts");
            roomScroll.Direction = 2;
            roomScroll.Behavior = "scroll";
            //roomScroll.Step = 2;
            //roomScroll.Width = 630;
            roomScroll.Height = 36;
            roomScroll.Timer = 30;
            roomScroll.DelayTime = 4000;
            roomScroll.WaitTime = 2000;
            roomScroll.Start();
        }
    },
    showCompletePost: function () {
        var heig = $(".service-infoMore").attr("actualheight");
        var hh = $("#ServiceMsg").height();
        if (heig <= hh) {
            $("#ServiceMsg").height("22");
            $("#ChatMsgWrapper .service-info").height("22");
            $("#ChatMsgWrapper .service-infoMore").css("background", "url('/Image/images/down_arrow_black.png')");
            $("#ChatMsgWrapper .service-infoMore").css("top", -12);
            //$("#ServiceQQs").css("margin-top", "30px"); 
        } else {
            $("#ServiceMsg").height(heig);
            $("#ChatMsgWrapper .service-info").height(heig);
            $("#ChatMsgWrapper .service-infoMore").css("background", "url('/Image/images/up_arrow_black.png')");
            $("#ChatMsgWrapper .service-infoMore").css("top", -22);
            $("#ServiceQQs").css("margin-top", "15px");
        }

    },
    showMoreServiceQQ: function () {
        var hei = $("#ServiceQQs .serviceqq-list").height();
        var actHei = $("#ServiceQQs .serviceqq-More").attr("actualheight");
        if (actHei <= hei) {
            $("#ServiceQQs").height("40");
            $("#ServiceQQs .serviceqq-list").height("32");
            //$("#ServiceQQs .serviceqq-More").css("background", "url('/Image/images/qq_more_plus.png')");
        } else {
            $("#ServiceQQs").height(actHei);
            $("#ServiceQQs .serviceqq-list").height(actHei);
            //$("#ServiceQQs .serviceqq-More").css("background", "url('/Image/images/qq_more_less.png')");
        }
    },
    UploadPostFile: function () {
        $("#txtPostFile").fileupload({
            url: '/Home/UploadFile',
            autoUpload: true,
            type: 'POST',
            dataType: 'json',
            acceptFileTypes: '/(\.|\/)(jpg|jpeg|png|bmp|gif)$/i',
            done: function (e, data) {
                if (data.result) {
                    homeMain.OnlineData.PostedFilePath(data.result);
                    homeMain.OnlineData.SayWord(homeMain.OnlineData.SayWord() + "[pf_url]");
                } else {
                    homeMain.OnlineData.PostedFilePath("");
                }
            }
        });
    },
    clearToUser: function () {
        homeMain.OnlineData.ToSay("所有人");
        homeMain.OnlineData.ToSayUserId(0);
        $(".to-user").tooltipster("destroy");
    },
    Refresh: function () {
        var src = $("#MainLiveTVWapper .video-wrapper embed").attr("src");
        var embed = '<embed src=' + src + ' quality="high" bgcolor="#000000" wmode="transparent" width="100%" height="100%" name="_GS_FLASH_ID_videoComponent" align="middle" play="true" loop="false"  allowscriptaccess="always" allowfullscreen="true"  type="application/x-shockwave-flash" flashvars="sc=0&amp;entry=http%3A%2F%2Fguijinshuzhibo.gensee.com%2Fwebcast&amp;code=83688736907b49e582fd1b868b10fcbc__0b2104d1c7214d3aa4a45560e090256e&amp;lang=&amp;nickName=visitor_cDiP6B&amp;httpMode=false&amp;group=&amp;widgetid=videoComponent&amp;userdata=&amp;showCBar=&amp;backURI=&amp;ver=4.0&amp;publicChat=&amp;init=&amp;liveImprovedMode=false&amp;visible=true" > ';
        $("#MainLiveTVWapper .video-wrapper embed").remove();
        $("#MainLiveTVWapper .video-wrapper object").append(embed);
    },
    SendSmsByPhone: function () {
        if (!homeMain.RegisterData.Phone()) {
            alert("请输入手机号");
        }
        $.ajax({ url: '/Account/SendSmsByPhone', data: { phone: homeMain.RegisterData.Phone(), token: homeMain.OnlineData.Token() } }).done(function (result) {
            if (result) {
                $("#btnMoblie").html("己发送");
                $("#btnMoblie").attr("disabled", "disabled");

                setTimeout(function () {
                    $("#btnMoblie").removeAttr("disabled");
                    $("#btnMoblie").html("发送验证码");
                }, 60000);
            } else {

            }
        });
    },
    Init: function () {
        $.ajaxSetup({ cache: false });
        homeMain.InitStartUserItem();
        homeMain.Query();
        homeMain.QueryAdvancedTechnology();
        homeMain.QueryBasicKnowledges();
        homeMain.QueryIntelligentTradings();
        homeMain.QueryNewsFlashs();
        homeMain.QueryComments();
        homeMain.QueryActivitys();
        homeMain.loadRoomInfo();
        homeMain.showUserRole();
        homeMain.QueryMessages();
        homeMain.LoadSystemInfos();
        homeMain.QueryDisclaimer();
        //homeMain.validateLoginInputError();
        //homeMain.validateRegInputError();
        homeMain.UploadPostFile();
    },
    InitContext: function () {
        $(".nano").nanoScroller({
            preventPageScrolling: true,
            alwaysVisible: true,
            sliderMinHeight: 200
        });
        $(".nano").nanoScroller({ scroll: 'bottom' });
        $("#SubColumnsMenu a").mouseover(function () {
            //var $this = $(this);
            //if ($this.attr("divid") == "DisclaimerInfo") {
            //    $("#DisclaimerInfo").show();
            //    $("#commits").hide();
            //} else {
            //    $("#ColumnInfoWrapper").show();
            //    $("#DisclaimerInfo").hide();
            //}
        });
        $(".to-user").mouseover(function () {
            if (homeMain.OnlineData.ToSay() != "所有人") {
                $(".to-user").tooltipster({
                    delay: 0,
                    theme: 'tooltipster-light',
                    content: '<a href="javascript:void(0);" style="display:inline-block;text-decoration:none;cursor:pointer;" onclick="homeMain.clearToUser()">删除</a>',
                    touchDevices: true,
                    trigger: 'hover',
                    contentAsHTML: true,
                    multiple: true,
                    position: 'bottom',
                    interactive: true,
                });
                $(".to-user").tooltipster("show");
            }
        }).mouseout(function () {
            $("#ClearToName").hide();
        });
        $(".face-set").qqFace({
            id: "Facebox",
            assign: 'SayingInputVal',
            path: '../../Scripts/qqface/emoji/',
        });
        $('input').placeholder();
        var upcount = 0;
        var downcount = 0;
        var oldvalue = 0;
        var isdown = false;
        $("#dofly").bind('mousewheel', function (event, delta) {
            //console.log("event:" + event + "delta:" + delta);
            var height = $(".nano").height();
            if (delta > 0) { //往上滚动
                if (!oldvalue) upcount++;
                if (downcount > 1) downcount--; isdown = false;
                var upY = height - (upcount * 40);
                if (upY < 0) {
                    upY = 0;
                    downcount = 1;
                    oldvalue = true;
                } else {
                    oldvalue = false;
                }
                $(".nano").nanoScroller({ scrollTop: upY });
            } else { //
                if (!isdown) downcount++;
                if (upcount > 1) upcount--;
                var downY = height - (downcount * 40);
                if (downY < 0) {
                    downY = 0;
                    upcount = 1;
                    oldvalue = false;
                    isdown = true;
                } else {
                    isdown = false;
                }
                $(".nano").nanoScroller({ scrollBottom: downY });
            }
        });
        $("#getcalendars").hover(function () {
            var hei = $("body").height() - $("#TopbarWapper").height();
            $(".MonitorData_Zoom").attr("style", "display:block;visibility: initial;height:" + hei + "px;");
            $("#FinceIframe").css("height", hei);
        }, function () {
            $(".MonitorData_Zoom").removeAttr("style");
        });
         $('#jczs,#gjjs,#zncx,#xwkx').myScroll({
        speed: 40, //数值越大，速度越慢
        rowHeight: 32 //li的高度
    });

    },
    IreSize: function () {
        $("#MainContent").height($(window).height() - 52);
        $("#ChatMsgWrapper").height($(window).height() - 93);
        $("#MsgListWrapper").height($("#ChatMsgWrapper").height() - $("#ServiceQQs").height() - $("#ToolBarWapper").height() - $("#ClearToName").height() - $("#SayingInfoWrapper").height() - 100);
        if ($("#SysColumnWapper").is(":visible")) {
            $("#MainChatWapper").width($("#MainContent").width() - $("#SysColumnWapper").width() - $("#MainLiveTVWapper").width() - 40);
        } else {
            $("#MainChatWapper").width($("#MainContent").width() - $("#MainLiveTVWapper").width() - 40);
        }
        if (homeMain.ibrowser.mobile == true) {
            $("#MainChatWapper").height($("#MainContent").height() - $("#MainLiveTVWapper").height());
            $("#ChatMsgWrapper").height($("#MainChatWapper").height() - 42);
            $("#MsgListWrapper").height($("#ChatMsgWrapper").height() - 150);
        }
    },
};
$(function () {
    var oldError = window.onerror;
    window.onerror = function myErrorHandler(errorMsg, url, lineNumber) {
        if (oldError)
            return oldError(errorMsg, url, lineNumber);
        return false;
    }
    homeMain.IreSize();
    ko.applyBindings(homeMain);
    homeMain.Init();
    homeMain.InitContext();
});
$(window).resize(function () {
    homeMain.IreSize();
});
