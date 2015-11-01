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
var https = require('https');
var querystring = require('querystring');
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

			var post_data = querystring.stringify({
				'playername' : msg.login,
				'password': msg.password
				});


			var post_options = {
	      host: 'threedator.appspot.com',
	      port: '443',
	      path: '/admin.checklogin',
	      method: 'POST',
	      headers: {
	          'Content-Type': 'application/x-www-form-urlencoded',
	          'Content-Length': Buffer.byteLength(post_data)
	      }
  		};

			var post_req = https.request(post_options, function(res) {
	      res.setEncoding('utf8');
	      res.on('data', function (chunk) {
	          console.log('Response: ' + chunk);
	      });
	  	});
			// post the data
		  post_req.write(post_data);
		  post_req.end();

    }
    io.emit('logonnack',msg.login);
  });
});


http.listen(PORT,function(){
        console.log('listening on *:'+ PORT);
});
