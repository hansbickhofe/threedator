/// <reference path="typings/node/node.d.ts"/>



/*
* Author : TeYoU
* pudding [dot] pearl [dot] tea [at] gmail [dot] com
**/

var io = require('socket.io')(http);
var PORT = process.env.PORT || 3000;
var http = require('http').createServer(app);


io.on('connection', function(socket){

  //socket.broadcast.emit('hi');

  //Color
  socket.on('channelname', function(msg){
    //console.log('message: ' + msg);
    if(msg){
      console.log('channelname: (' + msg.id + "," + msg.xPos + "," + msg.yPos + "," + msg.time +")");
    }
    io.emit('channelname',msg);
  });

});


http.listen(PORT,function(){
	console.log('listening on *:'+ PORT);
});
