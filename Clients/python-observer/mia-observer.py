#!/usr/bin/env python

import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation as animation
import socket

HOST = '192.168.0.14'
PORT = 5000

def connect_to_miaserver():
    sock.settimeout(2)
    while True:
        sock.sendto("REGISTER_SPECTATOR", (HOST, PORT))
        try:
            data = sock.recv(5012)
            if data.startswith("REGISTERED"): break
        except socket.timeout:
            print "MIA Server does not respond, retrying"
    print "Connected to MIA Server"
    sock.setblocking(1)

def retrieve_data(turn=0):
    while True:
        data = sock.recv(1024)
        if data.startswith("SCORE;"):
            _, _, scorelist = data.strip().partition(";")
            rawscores = [r.partition(":") for r in scorelist.split(",")]
            scores = [(p,int(s)) for p,_,s in rawscores]
            turn += 1
            yield turn, scores
        elif data.startswith("PLAYER LOST;"):
            print data

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
xdata = []
ydata = dict()
xmin, xmax, ymin, ymax = (0,20,-1,20)
fig, ax = plt.subplots()

def init():
    ax.set_ylim(0, 20)
    ax.set_xlim(0, 20)
    del xdata[:]
    return []

def run(data):
    global xmin, xmax, ymin, ymax
    turn, scores = data
    xdata.append(turn)
    ax.clear()
    lines = []

    for p,s in scores:
        if p in ydata:
            ydata[p].append(s)
        else:
            ydata[p] = [s]
        if len(ydata[p]) < len(xdata):
            ydata[p] = ([0] * (len(xdata)-len(ydata[p]))) + ydata[p]
        if int(s*1.05) > ymax: ymax = int(s*1.10)
        lines.append(ax.plot(xdata, ydata[p], lw=2, label=p+": "+str(s)))

    if int(turn*1.05) > xmax: xmax = int(turn*1.10)

    ax.set_xlim(xmin, xmax)
    ax.set_ylim(ymin, ymax)
    ax.grid()

    plt.legend(loc=2)

    ax.figure.canvas.draw()

    return lines

if __name__ == "__main__":
    connect_to_miaserver()
    ani = animation.FuncAnimation(fig, run, retrieve_data, blit=False, interval=200,
                              repeat=False, init_func=init)
    plt.show()