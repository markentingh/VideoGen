// Registry of installed services.
// Each service registration file ({service-key}.js in src/services/) exports a
// default object with { key, name, description, icon, serviceConfig, projectDetails }
// — see Docs/Services.md. Installed services are copied there by the installer.
const modules = import.meta.glob('../services/*.js', { eager: true });

export const serviceModules = Object.fromEntries(
  Object.values(modules)
    .map((module) => module.default)
    .filter((service) => service && service.key)
    .map((service) => [service.key, service])
);
