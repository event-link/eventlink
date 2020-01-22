```
   __                      _      __   _         _    
  /__\__   __  ___  _ __  | |_   / /  (_) _ __  | | __
 /_\  \ \ / / / _ \| '_ \ | __| / /   | || '_ \ | |/ /
//__   \ V / |  __/| | | || |_ / /___ | || | | ||   < 
\__/    \_/   \___||_| |_| \__|\____/ |_||_| |_||_|\_\
                                                      
  ▓▓▓▓▓▓▓▓▓▓
 ░▓ about  ▓ EventLink backend system
 ░▓ author ▓ iyyel <i@iyyel.io>, tmascagni <t.mascagni@gmail.com>
 ░▓ code   ▓ https://github.com/Event-Link/EventLink
 ░▓ mirror ▓ https://git.iyyel.io/EventLink/EventLink
 ░▓▓▓▓▓▓▓▓▓▓
 ░░░░░░░░░░
```

## Table of Contents
 - [Introduction](#Introduction)
 - [Installing](#Installing)
 - [How it works](#How-it-works)
 - [License](#License)

# Introduction

EventLink is an event-management platform, with the purpose of gathering event information from a diverse set of sources, such as TicketMaster, Billetlugen and SeatGeek as well as personal events from Facebook, Google, etc. This will be synchronized with the users calendar, such that they can decide whether they can attend the event in question. This will be presented in a single user-friendly application.

EventLink started out as a bachelor's thesis at the [Technical University of Denmark, DTU](https://www.dtu.dk).

# Installing

The backend consist of 4 components:

* The GraphQL api (EventLink.API)
* The access control api (EventLink.Auth)
* The crawler (EventLink.Crawler)
* The database integration (EventLink.DataAccess)

These all communicate with each other in some way, and are therefore all required to be deployed
to either the same server or 4 distinct servers. `appsettings.json` files has to be made for required
confidential information.

# How it works

To give a high level overview on how the components communicate together, see the following deployment diagram.

# License

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.md)
