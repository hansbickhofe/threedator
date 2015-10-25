import os
import pprint
import logging

from google.appengine.api import memcache
from ndbutil import *

print Event.create_defaultevents()
print Ship.create_defaultships()
print Object.create_defaultobjects()
print getBoundingCoords(50.954697484233705,6.938567376651804,0.01)

"""
[]
[]
['5771336534196224', '5208386580774912']
[['50.95460765', ' 6.93842477'], ['50.95478732', ' 6.93870998']] # SW,NE ## Lat N+ Long E+
"""