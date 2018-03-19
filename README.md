# LogicMine

[![Build status](https://ci.appveyor.com/api/projects/status/s036pp0dfbv7jtlp?svg=true)](https://ci.appveyor.com/project/tdinucci/logicmine)

***Note** - While the code checked in should be very useable please note that it, and all documentation is still in development and subject to change.  All questions and suggestions are very welcome.*

Developing good quality software is hard.  Ensuring that software remains good quality can be much harder.  LogicMine is a general purpose .Net Standard 2.0 application framework which aims to help with the development of highly maintainable systems following SOLID principles.  

While developing with LogicMine can be very productive it is not striving to allow you to develop software faster than ever before.  Early, rapid development is very often a false economy, where the price for a few months of easy development is years of hacking at a fragile codebase - which is slow, error prone and will very often dwarf the original savings.

There are no claims that by using LogicMine you will magically develop great software.  It certainly aims to help with this however if it's used poorly you will still end up with a poor quality product.

What LogicMine is trying to do is give developers a solid architecture on which to build well-formed software, using practices which have been demonstrated over the years to lead to highly maintainable systems.  It also aims to be completely extensible so as not to become a limiting factor in any of your future design decisions.

### Messages
While you can use LogicMine within any type of application it is especially well suited to the development of RESTful and message based API’s.

Generally speaking, message based API’s can be much more maintainable than procedure based API’s.  A couple of the most compelling benefits are:

* Versioning is generally easier.  If you change a method signature then there’s a very good chance you will also have modify all callers.  If however you pass messages it is often possible to build upon the structure of these messages without impacting existing callers.
 
* Making system wide changes is generally simpler and safer.  With message based API’s all messages can be funnelled through a small set of entry points.  For example, there may be a general *Get(TMessage)* entry point which is called whether you are requesting Car’s, People, Widgets, or whatever other type you can imagine.  This means that a change to this single entry point (adding logging for example) is effectively applied to every call to get something.  With procedure based API’s you end up with a very high number of entry points and therefore it can be much harder to apply system wide changes.

Truly RESTful API’s could be considered message based API’s.  With these you have a small set of general operations; GET, POST, PUT, etc. and you then pass resources/messages around using the standard set of operations.

### Mines
A LogicMine application is structured as a set of Mines.  Each Mine deals with a particular data type and they each have one or more Shafts.  A Shaft contains zero or more Stations and a single Terminal.

The diagram below shows, at a high level, what a mine for Widget’s may look like.

![alt text](Doc/Images/Mine_High-Level.JPG)
 
As you can hopefully tell, a Shaft is effectively a processing pipeline.  

When a request is passed to a Shaft it gets placed into a Basket.  In addition to being a container for the request the Basket allows waypoints (i.e. Stations and the Terminal) to communicate with one another by adding Notes to the Basket.

The Basket descends down the Shaft and each Station may inspect and manipulate the contents of the Basket.  If it so chooses, the Station may even indicate that the journey should be cut short and the Basket pulled immediately to the surface – potentially with a response that the Station itself has put in the Basket.

Assuming the Basket descends to the Terminal the Baskets request will be replaced with a response and sent back up the Shaft.  Again, each Station can inspect and manipulate the contents of this Basket before it emerges from the Shafts entrance.

LogicMine defines a standard set of Shaft types which should be suitable for most applications however new Shaft types can be created as required.

### Layers
While it’s possible to tailor custom Shafts for every data type and operation on those types this could quickly become tedious.  A Layer is a collection of related Stations or Terminals which when added to a mine adds functionality to multiple Shafts.  Layers are simply a handy organisational construct and they do not offer any functionality beyond the Stations or Terminals they contain.

The diagram below shows this concept pictorially.

![alt text](Doc/Images/Mine_Layers.JPG)

The key point to take away from this diagram is that layers are independent of one another, the only thing they share is the definition of the Shafts they operate on.  This means that if you don't like an existing layer or have a requirement for a new one that you can just develop a layer and slot it into your system without having to alter any existing code.
 
### Flexibility & Safety
The model of a Shaft is very flexible and if a Shaft is designed thoughtfully also very safe.  Put simply, a Shaft should be a chain of small, composable, loosely coupled Stations.  While each Station may be simple the combination of these may result in very sophisticated functionality.

As a developer, you are free to make individual Stations as simple or as complex as you wish.  However, if you do create large Stations which are tightly coupled with their neighbours then you will not be taking full advantage of the framework.  You will likely find that your Shafts become brittle (i.e. you change one Station and it causes a fault in another) and you lose the ability to be able to alter the Shafts configuration safely.  This framework aims to help you develop good quality software however, like all other frameworks, it cannot perform magic and you, the developer, are ultimately responsible for your applications makeup. 

The diagram below shows a good Shaft configuration. Here each Station has a single responsibility and is pretty much independent of the others.  This means you could potentially add new Stations and alter/remove existing Stations without breaking the application.  In addition to the safety aspect, a Shaft with independent Stations could be easily configured differently for different situations.  For example, if this Shaft was part of an off-the-shelf shopping cart system then certain companies may wish to switch out the "Send Email" Station for a "Send SMS" station.

![alt text](Doc/Images/Good_Post-Order_Shaft.JPG)

The diagram below shows a less than ideal configuration for a Shaft which performs the same functionality as the one above.  This one is obviously a bit simpler looking, and it would probably be a bit simpler to initially implement too.  However maintenance of this could be much more complex and expensive than the previous Shaft.  Here, whenever you need to make a change to the order placement process you could be in around complex code that has multiple responsibilities which have become intertwined, making alterations much more error prone.  In addition to this, you can forget about the easy switching out of functionality.

![alt text](Doc/Images/Poor_Post-Order_Shaft.JPG)


### Modelling Operations
As mentioned, LogicMine follows a message based pattern and this means you will not have methods with signatures like *void ExportCar(Car car)*.  Instead you would create a data type called CarExport which contained a Car property and you would send an instance of this type into a Shaft.

This means, no matter whether you’re simply pulling data from a database or performing complex operations your code looks the same and flows down common framework paths (making it possible to easily add system wide modifications).

### Class Reference

https://rawgit.com/tdinucci/LogicMine/master/Doc/Help/index.html

or

https://github.com/tdinucci/LogicMine/raw/master/Doc/Help/LogicMine.chm
