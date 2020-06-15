# Parsing Prefer Header in C\#

## Introduction

This repository is a port of the Node version of parsing HTTP `Prefer` header to C#. The original code for Node written by Patrick Paskaris is available [here](https://github.com/ppaskaris/node-parse-prefer-header).

I ported this code to C\# for a project, I worked on and thought it might be useful for others using [RFC 7240](https://tools.ietf.org/html/rfc7240).

## Usage

The code has a main class: **`PreferHeaderParser`**, which implements parsing of the `Prefer` HTTP header via the `Parse` method. The method returns an enumerable of `Preference` class.

Usage:

```csharp
using HttpPreferHeaderParser;

....

var parser = new PreferHeaderParser();
var preferences = parser.Parse("foo, bar");

/*
  preferences = [
      { Name: "foo", Value:"true", Parameters:{}},
      { Name: "bar", Value:"true", Parameters:{}},

  ]
*/

....

var preferences = parser.parse("return=minimal; foo=\"some parameter\"");

/*
  preferences = [
      { Name: "return",
        Value="minimal",
        Parameters: {
            "foo" : "some parameter"
        }
      },
  ]
*/


....

```

## API

### Public class - `PreferHeaderParser`

#### `PreferHeaderParser` - Properties

- None

#### `PreferHeaderParser` Methods

- `PreferHeaderParser()` :
  - Default Constructor
  - Returns instance of `PreferHeaderParser` class
- `List<Preferences> Parse(string headerValue)`:
  - Parses the string containing the contents of Prefer Header
  - Returns a list of `Preference` class, if successfully processed.
  - `null`, if header value is empty.
- `List<Preferences> Parse(string[] headerValue)`:
  - Parses the array of string containing the contents of multiple Prefer Header
  - Returns a list of `Preference` class, if successfully processed.
  - `null`, if array is empty.

### Public class `Preference`

#### `Preferences` - Properties

- `Name` : string
  - The name of the preference.
- `Value`: string
  - The value for the preference. default to "true", if value is not provided.
- `Parameters`: Dictionary<string, string>
  - A key value pair containing the parameters for the preference

#### `Preferences` - Methods

- None
  