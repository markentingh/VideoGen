import { lazy } from 'react';
import { Navigate } from 'react-router-dom';

const routes = [
  { path: '/dashboard',            Element: () => <Navigate to="/dashboard/projects" replace /> },
  { path: '/dashboard/projects',   Element: lazy(() => import('@/app/dashboard/projects/page')) },
  { path: '/dashboard/automation', Element: lazy(() => import('@/app/dashboard/automation/page')) },
  { path: '/dashboard/services',   Element: lazy(() => import('@/app/dashboard/services/page')) },
  { path: '/dashboard/accounts',   Element: lazy(() => import('@/app/dashboard/accounts/page')) },
  { path: '/dashboard/users',      Element: lazy(() => import('@/app/dashboard/users/page')) }
];

export default routes;
