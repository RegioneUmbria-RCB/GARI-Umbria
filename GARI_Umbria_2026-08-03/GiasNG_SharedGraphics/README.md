# Agronica Shared CSS Library

This repository contains the shared CSS styles and icon assets used across Agronica's web applications.  
It provides a centralized and consistent design language for all projects within the organization.

## Features

- Common CSS styles used across Agronica applications  
- A comprehensive set of Agronica-specific icons  
- Easy to integrate and maintain across multiple projects

## Installation

Temporarily, install the package manually by copying the `.tgz` file into your project abd the adding the following line to your dependencies:

```
"gias-shared-css": "file:src/lib/gias-shared-css-1.0.0.tgz"
```

with the correct version. In the near future this library will be handled using npm. 

Import the shared styles and icons into your project by including the `index.css` file into your `angular.json` file:

``` 
"architect" >> "build" >> "options" >> "styles"

"node_modules/gias-shared-css/src/index.css"
```

## Contributing

To contribute changes to the shared styles or icons, follow these steps:

1. Apply the actual changes to the relevant CSS or icon files.
2. Update the `version` field in the `package.json` file according to the type of change (see below).
3. Generate the `.tgz` archive using `npm pack` and test it by installing it in another project to ensure the changes work as expected.
4. Keep all changes on a separate branch until they have been successfully tested in another project. Once verified, merge the changes into the `dev` branch.

We follow [Semantic Versioning (SemVer)](https://semver.org/), which uses the `MAJOR.MINOR.PATCH` format

Update the version in `package.json` based on the type of change:

- **PATCH** (`x.x.1`): Small fixes or adjustments that do not break compatibility  
  _Example: Fixing a typo or adjusting spacing in a component_
- **MINOR** (`x.1.0`): Additions that are backward compatible  
  _Example: Adding a new icon or a new utility class_
- **MAJOR** (`1.0.0`): Breaking changes that are not backward compatible  
  _Example: Renaming classes or removing existing icons_


"version": "1.3.2" → "1.3.3"  // PATCH  
"version": "1.3.2" → "1.4.0"  // MINOR  
"version": "1.3.2" → "2.0.0"  // MAJOR

## Changelog

### 07/01/2026 Version 1.6.0

1. Added new classes `innesco-scaduto` and `innesco-in-scadenza` in `stylesXonneTables.scss`