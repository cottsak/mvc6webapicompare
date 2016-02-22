# DNX/MVC6

## Pros

* `Startup.cs` with it's new `ConfigureServices()` and `Configure()` make a lot more sense keeping components and their dependency registration in one place. 
* `Startup.cs` makes testing easier by newing up `Startup` to set up the web app environment.
* `Microsoft.AspNet.TestHost.TestServer` is a great addition to the testing arsenal.
* Built-in DI should be suitable for many IoC requirements. Additionally, Autofac was easy to install.
* Root dependencies in `project.json` is a nice touch: non-root packages are resolved into the local user profile directory. `project.lock.json` can optionally be committed to source to lock specific child package versions.
* JSON config via `appsettings.json` is nice; particularly the helpers that let you bind JSON objects to strongly typed .NET POCOs.
* No more `ApiController`/[mvc]`Controller` - only one `Microsoft.AspNet.Mvc.Controller` type for everything. It can't be understated how awesome this is! 
* Default convention-based routing (via `app.UseMvc();` in `Startup.cs`) used in conjunction with attribute routing (`[Route("api/[controller]")]`, `[HttpGet("socks")]`, `[Route("{id}")]`) is a little to get used to but is extremely flexible and makes for very lightweight controllers.
* [Sub-c tests](https://github.com/cottsak/ControllerTests) tests in DNX are pretty easy and thanks to the unified `Controller`, much more efficient (ie. don't need as much infrastructure for [different](https://github.com/cottsak/ControllerTests/blob/master/ControllerTests/MVCControllerTestBase.cs) [controller types](https://github.com/cottsak/ControllerTests/blob/master/ControllerTests/ApiControllerTestBase.cs)).

## Cons

* `project.json` can take a little getting used to. We dropped the `dnxcore50` support as it was making it hard to pull in other packages that are not built against .NET Core. Understanding how DNX and non-DNX packages work takes a little time.
* Testing is not as straightforward. The easiest way to get up and running is to use xunit on the command line.
    * However this does require some setup: `dnvm setup`, `dnvm use <>` then to run tests `dnx test` as defined in `project.json`
    * use `dnvm use 1.0.0-rc1-update1 -runtime clr -architecture x86 -p` to stop it only setting it for the current console.
* I'm used to NCrunch for fast test feedback (particularly with Sub-c) and reproducing this with DNX was not easy or ideal.
    * `dnx-watch` was the closest experience on the command line. [Some hackery was needed](http://hammerproject.com/post/139158523874/ncrunch-like-test-automation-in-mvc6dnx-for) in Visual Studio to get this working quickly. This was a little painful to get set up thanks to the wonderful world of [`dnvm`](https://github.com/aspnet/dnvm) and `dnu` (another small learning curve).
    * Running these tests on the command line does not give you debug support however. I had to resort to the VS15 test runner which happens to support xunit thankfully to debug and step into tests.
    * Parallel test runs using `dnx-watch` or the VS15 test runner are limited to the ['Collection' scope as defined by xunit's parallel support](https://xunit.github.io/docs/running-tests-in-parallel.html). NCrunch makes properly isolated tests so much more efficient by giving me more parallelism control, which I know will save me time the more tests I write. If you're like me and use a TDD-ish workflow, building tests happens every day so it's important for this tooling/flow to be as lean as possible. It's not as nice using DNX at present and I'd not like to do it on a project as it grows given the current aspnet5 evoluaiton. I know the [DNX support for NCrunch](https://ncrunch.uservoice.com/forums/245203-feature-requests/suggestions/8065623-support-dnx-projects) will help tho having had a [good chat to @remcomulder about it](https://twitter.com/remcomulder/status/699912943454937088).
* getting a NCrunch-like experience requires the `dnx-watch` command. I used `dnu commands install Microsoft.Dnx.Watcher --fallbacksource https://myget.org/F/aspnetvnext/api/v3/index.json` to get it to work (from https://github.com/aspnet/dnx-watch)
* Sometimes VS won't rebuild correctly. Example: with changing dbup migration scripts and then only a Clean and Rebuild All fixed the issue. Seems the VS build engine that detects project changes might ignore files defined as `"resource": ["scripts"]` in the `project.json` file. bug?
* Building DNX in CI is new so it's expected to be harder. There is a good script [`build.cmd`](https://github.com/aspnet/Mvc/blob/dev/build.cmd) which should outline some of the things that need to be considered.