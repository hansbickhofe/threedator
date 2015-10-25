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
from google.appengine.api import memcache
from webapp2_extras import json
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
        self.render('pagecontent.html', pageheader = "ARR!", pagetitle = "ARR_2.0 Alpha")

		
class ScoresHandler(Handler):
    def get(self):
		scores = []
		shipscores = Ship.showScores()
		logging.info(shipscores)
		for ship in shipscores:
			score = {
			'name': ship["name"],
			'character': ship["character"],
			'cargo': ship["cargo"],
			'crew': ship["crew"],
			}
			scores.append(score)
		self.render('pagescore.html', pageheader = "ARR!", pagetitle = "ARR_2.0 Alpha", scores = scores)
    
app = webapp2.WSGIApplication([
    ('/', MainHandler),
    ('/scores', ScoresHandler),
], debug=True)
