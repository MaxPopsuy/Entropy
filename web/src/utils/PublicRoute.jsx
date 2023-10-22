import { useSelector } from "react-redux";
import { Navigate } from "react-router-dom";

// import * as authSelectors from "@store/auth/auth.selectors";
import { urls } from "@utils/common";

function PublicRoute({ isPublicOnly, children }) {
  // const isLoggedIn = useSelector(authSelectors.isLoggedIn);
  const isLoggedIn = true; //! Use your redux if you need private/public routes
  
  return isLoggedIn && isPublicOnly ? <Navigate to={urls.home} /> : children;
}

export { PublicRoute };
