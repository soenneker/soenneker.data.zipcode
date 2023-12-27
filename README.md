[![](https://img.shields.io/nuget/v/soenneker.data.zipcode.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.data.zipcode/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.data.zipcode/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.data.zipcode/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/nuget/dt/soenneker.data.zipcode.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.data.zipcode/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Data.ZipCode
### Simply adds a US ZIP code text file list from USPS, updated daily (if available)

## Installation

```
dotnet add package Soenneker.Data.ZipCode
```

The file is copied to the output directory, and located at the relative path: `Resources\zipcodes.txt`.

Alternatively, you can download the ZIP code list as a text file from the following URL:

```
https://raw.githubusercontent.com/soenneker/soenneker.data.zipcode/main/src/Resources/zipcodes.txt
```