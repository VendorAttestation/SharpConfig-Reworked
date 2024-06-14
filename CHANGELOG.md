# Change Log

## v4.1.0 2024-06-14
- Changed target framework to .NET 8.
- Upgraded dependencies.
- Simplified the code.

## v4.1.0 2024-01-12

- Fixed [issue #1](https://github.com/huanlin/SharpConfig/issues/1): Setting is not saved if it does not exist in the config file.
- Changed target framework to .NET 7.
- Upgraded dependencies and modify code accordingly.
- Updated Example2.

## v4.0.1 February 2022

- Add Configuration.SuppressArrayParsing property and a unit test.
- Upgrade old .csproj files to SDK style.
- Change target frameworks to .NET standard 2.0 and .NET 6.
- Use MinVer package to simply project versioning.
- Change appveyor.yml for automatic deploy NuGet package. Package name: SharpConfig-huanlin.
- Add Example2 project for showing separate configuration section classes.