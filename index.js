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
// größe des spielfelds
var xmin = -15;
var xmax = 15;
var zmin = -21;
var zmax = 21;
// max anzahl kisten
var maxcount = 3;
// blockieren der munitionskisten
var muniblock = [];
muniblock[333] = 0;
muniblock[666] = 0;
muniblock[999] = 0;
var muniID=0;

function getpos(box) {
pos = [];
pos['x'] = Math.random() * (xmax - xmin) + xmin;
pos['z'] = Math.random() * (zmax - zmin) + zmin;
var posstring = { id: box.toString(), posX: pos['x'].toString(), posZ: pos['z'].toString(), targetX: pos['x'].toString(), targetZ: pos['z'].toString(), time: "0" };
return posstring;
}
munition[333] = getpos(333);
munition[666] = getpos(666);
munition[999] = getpos(999);

setInterval(emitMunipositions, 3000);

function emitMunipositions() {

//  emit muni positions
  if ( muniblock[333] == 0 ) io.emit('newmuni', munition[333]);
  if ( muniblock[666] == 0 ) io.emit('newmuni', munition[666]);
  if ( muniblock[999] == 0 ) io.emit('newmuni', munition[999]);
}

function blockMuni(muniID,playerID) {
    muniblock[muniID] = 1 ;
    var playerIDtoSend = { id: playerID.toString() } ;
    io.emit('gotby',playerIDtoSend) ;
    // console.log("gotby "+ playerID);
    // console.log("blocked "+ muniID);

    setTimeout(function(){
      // console.log("unblocked " + muniID)
      muniblock[muniID] = 0 ;
      munition[muniID] = getpos(muniID);
      io.emit('newmuni', munition[muniID]);
    }, 3000);
}

io.on('connection', function(socket){

  //recv Pos
  socket.on('player', function(msg){
    if(msg){
      // console.log(msg);
    }
    io.emit('player',msg);
  });
// test keyword unity
  socket.on('test', function(msg){
    if(msg){
      // console.log(msg);
    }
    io.emit('test',msg);
  });
// test keyword unity
  socket.on('test1', function(msg){
    if(msg){
      // console.log(msg);
    }
    io.emit('test1',msg);
  });
// test keyword unity
  socket.on('test2', function(msg){
    if(msg){
      // console.log(msg);
    }
    io.emit('test2',msg);
  });
// test keyword unity
  socket.on('test3', function(msg){
    if(msg){
      // console.log(msg);
    }
    io.emit('test3',msg);
  });
// torpedo keyword unity
  socket.on('torpedo', function(msg){
    if(msg){
      console.log(msg);
    }
    io.emit('torpedo',msg);
  });

// water keyword unity
  socket.on('water', function(msg){
    if(msg){
      console.log('water: (' + msg + ')');
    }
    io.emit('water',msg);
  });

// pick up munition
  socket.on('pickedmuni', function(msg) {
    if(msg) {
      var p_ID = msg.p_id ;
      var k_ID = msg.k_id ;
      if (muniblock[k_ID] == 0) {
        // console.log("blocking "+ k_ID) ;
        blockMuni(k_ID,p_ID);
      }
    io.emit('pickedmuni',msg);
    }
  });

  //got hit
  socket.on('gothit', function(msg){
    if(msg){
    	console.log('gothit: (' + msg + ')');
    	io.emit('gothit',msg);

      /*
	var post_data = querystring.stringify({
  			'torpedoID' : msg.torpedID,
  			'shipID': msg.shipID
  			});
      // optionen für player score Appengine Backend Requests
      var score_post_options = {
        host: 'threedator.appspot.com',
        port: '443',
        path: '/user.score',
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'Content-Length': Buffer.byteLength(post_data)
        }
      };
      var serverReply;
      var post_req = https.request(score_post_options, function(res) {
        res.setEncoding('utf8');
        res.on('data', function (chunk) {
          var jsonResponse = JSON.parse(chunk);
            if (jsonResponse[0].logonack) {
              logonstatus = jsonResponse[0].logonack;
              serverReply = "logonack";
            }
          io.emit(emitMsg, JSON.stringify(logonstatus));
        });
      }); */
    }
  });

  socket.on('fire', function(msg){
    if(msg){
//      console.log('fire: (' + msg.id + "," + msg.posX + "," + "," + msg.posY +
//      "," + "," + msg.angle + "," + msg.time +")");
      io.emit('fire',msg);
    }
  });
});
http.listen(PORT,function(){
        console.log('listening on *:'+ PORT);
});
