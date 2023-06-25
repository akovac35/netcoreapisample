# .NET Core API sample

This project contains a .NET Core API sample. The following is demonstrated:

* authentication,
* authorization,
* mediator, command and query patterns,
* database repository pattern,
* transaction handling,
* error handling,
* model validation,
* caching (limited to some book related commands),
* logging, with log correlation,
* unit testing,
* integration testing,
* some Swagger tricks,
* using Newtonsoft for API,
* localization,
* multiple database types,
* in-memory database,
* integration tests cover multiple databases,
* generating openapi.json during build,
* generating .NET API client from openapi.json during build,
* integration testing using generated .NET API client,
* ...

To start the sample, run:

* `dotnet tool restore`
* `./scripts/build_docker.sh`
* `docker compose up -f compose-[sqlserver|oracle|in-memory].yaml`
* `localhost:8080/swagger/index.html`

Supported users using `password`:

* admin - full access
* user - not allowed to delete data
* unauthenticated users - may only initialize data

Authorization header pattern: `Bearer <token>`

Examine command handler `Infrastructure.InitializationRelated.CommandAndQuery.InitializeData` to understand how data is initialized.

The following is TODO:

* identity provider,
* ...
