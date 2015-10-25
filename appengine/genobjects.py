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
from ndbutil import *

def getRandomCratePos(userLat,userLon,count):
		userLatf = float(userLat)
		userLonf = float(userLon)
		rndRadius = Settings.rndRadius
		i = 0 
		objectList = [] 
		newObject = [] 
		while i < count:
			randRadLat = random.uniform((rndRadius * -1.0),rndRadius)
			randRadLon = random.uniform((rndRadius * -1.0),rndRadius)
			newLat = ((userLatf / 90 * 10000) + randRadLat) / 10000 * 90
			lonDist = math.cos(userLatf * math.pi / 180) * 20000
			newLon = ((userLonf / 180 * lonDist) + randRadLon) / lonDist * 180
			newObject = [newLat, newLon]
			objectList.append(newObject)
			i = i + 1
		return objectList
		
class MainHandler(webapp2.RequestHandler):
	def get(self):
		if self.request.get('centerposlat') and self.request.get('centerposlon'):
			createList = []
			lat=float(cgi.escape(self.request.get('centerposlat')))
			lon=float(cgi.escape(self.request.get('centerposlon')))
		
			boundingCoords = getBoundingCoords(lat,lon,Settings.objectsRadius)
			boundSW = ndb.GeoPt(boundingCoords[0][0],boundingCoords[0][1])
			boundNE = ndb.GeoPt(boundingCoords[1][0],boundingCoords[1][1])
			
			## hier objects aus db holen
			objectQuery = Object.query()
			objectQuerySW = objectQuery.filter(Object.objectPos >= boundSW)
			objectQueryNE = objectQuerySW.filter(Object.objectPos <= boundNE)
			selectedObjects = objectQueryNE.fetch()
			numObjects = len(selectedObjects) 

			## hier ships aus db holen
			shipQuery = Ship.query()
			shipQuerySW = shipQuery.filter(Ship.shipPos >= boundSW)
			shipQueryNE = shipQuerySW.filter(Ship.shipPos <= boundNE)
			selectedShips = shipQueryNE.fetch()
			numShips = len(selectedShips) 
			
			if numObjects < (numShips * 3) :
				

							
			
				
			
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(createList))		
	

			
app = webapp2.WSGIApplication([
    ('/create', MainHandler),

], debug=True)

