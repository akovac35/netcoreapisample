### Creating migration - execute in solution root and bash

```sh
(export SampleConfig__UseInMemoryDb=false && export SampleConfig__DbType=Oracle && dotnet ef migrations add InitialCreate -s ./src/WebApi -p ./src/Infrastructure.Migrations.Oracle)
```
