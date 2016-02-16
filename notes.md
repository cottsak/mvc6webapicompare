# DNX/MVC6

## Pros



## Cons

* Setting up xunit tests on the command line:
** `dnvm setup`, `dnvm use <>` then to run tests `dnx test` as defined in `project.json`
* getting a NCrunch-like experience requires the `dnx-watch` command. I used `dnu commands install Microsoft.Dnx.Watcher --fallbacksource https://myget.org/F/aspnetvnext/api/v3/index.json` to get it to work (from https://github.com/aspnet/dnx-watch)
* use `dnvm use 1.0.0-rc1-update1 -runtime clr -architecture x86 -p` to stop it only setting it for the current console.