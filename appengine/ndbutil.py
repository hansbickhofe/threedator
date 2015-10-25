from google.appengine.ext import ndb
import os
import jinja2
import logging
import time
import datetime
import random
import math
from geolocation import GeoLocation
from google.appengine.api import memcache


TEMPLATES_DIR = os.path.join(os.path.dirname(__file__), 'jinja2_templates')
JINJA_ENVIRONMENT = jinja2.Environment(loader = jinja2.FileSystemLoader(TEMPLATES_DIR),autoescape = True)

DEFAULT_SHIP = 'Ship'
DEFAULT_EVENT = 'Event'
DEFAULT_OBJECT = 'Object'

class Settings():
	"""GameSettings """
	objectsRadius = 0.5
	adminRadius = 0.5
	rndRadius = 0.020
	objectCountMin = 1
	objectCountMax = 5	
	timeoutShowShip = 300	
	rndRadius = 0.0400
	ammodefault = 5
	
def getRandomPhrase():	
	words = [line.strip() for line in open('words.txt')]
	return(random.choice(words))
	
def ship_key(shipkey=DEFAULT_SHIP):
	"""Constructs a Datastore key for a Ship entity.

	We use shipname as the key.
	"""
	return ndb.Key('Ship', shipkey)		
	
class Ship(ndb.Model):
	"""Model for Playerdata"""
	shipKey = ship_key()
	shipCreated = ndb.DateTimeProperty(auto_now_add=True)
	shipPos = ndb.GeoPtProperty(indexed=True)
	shipTargetPos = ndb.GeoPtProperty(indexed=True)
	shipName = ndb.StringProperty(required=True,default="")
	shipPassCode = ndb.StringProperty(required=True)
	shipIsMovin = ndb.BooleanProperty(default=False)
	shipFaction = ndb.StringProperty(required=True,default="")
	shipHeading = ndb.FloatProperty(required=True,default=0.0)
	shipDamage = ndb.IntegerProperty(indexed=True,required=True,default=0)
	shipCrew = ndb.IntegerProperty(indexed=True,required=True,default=10)
	shipCargo = ndb.IntegerProperty(indexed=True,required=True,default=0)
	shipGold = ndb.IntegerProperty(indexed=True,required=True,default=0)
	shipAmmo = ndb.IntegerProperty(indexed=True,required=True,default=Settings.ammodefault)
	shipLastAttackBy = ndb.IntegerProperty(required=True,default=0)
	shipLastSeen = ndb.DateTimeProperty(auto_now=True)
	
	# list all registerd Ships
	@classmethod 
	def showAllShips(self):
		ships = []
		shipquery = self.query().fetch()
		for ship in shipquery:
			showship = {
			'id': ship.key.id(),
			'name': ship.shipName,
			'character': ship.shipFaction,
			'heading': ship.shipHeading,
			'damage': ship.shipDamage,
			'crew': ship.shipCrew,
			'pos': str(ship.shipPos),
			'targetpos': str(ship.shipTargetPos),
			'ismovin': ship.shipIsMovin,
			'created': str(ship.shipCreated),
			'lastseen': str(ship.shipLastSeen),
			}
			ships.append(showship)
		return ships	
		
	# reset all registerd Ships
	@classmethod 
	def resetAllShips(self):
		ships = []
		shipquery = self.query().fetch()
		for ship in shipquery:
			ship. shipTargetPos = ship.shipPos
			ship.put()
	
	# show Scores
	@classmethod 
	def showScores(self):
		ships = []
		shipquery = self.query().order(-self.shipCargo,-self.shipCrew).fetch()
		for ship in shipquery:
			showship = {
			'name': ship.shipName,
			'character': ship.shipFaction,
			'cargo': ship.shipCargo,
			'crew': ship.shipCrew,
			}
			ships.append(showship)
			
				
		return ships		
	
	# update Positions 
	@classmethod 
	def updatePositions(self,id,spos,heading,tpos,ismovin):
		myship = self.get_by_id(id, parent=None)

		if ismovin == "False":
			tpos = spos
			ismovin = False

		else:
			ismovin = True
			
		myship.shipIsMovin = ismovin

		myship.shipPos = ndb.GeoPt(spos)
		myship.shipTargetPos = ndb.GeoPt(tpos)
		myship.shipHeading = float(heading)
		
		if myship.shipLastAttackBy != 0:
			lastAttacker = Ship.get_by_id(myship.shipLastAttackBy, parent=None)
			lastAttackerID = myship.shipLastAttackBy
			lastAttackerName = lastAttacker.shipName
			myship.shipLastAttackBy = 0
			
			
		else: 
			lastAttackerID = 0 
			lastAttackerName = ""
		myship.put()
		#logging.info(str(myship.shipIsMovin))
		response = { 
			'crew': myship.shipCrew,
			'damage': myship.shipDamage,
			'lastattackedby': lastAttackerID, 
			'attackername': lastAttackerName, 
			'ammo': myship.shipAmmo, 
			}
		return response
		
		
	# check free ShipName
	@classmethod	
	def checkfreeshipname(self,name):
		shipquery = self.query(self.shipName == name).fetch(1,keys_only=True)
		if len(shipquery) == 0:
			response = { 
			'status': "ayeaye",
			'reason': 'Name available',
			}
		else:
			response = { 
			'status': "noayeaye",
			'reason': 'Name already taken',
			}
		return response

	# set Shippos
	@classmethod
	def setShipPos(self,id,heading,pos):
		ship = Ship.get_by_id(id, parent=None)
		ship.shipHeading = float(heading)
		ship.shipPos = ndb.GeoPt(str(pos))
		ship.shipTargetPos = ndb.GeoPt(str(pos))
		ship.shipIsMovin = False
		ship.put()
		response = { 
		'status': "ayeaye",
		'msg': 'Ship posititioned',
		'heading': heading,
		'shipid': str(ship.key.id()),
		}
		return response
		
	# set Target
	@classmethod
	def setShipTarget(self,id,heading,pos):
		ship = Ship.get_by_id(id, parent=None)
		ship.shipHeading = float(heading)
		ship.shipTargetPos = ndb.GeoPt(str(pos))
		ship.shipIsMovin = True
		ship.put()
		response = { 
		'status': "ayeaye",
		'msg': 'Ship set sail',
		'heading': heading,
		'shipid': str(ship.key.id()),
		}
		return response

	@classmethod
	def loginShip(self,name,secret):
		shipquery = self.query(ndb.AND(self.shipName == name,self.shipPassCode == secret)).fetch(1,keys_only=True)
		if len(shipquery) == 1:
			ship = shipquery[0].get()
			response = { 
			'status': "ayeaye",
			'reason': 'Login ok',
			'character': str(ship.shipFaction),
			'shipid': str(ship.key.id()),
			}
		else:
			response = { 
			'status': "noayeaye",
			'reason': 'Login failed',
			}
		return response
		
	# save ShipConfig
	@classmethod
	def saveShipConfig(self,name,faction,pos,heading):
		ship = Ship()
		phrase = getRandomPhrase()
		ship.shipName = name
		ship.shipHeading = float(heading)
		ship.shipFaction = faction
		ship.shipPassCode = phrase
		ship.shipPos = ndb.GeoPt(str(pos))
		shipquery = self.query(self.shipName == name).fetch(1,keys_only=True)
		if len(shipquery) == 0:
			ship.put()
			response = { 
			'status': "ayeaye",
			'reason': 'Save ok',
			'secret': phrase,
			'shipid': str(ship.key.id()),
			}
		else:
			response = { 
			'status': "noayeaye",
			'reason': 'Name was just taken',
			'passphrase': '',
			}
		return response
	
	# create two default Ships
	@classmethod
	def create_defaultships(self):
		ships = []
		shipquery = self.query(self.shipName == "Flying ROFLCOPTER").fetch(1,keys_only=True)
		if len(shipquery) == 0:
			ship = Ship()
			ship.shipName = "Flying ROFLCOPTER"
			ship.shipTargetPos = ndb.GeoPt("50.95466031265714,6.938706851520578")
			ship.shipPos = ndb.GeoPt("50.95474310385525,6.939355946102182")
			ship.shipFaction = "Wikinger"
			ship.shipHeading = 0.0
			ship.put()
			ships.append(str(ship.key.id()))	
			
		shipquery = self.query(self.shipName == "Queen Anne's Revenge").fetch(1,keys_only=True)
		if len(shipquery) == 0:
			ship = Ship()
			ship.shipName = "Queen Anne's Revenge"
			ship.shipTargetPos = ndb.GeoPt("50.95477182729766,6.938521779098551")
			ship.shipPos = ndb.GeoPt("50.95458090055302,6.9385781054878635")
			ship.shipFaction = "Blackbeard"
			ship.shipHeading = 0.0
			ship.put()
			ships.append(str(ship.key.id()))
		return ships
			
	# create new Ship
	@classmethod
	def create_newship(self,name,pos,faction):
		ship = Ship()
		ship.shipName = name
		ship.shipTargetPos = ndb.GeoPt(str(pos))
		ship.shipFaction = str(faction)
		ship.put()
		return ship.key.id()	
	
	# Cannonhit Ship hit oder failed
	@classmethod
	def hit_ship(self,sourceid,tid):	
		tship = self.get_by_id(tid, parent=None)
		sourceship = Ship.get_by_id(sourceid, parent=None)

		if tship.shipCrew >= 2 and sourceship.shipAmmo >= 1:
			tship.shipCrew = tship.shipCrew - 1
			tship.shipDamage = tship.shipDamage + 5
			status = 'hit'
			sourceship.shipAmmo = sourceship.shipAmmo - 1
			tship.put()
			sourceship.put()
			saveevent = Event.create_event("attack","0.0,0.0",sourceid,tid)
		else: 
			status = 'failed'

		tship.shipLastAttackBy = sourceid
		
		hitresult = {
			'status': status,
			'targetid': str(tship.key.id()),
			'targetdamage': str(tship.shipDamage),
			}
			
		return hitresult
		
	
def event_key(evtkey=DEFAULT_EVENT):
	"""Constructs a Datastore key for a Event entity.

	We use sourceobject as the key.
	"""
	return ndb.Key('Event', evtkey)		
	
class Event(ndb.Model):
	""" EventClass for logging Events
	types:
		none
		test
		attack
		attacked
		ram
		rammed
		pickup
		clearcharge
		repair
		anchored
		setsails
		runnedin 
	"""
	evtKey = event_key()
	evtType = ndb.StringProperty(required=True,default="")
	evtTime = ndb.DateTimeProperty(indexed=True,auto_now=True)
	evtPos = ndb.GeoPtProperty()
	evtSourceID = ndb.IntegerProperty(required=True,default=0)
	evtTargetID = ndb.IntegerProperty(required=True,default=0)
	
	# create two default Events
	@classmethod
	def create_defaultevents(self):
		"""Classmethod to create two default Events.
			
		"""
		events = []
		eventquery = self.query(self.evtType == "none").fetch(1,keys_only=True)
		if len(eventquery) == 0:
			event = Event()
			event.evtType = "none"
			event.evtSourceID = 0
			event.evtTargetID = 0
			event.evtPos = ndb.GeoPt("50.95476, 6.93981")
			event.put()
			events.append(str(event.key.id()))
		
		eventquery = self.query(self.evtType == "test").fetch(1,keys_only=True)
		if len(eventquery) == 0:
			event = Event()
			event.evtType = "test"
			event.evtSourceID = 0
			event.evtTargetID = 0
			event.evtPos = ndb.GeoPt("50.95476, 6.93981")
			event.put()
			events.append(str(event.key.id()))	
		
		return events
	
	# create Event Entity
	@classmethod
	def create_event(self,type,pos,source,target):
		"""Classmethod to create an  Events.
		Takes type (string), pos (Geocoordinates), source (sourceID as int) and target (tagetID as int)
			
		"""
		events = []
		event = Event()
		event.evtType = type
		event.evtPos = ndb.GeoPt(pos)
		event.evtSourceID = source
		event.evtTargetID = target
		event.put()
		events.append(str(event.key.id()))	
		return events

def object_key(objectkey=DEFAULT_OBJECT):
	"""Constructs a Datastore key for a Object entity.

	We use objectType as the key.
	"""
	return ndb.Key('Object', objectkey)				
	
class Object(ndb.Model):
	""" ObjectClass for Objects
	types:
		island
		shipwrecked
		flotsam 
	"""
	objectKey = object_key()
	objectCreatedTime = ndb.DateTimeProperty(indexed=True,auto_now=True)
	objectType = ndb.StringProperty(required=True,default="")
	objectPos = ndb.GeoPtProperty()
	objectHeading = ndb.FloatProperty(required=True,default=0.001)
	
	# create Object
	@classmethod
	def create_object(self,pos,type,heading):
		"""Classmethod to create an Object.
			
		"""
		object = Object()
		object.objectType = str(type)
		object.objectPos = ndb.GeoPt(pos)
		object.objectHeading = float(heading)
		object.put()
		return int(object.key.id())
	
	# create two default Objects
	@classmethod
	def create_defaultobjects(self):
		"""Classmethod to create two default Objects.
			
		"""
		objects = []
		
		object = Object()
		object.objectType = "island"
		object.objectPos = ndb.GeoPt("50.95482589490586,6.93878463558201")
		object.objectHeading = 90
		object.put()
		objects.append(str(object.key.id()))
		
		object = Object()
		object.objectType = "shipwrecked"
		object.objectPos = ndb.GeoPt("50.95489854815528,6.939304984130899")
		object.objectHeading = 180
		object.put()
		objects.append(str(object.key.id()))
		
		object = Object()
		object.objectType = "flotsam"
		object.objectPos = ndb.GeoPt("50.954564004343155,6.938570058860819")
		object.objectHeading = 180
		object.put()
		objects.append(str(object.key.id()))
		
		return objects

	# anchor at island Object
	@classmethod
	def pickup_object(self,sid,tid):
		ship = Ship.get_by_id(sid, parent=None)
		island = Object.get_by_id(tid,parent=None)
		response =[]
		
		if object.objectType == "flotsam":
			ship.shipCargo = ship.shipCargo + 1
			ship.put()
			pickupresult = {
			'status': 'Pickup flotsam ok',
			'objectid': str(object.key.id()),
			}
			object.key.delete()
			
		elif object.objectType == "shipwrecked":
			ship.shipCrew = ship.shipCrew + 1
			ship.put()
			pickupresult = {
			'status': 'Pickup shipwrecked ok',
			'objectid': str(object.key.id()),
			}
			object.key.delete()
		
		else:
			pickupresult = {
			'status': 'Pickup error',
			'objectid': str(object.key.id()),
			}
			
		response.append(pickupresult)
		return response
		
		
	# pickup Object
	@classmethod
	def pickup_object(self,sid,tid):
		ship = Ship.get_by_id(sid, parent=None)
		object = Object.get_by_id(tid,parent=None)
		response =[]
		
		if object.objectType == "flotsam":
			ship.shipCargo = ship.shipCargo + 1
			ship.put()
			pickupresult = {
			'status': 'Pickup flotsam ok',
			'objectid': str(object.key.id()),
			}
			object.key.delete()
			
		elif object.objectType == "shipwrecked":
			ship.shipCrew = ship.shipCrew + 1
			ship.put()
			pickupresult = {
			'status': 'Pickup shipwrecked ok',
			'objectid': str(object.key.id()),
			}
			object.key.delete()
		
		else:
			pickupresult = {
			'status': 'Pickup error',
			'objectid': str(object.key.id()),
			}
			
		response.append(pickupresult)
		return response
		
def getRandomObjectPos(userLat,userLon):
		userLatf = float(userLat)
		userLonf = float(userLon)
		rndRadius = Settings.rndRadius
		objectCount = random.randint(Settings.objectCountMin,Settings.objectCountMax)		
		i = 0 
		objectList = [] 
		newObject = [] 
		while i < objectCount:
			randRadLat = random.uniform((rndRadius * -1.0),rndRadius)
			randRadLon = random.uniform((rndRadius * -1.0),rndRadius)
			objectRotate = random.randint(1,359)
			newLat = ((userLatf / 90 * 10000) + randRadLat) / 10000 * 90
			lonDist = math.cos(userLatf * math.pi / 180) * 20000
			newLon = ((userLonf / 180 * lonDist) + randRadLon) / lonDist * 180
			newObject = [newLat, newLon, objectRotate]
			objectList.append(newObject)
			i = i + 1
		return objectList			
		
def getBoundingCoords(userLat,userLon,distance): #distance in km
	#logging.info("userLat = " + str(userLat) + " userLon " + str(userLon))
	userLatf = float(userLat)
	userLonf = float(userLon)
	distancef = float(distance)
	
	loc = GeoLocation.from_degrees(userLatf, userLonf)
	distance = float(distance)
	SW_loc, NE_loc = loc.bounding_locations(distance)
	SW_latlon = str(SW_loc)
	NE_latlon = str(NE_loc)
	boundingCoords = [SW_latlon.split(','), NE_latlon.split(',')]
	return boundingCoords		