#!/usr/bin/env python
#
# Copyright 2007 Google Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#
import webapp2
import cgi
from webapp2_extras import json
from ndbutil import *
		
class MainHandler(webapp2.RequestHandler):
	def post(self):
		objectList = []
		if self.request.get('shipposlat') and self.request.get('shipposlon'):
			objectList = []
			lat=float(cgi.escape(self.request.get('shipposlat')))
			lon=float(cgi.escape(self.request.get('shipposlon')))
		
			boundingCoords = getBoundingCoords(lat,lon,Settings.objectsRadius)
			boundSW = ndb.GeoPt(boundingCoords[0][0],boundingCoords[0][1])
			boundNE = ndb.GeoPt(boundingCoords[1][0],boundingCoords[1][1])
			
			## hier objects aus db holen
			objectQuery = Object.query()
			objectQuerySW = objectQuery.filter(Object.objectPos >= boundSW)
			objectQueryNE = objectQuerySW.filter(Object.objectPos <= boundNE)
			selectedObjects = objectQueryNE.fetch()
			
			if len(selectedObjects) >=1 :
				for objectToShow in selectedObjects:
					latlon = str(objectToShow.objectPos).split(",")
					object = {
					'type': objectToShow.objectType,
					'id': objectToShow.key.id(),
					'lat':  latlon[0],
					'lon':  latlon[1],
					'heading':  objectToShow.objectHeading,
					'targetlat':  latlon[0],
					'targetlon':  latlon[1],
					'damage': 0,
					'ismovin': "false",
					'character': "nofaction",
					'name': objectToShow.objectType,
					}
					objectList.append(object)
			
			## hier ships aus db holen
			shipQuery = Ship.query()
			shipQuerySW = shipQuery.filter(Ship.shipPos >= boundSW)
			shipQueryNE = shipQuerySW.filter(Ship.shipPos <= boundNE)
			selectedShips = shipQueryNE.fetch()
			
			if len(selectedShips) >=1 :
				for shipToShow in selectedShips:
					latlon = str(shipToShow.shipPos).split(",")
					if shipToShow.shipTargetPos:
						tlatlon = str(shipToShow.shipTargetPos).split(",")
					else:
						tlatlon = latlon
					object = {
					'type': "ship",
					'id': shipToShow.key.id(),
					'lat':  latlon[0],
					'lon':  latlon[1],
					'heading':  shipToShow.shipHeading,
					'targetlat':  tlatlon[0],
					'targetlon':  tlatlon[1],
					'damage': shipToShow.shipDamage,
					'ismovin': shipToShow.shipIsMovin,
					'character': shipToShow.shipFaction,
					'name': shipToShow.shipName,
					}
					objectList.append(object)
				
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(objectList))		
	
class PickupHandler(webapp2.RequestHandler):
	def post(self):			
		response = []
		if int(cgi.escape(self.request.get('shipid'))) and int(cgi.escape(self.request.get('objectid'))):
			shipid = int(cgi.escape(self.request.get('shipid')))
			objectid = int(cgi.escape(self.request.get('objectid')))
			pickupresponse = Object.pickup_object(shipid,objectid)
			response.append(pickupresponse)
		else:
			response.append("error")
		
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(response))	
		
class AnchorHandler(webapp2.RequestHandler):
	def post(self):			
		response = []
		if int(cgi.escape(self.request.get('shipid'))) and int(cgi.escape(self.request.get('objectid'))):
			shipid = int(cgi.escape(self.request.get('shipid')))
			objectid = int(cgi.escape(self.request.get('objectid')))
			anchorresponse = Object.pickup_object(shipid,objectid)
			response.append(pickupresponse)
		else:
			response.append("error")
		
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(response))			

class GetOthersHandler(webapp2.RequestHandler):
	def post(self):
		objectList = []
		if self.request.get('shipposlat') and self.request.get('shipposlon'):
			objectList = []
			lat=float(cgi.escape(self.request.get('shipposlat')))
			lon=float(cgi.escape(self.request.get('shipposlon')))
		
			boundingCoords = getBoundingCoords(lat,lon,Settings.objectsRadius)
			boundSW = ndb.GeoPt(boundingCoords[0][0],boundingCoords[0][1])
			boundNE = ndb.GeoPt(boundingCoords[1][0],boundingCoords[1][1])
			
			## hier objects aus db holen
			objectQuery = Object.query()
			objectQuerySW = objectQuery.filter(Object.objectPos >= boundSW)
			objectQueryNE = objectQuerySW.filter(Object.objectPos <= boundNE)
			selectedObjects = objectQueryNE.fetch()
			
			if len(selectedObjects) >=1 :
				for objectToShow in selectedObjects:
					latlon = str(objectToShow.objectPos).split(",")
					object = {
					'type': objectToShow.objectType,
					'id': objectToShow.key.id(),
					'lat':  latlon[0],
					'lon':  latlon[1],
					'heading':  objectToShow.objectHeading,
					}
					objectList.append(object)		

		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(objectList))					

class GetShipsHandler(webapp2.RequestHandler):
	def post(self):
		objectList = []
		if self.request.get('shipposlat') and self.request.get('shipposlon'):
			objectList = []
			lat=float(cgi.escape(self.request.get('shipposlat')))
			lon=float(cgi.escape(self.request.get('shipposlon')))
			
			boundingCoords = getBoundingCoords(lat,lon,Settings.objectsRadius)
			boundSW = ndb.GeoPt(boundingCoords[0][0],boundingCoords[0][1])
			boundNE = ndb.GeoPt(boundingCoords[1][0],boundingCoords[1][1])
			
			shipQuery = Ship.query()
			shipQuerySW = shipQuery.filter(Ship.shipPos >= boundSW)
			shipQueryNE = shipQuerySW.filter(Ship.shipPos <= boundNE)
			selectedShips = shipQueryNE.fetch()
			
			if len(selectedShips) >=1 :
				for shipToShow in selectedShips:
					latlon = str(shipToShow.shipPos).split(",")
					unixlastseen = time.mktime(datetime.datetime.strptime(str(shipToShow.shipLastSeen), "%Y-%m-%d %H:%M:%S.%f").timetuple())
					if shipToShow.shipTargetPos:
						tlatlon = str(shipToShow.shipTargetPos).split(",")
					else:
						tlatlon = latlon
					object = {
					'type': "ship",
					'id': shipToShow.key.id(),
					'lat':  latlon[0],
					'lon':  latlon[1],
					'heading':  shipToShow.shipHeading,
					'targetlat':  tlatlon[0],
					'targetlon':  tlatlon[1],
					'damage': shipToShow.shipDamage,
					'ismovin': shipToShow.shipIsMovin,
					'character': shipToShow.shipFaction,
					'name': shipToShow.shipName,
					'crew': shipToShow.shipCrew,
					'lastseen': str(shipToShow.shipLastSeen),
					'unixlastseen': str(int(unixlastseen)),
					}
					objectList.append(object)
				
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(objectList))			

class GetFilteredShipsHandler(webapp2.RequestHandler):
	def post(self):
		objectList = []
		if self.request.get('shipposlat') and self.request.get('shipposlon'):
			objectList = []
			lat=float(cgi.escape(self.request.get('shipposlat')))
			lon=float(cgi.escape(self.request.get('shipposlon')))
			
			boundingCoords = getBoundingCoords(lat,lon,Settings.objectsRadius)
			boundSW = ndb.GeoPt(boundingCoords[0][0],boundingCoords[0][1])
			boundNE = ndb.GeoPt(boundingCoords[1][0],boundingCoords[1][1])
			
			shipQuery = Ship.query()
			shipQuerySW = shipQuery.filter(Ship.shipPos >= boundSW)
			shipQueryNE = shipQuerySW.filter(Ship.shipPos <= boundNE)
			selectedShips = shipQueryNE.fetch()
			
			if len(selectedShips) >=1 :
				for shipToShow in selectedShips:
					latlon = str(shipToShow.shipPos).split(",")
					unixlastseen = time.mktime(datetime.datetime.strptime(str(shipToShow.shipLastSeen), "%Y-%m-%d %H:%M:%S.%f").timetuple())
					timestampnow = int(time.time())
					lastseeninseconds = timestampnow - unixlastseen
					if lastseeninseconds < Settings.timeoutShowShip:
						if shipToShow.shipTargetPos:
							tlatlon = str(shipToShow.shipTargetPos).split(",")
						else:
							tlatlon = latlon
						object = {
						'type': "ship",
						'id': shipToShow.key.id(),
						'lat':  latlon[0],
						'lon':  latlon[1],
						'heading':  shipToShow.shipHeading,
						'targetlat':  tlatlon[0],
						'targetlon':  tlatlon[1],
						'damage': shipToShow.shipDamage,
						'ismovin': shipToShow.shipIsMovin,
						'character': shipToShow.shipFaction,
						'name': shipToShow.shipName,
						'crew': shipToShow.shipCrew,
						'lastseen': str(shipToShow.shipLastSeen),
						'unixlastseen': str(int(unixlastseen)),
						'secondssincelastseen': str(int(lastseeninseconds)),
						}
						objectList.append(object)
				
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(objectList))			

			
app = webapp2.WSGIApplication([
    ('/objects', MainHandler),
    ('/objects.pickup', PickupHandler),
    ('/objects.anchor', AnchorHandler),
    ('/objects.ships', GetShipsHandler),
    ('/objects.shipsfiltered', GetFilteredShipsHandler),
    ('/objects.others', GetOthersHandler),
], debug=True)

