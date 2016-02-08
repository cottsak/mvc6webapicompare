# mvc6webapicompare

The objective here is to compare building out a simple api application with these requirements:

* http api gateway for performing CRUD operations on a socks drawer, where
* socks can be added and removed, but always no more than 6 pairs of white socks at once
* no authentication
* MSSQL store
* Autofac for DI
* NHibernate for ORM
* low-effort automated tests that deliver confidence around the api gateway and it's expected behaviors

There will be two solutions:

* The first using ASP.NET 4.6 and Web Api 2
* The second using MVC6 built against Core/DNX where possible.

A summary will be included with the aim of quantifying:

* Effort to setup and build out each solution
* Tooling experience: pains, wins, etc
* Clarity and mass of code: which is 'cleaner'?
* Which of the external libraries worked or didn't work in MVC6 and what were the workarounds?
* Is there current ASP.NET 5 / MVC6 release "production ready" according to Microsoft
