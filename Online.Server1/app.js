//var express = require('express');
var path = require('path');
//var favicon = require('serve-favicon');
//var logger = require('morgan');
//var cookieParser = require('cookie-parser');
//var bodyParser = require('body-parser');
var app = require('http').createServer(handler);
var io = require('socket.io')(app);
var fs = require('fs');
//var promise = require('promise');
//var async = require('async');
//var t = require('./t');
//var log = t.log;
//var http = require('http');
var port = normalizePort(process.env.PORT || '1235');
app.listen(port);

//var app = express();

//var onlineAdminUsers = []; //存储在线管理员用户
var socketSet = []; //存储所有客户端集合
var userRoleID = 100;//巡官ID
//redis
var RedisProvider = require('ioredis');
//var redis = Redis(6379, '115.29.168.114');
var redis = RedisProvider(6379, '127.0.0.1');
var onLineUsers = 0;

////set socket.io server
//var debug = require('debug')('Runtor.Chat:server');
//var port = normalizePort(process.env.PORT || '1233');
//app.set('port', port);

//var server = http.createServer(app);
//server.listen(port);
//server.on('error', onError);
//server.on('listening', onListening);

////服务端参数配置
//var io = require('socket.io').listen(server);

//设置服务器端每隔多上时间应该发一个心跳信号,单位 s
//io.set('heartbeat interval', 15);
io.set('transports', [
    'websocket',
    'polling',
]);

//io.set('authorization', function (handshakeData, callback) {
//    //根据cookie检查用户信息
//    var cookie = handshakeData.headers.cookie;
//});

io.on('connection', function (socket) {
    var watch = new Stopwatch();
    watch.start();
    redis.lrange("test", 0, 60000, function (err, result) {
        watch.stop();
        console.log(watch.elapsedMilliseconds + "ms");
        console.log("---------------------------");
        console.log(result);        
    });
    redis.on("Chat_Message", function (channel, message) {
        console.log('Receive message %s from channel %s', message, channel);
    });
    onLineUsers++;
    //连线，缓存当前连接客户端
    var tmpSocket = new Object();
    tmpSocket.roomid = 0;
    tmpSocket.from = '';
    tmpSocket.uid = 0;
    tmpSocket.socketid = socket.id;
    tmpSocket.roleid = 0;
    tmpSocket.sesstionid = ParseCookie(socket);
    tmpSocket.remoteAddress = getRemoteAddressIP(socket);
    tmpSocket.AccessCount = 1;


    var hasSocketFlag = 0;
    if (socketSet.length > 0) {
        for (var j = 0; j < socketSet.length; j++) {
            if (socketSet[j].id == socket.id || socketSet[j].sesstionid == tmpSocket.sesstionid) {
                hasSocketFlag++;
            }
            if (!socketSet[j].from || socketSet[j].from == '') {
                socketSet.splice(j, 1);
            }
        }
    }
    if (hasSocketFlag == 0) {
        socketSet.push(tmpSocket);
    }
    if (!IsDDosAttack(socket)) {
        socket.emit('connection', {
            msgtype: 1,
            socketid: socket.id,
            totalnum: onLineUsers,//socketSet.length,
        });
    }

    //上线  data:{roomid:18,uid:2,from:'test',roleid:1,socketid:''}  
    //1 会员 10 子爵 20 伯爵 30 白银VIP 40 黄巾VIP 60 钻石VIP 70 至尊VIP 80 大亨VIP 100 巡管 110 频道管理 120 超管  90 讲师 50  铂金 
    socket.on('onlineEvent', function (data) {
        if (IsDDosAttack(socket)) { return false; }
        if (!!data && data != undefined) {
            if (data.from && data.roomid) {
                try {
                    //不同客户端进入不同房间
                    socket.join(data.roomid.toString());
                    //users 客户端上线后无论是游客或是会员存入缓存
                    var existsFlag = 0;
                    /*用户重复登录*/
                    if (socketSet.length > 0) {
                        for (var j = 0; j < socketSet.length; j++) {
                            //是同一用户，不同socket实例，T掉原来的
                            if (!!socketSet[j] && !!socketSet[j].from && (socketSet[j].from == data.from) && (socketSet[j].socketid != socket.id)) {
                                /*if (parseInt(data.uid) > 0) {*/
                                var forceData = { eventTyp: 1 };
                                if (ParseCookie(socket) == socketSet[j].sesstionid) {
                                    forceData = { eventTyp: 2 };
                                }
                                var ioSocket = io.sockets.connected[socketSet[j].socketid];
                                if (!!ioSocket) {
                                    ioSocket.emit('forceLogOutEvent', forceData);
                                }
                                /*}*/
                                //existsFlag++;
                                socketSet.splice(j, 1);
                            }
                            else if (!!socketSet[j] && !!socketSet[j].socketid && socketSet[j].socketid == socket.id) {
                                socketSet[j].from = data.from;
                                socketSet[j].roomid = data.roomid;
                                socketSet[j].roleid = data.roleid;
                                socketSet[j].uid = data.uid;
                                existsFlag++;
                            }
                            //清除socketset中未赋值的连接
                            if (!socketSet[j].from || socketSet[j].from === "" || socketSet[j].from === '') {
                                try {
                                    if (!!socketSet[j].roomid && socketSet[j].roomid != undefined) {
                                        socket.leave(socketSet[j].roomid.toString());//离开房间
                                    }
                                    socketSet.splice(j, 1);//从缓存中清除

                                } catch (e) {
                                    console.log("清除socketset中未赋值的连接 " + e);
                                }
                            }

                            //if (!!socketSet[j].roleid && socketSet[j].roleid === 85) {
                            //    var adminSocket = io.sockets.connected[socketSet[j].socketid];
                            //    if (!!adminSocket)
                            //        adminSocket.emit('onlineEvent', {
                            //            roomid: data.roomid,
                            //            from: data.from,
                            //            uid: data.uid,
                            //            msgtype: 1,
                            //            socketid: socket.id,
                            //            totalnum: onLineUsers,//socketSet.length
                            //        });
                            //}

                        }
                    }
                    data.sesstionid = ParseCookie(socket);
                    data.socketid = socket.id;
                    if (existsFlag == 0) {
                        socketSet.push(data);
                    }
                }
                catch (ex) {
                    console.log("FUNC[onlineEvent]-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
                }
                finally {

                }
            }
        }
    });

    //审核消息  data:{roomid:18,uid:'23',from:'fn',touid:'',to:'tn',roleid:1,rolename:'管理员',msg:'this is a  test',postfile:'',sendtime:'',createTime:'2015-7-27 10:58:33',msgtype:1,ischeck:1,isOVerMaxMsgCount:true}
    socket.on('adminCheckMsgEvent', function (data) {
        if (IsDDosAttack(socket)) { return false; }
        //async.each(socketSet, function (item, callback) {
        //    if (!!socketSet[i].socketid && !!socketSet[i].roleid && parseInt(socketSet[i].roleid) >= userRoleID) {
        //        var sid = socketSet[i].socketid;
        //        var adminClient = io.sockets.connected[sid];
        //        if (!!adminClient) {
        //            adminClient.emit('adminCheckMsgEvent', data);
        //            //socket.broadcast.to(data.roomid.toString()).emit('adminCheckMsgEvent', data);
        //        }
        //    }
        //}, function (err) {
        //    log("FUNC[adminCheckMsgEvent]-exception:{msg:" + err + "}");
        //});

        //如果是管理员发的消息直接广播到相应的房间用户，不需要审核
        if (!!data && !!data.roomid) {
            data.socketid = socket.id;
            if (!!data.roleid && parseInt(data.roleid) >= userRoleID) {
                //超管全房间推消息
                if (parseInt(data.roleid) == 120) {
                    socket.broadcast.to(data.roomid.toString()).emit('toSayEvent', data);
                    //io.sockets.emit('toSayEvent', data);//所有房间发送通知
                } else {
                    socket.broadcast.to(data.roomid.toString()).emit('toSayEvent', data);
                }
            }
            else {
                var admins = getOnlineAdmins(data.roomid);
                if (admins.length > 0) {
                    for (var i = 0; i < admins.length; i++) {
                        var sid = admins[i].socketid;
                        var adminClient = io.sockets.connected[sid];
                        if (!!adminClient) {
                            adminClient.emit('adminCheckMsgEvent', data);
                        }
                    }
                }
                ////取管理员列表
                //if (socketSet.length > 0) {
                //    for (var i = 0; i < socketSet.length; i++) {
                //        if (!!socketSet[i].socketid && !!socketSet[i].roleid && parseInt(socketSet[i].roleid) >= userRoleID) {
                //            var sid = socketSet[i].socketid;
                //            var adminClient = io.sockets.connected[sid];
                //            if (!!adminClient) {
                //                adminClient.emit('adminCheckMsgEvent', data);
                //                //socket.broadcast.to(data.roomid.toString()).emit('adminCheckMsgEvent', data);
                //            }
                //        }
                //    }
                //}
                //try {
                //    redis.hvals("ONLINE_Admin_USERS_" + data.roomid, function (err, result) {
                //        if (!!err) {
                //            console.log("error:" + err);
                //        }
                //        else {
                //            if (!!result && result != undefined) {
                //                result.forEach(function (item, i) {
                //                    if (!!item) {
                //                        var itemObj = JSON.parse(item);
                //                        if (socketSet.length > 0) {
                //                            for (var i = 0; i < socketSet.length; i++) {
                //                                if (!!socketSet[i].from && socketSet[i].from == itemObj.from) {
                //                                    var sid = socketSet[i].socketid;
                //                                    var adminClient = io.sockets.connected[sid];
                //                                    if (!!adminClient) {
                //                                        adminClient.emit('adminCheckMsgEvent', data);
                //                                        //socket.broadcast.to(data.roomid.toString()).emit('adminCheckMsgEvent', data);
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                });
                //            }
                //        }
                //    });
                //}
                //catch (ex) {
                //    console.log("FUNC[redis.hvals-GetAdminUsers]-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
                //}
            }
        }
    });

    //发消息 data:{roomid:18,uid:'23',from:'fn',touid:'',to:'tn',roleid:1,rolename:'管理员',msg:'this is a  test',postfile:'',sendtime:'',createTime:'2015-7-27 10:58:33',msgtype:1,ischeck:1,isOVerMaxMsgCount:true}
    socket.on('toSayEvent', function (data) {
        if (IsDDosAttack(socket)) { return false; }
        if (!!data && !!data.roomid) {
            data.socketid = socket.id;
            if (data.ischeck == "1") {
                socket.broadcast.to(data.roomid).emit("toSayEvent", data);
            }
        }

    });//禁言
    socket.on('kickRoomEvent', function (data) {
        try {
            if (IsDDosAttack(socket)) { return false; }
            if (socketSet.length > 0 && !!data && !!data.from) {
                for (var i = 0; i < socketSet.length; i++) {
                    if (socketSet[i].from == data.from) {
                        var sid = socketSet[i].socketid;
                        var client = io.sockets.connected[sid];
                        if (!!client) {
                            client.emit('kickRoomEvent', data);
                        }
                    }
                }
            }
        }
        catch (e) {
            console.log("kickRoomEvent exception:" + ex.name + ",msg:" + ex.message + "}");
        }

    });

    //解禁
    socket.on('recoveryPostEvent', function (data) {
        if (IsDDosAttack(socket)) { return false; }
        if (socketSet.length > 0 && !!data && !!data.from) {
            for (var i = 0; i < socketSet.length; i++) {
                if (socketSet[i].from == data.from) {
                    var sid = socketSet[i].socketid;
                    var client = io.sockets.connected[sid];
                    if (!!client) {
                        client.emit('recoveryPostEvent', data);
                    }
                }
            }
        }
    });

    socket.on("sendDanmuEvent", function (data) {
        if (IsDDosAttack(socket)) { return false; }
        if (!!data && !!data.roomid) {
            socket.broadcast.to(data.roomid).emit("sendDanmuEvent", data);
        }
    });

    //下线
    socket.on('disconnect', function () {
        try {
            onLineUsers--;
            if (socketSet.length > 0) {
                //var sids = '';
                //for (var i = 0; i < socketSet.length; i++) {
                //    sids += socketSet[i].socketid + "[first_separator]";
                //}
                for (var i = 0; i < socketSet.length; i++) {
                    if (!!socketSet[i].roomid && !!socketSet[i].from && socketSet[i].socketid == socket.id) {

                        //向其他房间用户广播该用户下线信息
                        socket.broadcast.in(socketSet[i].roomid).emit('offlineEvent', {
                            from: socketSet[i].from,
                            msgtype: 2,
                            totalnum: onLineUsers, // socketSet.length,
                            sockets: socket.id,
                        });
                        socket.leave(socketSet[i].roomid.toString()); //离开房间
                        socketSet.splice(i, 1); //从缓存中清除                  } else {
                        //if (socketSet[i].roleid >= 85) {
                        //    var adminSocket = io.sockets.connected[socketSet[i].socketid];
                        //    if (adminSocket)
                        //        adminSocket.emit('offlineEvent', {
                        //            from: socketSet[i].from,
                        //            msgtype: 2,
                        //            totalnum: onLineUsers,
                        //            sockets: socket.id,
                        //        });
                        //}
                    }
                    if (!!socketSet[i] && (!socketSet[i].from || socketSet[i].from === "")) {
                        socket.leave(socketSet[i].roomid.toString());//离开房间
                        socketSet.splice(i, 1);//从缓存中清除
                    }
                }
            }
        }
        catch (ex) {
            console.log("FUNC[redis.hdel]-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
        }
        finally {

        }
    });

    socket.on('forceLogOutEvent', function (data) {
        if (IsDDosAttack(socket)) { return false; }
        console.log("forceLogOutEvent" + JSON.stringify(data));
        if (data && data != undefined) {
            if (socketSet.length > 0) {
                for (var j = 0; j < socketSet.length; j++) {
                    try {

                        if (socketSet[j].uid == data.uid && socketSet[j].from == data.from && socketSet[j].roomid == data.roomid) {


                            var forceData = { eventTyp: 3 };
                            var ioSocket = io.sockets.connected[socketSet[j].socketid];
                            if (ioSocket)
                                ioSocket.emit('forceLogOutEvent', forceData);

                            socketSet.splice(j, 1);
                        }
                    } catch (e) {
                        console.log(e);
                    }

                }
            }
        }
    });
    socket.on('RemoveMessageEvent', function (data) {
        try {
            if (IsDDosAttack(socket)) { return false; }
            //取管理员列表
            if (socketSet.length > 0) {
                for (var i = 0; i < socketSet.length; i++) {
                    if (!!socketSet[i].socketid && !!socketSet[i].roleid && parseInt(socketSet[i].roleid) >= userRoleID) {
                        var sid = socketSet[i].socketid;
                        var adminClient = io.sockets.connected[sid];
                        if (!!adminClient) {
                            adminClient.emit('RemoveMessageEvent', data);
                        }
                    }
                }
            }
            //redis.hvals("ONLINE_Admin_USERS_" + data.roomid, function (err, result) {
            //    if (!!err) {
            //        console.log("error:" + err);
            //    }
            //    else {
            //        if (!!result && result != undefined) {
            //            result.forEach(function (item, i) {
            //                if (!!item) {
            //                    var itemObj = JSON.parse(item);
            //                    if (socketSet.length > 0) {
            //                        for (var i = 0; i < socketSet.length; i++) {
            //                            if (socketSet[i].from == itemObj.from) {
            //                                var sid = socketSet[i].socketid;
            //                                var adminClient = io.sockets.connected[sid];
            //                                if (!!adminClient) {
            //                                    adminClient.emit('RemoveMessageEvent', data);
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            });
            //        }
            //    }
            //});
        }
        catch (ex) {
            console.log("FUNC[RemoveMessageEvent-Event]-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
        }
    });
    socket.on('RemoveAllMessageEvent', function (data) {
        try {
            if (IsDDosAttack(socket)) { return false; }
            if (!!data && !!data.roomid) {
                socket.broadcast.to(data.roomid).emit("RemoveMessageEvent", data);
            }
        }
        catch (ex) {
            console.log("FUNC[redis.hvals-GetAdminUsers]-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
        }
    });
    socket.on('CheckedMessageEvent', function (data) {
        if (IsDDosAttack(socket)) { return false; }
        try {

            redis.hvals("ONLINE_Admin_USERS_" + data.roomid, function (err, result) {
                if (!!err) {
                    console.log("error:" + err);
                }
                else {
                    if (!!result && result != undefined) {
                        result.forEach(function (item, i) {
                            if (!!item) {
                                var itemObj = JSON.parse(item);
                                if (socketSet.length > 0) {
                                    for (var i = 0; i < socketSet.length; i++) {
                                        if (socketSet[i].from == itemObj.from) {
                                            var sid = socketSet[i].socketid;
                                            var adminClient = io.sockets.connected[sid];
                                            if (!!adminClient) {
                                                adminClient.emit('CheckedMessageEvent', data);
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            });
        }
        catch (ex) {
            console.log("FUNC[redis.hvals-GetAdminUsers]-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
        }
    });
    //私聊
    socket.on('ServerChatSendMessageEvent', function (data) {
        if (IsDDosAttack(socket)) { return false; }
        try {
            if (socketSet.length > 0) {
                for (var i = 0; i < socketSet.length; i++) {
                    if (socketSet[i].from == data.to) {
                        var sid = socketSet[i].socketid;
                        var client = io.sockets.connected[sid];
                        if (client) {
                            client.emit('ClientChatSendMessageEvent', data);
                        }
                    }
                }
            }
        }
        catch (ex) {
            console.log("ServerChatSendMessageEvent-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
        }
    });
    socket.on('ServerShowVoteEvent', function (data) {
        if (IsDDosAttack(socket)) { return false; }
        try {
            if (socketSet.length > 0) {
                for (var i = 0; i < socketSet.length; i++) {
                    if (socketSet[i].from != data.from) {
                        var sid = socketSet[i].socketid;
                        var client = io.sockets.connected[sid];
                        if (client) {
                            client.emit('ClientShowVoteEvent', data);
                        }
                    }
                }
            }
        }
        catch (ex) {
            console.log("ServerShowVoteEvent-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
        }
    });
    socket.on('ServerRefrshVoteEvent', function (data) {
        if (IsDDosAttack(socket)) { return false; }
        try {
            if (socketSet.length > 0) {
                for (var i = 0; i < socketSet.length; i++) {
                    if (socketSet[i].from != data.from) {
                        var sid = socketSet[i].socketid;
                        var client = io.sockets.connected[sid];
                        if (client) {
                            client.emit('ClientRefrshVoteEvent', data);
                        }
                    }
                }
            }
        }
        catch (ex) {
            console.log("ServerShowVoteEvent-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
        }
    });
    //游客登陆之后助理的聊天窗更新
    socket.on('RefreshUserList', function (data) {
        if (socketSet.length > 0) {
            for (var j = 0; j < socketSet.length; j++) {
                if (!!socketSet[j].roleid && socketSet[j].roleid === 85) {
                    var adminSocket = io.sockets.connected[socketSet[j].socketid];
                    if (!!adminSocket)
                        adminSocket.emit('RefreshUserList', {
                            roomid: data.roomid,
                            from: data.from,
                            uid: data.uid,
                            socketid: socket.id,
                        });
                }
            }
        }
    });
});

function getOnlineAdmins(roomid) {
    var admins = [];
    try {
        if (socketSet.length > 0) {
            for (var i = 0; i < socketSet.length; i++) {
                if (!!socketSet[i].socketid && !!socketSet[i].roleid && parseInt(socketSet[i].roleid) >= userRoleID) {
                    admins.push(socketSet[i]);
                }
            }
        }
        //redis.hvals("ONLINE_Admin_USERS_" + roomid, function (err, result) {
        //    if (!!err) {
        //        log("erro-[getOnlineAdmins]:" + err);
        //    }
        //    else if (!!result && result != undefined && result instanceof Array) {
        //        admins = JSON.parse(result);
        //    }
        //    return admins;
        //});
        return admins;
    }
    catch (ex) {
        console.log("FUNC[redis.hvals-GetAdminUsers]-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
    }

}


function IsDDosAttack(socket) {
    var res = true;
    if (!!socket && socket != undefined) {
        try {
            var clientSocIP = getRemoteAddressIP(socket);
            var cookiestr = ParseCookie(socket);
            var origin = socket.handshake.headers.origin;
            var referer = socket.handshake.headers.referer;
            //var sendtime = socket.handshake.time.getTime();
            //var currdate = new Date().getTime();
            //var timeTicket=Math.round((new Date()-new Date(socket.handshake.time))/1000 *10)/10;
            if (!referer || referer === "" || referer == undefined || !origin || origin === "" || origin == undefined) {
                return res = true;
            }
            var accCount = 0;
            if (socketSet.length > 0) {
                for (var i = 0; i < socketSet.length; i++) {
                    if (!!socketSet[i].remoteAddress && socketSet[i].remoteAddress != "" && socketSet[i].remoteAddress != undefined) {

                        if (socketSet[i].remoteAddress == clientSocIP && clientSocIP != "127.0.0.1") {
                            accCount += socketSet[i].AccessCount;
                        }
                    }
                }
            }

            if (accCount > 500) {
                for (var i = 0; i < socketSet.length; i++) {
                    if (!!socketSet[i].remoteAddress && socketSet[i].remoteAddress != "" && socketSet[i].remoteAddress != undefined) {
                        if (socketSet[i].remoteAddress == clientSocIP && clientSocIP != "127.0.0.1") {
                            var sid = socketSet[i].socketid;
                            var client = io.sockets.connected[sid];
                            client.conn.close();
                            socketSet[i].splice(i, 1);
                        }
                    }
                }
                return res = true;
            }
            else {
                return res = false;
            }
        }
        catch (ex) {
            console.log("FUNC[methods-FilterDDosAttackHelper]-exception:{type:" + ex.name + ",msg:" + ex.message + "}");
        }
    }

    return res;
}

function getRemoteAddressIP(socket) {
    var clientSocIP = "";//socket.handshake.address.split(':')[3];
    if (!!socket.handshake.address && socket.handshake.address.split(':').length > 0) {
        var sAddress = socket.handshake.address;
        var slen = socket.handshake.address.split(':').length;
        for (var i = 0; i < slen; i++) {
            if (sAddress.split(':')[i] != "" && sAddress.split(':')[i] != undefined && sAddress.split(':')[i].indexOf('.') >= 0) {
                clientSocIP = sAddress.split(':')[i];
            }
        }
    }

    return clientSocIP;
}

////app configure start
//// view engine setup
//app.set('views', path.join(__dirname, 'views'));
//// app.set('view engine', 'ejs');
////app.engine('html',require('jade')._express);
//app.engine('html', require('ejs').renderFile);
//app.set('view engine', 'html');

//// uncomment after placing your favicon in /public
//app.use(favicon(path.join(__dirname, 'public', '/images/favicon.ico')));
//app.use(logger('dev'));
//app.use(bodyParser.json());
//app.use(bodyParser.urlencoded({
//    extended: false
//}));
//app.use(cookieParser());
//app.use(express.static(path.join(__dirname, 'public')));

// app.use('/', routes);
// app.use('/users', users);
//app.get('/', function (req, res) {
//    res.sendfile('views/chat.html');
//});

//// catch 404 and forward to error handler
//app.use(function (req, res, next) {
//    var err = new Error('Not Found');
//    err.status = 404;
//    next(err);
//});

//// error handlers
//// development error handler
//// will print stacktrace
//if (app.get('env') === 'development') {
//    app.use(function (err, req, res, next) {
//        res.status(err.status || 500);
//        res.render('error', {
//            message: err.message,
//            error: err
//        });
//    });
//}

//// production error handler
//// no stacktraces leaked to user
//app.use(function (err, req, res, next) {
//    res.status(err.status || 500);
//    res.render('error', {
//        message: err.message,
//        error: {}
//    });
//});
////app configure end

function ParseCookie(socket) {
    try {
        var data = socket.handshake.headers.cookie;
        if (!data || data == undefined || data.indexOf('=') < 0) { return ""; }
        var array = data.split(';');
        for (var i = 0; i < array.length; i++) {
            var ss = array[i].split('=');
            if (ss.length >= 1) {
                return decodeURIComponent(ss[1]);
            }
        }
    }
    catch (ex) {
        console.log("ParseCookieError：{type:" + ex.name + ",msg:" + ex.message + "}");
        return null;
    }
}

function normalizePort(val) {
    var port = parseInt(val, 10);

    if (isNaN(port)) {
        // named pipe
        return val;
    }

    if (port >= 0) {
        // port number
        return port;
    }

    return false;
}

function handler(req, res) {
    fs.readFile(__dirname + '/index.html',
    function (err, data) {
        if (err) {
            res.writeHead(500);
            return res.end('Error loading index.html');
        }

        res.writeHead(200);
        res.end(data);
    });
}



///**
// * Event listener for HTTP server "error" event.
// */

//function onError(error) {
//    if (error.syscall !== 'listen') {
//        throw error;
//    }

//    var bind = typeof port === 'string' ? 'Pipe ' + port : 'Port ' + port;

//    // handle specific listen errors with friendly messages
//    switch (error.code) {
//        case 'EACCES':
//            console.error(bind + ' requires elevated privileges');
//            process.exit(1);
//            break;
//        case 'EADDRINUSE':
//            console.error(bind + ' is already in use');
//            process.exit(1);
//            break;
//        default:
//            throw error;
//    }
//}

///**
// * Event listener for HTTP server "listening" event.
// */

//function onListening() {
//    var addr = server.address();
//    var bind = typeof addr === 'string' ? 'pipe ' + addr : 'port ' + addr.port;
//    debug('Listening on ' + bind);
//}

var getTime = function () {
    var date = new Date();
    return date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();
}
function Stopwatch() {
    this.startDate = null;
    this.endDate = null;
    this.elapsedMilliseconds = null;

    this.start = function () {
        this.startDate = new Date();
    }

    this.stop = function () {
        this.endDate = new Date();

        this.elapsedMilliseconds = this.endDate - this.startDate;
    }

    //    this.reset = function()
    //    {
    //        this.startDate = null;
    //        this.endDate = null;
    //    }
}
//var getColor = function () {
//    var colors = ['aliceblue', 'antiquewhite', 'aqua', 'aquamarine', 'pink', 'red', 'green',
//      'orange', 'blue', 'blueviolet', 'brown', 'burlywood', 'cadetblue'
//    ];
//    return colors[Math.round(Math.random() * 10000 % colors.length)];
//}

//module.exports = app;