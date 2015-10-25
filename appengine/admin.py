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
import logging
from webapp2_extras import json
from google.appengine.api import users
from ndbutil import *


# using Handlerhelper from Videolesson
class Handler(webapp2.RequestHandler):
	def write(self, *a, **kw):
		self.response.out.write(*a,**kw)

	def render_str(self, template, **params):
		t = JINJA_ENVIRONMENT.get_template(template)
		return t.render(params)

	def render(self, template, **kw):
		self.write(self.render_str(template, **kw))
		
class MainHandler(Handler):
	def get(self):
		user = users.get_current_user()
		url = users.create_logout_url(self.request.uri)
		url_linktext = 'Logout'
		# Check if User is logged in and admin
		if users.is_current_user_admin():
			user_mail = user.email()
			user_nickname = user.nickname()
			user_userid = user.user_id()
			url = users.create_logout_url(self.request.uri)
			url_linktext = 'Logout'
			self.render('adminform.html', pageheader = "Admin", pagetitle = "ARR_2.0 Admin", user = user_userid, login_out_url = url, linktext = url_linktext, is_admin = 1 )
		else:
			user_mail = "unknown"
			user_nickname = "unknown"
			user_userid = "unknown"
			url_linktext = 'Login'
			url = users.create_login_url(self.request.uri)
			self.render('adminform.html', pageheader = "Admin", pagetitle = "ARR_2.0 Admin", user = user_userid, login_out_url = url, linktext = url_linktext, is_admin = 0 )	

class GetObjectsHandler(webapp2.RequestHandler):
	def post(self):
		objectList = []
		user = users.get_current_user()
		if users.is_current_user_admin() and self.request.get('pos'):
			objectList = []
			# pos string "zerlegen"
			latlon = self.request.get('pos').split(',')
			lat=latlon[0]
			lon=latlon[1]
		
			boundingCoords = getBoundingCoords(lat,lon,Settings.adminRadius)
			boundSW = ndb.GeoPt(boundingCoords[0][0],boundingCoords[0][1])
			boundNE = ndb.GeoPt(boundingCoords[1][0],boundingCoords[1][1])
			## hier objects aus db holen
			objectQuery = Object.query()
			objectQuerySW = objectQuery.filter(Object.objectPos >= boundSW)
			objectQueryNE = objectQuerySW.filter(Object.objectPos <= boundNE)
			selectedObjects = objectQueryNE.fetch()
			
			if len(selectedObjects) >=1 :
				for objectToShow in selectedObjects:
					object = {
					'type': objectToShow.objectType,
					'id': objectToShow.key.id(),
					'pos':  str(objectToShow.objectPos),
					'heading':  objectToShow.objectHeading,
					'target':  str(objectToShow.objectPos),
					'damage': 0,
					'ismovin': "false",
					'faction': "nofaction",
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
					object = {
					'type': "ship",
					'id': shipToShow.key.id(),
					'pos':  str(shipToShow.shipPos),
					'heading':  shipToShow.shipHeading,
					'target':  str(shipToShow.shipTargetPos),
					'damage': shipToShow.shipDamage,
					'crew': shipToShow.shipCrew,
					'ismovin': str(shipToShow.shipIsMovin),
					'faction': shipToShow.shipFaction,
					'name': shipToShow.shipName,
					}
					objectList.append(object)
				
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(objectList))		
		
class PlaceObjectHandler(webapp2.RequestHandler):
	def post(self):
		objectList = []
		user = users.get_current_user()
		if users.is_current_user_admin():
			pos = cgi.escape(self.request.get('geopos'))
			type = cgi.escape(self.request.get('object'))
			heading = cgi.escape(self.request.get('heading'))
			id = Object.create_object(pos,type,heading)
			object = {
			'id': id,
			'pos':  str(pos),
			'heading':  str(heading),
			'target':  str(pos),
			'damage': 0,
			'ismovin': "false",
			}
			objectList.append(object)

		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(objectList))		

class ListShipsHandler(webapp2.RequestHandler):
	def post(self):
		user = users.get_current_user()
		if users.is_current_user_admin():
			allShips = Ship.showAllShips()
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(allShips))	
		
class ResetShipHandler(webapp2.RequestHandler):
	def post(self):
		user = users.get_current_user()
		if users.is_current_user_admin():
			allShips = Ship.resetAllShips()
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(allShips))	
		

class SetTargetHandler(webapp2.RequestHandler):
	def post(self):
		shipList = []
		user = users.get_current_user()
		if users.is_current_user_admin():
			id = long(cgi.escape(self.request.get('shipid')))
			heading = cgi.escape(self.request.get('heading'))
			tpos = cgi.escape(self.request.get('geopos'))
			newship = Ship.setShipTarget(id,heading,tpos)
			shipList.append(newship)

		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(shipList))	

class SetShipHandler(webapp2.RequestHandler):
	def post(self):
		shipList = []
		user = users.get_current_user()
		if users.is_current_user_admin():
			id = long(cgi.escape(self.request.get('shipid')))
			heading = cgi.escape(self.request.get('heading'))
			spos = cgi.escape(self.request.get('geopos'))
			newship = Ship.setShipPos(id,heading,spos)
			shipList.append(newship)

		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(shipList))			
		
class AddShipHandler(webapp2.RequestHandler):
	def post(self):
		shipList = []
		user = users.get_current_user()
		if users.is_current_user_admin():
			name = cgi.escape(self.request.get('shipname'))
			heading = cgi.escape(self.request.get('heading'))
			character = cgi.escape(self.request.get('character'))
			pos = cgi.escape(self.request.get('geopos'))
			newship = Ship.saveShipConfig(name,character,pos,heading)
			shipList.append(newship)

		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(shipList))			
			
app = webapp2.WSGIApplication([
    ('/admin', MainHandler),
    ('/admin.getobjects', GetObjectsHandler),
    ('/admin.placeobject', PlaceObjectHandler),
    ('/admin.addship', AddShipHandler),
    ('/admin.listships', ListShipsHandler),
    ('/admin.settargetpos', SetTargetHandler),
    ('/admin.setshippos', SetShipHandler),
    ('/admin.reset', ResetShipHandler),
], debug=True)

