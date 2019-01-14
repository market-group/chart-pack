# Market.Tools.ChartPack

[![Build Status](https://travis-ci.org/market-group/chart-pack.svg?branch=master)](https://travis-ci.org/market-group/chart-pack)

|                                            |                Stable                                                                                     |                                                       Pre-release                                           |                                   Downloads                                                             |
| -----------------------------------------: | :-------------------------------------------------------------------------------------------------------: | :---------------------------------------------------------------------------------------------------------: | :-----------------------------------------------------------------------------------------------------: |
| **Market.Tools.ChartPack**  |     ![nuget-extensions-stable](https://img.shields.io/nuget/v/Market.Tools.ChartPack.svg)  | ![nuget-extensions-unstable](https://img.shields.io/nuget/vpre/Market.Tools.ChartPack.svg)   | ![nuget-extensions-unstable](https://img.shields.io/nuget/dt/Market.Tools.ChartPack.svg) |

This tool pack a helm chart into a `.nupkg` including its dependencies. 

## Install

This tool is published using NuGet and can be install using dotnet-cli:  
`dotnet tool install -g Market.Tools.ChartPack`

(`dotnet tool` updates the `PATH` variable, so you might need to re-open shell to use the tool)

## Usage

```
CLI tool for packing charts

Usage: chart-pack [options]

Options:
  --help             Show help information
  -p|--path          Path to the chart directory
  -od|--out-dir      Directory to output nupkg
  -v|--version       Name of the chart
  -n|--name          Version of the chart
  -o|--override      [Optional] Override if exists. Default: false
  -d|--dependencies  Path to a dependencies.yaml
```

## Dependencies

The dependencies files to be specified with the chart package is in the following format:

```
dependencies:
- name: <name-of-dependant-chart>
  version: "<version-of-depandant-chart>"
```

### Chart Version
Chart version in depedencies files can be specified in nuget version range syntax:

|Notation|Applied rule|Description|
|--- |--- |--- |
|1.0|x ≥ 1.0|Minimum version, inclusive|
|(1.0,)|x > 1.0|Minimum version, exclusive|
|[1.0]|x == 1.0|Exact version match|
|(,1.0]|x ≤ 1.0|Maximum version, inclusive|
|(,1.0)|x < 1.0|Maximum version, exclusive|
|[1.0,2.0]|1.0 ≤ x ≤ 2.0|Exact range, inclusive|
|(1.0,2.0)|1.0 < x < 2.0|Exact range, exclusive|
|[1.0,2.0)|1.0 ≤ x < 2.0|Mixed inclusive minimum and exclusive maximum version|
|(1.0)|invalid|invalid|

From: [https://docs.microsoft.com/en-us/nuget/reference/package-versioning](https://docs.microsoft.com/en-us/nuget/reference/package-versioning)

