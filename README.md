[![](https://img.shields.io/nuget/v/soenneker.data.zipcode.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.data.zipcode/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.data.zipcode/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.data.zipcode/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/nuget/dt/soenneker.data.zipcode.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.data.zipcode/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.data.zipcode/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.data.zipcode/actions/workflows/codeql.yml)

# Soenneker.Data.ZipCode

Simply adds a US ZIP code text file list from USPS, updated daily (if available).

## Install

```bash
dotnet add package Soenneker.Data.ZipCode
```

## What it provides

- Simply adds a US ZIP code text file list from USPS, updated daily (if available).
- The file is copied to the output directory, and located at the relative path: `Resources\zipcodes.txt`.
- Alternatively, you can download the ZIP code list as a text file from the following URL:.
- https://raw.githubusercontent.com/soenneker/soenneker.data.zipcode/main/src/Resources/zipcodes.txt.

## How to use it

After installation, resolve the packaged file from the output-relative path above. The package deploys the asset but does not invoke it for you.
