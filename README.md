# LuccaDevises

Simple .NET Core currency converter for a given exchange rates table.

## Installation

Make sure you have the .NET Core SDK installed.

Publish the app using:

```
dotnet publish -r <platform> -c Release --self-contained
```
platform example: win10-x64

Or as a single file:
```
dotnet publish -r win10-x64 -c Release /p:PublishSingleFile=true
```


## Usage

You can then find the exe in the "publish" directory and run

```
LuccaDevises <path_to_table_file>
```