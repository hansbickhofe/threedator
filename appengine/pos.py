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
from ndbutil import *

class SendPosHandler(webapp2.RequestHandler):
	def post(self):
		id = long(float(cgi.escape(self.request.get('shipid'))))
		heading = float(cgi.escape(self.request.get('shipheading')))
		spos = str(cgi.escape(self.request.get('shipposlat'))+','+cgi.escape(self.request.get('shipposlon')))
		ismovin = str(cgi.escape(self.request.get('shipismovin')))
		
		if ismovin == "false" or ismovin == "False":
			ismovin = "False"
			tpos = spos     
		elif ismovin == "true" or ismovin == "True":
			ismovin = "True"
			tpos = str(cgi.escape(self.request.get('targetposlat'))+','+cgi.escape(self.request.get('targetposlon')))
		
		setShipPos = Ship.updatePositions(id,spos,heading,tpos,ismovin)		
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(setShipPos))	
		
		
app = webapp2.WSGIApplication([
    ('/pos', SendPosHandler),
], debug=True)