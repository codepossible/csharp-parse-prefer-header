# Parsing Prefer Header in C\#

## Introduction

This repository is a port of the Node version of parsing HTTP `Prefer` header to C#. The original code for Node written by Patrick Paskaris is available [here](https://github.com/ppaskaris/node-parse-prefer-header).

I ported this code to C\# for a project, I worked on and thought it might be useful for others using [RFC 7240](https://tools.ietf.org/html/rfc7240).

## API

The code has a main class: **`PreferHeaderParser`**, which implements parsing of the `Prefer` HTTP header via the `Parse` method. The method returns an enumerable of `Preference` class.

Usage:

```csharp
using HttpPreferHeaderParser;

....

var parser = new PreferHeaderParser();
var preferences = parser.Parse("foo, bar");

/*
  preferences = [
      { Name: "foo", Value="true", Parameters={}},
      { Name: "bar", Value="true", Parameters={}},

  ]
*/

....

var preferences = parser.parse("return=minimal; foo=\"some parameter\"");

/*
  preferences = [
      { Name: "return", Value="minimal",
        Parameters={
            "foo" : "some parameter"
        }
      },
  ]
*/


....

```
