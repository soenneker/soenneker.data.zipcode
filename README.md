[![](https://img.shields.io/nuget/v/soenneker.data.zipcode.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.data.zipcode/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.data.zipcode/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.data.zipcode/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/nuget/dt/soenneker.data.zipcode.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.data.zipcode/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.data.zipcode/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.data.zipcode/actions/workflows/codeql.yml)

# Soenneker.Data.ZipCode

A packaged newline-delimited list of five-digit United States ZIP codes.

## Installation

```bash
dotnet add package Soenneker.Data.ZipCode
```

The package copies this file to the consuming application's output directory:

```text
Resources/zipcodes.txt
```

It contains one five-character ZIP code per line. The package provides data only—there is no service, parser, address validator, or dependency-injection registration.

## Load the ZIP-code set

```csharp
string path = Path.Combine(
    AppContext.BaseDirectory,
    "Resources",
    "zipcodes.txt");

HashSet<string> zipCodes = File.ReadLines(path)
    .Select(static line => line.Trim())
    .Where(static line => line.Length == 5)
    .ToHashSet(StringComparer.Ordinal);
```

Keep ZIP codes as strings so leading zeroes are preserved:

```csharp
bool exists = zipCodes.Contains("00601");
```

For ZIP+4 input, validate the full input according to your application's rules and look up only the first five digits. Membership in this file does not validate the four-digit delivery segment.

## Operational guidance

The list is a snapshot shipped with the installed package version. Updating the NuGet package updates the output asset; deployed applications do not download changes automatically.

A matching ZIP code indicates only that the five-digit code appears in the dataset. It does not prove that an address exists, is deliverable, belongs to a claimed city or state, or is current. Use an address-validation service when delivery accuracy matters.

Loading the file into a `HashSet<string>` provides fast repeated lookup while retaining the complete list in memory. The data is small enough for many server applications, but occasional or constrained consumers can scan the file or build a different index.
