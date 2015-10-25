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
		if int(cgi.escape(self.request.get('shipid'))) and int(cgi.escape(self.request.get('enemyid'))):
			shipid = int(cgi.escape(self.request.get('shipid')))
			targetid = int(cgi.escape(self.request.get('enemyid')))
			hitresult = Ship.hit_ship(shipid,targetid)
			response.append(hitresult)			
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(response))			
	
	
app = webapp2.WSGIApplication([
    ('/hit', MainHandler),
], debug=True)	