# mvc6webapicompare

# Goals

The objective here is to compare building out a simple api application with these requirements:

* http api gateway for performing CRUD operations on a socks drawer, where
* socks can be added and removed, but always no more than 6 pairs of white socks at once
* no authentication
* MSSQL store
* Autofac for DI
* NHibernate for ORM
* low-effort automated tests that deliver confidence around the api gateway and it's expected behaviors

There will be two solutions:

* The first using [ASP.NET 4.5 and Web Api 2](https://github.com/cottsak/mvc6webapicompare/tree/master/webapi2)
* The second using [MVC6 built against Core/DNX where possible](https://github.com/cottsak/mvc6webapicompare/tree/master/mvc6).

A summary will be included with the aim of quantifying:

* Effort to setup and build out each solution
* Tooling experience: pains, wins, etc
* Clarity and mass of code: which is 'cleaner'?
* Which of the external libraries worked or didn't work in MVC6 and what were the workarounds?
* Is there current ASP.NET 5 / MVC6 release "production ready" according to Microsoft

# Summary

Please note my opinions are mixed with emperical fact. #dealwithit

* In my pragmatic opinion, I would not prefer to build out an MVP or maintain a long running project with MVC6 at this stage (RC1). I think the ongoing costs outweigh the benefits resulting in a net loss in productivity over time.
* Limiting my evaluation to Visual Studio 2015 and the command line, I think the experience is certainly different: more command line focus. This is not a bad thing and my personal preference in some ways. I do think however that the tooling, particularly for testing is not as good as the options we have for VS on .NET 4.5.x. This should be no surprise as ISVs are not going to invest enormous effort into a highly shifting framework. Sure RC1 isn't moving as much as the beta versions, but it's [still](https://twitter.com/ntaylormullen/status/697677617487450115) [moving](https://ncrunch.uservoice.com/forums/245203-feature-requests/suggestions/8065623-support-dnx-projects).
* I do think that the new MVC framework (and aspnet5 in general) is cleaner and better designed having now worked with it and [spent some time in the source](https://github.com/aspnet/Mvc/issues/4085). This is to be expected I guess. When this is finally released I think this will generate long term build and maintenance savings.
* External packages that I'd used before (Autofac and NHibernate) worked fine once I figured out `project.json` so I'm not worrried about that.
* Finally, aspnet5/MVC6 is production ready in regards to [MS releasing a "Go Live" licence with the RC1](https://blogs.msdn.microsoft.com/webdev/2015/11/18/announcing-asp-net-5-release-candidate-1/). Does this mean you *should* be building production apps with it? I'll leave that decision up to you. 

I've detailed **a lot more** in [`notes.md`](https://github.com/cottsak/mvc6webapicompare/blob/master/notes.md). Check it out and make up your own mind.



## Later/TODO

I didn't get to these coz time and/or it didn't make sense for various reasons:

* authentication
* [VSCode](https://code.visualstudio.com/) tooling comparason for these projects
