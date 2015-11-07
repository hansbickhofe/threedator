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

// kisten array
var munition = [];
// grÃ¶ÃŸe des spielfelds
var xmin = -15;
var xmax = 15;
var zmin = -25;
var zmax = 25;
// max anzahl kisten
var maxcount = 3;

function getpos(box) {
pos = [];
pos['x'] = Math.random() * (xmax - xmin) + xmin;
pos['z'] = Math.random() * (zmax - zmin) + zmin;
//posstring = "{ id: '"+ box + "', posX: '"+ pos['x'] +"', posZ: '"+ pos['z'] +"', targetX: '"+ pos['x'] +"', targetZ: '"+ pos['z'] +"', time: 0}";
posstring = "{ id: '"+ box + "',\n  posX: '0',\n  posZ: '0',\n  targetX: '"+ pos['x'] +"', \n  targetZ: '"+ pos['z'] +"',\n  time: 0\n}";
return posstring;
}
munition["333"] = getpos(333)
// munition["666"] = getpos(666);
// munition["999"] = getpos(999);

function emitMunipositions() {

//  emit muni positions
  console.log(munition["333"]);
//  console.log("channelname" + munition["666"]);
//  console.log("channelname" + munition["999"]);

  io.emit('channelname', munition["333"]);
//  io.emit('channelname', munition["666"]);
//  io.emit('channelname', munition["999"]);
}


setInterval(emitMunipositions, 5000);
io.on('connection', function(socket){


  //recv Pos
  socket.on('channelname', function(msg){
    if(msg){
      console.log(msg);
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
      var emitMsg;
      var logonstatus;
      var post_req = https.request(post_options, function(res) {


      res.setEncoding('utf8');
      res.on('data', function (chunk) {
      var jsonResponse = JSON.parse(chunk);
        if (jsonResponse[0].logonack) {
          logonstatus = jsonResponse[0].logonack;
          emitMsg = "logonack";
        }
        else if (jsonResponse[0].logonnack) {
          logonstatus = jsonResponse[0].logonnack;
          emitMsg = "logonnack";
        }
          console.log(emitMsg + ': ' + JSON.stringify(logonstatus));
          io.emit(emitMsg, JSON.stringify(logonstatus));
	      });
	  	});
			// post the data
		  post_req.write(post_data);
		  post_req.end();

    }
  });
});


http.listen(PORT,function(){
        console.log('listening on *:'+ PORT);
});
