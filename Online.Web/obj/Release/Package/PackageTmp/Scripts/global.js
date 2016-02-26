
//封装StringBuilder
function StringBuilder() { this._string_ = new Array(); }
StringBuilder.prototype.Append = function (str) { this._string_.push(str); }
StringBuilder.prototype.toString = function () { return this._string_.join(""); }
StringBuilder.prototype.AppendFormat = function () {
    if (arguments.length > 1) {
        var TString = arguments[0];
        if (arguments[1] instanceof Array) {
            for (var i = 0, iLen = arguments[1].length; i < iLen; i++) {
                var jIndex = i;
                var re = eval("/\\{" + jIndex + "\\}/g;");
                TString = TString.replace(re, arguments[1][i]);
            }
        } else {
            for (var i = 1, iLen = arguments.length; i < iLen; i++) {
                var jIndex = i - 1;
                var re = eval("/\\{" + jIndex + "\\}/g;");
                TString = TString.replace(re, arguments[i]);
            }
        }
        this.Append(TString);
    } else if (arguments.length == 1) {
        this.Append(arguments[0]);
    }
};

//trim去掉字符串两边的指定字符，默认去空格
String.prototype.Trim = function (str) { if (!str) { str = '\\s'; } else { if (str == '\\') { str = '\\\\'; } else if (str == ',' || str == '|' || str == ';' || str == '-') { str = '\\' + str; } else { str = '\\s'; } } eval('var reg=/(^' + str + '+)|(' + str + '+$)/g;'); return this.replace(reg, ''); };
String.prototype.trim = function (str) { return this.Trim(str); };
//判断一个字符串是否为NULL或者空字符串
String.prototype.isNull = function () { return this == null || this.trim().length == 0; }
String.prototype.equals = function (str) { return this == str; }
String.prototype.contains = function (str) { if (str) return this.indexOf(str) != -1; else return false; }
//字符串截取，后面加入...
String.prototype.interceptString = function (len) {
    if (this.length > len) {
        return this.substring(0, len - 1) + "...";
    }
    else {
        return this;
    }
}

////获取URL里的参数，返回一个参数数组
////调用方法如下
//var Request = GetRequest();
//var 参数1,参数2,参数N;
//参数1 = Request['参数1'];
//参数2 = Request['参数2'];
//参数N = Request['参数N'];  
function GetRequest() {
    var url = location.search;; //获取url中"?"符后的字串  
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substring(url.indexOf("?") + 1);
        str = str.replace(/#/g, "");
        if (url.indexOf("&") == -1)
            theRequest[str.substring(0, str.indexOf("="))] = str.substring(str.indexOf("=") + 1);
        else {
            var strs = str.split("&");
            for (var i = 0; i < strs.length; i++) {
                theRequest[strs[i].substring(0, strs[i].indexOf("="))] = strs[i].substring(strs[i].indexOf("=") + 1);
            }

        }

    }
    return theRequest;
}

/*
* Placeholder plugin for jQuery
* ---
* Copyright 2010, Daniel Stocks (http://webcloud.se)
* Released under the MIT, BSD, and GPL Licenses.
*/
//(function ($) {
//    function Placeholder(input) {
//        this.input = input;
//        if (input.attr('type') == 'password') {
//            this.handlePassword();
//        }
//        // Prevent placeholder values from submitting
//        $(input[0].form).submit(function () {
//            if (input.hasClass('placeholder') && input[0].value == input.attr('placeholder')) {
//                input[0].value = '';
//            }
//        });
//    }
//    Placeholder.prototype = {
//        show: function (loading) {
//            // FF and IE saves values when you refresh the page. If the user refreshes the page with
//            // the placeholders showing they will be the default values and the input fields won't be empty.
//            if (this.input[0].value === '' || (loading && this.valueIsPlaceholder())) {
//                if (this.isPassword) {
//                    try {
//                        this.input[0].setAttribute('type', 'text');
//                        $(this.input[0]).removeAttr("readonly");
//                    } catch (e) {
//                        this.input.after(this.fakePassword.show()).hide();
//                    }
//                }
//                this.input.addClass('placeholder');
//                this.input[0].value = this.input.attr('placeholder');
//            }
//        },
//        hide: function () {
//            if (this.valueIsPlaceholder() && this.input.hasClass('placeholder')) {
//                this.input.removeClass('placeholder');
//                this.input[0].value = '';
//                if (this.isPassword) {
//                    try {
//                        this.input[0].setAttribute('type', 'password');
//                        $(this.input[0]).removeAttr("readonly");
//                    } catch (e) { }
//                    // Restore focus for Opera and IE
//                    this.input.show();
//                    this.input[0].focus();
//                }
//            }
//        },
//        valueIsPlaceholder: function () {
//            return this.input[0].value == this.input.attr('placeholder');
//        },
//        handlePassword: function () {
//            var input = this.input;
//            input.attr('realType', 'password');
//            this.isPassword = true;
//            // IE < 9 doesn't allow changing the type of password inputs
//            if ($.browser.msie && input[0].outerHTML) {
//                var fakeHTML = $(input[0].outerHTML.replace(/type=(['"])?password\1/gi, 'type=$1text$1'));
//                this.fakePassword = fakeHTML.val(input.attr('placeholder')).addClass('placeholder').focus(function () {
//                    input.trigger('focus');
//                    $(this).hide();
//                }).attr("readonly", "readonly");
//                $(input[0].form).submit(function () {
//                    fakeHTML.remove();
//                    input.show()
//                });
//            }
//        }
//    };
//    var NATIVE_SUPPORT = !!("placeholder" in document.createElement("input"));
//    $.fn.placeholder = function () {
//        return NATIVE_SUPPORT ? this : this.each(function () {
//            var input = $(this);
//            var placeholder = new Placeholder(input);
//            placeholder.show(true);
//            input.focus(function () {
//                placeholder.hide();
//            });
//            input.blur(function () {
//                placeholder.show(false);
//            });

//            // On page refresh, IE doesn't re-populate user input
//            // until the window.onload event is fired.
//            if ($.browser.msie) {
//                $(window).load(function () {
//                    if (input.val()) {
//                        input.removeClass("placeholder");
//                    }
//                    placeholder.show(true);
//                });
//                // What's even worse, the text cursor disappears
//                // when tabbing between text inputs, here's a fix
//                input.focus(function () {
//                    if (this.value == "") {
//                        var range = this.createTextRange();
//                        range.collapse(true);
//                        range.moveStart('character', 0);
//                        range.select();
//                    }
//                });
//            }
//        });
//    }
//})(jQuery);
//$(document).ready(function () {
//    if ($('[placeholder]').placeholder) {

//        $('[placeholder]').placeholder();
//    }
//});

//为异步请求提供公用方法
//requestUrl 请求连接，如：GetAjaxValue.aspx
//requestType 请求类型，如：GET、POST、JSON
//requestData 请求传递数据，如：name=mytest&psd=meihua
//callbackFunction 返回处理函数，如：function SelectedItem(data){}
//loadingElementId 请求时呈现图片元素ID，如：#city 可以不填充
function AjaxRequest(requestUrl, requestType, requestData, loadingElementId, callbackFunction) {
    if (loadingElementId != null && loadingElementId != '')
        $(loadingElementId).html("<div class=\"TxtCenter\"><img src=\"images/ajax-loader.gif\"></div>");
    if (requestType != null && requestType != '' && requestType.toUpperCase() != 'JSON') {
        $.ajax({
            url: requestUrl,
            type: requestType,
            data: requestData,
            cache: false,
            success: function (data) {
                if (loadingElementId != null && loadingElementId != '') $(loadingElementId).html('');
                callbackFunction(data);
            }
        });
    }
    else {
        $.ajax({
            url: requestUrl,
            data: requestData,
            cache: false,
            success: function (data) {
                if (loadingElementId != null && loadingElementId != '') $(loadingElementId).html('');
                callbackFunction(data);
            }
        });
    }
}


/**
03
* 阿拉伯数字转换为中文大写
04
* 整理：www.jbxue.com
05
*/

function NumberToString(num) {
    if (!/^\d*(\.\d*)?$/.test(num)) { alert("Number is wrong!"); return "Number is wrong!"; }
    var AA = new Array("零", "一", "二", "三", "四", "五", "六", "七", "八", "九");
    var BB = new Array("", "十", "百", "千", "万", "亿", "点", "");
    var a = ("" + num).replace(/(^0*)/g, "").split("."), k = 0, re = "";
    for (var i = a[0].length - 1; i >= 0; i--) {
        switch (k) {
            case 0: re = BB[7] + re;
                break;
            case 4: if (!new RegExp("0{4}\\d{" + (a[0].length - i - 1) + "}$").test(a[0]))
                re = BB[4] + re;
                break;
            case 8: re = BB[5] + re; BB[7] = BB[5]; k = 0;
                break;
        }
        if (k % 4 == 2 && a[0].charAt(i + 2) != 0 && a[0].charAt(i + 1) == 0) re = AA[0] + re;
        if (a[0].charAt(i) != 0) re = AA[a[0].charAt(i)] + BB[k % 4] + re; k++;
    }

    if (a.length > 1) //加上小数部分(如果有小数部分)
    {
        re += BB[6];
        for (var i = 0; i < a[1].length; i++) re += AA[a[1].charAt(i)];
    }
    return re;
}
/*验证**/
//验证一个字符串是否包含特殊字符
RegExp.isContainSpecial = function (str) {
    var containSpecial = RegExp(/[(\,)(\\)(\/)(\:)(\*)(\')(\?)(\\\)(\<)(\>)(\|)]+/);
    return (containSpecial.test(str));
}
//验证一个字符串时候是email
RegExp.isEmail = function (str) {
    var emailReg = /^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)*\.[\w-]+$/i;
    return emailReg.test(str);
}
//验证一个字符串是否是
RegExp.isUrl = function (str) {
    var patrn = /^http(s)?:\/\/[A-Za-z0-9\-]+\.[A-Za-z0-9\-]+[\/=\?%\-&_~`@[\]\:+!]*([^<>])*$/;
    return patrn.exec(str);
}
//验证一个字符串是否是电话或传真
RegExp.isTel = function (str) {
    var pattern = /^[+]?((\d){3,4})?([ ]?[-]?(\d){7,8})([ ]?[-]?(\d){1,12})?$/;
    return pattern.exec(str);
}
//验证一个字符串是否是手机号码
RegExp.isMobile = function (str) {
    var patrn = /^(1[3-8]{1})\d{9}$/;
    return patrn.exec(str);

}
//验证一个字符串是否是传真号
RegExp.isFax = function (str) {
    var patrn = /^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$/;
    return patrn.exec(str);

}
//验证一个字符串是否为外国手机号码
RegExp.isElseMobile = function (str) {
    var patrn = /^\d{5}\d*$/;
    return patrn.exec(str);
}
//验证一个字符串是否是汉字
RegExp.isZHCN = function (str) {
    var p = /^[\u4e00-\u9fa5\w]+$/;
    return p.exec(str);
}
//验证一个字符串是否是数字
RegExp.isNum = function (str) {
    var p = /^\d+$/;
    return p.exec(str);
}
//验证一个字符串是否是纯英文
RegExp.isEnglish = function (str) {
    var p = /^[a-zA-Z., ]+$/;
    return p.exec(str);
}
// 判断是否为对象类型
RegExp.isObject = function (obj) {
    return (typeof obj == 'object') && obj.constructor == Object;
}
//验证字符串是否不包含特殊字符 返回bool
RegExp.isUnSymbols = function (str) {
    var p = /^[A-Za-z0-9\u0391-\uFFE5 \.,()，。（）]+$/;
    return p.exec(str);
}
//设为首页
function SetHome(obj, url) {
    try {
        obj.style.behavior = 'url(#default#homepage)';
        obj.setHomePage(url);
    } catch (e) {
        if (window.netscape) {
            try {
                netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
            } catch (e) {
                alert("抱歉，此操作被浏览器拒绝！\n\n请在浏览器地址栏输入“about:config”并回车然后将[signed.applets.codebase_principal_support]设置为'true'");
            }
        } else {
            alert("抱歉，您所使用的浏览器无法完成此操作。\n\n您需要手动将【" + url + "】设置为首页。");
        }
    }
}
//收藏本站
function AddFavorite(title, url) {
    try {
        window.external.addFavorite(url, title);
    }
    catch (e) {
        try {
            window.sidebar.addPanel(title, url, "");
        }
        catch (e) {
            alert("抱歉，您所使用的浏览器无法完成此操作。\n\n加入收藏失败，请使用Ctrl+D进行添加");
        }
    }
}
//保存到桌面
function ToDesktop(sUrl, sName) {
    try {
        var WshShell = new ActiveXObject("WScript.Shell");
        var oUrlLink = WshShell.CreateShortcut(WshShell.SpecialFolders("Desktop") + "\\" + sName + ".url");
        oUrlLink.TargetPath = sUrl;
        oUrlLink.Save();
    }
    catch (e) {
        alert("当前IE安全级别不允许操作！");
    }
}

var JumboTCMS = new Object();
var hexcase = 0;//1为大写
var chrsz = 8;

JumboTCMS.MD5 = function (s) {
    if (s.length == 32)
        return s;
    return _jcms_binl2hex(_jcms_core_md5(_jcms_str2binl(s), s.length * chrsz));
}

function _jcms_core_md5(x, len) {

    x[len >> 5] |= 0x80 << ((len) % 32);
    x[(((len + 64) >>> 9) << 4) + 14] = len;

    var a = 1732584193;
    var b = -271733879;
    var c = -1732584194;
    var d = 271733878;

    for (var i = 0; i < x.length; i += 16) {
        var olda = a;
        var oldb = b;
        var oldc = c;
        var oldd = d;

        a = md5_ff(a, b, c, d, x[i + 0], 7, -680876936);
        d = md5_ff(d, a, b, c, x[i + 1], 12, -389564586);
        c = md5_ff(c, d, a, b, x[i + 2], 17, 606105819);
        b = md5_ff(b, c, d, a, x[i + 3], 22, -1044525330);
        a = md5_ff(a, b, c, d, x[i + 4], 7, -176418897);
        d = md5_ff(d, a, b, c, x[i + 5], 12, 1200080426);
        c = md5_ff(c, d, a, b, x[i + 6], 17, -1473231341);
        b = md5_ff(b, c, d, a, x[i + 7], 22, -45705983);
        a = md5_ff(a, b, c, d, x[i + 8], 7, 1770035416);
        d = md5_ff(d, a, b, c, x[i + 9], 12, -1958414417);
        c = md5_ff(c, d, a, b, x[i + 10], 17, -42063);
        b = md5_ff(b, c, d, a, x[i + 11], 22, -1990404162);
        a = md5_ff(a, b, c, d, x[i + 12], 7, 1804603682);
        d = md5_ff(d, a, b, c, x[i + 13], 12, -40341101);
        c = md5_ff(c, d, a, b, x[i + 14], 17, -1502002290);
        b = md5_ff(b, c, d, a, x[i + 15], 22, 1236535329);

        a = md5_gg(a, b, c, d, x[i + 1], 5, -165796510);
        d = md5_gg(d, a, b, c, x[i + 6], 9, -1069501632);
        c = md5_gg(c, d, a, b, x[i + 11], 14, 643717713);
        b = md5_gg(b, c, d, a, x[i + 0], 20, -373897302);
        a = md5_gg(a, b, c, d, x[i + 5], 5, -701558691);
        d = md5_gg(d, a, b, c, x[i + 10], 9, 38016083);
        c = md5_gg(c, d, a, b, x[i + 15], 14, -660478335);
        b = md5_gg(b, c, d, a, x[i + 4], 20, -405537848);
        a = md5_gg(a, b, c, d, x[i + 9], 5, 568446438);
        d = md5_gg(d, a, b, c, x[i + 14], 9, -1019803690);
        c = md5_gg(c, d, a, b, x[i + 3], 14, -187363961);
        b = md5_gg(b, c, d, a, x[i + 8], 20, 1163531501);
        a = md5_gg(a, b, c, d, x[i + 13], 5, -1444681467);
        d = md5_gg(d, a, b, c, x[i + 2], 9, -51403784);
        c = md5_gg(c, d, a, b, x[i + 7], 14, 1735328473);
        b = md5_gg(b, c, d, a, x[i + 12], 20, -1926607734);

        a = md5_hh(a, b, c, d, x[i + 5], 4, -378558);
        d = md5_hh(d, a, b, c, x[i + 8], 11, -2022574463);
        c = md5_hh(c, d, a, b, x[i + 11], 16, 1839030562);
        b = md5_hh(b, c, d, a, x[i + 14], 23, -35309556);
        a = md5_hh(a, b, c, d, x[i + 1], 4, -1530992060);
        d = md5_hh(d, a, b, c, x[i + 4], 11, 1272893353);
        c = md5_hh(c, d, a, b, x[i + 7], 16, -155497632);
        b = md5_hh(b, c, d, a, x[i + 10], 23, -1094730640);
        a = md5_hh(a, b, c, d, x[i + 13], 4, 681279174);
        d = md5_hh(d, a, b, c, x[i + 0], 11, -358537222);
        c = md5_hh(c, d, a, b, x[i + 3], 16, -722521979);
        b = md5_hh(b, c, d, a, x[i + 6], 23, 76029189);
        a = md5_hh(a, b, c, d, x[i + 9], 4, -640364487);
        d = md5_hh(d, a, b, c, x[i + 12], 11, -421815835);
        c = md5_hh(c, d, a, b, x[i + 15], 16, 530742520);
        b = md5_hh(b, c, d, a, x[i + 2], 23, -995338651);

        a = md5_ii(a, b, c, d, x[i + 0], 6, -198630844);
        d = md5_ii(d, a, b, c, x[i + 7], 10, 1126891415);
        c = md5_ii(c, d, a, b, x[i + 14], 15, -1416354905);
        b = md5_ii(b, c, d, a, x[i + 5], 21, -57434055);
        a = md5_ii(a, b, c, d, x[i + 12], 6, 1700485571);
        d = md5_ii(d, a, b, c, x[i + 3], 10, -1894986606);
        c = md5_ii(c, d, a, b, x[i + 10], 15, -1051523);
        b = md5_ii(b, c, d, a, x[i + 1], 21, -2054922799);
        a = md5_ii(a, b, c, d, x[i + 8], 6, 1873313359);
        d = md5_ii(d, a, b, c, x[i + 15], 10, -30611744);
        c = md5_ii(c, d, a, b, x[i + 6], 15, -1560198380);
        b = md5_ii(b, c, d, a, x[i + 13], 21, 1309151649);
        a = md5_ii(a, b, c, d, x[i + 4], 6, -145523070);
        d = md5_ii(d, a, b, c, x[i + 11], 10, -1120210379);
        c = md5_ii(c, d, a, b, x[i + 2], 15, 718787259);
        b = md5_ii(b, c, d, a, x[i + 9], 21, -343485551);

        a = _jcms_safe_add(a, olda);
        b = _jcms_safe_add(b, oldb);
        c = _jcms_safe_add(c, oldc);
        d = _jcms_safe_add(d, oldd);
    }
    return Array(a, b, c, d);

}


function _jcms_md5_cmn(q, a, b, x, s, t) {
    return _jcms_safe_add(_jcms_bit_rol(_jcms_safe_add(_jcms_safe_add(a, q), _jcms_safe_add(x, t)), s), b);
}
function md5_ff(a, b, c, d, x, s, t) {
    return _jcms_md5_cmn((b & c) | ((~b) & d), a, b, x, s, t);
}
function md5_gg(a, b, c, d, x, s, t) {
    return _jcms_md5_cmn((b & d) | (c & (~d)), a, b, x, s, t);
}
function md5_hh(a, b, c, d, x, s, t) {
    return _jcms_md5_cmn(b ^ c ^ d, a, b, x, s, t);
}
function md5_ii(a, b, c, d, x, s, t) {
    return _jcms_md5_cmn(c ^ (b | (~d)), a, b, x, s, t);
}

function _jcms_core_hmac_md5(key, data) {
    var bkey = _jcms_str2binl(key);
    if (bkey.length > 16) bkey = _jcms_core_md5(bkey, key.length * chrsz);

    var ipad = Array(16), ōpad = Array(16);
    for (var i = 0; i < 16; i++) {
        ipad[i] = bkey[i] ^ 0x36363636;
        opad[i] = bkey[i] ^ 0x5C5C5C5C;
    }

    var hash = _jcms_core_md5(ipad.concat(_jcms_str2binl(data)), 512 + data.length * chrsz);
    return _jcms_core_md5(opad.concat(hash), 512 + 128);
}

function _jcms_safe_add(x, y) {
    var lsw = (x & 0xFFFF) + (y & 0xFFFF);
    var msw = (x >> 16) + (y >> 16) + (lsw >> 16);
    return (msw << 16) | (lsw & 0xFFFF);
}

function _jcms_bit_rol(num, cnt) {
    return (num << cnt) | (num >>> (32 - cnt));
}


function _jcms_str2binl(str) {
    var bin = Array();
    var mask = (1 << chrsz) - 1;
    for (var i = 0; i < str.length * chrsz; i += chrsz)
        bin[i >> 5] |= (str.charCodeAt(i / chrsz) & mask) << (i % 32);
    return bin;
}



function _jcms_binl2hex(binarray) {
    var hex_tab = hexcase ? "0123456789ABCDEF" : "0123456789abcdef";
    var str = "";
    for (var i = 0; i < binarray.length * 4; i++) {
        str += hex_tab.charAt((binarray[i >> 2] >> ((i % 4) * 8 + 4)) & 0xF) +
               hex_tab.charAt((binarray[i >> 2] >> ((i % 4) * 8)) & 0xF);
    }
    return str;
}