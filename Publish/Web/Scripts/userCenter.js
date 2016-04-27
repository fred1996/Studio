
var user = {
    ajaxUrl: '../ajax/ajaxUserCenter.aspx',
    roomAjaxUrl: '../ajax/ajaxRoomMan.aspx',
    result: null,
    username: "Mars",
    method: {
        editUser: function (para) {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: para,
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        loadUserBaseInfo: function () {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: { op: 'loadUserBaseInfo' },
                dataType: "json",
                async: false,
                error: function (msg) {
                    alert(msg);
                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        getAllTags: function () {
            $.ajax({
                url: user.ajaxUrl,
                data: { op: 'getalltags' },
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        getUseTags: function () {
            $.ajax({
                url: user.ajaxUrl,
                data: { op: 'getusertags' },
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        editUserTags: function (tags) {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: tags,
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        getUserAddress: function (param) {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: param,
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        address_add: function (address) {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: address,
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        getAddressbyid: function (para) {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: para,
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },

        address_dialog: function () {
            $.fancybox.open($("#Address"), {
                width: 525,
                height: 250,
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
                hideOnOverlayClick: false,
                hideOnContentClick: false,
                overlayShow: true,
                //title: '添加地址'
            });
        },
        delAddress: function (param) {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: param,
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },


        sendsms: function (param) {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: param,
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        sendmail: function (param) {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: param,
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        checkcode: function (param) {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: param,
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        setpwd: function (param) {
            $.ajax({
                type: 'post',
                url: user.ajaxUrl,
                data: param,
                dataType: 'json',
                async: false,
                error: function (msg) {

                },
                success: function (res) {
                    user.result = res;
                }
            });
            return user.result;
        },
        //////////////////////////////用户登录、注册、找回密码////////////////////////////////////

        isValExists: function (value, vType) {
            if (!!value && value.length > 0) {
                var setting = {};
                setting.op = "hasvalexists";
                setting.inputval = value;
                setting.inputtype = vType;
                $.ajax({
                    type: "POST",
                    url: user.roomAjaxUrl,
                    dataType: "JSON",
                    data: setting,
                    error: function (msg) {
                        console.log(msg);
                        return false;
                    },
                    success: function (res) {
                        if (!!res && !res.IsSuccess) {
                            if (!!res.Errors) {
                                $(res.Errors[0].ObjectInstance).children().eq(1).text(res.Errors[0].Message);
                                $(res.Errors[0].ObjectInstance).show();
                            }
                            else {
                                $(res.Errors[0].ObjectInstance).empty();
                            }
                        }
                    }
                });
            }
            return false;
        },
        userRegister: function (param) {
            $.ajax({
                type: "POST",
                url: user.roomAjaxUrl,
                data: param,
                dataType: "JSON",
                error: function (msg) {
                    console.log(msg);
                    return false;
                },
                success: function (res) {
                    if (!!res && !!res.IsSuccess) {

                        window.location.href = "Login.html";

                    } else if (!!res.Errors) {
                        alert(res.Errors[0].Message)
                        //for (var i = 0; i < res.Errors.length; i++) {
                        //    var $obj = $(res.Errors[i].ObjectInstance);
                        //    $obj.show();
                        //    $obj.text(res.Errors[i].Message);
                        //}
                    }
                }
            });
        },
        RefreshValidateCode: function () {
            //$("#UserCodeimg").attr('src', 'http://localhost:40475/GenerateImg.aspx?refresh=0.7624140952248126');
            var codeUrl = window.location.protocol + "//" + window.location.host + '/GenerateImg.aspx?refresh=' + Math.random();
            $("#UserCodeimg").attr('src', codeUrl);
            $("#UserRandcodeError").hide();
        },
        isValidateCode: function (rcode) {
            if (!!rcode && rcode.length > 0) {
                var setting = {};
                setting.op = "validatecode";
                setting.vcode = rcode;
                $.ajax({
                    type: "POST",
                    url: user.roomAjaxUrl,
                    data: setting,
                    dataType: "JSON",
                    async: false,
                    error: function (msg) {
                        console.log(msg);
                        return false;
                    },
                    success: function (res) {
                        if (!!res && !res.IsSuccess) {
                            if (!!res.Errors) {
                                $(res.Errors[0].ObjectInstance).children().eq(1).text(res.Errors[0].Message);
                                $(res.Errors[0].ObjectInstance).show();
                                user.result = false;
                            }
                        }
                        else {
                            user.result = true;
                            // $(res.Errors[0].ObjectInstance).hide();
                            //return true;
                        }
                    }
                });
            }
            return user.result;
        },
        isValidateName: function(para) {
            $.ajax({
                type: "post",
                url: user.ajaxUrl,
                data: para,
                dataType: "json",
                error: function (msg) {
                    console.log(msg);
                    return false;
                },
                success: function (res) {
                    if (!!res && !res.IsSuccess) {
                        if (!!res.Errors) {
                            $(res.Errors[0].ObjectInstance).children().eq(1).text(res.Errors[0].Message);
                            $(res.Errors[0].ObjectInstance).show();
                            return true;
                        }
                    }
                    else {
                        alert("邮件已发送，请立即查收");
                        $("#UserNameError").hide();
                    }
                }
            });
            return false;
        },
        userLogout: function (setting) {
            $.ajax({
                type: "POST",
                url: user.roomAjaxUrl,
                data: setting,
                dataType: "JSON",
                error: function(err) {

                },
                success: function(res) {
                    window.location.href = "../Login.html";
                }
            });
        }
    },
    events: {
        userRegisterEvent: function () {
            if (!$("#check").is(':checked')) {
                alert("请选择同意用户注册协议");
                return false;
            }
            var options = {};
            options.email = $("#UserEmail").val();
            options.nickname = $("#UserNickname").val();
            options.tel = $("#UserTel").val();
            options.pwd = $("#UserPwd").val();
            options.vcode = $("#UserRandcode").val();
            options.op = 'ureg';
            if (options.username === "" || options.nickname === "" || options.tel === "" || options.pwd === "") {
                return false;
            }
            user.method.userRegister(options);
        },
        userLogoutEvent: function () {
            var setting = {};
            setting.op = "logout";
            user.method.userLogout(setting);
            //client.socket().emit('disconnect');

        },
        validateRegInputError: function () {
            var errorNum = 0;
            $("#UserEmail").bind({
                keyup: function () {
                    var tmpVal = $(this).val();
                    if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {

                        $("#UserEmailError").children().eq(1).text('注册邮箱不能空');
                        errorNum++;
                    }
                    else if (!RegExp.isEmail(tmpVal)) {

                        $("#UserEmailError").children().eq(1).text('请输入有效邮箱');
                        errorNum++;
                    }
                    else if (homeMain.isValExists(tmpVal, 'Email', '#UserEmailError')) {

                        //$("#UserEmailError").text('此邮箱已经被占用');
                        errorNum++;
                    }
                    else {
                        errorNum = 0;
                    }
                    if (errorNum > 0) {
                        $("#UserEmailError").children().eq(0).attr("src", "images/e.png");
                        $("#UserEmailError").show();
                        $("#btnUserReg").attr('disabled', 'disabled');
                    }
                    else {
                        $("#UserEmailError").children().eq(0).attr("src", "images/q.png");
                        $("#UserEmailError").children().eq(1).text('');
                        $("#UserEmailError").show();
                    }
                    if ($("#UserEmail").val() === "" || $("#UserNickname").val() === "" || $("#UserTel").val() === "" || $("#UserPwd").val() === "" || $("#UserRepwd").val() === "" || $("#UserRandcode").val() === "") {
                        $("#btnUserReg").attr('disabled', 'disabled');
                        return false;
                    } else {
                        if (errorNum == 0) $("#btnUserReg").removeAttr('disabled');
                    }
                    return false;
                }
            });
            $("#UserNickname").bind({
                keyup: function () {
                    var tmpVal = $(this).val();
                    if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {

                        $("#UserNMError").children().eq(1).text('昵称不能空');
                        errorNum++;
                    }
                    else if (RegExp.isContainSpecial(tmpVal)) {
                        $("#UserNMError").children().eq(1).text('昵称只支持数字，字母和_组合');
                        errorNum++;
                    }
                    else if (tmpVal.length > 64) {

                        $("#UserNMError").children().eq(1).text('昵称长度不允许超出64个字符');
                        errorNum++;
                    }
                    else if (tmpVal.length <= 4) {

                        $("#UserNMError").children().eq(1).text('昵称长度不允许少于4个字符');
                        errorNum++;
                    }
                    else if (user.method.isValExists(tmpVal, 'UserName', '#UserNMError')) {
                        $("#UserNMError").children().eq(1).text('此昵称太火，已被注册');
                        errorNum++;
                    }
                    else {
                        errorNum = 0;
                    }
                    if (errorNum > 0) {
                        $("#UserNMError").children().eq(0).attr("src", "images/e.png");
                        $("#UserNMError").show();
                        $("#btnUserReg").attr('disabled', 'disabled');
                    }
                    else {
                        $("#UserNMError").children().eq(0).attr("src", "images/q.png");
                        $("#UserNMError").children().eq(1).text('');
                        $("#UserNMError").show();
                    }
                    if ($("#UserEmail").val() === "" || $("#UserNickname").val() === "" || $("#UserTel").val() === "" || $("#UserPwd").val() === "" || $("#UserRepwd").val() === "" || $("#UserRandcode").val() === "") {
                        $("#btnUserReg").attr('disabled', 'disabled');
                        return false;
                    } else {
                        if (errorNum == 0) $("#btnUserReg").removeAttr('disabled');
                    }
                    return false;
                }
            });

            $("#UserTel").bind({
                keyup: function () {
                    var tmpVal = $(this).val();
                    if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {

                        $("#UserTelError").children().eq(1).text('手机号不能空');
                        errorNum++;
                    }
                    else if (!RegExp.isMobile(tmpVal)) {
                        $("#UserTelError").children().eq(1).text('请输入有效手机号');
                        errorNum++;
                    }
                    else if (user.method.isValExists(tmpVal, 'Telephone', '#UserTelError')) {
                        errorNum++;
                    }
                    else {
                        errorNum = 0;
                    }
                    if (errorNum > 0) {
                        $("#UserTelError").children().eq(0).attr("src", "images/e.png");
                        $("#UserTelError").show();
                        $("#btnUserReg").attr('disabled', 'disabled');
                    }
                    else {
                        $("#UserTelError").children().eq(0).attr("src", "images/q.png");
                        $("#UserTelError").children().eq(1).text('');
                        $("#UserTelError").show();
                    }
                    if ($("#UserEmail").val() === "" || $("#UserNickname").val() === "" || $("#UserTel").val() === "" || $("#UserPwd").val() === "" || $("#UserRepwd").val() === "" || $("#UserRandcode").val() === "") {
                        $("#btnUserReg").attr('disabled', 'disabled');
                        return false;
                    } else {
                        if (errorNum == 0) $("#btnUserReg").removeAttr('disabled');
                    }
                    return false;
                }
            });
            $("#UserPwd").bind({
               
                keyup: function () {
                    var istrue = true;
                    var tmpVal = $(this).val();
                    if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
                        $("#UserPwdError").children().eq(1).text('密码不能空');
                        errorNum++;
                    }
                    else if (tmpVal.length < 6) {
                        $("#UserPwdError").children().eq(1).text('密码长度不允许少于6位');
                        errorNum++;
                    } else if (tmpVal.length > 12) {
                        $("#UserPwdError").children().eq(1).text('密码长度不允许超过12位');
                        errorNum++;
                    } else if ($("#UserRepwd").val() != "" && tmpVal != $("#UserRepwd").val()) {
                        $("#UserRePwdError").children().eq(1).text('两次输入的密码不一致');
                        $("#UserRePwdError").children().eq(0).attr("src", "images/e.png");
                        $("#UserPwdError").children().eq(0).attr("src", "images/q.png");
                        $("#UserPwdError").children().eq(1).text('');
                        $("#btnUserReg").attr('disabled', 'disabled');
                        istrue = false;
                        errorNum --;
                        //errorNum++;
                    } else {
                        errorNum = 0;
                    }
                    if (errorNum > 0) {
                        $("#UserPwdError").children().eq(0).attr("src", "images/e.png");
                        $("#UserPwdError").show();
                        $("#btnUserReg").attr('disabled', 'disabled');
                    }
                    else {
                        $("#UserPwdError").children().eq(0).attr("src", "images/q.png");
                        $("#UserPwdError").children().eq(1).text('');
                        $("#UserPwdError").show();
                    }
                    if ($("#UserEmail").val() === "" || $("#UserNickname").val() === "" || $("#UserTel").val() === "" || $("#UserPwd").val() === "" || $("#UserRepwd").val() === "" || $("#UserRandcode").val() === "") {
                        $("#btnUserReg").attr('disabled', 'disabled');
                        return false;
                    } else {
                        if (errorNum == 0 && istrue) $("#btnUserReg").removeAttr('disabled');
                    }
                    return false;
                }
            });
            $("#UserRepwd").bind({
                keyup: function () {
                    var tmpVal = $(this).val();
                    if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
                        $("#UserRePwdError").children().eq(1).text('密码不能空');
                        errorNum++;
                    }
                    else if (tmpVal.length < 6) {
                        $("#UserRePwdError").children().eq(1).text('密码长度不允许少于6位');
                        errorNum++;
                    } else if (tmpVal.length > 12) {
                        $("#UserRePwdError").children().eq(1).text('密码长度不允许超过12位');
                        errorNum++;
                    } else if (tmpVal != $("#UserPwd").val()) {
                        $("#UserRePwdError").children().eq(1).text('两次输入的密码不一致');
                        errorNum++;
                    } else {
                        errorNum = 0;
                    }
                    if (errorNum > 0) {
                        $("#UserRePwdError").children().eq(0).attr("src", "images/e.png");
                        $("#UserRePwdError").show();
                        $("#btnUserReg").attr('disabled', 'disabled');
                    }
                    else {
                        $("#UserRePwdError").children().eq(0).attr("src", "images/q.png");
                        $("#UserRePwdError").children().eq(1).text('');
                        $("#UserRePwdError").show();
                    }
                    if ($("#UserEmail").val() === "" || $("#UserNickname").val() === "" || $("#UserTel").val() === "" || $("#UserPwd").val() === "" || $("#UserRepwd").val() === "" || $("#UserRandcode").val() === "") {
                        $("#btnUserReg").attr('disabled', 'disabled');
                        return false;
                    } else {
                        if (errorNum ==0) $("#btnUserReg").removeAttr('disabled');
                    }
                    return false;
                }
            });
            $("#UserRandcode").bind({
                keyup: function () {
                    var tmpVal = $(this).val();
                    if (!tmpVal || tmpVal == '' || tmpVal == 'undefined') {
                        $("#UserRandcodeError").children().eq(1).text('请输入随机码');
                        errorNum++;
                    } else if (!user.method.isValidateCode(tmpVal)) {
                        $("#UserRandcodeError").children().eq(1).text('请输入正确的随机码');
                        errorNum++;
                    }
                    else {
                        errorNum = 0;
                    }
                    if (errorNum > 0) {
                        $("#UserRandcodeError").children().eq(0).attr("src", "images/e.png");
                     
                        $("#btnUserReg").attr('disabled', 'disabled');
                    }
                    else {
                     
                        homeMain.RegisterData.CodeError('');
                    }
                    if ($("#UserEmail").val() === "" || $("#UserNickname").val() === "" || $("#UserTel").val() === "" || $("#UserPwd").val() === "" || $("#UserRepwd").val() === "" || homeMain.RegisterData.CodeError()) {
                        $("#btnUserReg").attr('disabled', 'disabled');
                        return false;
                    } else {
                        if (errorNum == 0) $("#btnUserReg").removeAttr('disabled');
                    }
                    return false;
                }
            });

        }
    }
};