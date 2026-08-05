# Gias Kendo Grid

This library was generated with [Angular CLI](https://github.com/angular/angular-cli) version 14.2.0.

This project was created to extract the `gias-kendo-grid` library into an NPM package. `gias-kendo-grid` is a wrapper around the Kendo Grid, initially developed within the `GiasNG` project. The main goal is to abstract as many common and reusable grid behaviors as possible, making them available for use across different projects.

## Features

- Kendo grid wrapper tailored for Gias applications
- Easy integration into any Angular project

## Installation

Currently, the library must be installed manually. Follow these steps:

1. Copy the `.tgz` file to your target Angular project and add the dependency in `package.json`:

```
"gias-kendo-grid": "file:src/lib/gias-kendo-grid-1.0.0.tgz"
```

Make sure to update the version number as needed.

2. Before installing, remove existing `node_modules` and the lock file to ensure a clean install:

```
rm -r node_modules/
rm package-lock.json
```

3. Install the dependency:

```
npm install gias-kendo-grid
npm install
```

4. Import the required modules and components from the library into your Angular project modules.

```
import { GiasKendoGridModule } from 'gias-kendo-grid';

@NgModule({
  imports: [
    GiasKendoGridModule,
    // other imports
  ],
})
export class SomeModule {}
```

## Contributing

To contribute to the Gias Kendo Grid:

1. Make your changes or add new components following Angular best practices.
2. **If you add, remove, or modify any component exported from `projects/gias-kendo-grid/src/public-api.ts`, you must update the documentation in `specs.md` accordingly (inputs, outputs, behavior, and a small usage example).**
3. Update the `version` in both `package.json` files based on the type of change (see below).
4. Run:

```
npm run build-library
npm run pack-lib
```

5. Test the generated `.tgz` file by installing it in another Angular project.
6. Keep all development work on a separate branch until the changes have been validated. Once confirmed, merge them into the `dev` branch.
7. Update the changelog at the end of this file

We follow [Semantic Versioning (SemVer)](https://semver.org/) using the `MAJOR.MINOR.PATCH` format:

- **PATCH** (`x.x.1`): Bug fixes or minor internal changes  
- **MINOR** (`x.1.0`): Backward-compatible new features or components  
- **MAJOR** (`1.0.0`): Breaking changes or major refactors

**Examples**:  
`"version": "1.2.0"` → `"1.2.1"` // PATCH  
`"version": "1.2.0"` → `"1.3.0"` // MINOR  
`"version": "1.2.0"` → `"2.0.0"` // MAJOR

## Changelog

### 23/04/2026 Version 3.4.0

1. Update angular version

### 16/04/2026 Version 3.3.20

1. Update GiasNG-UI-Kit version

### 01/03/2026 Version 3.3.19

1. Fix dropdown colunm filter menu in case of null values

### 23/03/2026 Version 3.3.18

1. Added a method getMessagePrefix inside grid-config.service for allowing classes that inherit from it to override the method

### 10/03/2026 Version 3.3.17

1. Update GiasNG-UI-Kit version

### 04/03/2026 Version 3.3.16

1. Add specs.md file

### 03/03/2026 Version 3.3.15

1. Update GiasNG-UI-Kit version

### 02/03/2026 Version 3.3.14

1. Update GiasNG-UI-Kit version

### 19/09/2025 Version 3.3.0

1. Check if `format` property is changed for update kendo-grid-column

### 29/05/2025 Version

1. Updated `gis-ui-kit.tgz`
2. Removed `listview` component
