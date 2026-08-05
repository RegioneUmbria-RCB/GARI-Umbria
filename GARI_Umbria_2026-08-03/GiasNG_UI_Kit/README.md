# Gias UI Kit

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 19.1.7.

The **GiasNG UI Kit** is a shared library of reusable Angular components designed to provide a consistent and efficient user interface across all Gias applications.  
It enables faster development by centralizing common UI elements and ensuring design and behavior consistency throughout the organization’s Angular projects.

## Features

- A set of reusable Angular components tailored for Gias applications
- Shared UI patterns and behaviors to improve consistency and maintainability
- Easy integration into any Angular project

## Installation

Currently, the library must be installed manually. Follow these steps:

1. Copy the `.tgz` file to your target Angular project and add the dependency in `package.json`:

```
"gias-ui-kit": "file:src/lib/gias-ui-kit-1.0.0.tgz"
```

Make sure to update the version number as needed.

2. Before installing, remove existing `node_modules` and the lock file to ensure a clean install:

```
rm -r node_modules/
rm package-lock.json
```

3. Install the dependency:

```
npm install gias-ui-kit
npm install
```

4. Import the required modules and components from the library into your Angular project modules.

```
import { GiasUikitModule } from 'gias-ui-kit';

@NgModule({
  imports: [
    GiasUikitModule,
    // other imports
  ],
})
export class SomeModule {}
```

## Contributing

To contribute to the Gias UI Kit:

1. Make your changes or add new components following Angular best practices.
2. **If you add, remove, or modify any component exported from `projects/gias-ui-kit/src/public-api.ts`, you must update the documentation in `specs.md` accordingly (inputs, outputs, behavior, and a small usage example).**
3. Update the `version` in `package.json` based on the type of change (see below).
4. Run:

```
npm run build-library
npm run pack-lib
```

5. Test the generated `.tgz` file by installing it in another Angular project.
6. Keep all development work on a separate branch until the changes have been validated. Once confirmed, merge them into the `dev` branch.

We follow [Semantic Versioning (SemVer)](https://semver.org/) using the `MAJOR.MINOR.PATCH` format:

- **PATCH** (`x.x.1`): Bug fixes or minor internal changes  
- **MINOR** (`x.1.0`): Backward-compatible new features or components  
- **MAJOR** (`1.0.0`): Breaking changes or major refactors

**Examples**:  
`"version": "1.2.0"` → `"1.2.1"` // PATCH  
`"version": "1.2.0"` → `"1.3.0"` // MINOR  
`"version": "1.2.0"` → `"2.0.0"` // MAJOR

## Changelog

### 23/04/2026 Version 1.3.0

1. Update angular version

### 16/04/2026 Version 1.2.13

1. Fix `gias-numeric-template` initial value being replaced on blur

### 10/03/2026 Version 1.2.12

1. Fix broken reference

### 03/03/2026 Version 1.2.11

1. Add specs.md file

### 02/03/2026 Version

1. Add `Username` property to `GuidaValoreImpostazione`

### 29/05/2025 Version

1. Removed `gias-indirizzo-template` component
2. Added `listview` component

### 09/06/2025 Version

1. Imported `listview` directives, to correctly display the component

### 02/07/2025 Version
1. Removed `Impostazioni_SuperUser` property from Utente

### 01/09/2025 Version
1. Updated `gias-update-toolbar`
 
### 05/02/2026 Version
1. Updated `gias-form-error-visualizer`
 
### 02/03/2026 Version
1. Updated `gias-numeric-template`
