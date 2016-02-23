var VoteObj = new Object();
VoteObj.removeVote = function (obj) {
    if (confirm("是否删除？")) {
        var id = $(obj).attr("vid");
        $.ajax({ url: "/Vote/delVote", data: { vid: id } }).done(function (data) {
            if (data.start > 0) {
                alert("删除成功");
                $(obj).parents("tr").remove();
            }
        });
    }
}
VoteObj.addvoteCheck = function () {
    if ($("#votetitle").val().length <= 0) {
        $("#votetitle").next("img").remove();
        $("#votetitle").after('<img src="/Image/images/icon_reg_error.png" class="img_error" width="15" height="15" style="display: inline;margin-left:10px;">');
        return false;
    } else {
        $("#votetitle").next("img").remove();
    }
    var f = true;
    $("[name='voteoption']").each(function () {
        if ($(this).val().length <= 0) {
            f = false;
            $(this).next("img").remove();
            $(this).after('<img src="/Image/images/icon_reg_error.png" class="img_error" width="15" height="15" style="display: inline;margin-left:10px;">');
        } else {
            $(this).next("img").remove();
        }
    });
    if (f == false) {
        return false;
    }
    if ($("#uservotecount").val().length <= 0) {
        $("#uservotecount").next("img").remove();
        $("#uservotecount").after('<img src="/Image/images/icon_reg_error.png" class="img_error" width="15" height="15" style="display: inline;margin-left:10px;">');
        return false;
    } else {
        $("#uservotecount").next("img").remove();
    }
    if ($("#begintime").val().length <= 0) {
        $("#begintime").next("img").remove();
        $("#begintime").after('<img src="/Image/images/icon_reg_error.png" class="img_error" width="15" height="15" style="display: inline;margin-left:10px;">');
        return false;
    } else {
        $("#begintime").next("img").remove();
    }
    if ($("#endtime").val().length <= 0) {
        $("#endtime").next("img").remove();
        $("#endtime").after('<img src="/Image/images/icon_reg_error.png" class="img_error" width="15" height="15" style="display: inline;margin-left:10px;">');
        return false;
    } else {
        $("#endtime").next("img").remove();
    }
    $(".img_error").remove();
    if (checkdate($("#begintime").val(), $("#endtime").val())) {
        return true;
    } else {
        return false;
    }
    return true;
}
VoteObj.addVote = function () {
    if (VoteObj.addvoteCheck()) {
        var votetitle = $("#votetitle").val();
        var voteoptions = [];
        $("[name='voteoption']").each(function () {
            if ($(this).val().length > 0) {
                voteoptions.push($(this).val());
            }
        });
        var opitem = '';
        for (var i = 0; i < voteoptions.length; i++) {
            opitem += voteoptions[i] + ",";
        }
        if (opitem.length > 0) {
            opitem = opitem.substring(0, opitem.length - 1);
        }
        var time = $("#endtime").val();
        var betime = $("#begintime").val();
        var voteanonymous = $("#selAnonymous").val();
        var voteMult = $("#selMult").val();
        var votereulstview = $("#selResult").val();
        var votedetail = $("#votedetail").val();
        var uservotecount = $("#uservotecount").val();
        $.ajax({ url: "/Vote/addVote", data: { usercount: uservotecount, votetitle: votetitle, option: opitem, endtime: time, anony: voteanonymous, mult: voteMult, viewresult: votereulstview, detail: votedetail, betime: betime } }).done(function (data) {
            if (data.start > 0) {
                alert("添加成功");
                window.parent.client.methods.NotifyServerShowVote();
            } else if (data.start == -1) {
                alert("投票标题已存在");
            } else {
                alert("添加失败");
                
            }
        });
    }
}
VoteObj.editVoteMapped = function (obj) {
    $("#votenav li:eq(3)").click();
    var title = $(obj).attr("title");
    var votecount = $(obj).attr("votecount");
    var uendtime = $(obj).attr("uendtime");
    var anony = $(obj).attr("anony");
    var mult = $(obj).attr("mult");
    var result = $(obj).attr("result");
    var detail = $(obj).attr("detail");
    var voteop = $(obj).attr("voteop");
    var begintime = $(obj).attr("begintime");
    $("#uvotetitle").val(title);
    $("#uuservotecount").val(votecount);
    $("#uendtime").val(uendtime);
    $("#ubegintime").val(begintime);
    mult = mult == "True" ? 1 : 0;
    result = result == "True" ? 1 : 0;
    anony = anony == "True" ? 1 : 0;
    $("#uselAnonymous").find("option[value='" + anony + "']").attr("selected", true);
    $("#uselMult").find("option[value='" + mult + "']").attr("selected", true);
    $("#uselResult").find("option[value='" + result + "']").attr("selected", true);
    $("#uvotedetail").val(detail);
    $.ajax({ url: "/Vote/GetUserVoteColum", data: { vid: $(obj).attr("vid") } }).done(function (data) {
        if (data.length > 0) {
            $(".setvotebox1").empty();
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                if (i == 0) {
                    $(".setvotebox1").append("<div><label>选项：</label><input type='text' disabled=\"disabled\" name='uvoteoption' value=" + item.Columname + "><a class='mLeft15' href='javascript:;' id='ubtnvoteaddop'>添加选项</a></div>");
                } else {
                    $(".setvotebox1").append("<div><label></label><input type='text' disabled=\"disabled\" name='uvoteoption' value=" + item.Columname + "><a class=\"mLeft15\" href=\"javascript:;\" onclick=\"removeUserVoteColum(" + item.ID + ",this)\">删除</a></div>");
                }
            }
            $("#ubtnvoteaddop").bind("click", function () {
                $(".setvotebox1").append("<div><label></label><input type='text' name='voteoption' /><a class='mLeft15' href=\"javascript:;\" onclick=\"SaveColum(" + $(obj).attr("vid") + ",this)\">保存添加</a><a class='mLeft15' href=\"javascript:;\" onclick=\"DelVoteColumDom(this)\">删除</a></div>");
            });
        }
    });
    $("#ubtnaddvote").attr("vid", $(obj).attr("vid"));
}
VoteObj.editSave = function () {
    if (checkupdatevote()) {
        var votetitle = $("#uvotetitle").val();
        var time = $("#uendtime").val();
        var voteanonymous = $("#uselAnonymous").val();
        var voteMult = $("#uselMult").val();
        var votereulstview = $("#uselResult").val();
        var votedetail = $("#uvotedetail").val();
        var uservotecount = $("#uuservotecount").val();
        var vid = $("#ubtnaddvote").attr("vid");
        $.ajax({ url: "/Vote/editVote", data: { id: vid, usercount: uservotecount, votetitle: votetitle, endtime: time, anony: voteanonymous, mult: voteMult, viewresult: votereulstview, detail: votedetail } }).done(function (data) {
            if (data.start >= 0) {
                alert("修改成功");
            } else {
                alert("修改失败");
            }
        });
    }
};
VoteObj.GetUserVoteResult = function () {
    $.ajax({ url: "/Vote/GetUserVoteResult", data: {} }).done(function (data) {
        if (data.length > 0) {
            var html = new StringBuilder();
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                console.log(item);
                for (var j = 0; j < item.UserVoteColums.length; j++) {
                    var im = item.UserVoteColums[j];
                    if (j == 0) {
                        html.Append("<tr><td>" + item.VoteTitle + "</td><td>" + item.CreateUser + "</td><td>" + im.Columname + "</td><td>" + im.VoteCount + "</td></tr>");
                    } else {
                        html.Append("<tr><td></td><td></td><td>" + im.Columname + "</td><td>" + im.VoteCount + "</td></tr>");
                    }
                }
            }
            $(".voteresult tbody").empty().append(html.toString());
        }
    });
}
$(function () {
    $(".voteDel").click(function () {
        VoteObj.removeVote(this);
    });
    $("#btnaddvote").bind('click', function () {
        VoteObj.addVote();
    });
    $(".voteEdid").click(function () {
        VoteObj.editVoteMapped($(this));
    });
    $("#ubtnaddvote").click(function () {
        VoteObj.editSave();
    });
    $("#btnvoteaddop").bind("click", function () {
        $(".setvotebox").append("<div><label></label><input type='text' name='voteoption' /><a class='mLeft15' href=\"javascript:;\" onclick=\"DelVoteColumDom(this)\">删除</a></div>");
    });
    $.datepicker.regional['zh-CN'] = {
        closeText: '关闭',
        prevText: '<上月',
        nextText: '下月>',
        currentText: '今天',
        monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
        '七月', '八月', '九月', '十月', '十一月', '十二月'],
        monthNamesShort: ['一', '二', '三', '四', '五', '六',
        '七', '八', '九', '十', '十一', '十二'],
        dayNames: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
        dayNamesShort: ['周日', '周一', '周二', '周三', '周四', '周五', '周六'],
        dayNamesMin: ['日', '一', '二', '三', '四', '五', '六'],
        weekHeader: '周',
        dateFormat: 'yy-mm-dd',
        firstDay: 1,
        showMonthAfterYear: true,
        yearSuffix: '年'
    };
    $.datepicker.setDefaults($.datepicker.regional['zh-CN']);
    $("#uendtime,#endtime").datepicker({
        inline: true
    });
    var myDate = new Date();
    var ye = myDate.getFullYear();
    var mo = myDate.getMonth() + 1;
    var da = myDate.getDate();
    $("#begintime").val(ye + "-" + mo + "-" + da);
    $("#begintime").datepicker({
    });

    $("#votenav li").bind("click", function () {
        $("#votenav li").removeClass("curren");
        $(this).addClass("curren");
        $("div.vote_box").removeClass("show");
        if ($(this).index() == 0) {
        }
        if ($(this).index() == 2) {
            VoteObj.GetUserVoteResult();
        }
        $("div.vote_box:eq(" + $(this).index() + ")").addClass("show");
    });
});

function checkdate(betime, entime) {
    //得到日期值并转化成日期格式，replace(//-/g, "//")是根据验证表达式把日期转化成长日期格式，这样
    //再进行判断就好判断了
    var sDate = new Date(betime.replace(/-/g, "//"));
    var eDate = new Date(entime.replace(/-/g, "//"));
    if (sDate > eDate) {
        alert("结束日期不能小于开始日期");
        return false;
    }
    return true;
}
function checkupdatevote() {
    if ($("#uvotetitle").val().length <= 0) {
        $("#uvotetitle").next("img").remove();
        $("#uvotetitle").after('<img src="/Image/images/icon_reg_error.png" class="img_error" width="15" height="15" style="display: inline;margin-left:10px;">');
        return false;
    } else {
        $("#uvotetitle").next("img").remove();
    }
    var f = true;
    $("[name='uvoteoption']").each(function () {
        if ($(this).val().length <= 0) {
            f = false;
            $(this).next("img").remove();
            $(this).after('<img src="/Image/images/icon_reg_error.png" class="img_error" width="15" height="15" style="display: inline;margin-left:10px;">');
        } else {
            $(this).next("img").remove();
        }
    });
    if (f == false) {
        return false;
    }
    if ($("#uuservotecount").val().length <= 0) {
        $("#uuservotecount").next("img").remove();
        $("#uuservotecount").after('<img src="/Image/images/icon_reg_error.png" class="img_error" width="15" height="15" style="display: inline;margin-left:10px;">');
        return false;
    } else {
        $("#uuservotecount").next("img").remove();
    }
    if (checkdate($("#ubegintime").val(), $("#uendtime").val())) {
        return true;
    } else {
        return false;
    }
    $(".img_error").remove();
    return true;
}
function removeUserVoteColum(id, obj) {
    $.ajax({ url: "/Vote/DelUserVoteColum", data: { id: id } }).done(function (data) {
        if (data) {
            alert("删除成功");
            $(obj).parent("div").remove();
        } else {
            alert("删除失败");
        }
    });
}

function SaveColum(id, obj) {
    var v = "";
    if ($(obj).parent("div").find("input").val().length > 0) {
        v = $(obj).parent("div").find("input").val();
    }
    $.ajax({ url: "/Vote/AddUserVoteColum", data: { vid: id, name: v } }).done(function (data) {
        if (data > 0) {
            alert("添加成功");
            $(obj).parent("div").find("input").nextAll().addClass("Hidden");
            $(obj).parent("div").find("input").attr("disabled", "disabled");
            $(obj).parent("div").append('<a class="mLeft15" href="javascript:;" onclick="removeUserVoteColum(' + data + ',this)">删除</a>');
        } else {
            alert("添加失败");
        }
    });
}
function DelVoteColumDom(obj) {
    $(obj).parent("div").remove();
}