var client = {
	config: {
		content: function() {
			return $("#MsgListWrapper");
		},
		fromUN: ($.cookie('curUser') || $("#loginInfo .usernameInput").val()),
		toUN: 'all'
	},
	socket: function() {
		return io.connect('http://127.0.0.1:1235');
	},
	methods: {
		online: function(jobj) {
			var p = '';
			if (jobj.user != client.config.fromUN) {
				p = '<div class="systemInfo">' + client.methods.now() + '&nbsp;&nbsp;管理员：<span class="sayingMan">' + jobj.user + '</span>上线了！</div>';
			} else {
				p = '<div class="systemInfo">' + client.methods.now() + '&nbsp;&nbsp;管理员：<span class="sayingMan">你</span>进入了聊天室！</div>';
			}

			client.config.content().prepend(p);
			//刷新当前在线用户
			client.methods.flushCurUsers(jobj.users);
			//显示对谁说话
			client.methods.showSayTo();
		},
		offline: function(jobj) {
			var p = '<div class="systemInfo">' + client.methods.now() + '&nbsp;&nbsp;管理员：用户' + jobj.user + ' 下线了！</div>'
			client.config.content().prepend(p);

			client.methods.flushCurUsers(jobj.users);

			if (jobj.user == client.config.toUN) { //自己下线默认发送人为'all'
				client.config.toUN = 'all';
			}
			client.methods.showSayTo();
		},
		disconnect: function() {
			var p = '';
			p = '<div class="systemInfo">系统:连接服务器失败！</div>'
			client.config.content().prepend(p);

			$("#UserList").empty();
		},
		reconnect: function() {
			var p = '';
			p = '<div class="systemInfo">系统:重新连接服务器！</div>'
			client.config.content().prepend(p);

			client.socket().emit('onlineEvent', {
				user: client.config.fromUN
			});
		},
		toSay: function(jobj) {
			var p = '';
			//对所有人说
			if (jobj.to == 'all') {
				p = '<div class="msgInfo"> <span class="sendTime">' + client.methods.now() + '</span><span class="userRole" >大管家</span><span class="sayingMan">' + jobj.from + '</span>对所有人说:<span id="sayingInfo">' + jobj.msg + '</span></div>';
			}
			//对你密语
			if (jobj.to == client.config.fromUN) {
				p = '<div class="msgInfo"> <span class="sendTime">' + client.methods.now() + '</span><span class="userRole" >大管家</span><span class="sayingMan">' + jobj.from + '</span>对你说:<span id="sayingInfo">' + jobj.msg + '</span></div>';
			}

			client.config.content().prepend(p);
		},
		sendMsg: function() {
		    var msg = homeMain.OnlineData.SayWord();//client.config.inputStr().val();
			if (!msg) return;
			var p = '';

			if (client.config.toUN == "all") {
				p = '<div class="msgInfo"> <span class="sendTime">' + client.methods.now() + '</span><span class="userRole" >太子</span><span class="sayingMan">你</span>对所有人说:<span id="sayingInfo">' + msg + '</span></div>';
			} else {
				p = '<div class="msgInfo"> <span class="sendTime">' + client.methods.now() + '</span><span class="userRole" >太子</span><span class="sayingMan">你</span>对' + client.config.toUN + '说:<span id="sayingInfo">' + msg + '</span></div>';
			}
			client.config.content().prepend(p);

			client.socket().emit('toSayEvent', {
				from: client.config.fromUN,
				to: client.config.toUN,
				msg: msg
			});
			client.config.inputStr().val('').focus();
		},
		//显示正在对谁说话
		showSayTo: function() {
			$("#ToolBarWapper .fromUser").html(client.config.fromUN);
			$("#ToolBarWapper .toUser").html(client.config.toUN == "all" ? "所有人" : client.config.toUN);
		},
		//刷新当前在线用户
		flushCurUsers: function(us) {
			var usWrapper = $("#UserList");
			var usStr = '';
			if (!!us) {
				usStr += '<li title="双击聊天" alt="all" class="sayingTo" onselectstart="return false">所有人</li>'
				for (var i in us) {
					usStr += '<li title="双击聊天" alt="' + us[i] + '" onselectstart="return false">' + us[i] + '</li>'
				}

				usWrapper.empty().append(usStr);
			}
			//添加选中聊天对象事件
			usWrapper.find('li').dblclick(function() {
				if ($(this).attr('alt') != client.config.fromUN) {
					client.config.toUN = $(this).attr('alt');

					usWrapper.find('li').removeClass('sayingTo');
					$(this).addClass('sayingTo');

					client.methods.showSayTo();
				}
			});
		},
		//获取当前时间
		now: function() {
			var date = new Date();
			var time = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate() + ' ' + date.getHours() + ':' + (date.getMinutes() < 10 ? ('0' + date.getMinutes()) : date.getMinutes()) + ":" + (date.getSeconds() < 10 ? ('0' + date.getSeconds()) : date.getSeconds());
			return time;
		}
	},
	events: {
		onlineEvent: function() {
			client.socket().on('onlineEvent', function(data) {
				client.methods.online(data);
			});
		},
		offlineEvent: function() {
			client.socket().on('offlineEvent', function(data) {
				client.methods.offline(data);
			});
		},
		disconnectEvent: function() {
			client.socket().on('disconnect', function() {
				client.methods.disconnect();
			});
		},
		reconnectEvent: function() {
			client.socket().on('reconnect', function() {
				client.methods.reconnect();
			});
		},
		toSayEventEvent: function() {
			client.socket().on('toSayEvent', function(data) {
			    client.methods.toSay(data);
	
			});
		},
		sendMsgClick: function() {
			$("#btnSayingTo").click(function() {
				client.methods.sendMsg();
			});
			$('#SayingInput').keydown(function(e) {
				if (e.keyCode == 13) {
					client.methods.sendMsg();
				}
			});
		}
	}

}

$(function() {
 
	//login
	$.fancybox.open($("#loginInfo"));
	$("#btnLogin").click(function() {
		$.fancybox.close();
		client.config.fromUN = $("#loginInfo .usernameInput").val();
		client.socket().emit('onlineEvent', {
			user: client.config.fromUN
		});
		client.config.status().text(client.config.fromUN + '，你好！');
	});

	//f5
	$(window).keydown(function(e) {
		if (e.keyCode == 116) {
			if (!confirm("刷新将会清除所有聊天记录，确定要刷新么？")) {
				e.preventDefault();
			}
		}
	});


	client.events.onlineEvent();
	client.events.offlineEvent();
	client.events.disconnectEvent();
	client.events.reconnectEvent();
	client.events.toSayEventEvent();
	client.events.sendMsgClick();
	 
});