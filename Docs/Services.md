# VideoGen
### Services Architecture

1. (Service Registry)[#service-registry]
2. (Service Installer)[#service-installer]
3. (React Components)[#react-components]
4. (Server Plugins)[#server-plugins]
5. (Database Migrations)[#database-migrations]
6. (Uninstalling Services)[#uninstalling-services]

---

## Service Registry

The service registry is a URL or relative/absolute filesystem path to a JSON file that describes a list of available services that can be installed and used within VideoGen.

The value of the Available Services field can include multiple service registries, separated by semi-colon (;).

The service registry JSON file format is as follows:
```json
{
  "services": [
    {
      "key": "my-service",
      "version": "0.1.0",
      "type": "service-type",
      "name": "My Service",
      "summary": "My service description",
      "gitrepo": "Service Git Repository URL",
      "iconurl": "Service Icon PNG file URL"
    }
  ]
}
```

* `key`: A unique identifier for the service
* `version`: The version of the service
* `type`: The type of the service, and must be one of the following:
    * transcript-generation
    * video-library
    * video-generation
    * video-transitions
    * image-generation
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
    * This folder contains the React components for the service. All files from this folder will get copied into the `VideoGen.Web.Client` project.
2. **VideoGen.Plugins**
    * This folder contains the `.cs` files for the service. These files will get copied into the `VideoGen.Plugins` project into a `Services` sub-folder using the service `key` as the folder name (e.g. `VideoGen.Plugins/Services/my-service`).
3. **VideoGen.SQL** 
    * This folder contains SQL scripts for database migrations. This folder will be copied into the `VideoGen.SQL` project and the `gulpfile.js` & `deploy.sql` scripts will be executed to apply the migrations.

Once the user clicks the "Install" button, the service installer will:

1. Download the service repository from the Git repository or local file system path into the web server's executable folder in a sub-folder called `Downloads` (e.g. `VideoGen.Web.Server/bin/Debug/net9.0/Downloads`).
2. Copy the files from the repository's `VideoGen.Web.Client` folder into the `VideoGen.Web.Client` project
3. Copy the `.cs` files from the repository's `VideoGen.Plugins` folder into the `VideoGen.Plugins` project into a `Services` sub-folder using the service `key` as the folder name (e.g. `VideoGen.Plugins/Services/my-service`)
4. Copy the SQL scripts from the repository's `VideoGen.SQL` folder into the `VideoGen.SQL` project and execute the `gulpfile.js` & `deploy.sql` scripts to apply the migrations.

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

The server plugins are the backend components that are installed into the VideoGen server application. Within a service repository, the `VideoGen.Plugins` folder contains the `.cs` files for the service, which get copied into the `VideoGen.Plugins` project and compiled with it.

## Database Migrations

The `VideoGen.SQL` folder is optional and can contain SQL scripts for database migrations if the service requires any database schema additions. You must include an `uninstall.sql` script to remove the database schema when the service is uninstalled.

## Uninstalling Services

To uninstall a service, you can use the `/api/admin/services/uninstall` API endpoint. This will remove the service from the VideoGen web application, including the React components, server plugins, and database migrations.

The `Services.sql` table includes a `InstalledFiles` column that stores a JSON array of files relative to the VideoGen root folder that were installed for the service. This is used to remove the files when the service is uninstalled. If `HasSchema` is true, the `uninstall.sql` script will be executed from the downloaded repository to remove the database schema.

