/// <reference path="typings/node/node.d.ts"/>



/*
* Author : TeYoU
* pudding [dot] pearl [dot] tea [at] gmail [dot] com
**/
var express = require('express');
var app = express();
var http = require('http').createServer(app);
var io = require('socket.io')(http);
var PORT = process.env.PORT || 3000;

//static
app.use(express.static(__dirname + '/public'));

app.get('/',function(req,res){
        res.sendFile(__dirname + '/public/index.html');
});

io.on('connection', function(socket){

  //recv Pos
  socket.on('channelname', function(msg){
    if(msg){
      console.log('channelname: (' + msg.id + "," + msg.xPos + "," + msg.yPos + "," + msg.time +")");
    }
    io.emit('channelname',msg);
  });

  //got hit
  socket.on('fire', function(msg){
    if(msg){
      console.log('fire: (' + msg.id + "," + msg.tid + "," + msg.time +")");
    }
    io.emit('fire',msg);
  });

  //logon from user (failed for testing)
  socket.on('logon', function(msg){
    if(msg){
      console.log('logon failed: (' + msg.login + "," + msg.password + ")");
    }
    io.emit('logonnack',msg.login);
  });
});


http.listen(PORT,function(){
        console.log('listening on *:'+ PORT);
});
