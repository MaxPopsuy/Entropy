//! Configure your routes in this file

import { lazy } from "react";

import { urls } from "@utils/common";

export const routes = [
  {
    key: "home",
    path: urls.home,
    Component: lazy(() => import("@pages/HomePage/HomePage")),
    isPrivate: false, //? If true, route only for authorized users
    isPublicOnly: false, //? If true, route only for unauthorized users
  },
];
