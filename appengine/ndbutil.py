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

DEFAULT_SHIP = 'Player'
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

def player_key(shipkey=DEFAULT_SHIP):
	"""Constructs a Datastore key for a Player entity.

	We use shipname as the key.
	"""
	return ndb.Key('Player', shipkey)

class Player(ndb.Model):
	"""Model for Playerdata"""
	playerKey = player_key()
	playerCreated = ndb.DateTimeProperty(auto_now_add=True)
	playerName = ndb.StringProperty(required=True,default="")
	playerPW = ndb.StringProperty(required=True)
	playerScore = ndb.IntegerProperty(default=0)
	playerTeam = ndb.StringProperty(required=True,default="")

	# list all registerd Ships
	# @classmethod
	# def showAllShips(self):
	# 	ships = []
	# 	shipquery = self.query().fetch()
	# 	for ship in shipquery:
	# 		showship = {
	# 		'id': ship.key.id(),
	# 		'name': ship.shipName,
	# 		'team': ship.shipFaction,
	# 		'score': ship.shipScore,
	# 		}
	# 		ships.append(showship)
	# 	return ships

	# show Scores
	# @classmethod
	# def showScores(self):
	# 	ships = []
	# 	shipquery = self.query().order(-self.shipScore).fetch()
	# 	for ship in shipquery:
	# 		showship = {
	# 		'name': ship.shipName,
	# 		'team': ship.shipFaction,
	# 		'score': ship.shipScore,
	# 		}
	# 		ships.append(showship)
	#
	# 	return ships
	#
	#
	# # check free ShipName
	# @classmethod
	# def checkfreeshipname(self,name):
	# 	shipquery = self.query(self.shipName == name).fetch(1,keys_only=True)
	# 	if len(shipquery) == 0:
	# 		response = {
	# 		'status': "ACK",
	# 		'reason': 'Name ok',
	# 		}
	# 	else:
	# 		response = {
	# 		'status': "NACK",
	# 		'reason': 'Name already taken',
	# 		}
	# 	return response
	#
	# @classmethod
	# def loginShip(self,name,secret):
	# 	shipquery = self.query(ndb.AND(self.shipName == name,self.shipPassCode == secret)).fetch(1,keys_only=True)
	# 	if len(shipquery) == 1:
	# 		ship = shipquery[0].get()
	# 		response = {
	# 		'status': "ayeaye",
	# 		'reason': 'Login ok',
	# 		'team': str(ship.shipFaction),
	# 		'shipid': str(ship.key.id()),
	# 		}
	# 	else:
	# 		response = {
	# 		'status': "noayeaye",
	# 		'reason': 'Login failed',
	# 		}
	# 	return response
	#
	# # save ShipConfig
	# @classmethod
	# def saveShipConfig(self,name,faction,pos,heading):
	# 	ship = Ship()
	# 	phrase = getRandomPhrase()
	# 	ship.shipName = name
	# 	ship.shipFaction = faction
	# 	ship.shipPassCode = phrase
	# 	shipquery = self.query(self.shipName == name).fetch(1,keys_only=True)
	# 	if len(shipquery) == 0:
	# 		ship.put()
	# 		response = {
	# 		'status': "ACK",
	# 		'reason': 'Save ok',
	# 		'secret': phrase,
	# 		'shipid': str(ship.key.id()),
	# 		}
	# 	else:
	# 		response = {
	# 		'status': "NACK",
	# 		'reason': 'Name was just taken',
	# 		'passphrase': '',
	# 		}
	# 	return response
	#
	#
	# # create new Ship
	# @classmethod
	# def create_newship(self,name,pos,faction):
	# 	ship = Ship()
	# 	ship.shipName = name
	# 	ship.shipTargetPos = ndb.GeoPt(str(pos))
	# 	ship.shipFaction = str(faction)
	# 	ship.put()
	# 	return ship.key.id()
