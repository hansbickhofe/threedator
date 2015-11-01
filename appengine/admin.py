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
			self.render('adminform.html', pageheader = "Admin", pagetitle = "Threedator Admin", user = user_userid, login_out_url = url, linktext = url_linktext, is_admin = 1 )
		else:
			user_mail = "unknown"
			user_nickname = "unknown"
			user_userid = "unknown"
			url_linktext = 'Login'
			url = users.create_login_url(self.request.uri)
			self.render('adminform.html', pageheader = "Admin", pagetitle = "Threedator Admin", user = user_userid, login_out_url = url, linktext = url_linktext, is_admin = 0 )

class ListPlayersHandler(webapp2.RequestHandler):
	def post(self):
		user = users.get_current_user()
		if users.is_current_user_admin():
			allPlayers = Player.showAllPlayers()
		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(allPlayers))

class AddPlayerHandler(webapp2.RequestHandler):
	def post(self):
		playerList = []
		user = users.get_current_user()
		if users.is_current_user_admin():
			playername = cgi.escape(self.request.get('playername'))
			playerpassword = cgi.escape(self.request.get('password'))
			playerteam = cgi.escape(self.request.get('team'))
			newplayer = Player.create_newplayer(playername,playerpassword,playerteam)
			playerList.append(newplayer)

		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(playerList))

class CheckPlayerLoginHandler(webapp2.RequestHandler):
	def post(self):
		playerList = []
		playername = cgi.escape(self.request.get('playername'))
		playerpassword = cgi.escape(self.request.get('password'))
		checkedplayer = Player.check_player_login(playername,playerpassword)
		playerList.append(checkedplayer)

		self.response.headers['Content-Type'] = 'application/json'
		self.response.out.write(json.encode(playerList))

app = webapp2.WSGIApplication([
    ('/admin', MainHandler),
    ('/admin.listplayers', ListPlayersHandler),
    ('/admin.addplayer', AddPlayerHandler),
    ('/admin.checklogin', CheckPlayerLoginHandler),
], debug=True)
