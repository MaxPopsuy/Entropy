import { useSelector } from "react-redux";
import { Navigate } from "react-router-dom";

// import * as authSelectors from "@store/auth/auth.selectors";
import { urls } from "@utils/common";

function PrivateRoute({ children }) {
  // const isLoggedIn = useSelector(authSelectors.isLoggedIn);
  const isLoggedIn = true; //! Use your redux if you need private/public routes

  return isLoggedIn ? children : <Navigate to={urls.login} />;
}

export {PrivateRoute};
