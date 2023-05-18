# .NET Core API sample

This project contains a .NET Core API sample. The following is demonstrated:

* authentication,
* authorization,
* mediator, command and query patterns,
* database repository pattern,
* transaction handling,
* error handling,
* model validation,
* caching,
* logging, with correlation,
* unit testing,
* some Swagger tricks,
* using Newtonsoft for API,
* localization,
* ...

Supported users using `password`:

* admin
* user (not allowed to delete)

Authorization header pattern: `Bearer <token>`

To start the sample, run:

* `./scripts/build_docker.sh`
* `docker compose up`

The following is TODO:

* identity provider,
* ...
