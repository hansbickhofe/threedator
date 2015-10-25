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
		response = []
		if self.request.get('name') != "":
			nameToCheck = cgi.escape(self.request.get('name'))
			shipNameResponse = []
			shipNameCheck = Ship.checkfreeshipname(nameToCheck)
			
			if len(shipNameCheck) >=1 :
				checkResponse = shipNameCheck
				response.append(checkResponse)
		else: 
			isEmptyResponse = { 
			'status': "noayeaye",
			'reason': 'No Name Entered',
			}
			response.append(isEmptyResponse)
			
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(response))		

class CheckNameHandler(webapp2.RequestHandler):
	def post(self):

		if self.request.get('name') and self.request.get('secret'):
			name = cgi.escape(self.request.get('name'))
			secret = cgi.escape(self.request.get('secret'))
			loginok = Ship.loginShip(name,secret)

			
			self.response.headers['Content-Type'] = 'application/json'
			self.response.out.write(json.encode(loginok))	
		
class SaveHandler(webapp2.RequestHandler):
	def post(self):
		response = []
		if self.request.get('name') and self.request.get('character'):
			name = cgi.escape(self.request.get('name')).upper()
			character = cgi.escape(self.request.get('character')).lower()
			pos = "50.954765068842214,6.9388034110451144"
			heading = 0.01
			newship = Ship.saveShipConfig(name,character,pos,heading)
			
			if len(newship) >=1 :
				checkResponse = newship
				response.append(checkResponse)
			else: 
				isEmptyResponse = { 
				'status': "noayeaye",
				'reason': 'Other Error',
				}
				response.append(isEmptyResponse)

		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(response))				
			
app = webapp2.WSGIApplication([
    ('/ships', MainHandler),
    ('/ships.check', CheckNameHandler),
    ('/ships.save', SaveHandler),
], debug=True)

