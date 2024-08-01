# CsvMerge

[![CsvMerge](https://img.shields.io/nuget/v/CsvMerge)](https://www.nuget.org/packages/CsvMerge)
[![CsvMerge.Core](https://img.shields.io/nuget/v/CsvMerge.Core)](https://www.nuget.org/packages/CsvMerge.Core)
[![ci](https://github.com/miles-till/csv-merge/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/miles-till/csv-merge/actions/workflows/build.yml)
[![cd](https://github.com/miles-till/csv-merge/actions/workflows/publish.yml/badge.svg)](https://github.com/miles-till/csv-merge/actions/workflows/publish.yml)

Simple tool to merge csv files.

## dotnet tool

### Quick start

```sh
dotnet tool install -g CsvMerge
csv-merge *.csv --output merged.csv
```

### Arguments

```
Description:
  CsvMerge

Usage:
  csv-merge <search-patterns>... [options]

Arguments:
  <search-patterns>  Glob search patterns for the csv files to merge.

Options:
  --output <output>                        The output result of merging csv files.
  --filepath-field <filepath-field>        The header for the filepath field included in the merged csv file. [default: Filepath]
  --working-directory <working-directory>  The working directory to search for files in. []
  --overwrite                              Overwrite output file if it already exists. [default: True]
  --version                                Show version information
  -?, -h, --help                           Show help and usage information
```

## Nuget package

### Quick start

```sh
dotnet add package CsvMerge.Core
```

```C#
using CsvMerge;

string[] files = ["file1.csv", "file2.csv", "file3.csv"];
MergeOptions options = new() { Output = new FileInfo("merge.csv") };
await Merger.Merge(files, options);
```
