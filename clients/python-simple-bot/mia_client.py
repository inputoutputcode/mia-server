#!/usr/bin/env python

import socket
import random

SERVERHOST = '192.168.0.14'
SERVERPORT = 5000

LOCALIP = '127.0.0.1'
LOCALPORT = 5010
LOCALNAME = "Python Starter Bot"

def connect_to_miaserver(sock):
  sock.settimeout(2)
  while True:
    sock.sendto("REGISTER;" + LOCALNAME, (SERVERHOST, SERVERPORT))
    try:
      data = sock.recv(5010)
      if "REGISTERED" in data:
        break
      else:
        print "Received '" + data + "'"
    except socket.timeout:
      print "MIA Server does not respond, retrying"
  print "Registered at MIA Server"
  sock.setblocking(1)

def play_mia(sock):
  announced = None
  while True:
    data = sock.recv(1024)
    if data.startswith("ROUND STARTING;"):
      _, _, token = data.strip().partition(";")
      sock.sendto("JOIN_ROUND;" + token, (SERVERHOST, SERVERPORT))
      announced = None
    elif data.startswith("ANNOUNCED;"):
      d1, _, d2 = data.strip().split(";")[2].partition(",")
      announced = (d1, d2)
    elif data.startswith("YOUR TURN;"):
      _, _, token = data.strip().partition(";")
      if announced == None or random.uniform(0,100) > 30.0:
        sock.sendto("ROLL;" + token, (SERVERHOST, SERVERPORT))

def mia_client_start():
  sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
  sock.bind((LOCALIP, LOCALPORT))
  connect_to_miaserver(sock)
  play_mia(sock)


if __name__ == "__main__":
  mia_client_start()