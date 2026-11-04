# VideoGen
### Services Architecture

1. [Service Registry](#service-registry)
2. [Service Installer](#service-installer)
3. [React Components](#react-components)
4. [Server Plugins](#server-plugins)
5. [Database Migrations](#database-migrations)
6. [Service Types](#service-types)
7. [Uninstalling Services](#uninstalling-services)

---

## Service Registry

The service registry is a URL or relative/absolute filesystem path to a JSON file that describes a list of available services that can be installed and used within VideoGen.

The value of the Available Services field can include multiple service registry URLs or file system paths, separated by semi-colon (;).

The service registry JSON file format is as follows:
```json
[
    {
      "key": "my-service",
      "version": "0.1.0",
      "type": "service-type",
      "name": "My Service",
      "summary": "My service description",
      "gitrepo": "Service Git Repository URL",
      "iconurl": "Service Icon PNG file URL"
    },
    {
      "key": "my-other-service",
      "version": "0.1.0",
      "type": "service-type",
      "name": "My Other Service",
      "summary": "My other service description",
      "gitrepo": "Service Git Repository URL",
      "iconurl": "Service Icon PNG file URL"
    }
]
```

* `key`: A unique identifier for the service
* `version`: The version of the service
* `type`: The type of the service, and must be one of the following:

    * llms
    * transcript-generation
    * image-library
    * image-generation
    * video-library
    * video-generation
    * video-transitions
    * music-library
    * music-generation
    * sound-fx-library
    * sound-fx-generation
    * text-to-speech
    * text-captions

* `name`: The name of the service
* `summary`: A brief description of the service
* `gitrepo`: The Git repository URL for the service (or relative/absolute file system path)
* `iconurl`: The URL to the service's icon PNG file (or relative/absolute file system path)

## Service Installer

The service installer is an API (`/api/admin/services/install`) that installs a service from a Git repository or local file system path into the VideoGen web application. You cannot install services on a production server. Services can only be installed to a development environment where the React frontend is not compiled for production.

There are three folders within a service repository that are vital for the service installer:

1. **VideoGen.Web.Client** 
    * This folder will contain all of your [React Components](#react-components)
2. **VideoGen.Plugins**
    * This folder contains files for [Server Plugins](#server-plugins)
3. **VideoGen.SQL** 
    * This folder contains SQL scripts for [Database Migrations](#database-migrations)

Once the user clicks the "Install" button, the service installer will:

1. Download the service repository from the Git repository or local file system path into the web server's executable folder in a sub-folder called `Downloads` (e.g. `VideoGen.Web.Server/bin/Debug/net9.0/Downloads`).
2. Copy the files from the repository's `VideoGen.Web.Client` folder into the `VideoGen.Web.Client` project
3. Copy the contents of the repository's `VideoGen.Plugins` folder into the `VideoGen.Plugins` project root, so files under `Services/{ServiceName}/` land at `VideoGen.Plugins/Services/{ServiceName}/` (e.g. `VideoGen.Plugins/Services/OpenAI`)
4. Copy the SQL scripts from the repository's `VideoGen.SQL` folder into the `VideoGen.SQL` project and execute the `gulpfile.js` & `deploy.sql` scripts to apply the migrations.

> **Gitignored destinations:** all installed service files land in folders that are excluded by `.gitignore`, so installed files are never committed to the main repository:
>
> * React components from the repo's `VideoGen.Web.Client` folder are copied into `VideoGen.Web.Client/src/services/` (gitignored).
> * `.cs` files from the repo's `VideoGen.Plugins` folder are copied into `VideoGen.Plugins/Services/{ServiceName}/` (gitignored).
> * SQL scripts must be placed inside the repo's `VideoGen.SQL/Services/{ServiceName}/` subfolder so they are copied into `VideoGen.SQL/Services/{ServiceName}/` (gitignored) and picked up by `deploy.sql` generation.

## React Components

The React components are the user interface components that are loaded into the VideoGen web application from the service repository `VideoGen.Web.Client` folder. These components are lazy loaded and are used to provide a user interface for the service.

In order to register your service within React, you must include a `{my-service-key}.js` file in the `VideoGen.Web.Client/src/services` folder within your service repository. This file should contain the following structure:

```js
import { lazy } from 'react';

export default {
  key: "my-service",
  name: "My Service",
  description: "Description of my service",
  icon: "icon-file.png",
  serviceConfig: lazy(() => import('./my-service/serviceConfig')),
  projectDetails: lazy(() => import('./my-service/projectDetails'))
}
```

* `key`: The unique identifier for the service. This is used to reference the service in the code.
* `name`: The display name of the service.
* `description`: A brief description of the service.
* `icon`: The filename for the icon file, which will be loaded from the `VideoGen.Web.Client/public/services/` folder.
* `serviceConfig`: A React component that is used for configuring the service from the Services page within the VideoGen web application.
* `projectDetails`: A React component that is used to display a UI for the service within the Project Details page.

## Server Plugins

The server plugins are the backend components that are installed into the `VideoGen.Plugins` project. All `.cs` files for the service **must** be located under a `Services/{ServiceName}/` subfolder (e.g. `VideoGen.Plugins/Services/OpenAI`), and should be organized into `Models/`, `Controllers/`, `Entities/`, `Interfaces/`, and `Repositories/` subfolders as needed. Namespaces must follow the folder path (e.g. `VideoGen.Plugins.Services.OpenAI.Controllers`). Files are copied into the `VideoGen.Plugins` project root, so they land at `VideoGen.Plugins/Services/{ServiceName}/` — never place service `.cs` files at the root of this folder, or they will collide with other services and the project's own root files.

## Database Migrations

The `VideoGen.SQL` folder is optional and can contain SQL scripts for database migrations if the service requires any database schema additions. You must include an `uninstall.sql` script to remove the database schema when the service is uninstalled.

When the user clicks the install button for your service, the `VideoGen.SQL` folder will be copied into the `VideoGen.SQL` project and then the `deploy.sql` script will be executed to apply the migrations. Your SQL scripts must be located under a `Services/{ServiceName}/` subfolder (e.g. `VideoGen.SQL/Services/OpenAI`) and files should be organized into `Functions/`, `Indexes/`, `SeedData/`, `Sequences`, `Tables/` folders as needed.

## Service Types

The following service types are available:

### LLMs
* `llms`
Services that provide access to Large Language Models that can be used by other services, such as transcript generation, but can also be used to come up with creative ideas for projects as well as other uses.

### Transcript Generation
* `transcript-generation`
Services that will write a script for a video based on a prompt. The script may include scene descriptions, character names, and other metadata, including narrative structure and pacing, chapters, and scene transitions.

### Image Library
* `image-library`
Services that provide access to a library of images that can be used by other services, such as video generation, or overlayed on top of videos, such as logos, watermarks, or other graphics.

### Image Generation
* `image-generation`
Services that generate images based on prompts or other input that can be used by other services, such as video generation or as background images within the project video output.

### Video Library
* `video-library`
Services that provide access to a library of videos that can be used as assets within the project video output.

### Video Generation
* `video-generation`
Services that provide access to video generation models that can be used to generate videos based on prompts or other input.

### Video Transitions
* `video-transitions`
Services that stitch together multiple videos into a single video with transitions between them.

### Music Library
* `music-library`
Services that provide access to a library of music that can be used to provide background music for project video output.

### Music Generation
* `music-generation`
Services that generate music based on prompts or other input that can be used to provide background music for project video output.

### Sound FX Library
* `sound-fx-library`
Services that provide access to a library of sound effects that can be used to provide sound effects for project video output.

### Sound FX Generation
* `sound-fx-generation`
Services that generate sound effects based on prompts or other input that can be used to provide sound effects for project video output.

### Text To Speech
* `text-to-speech`
Services that convert text to speech for project video output.

### Text Captions
* `text-captions`
Services that generate animated text caption videos that are layered on top of the project video output.

## Uninstalling Services

To uninstall a service, you can use the `/api/admin/services/uninstall` API endpoint. This will remove the service from the VideoGen web application, including the React components, server plugins, and database migrations.

The `Services.sql` table includes a `InstalledFiles` column that stores a JSON array of files relative to the VideoGen root folder that were installed for the service. This is used to remove the files when the service is uninstalled. If `HasSchema` is true, the `uninstall.sql` script will be executed from the downloaded repository to remove the database schema.