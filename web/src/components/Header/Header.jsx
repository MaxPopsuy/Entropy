import React from "react";
import Navbar from "../Navbar/Navbar";
import Burger from "../Burger/Burger";
import useWindow from "@utils/useWindow";
import styles from "./Header.module.scss";
import { NavLink } from "react-router-dom";
import { urls } from "../../utils/common";

const Header = () => {
  const [width, height] = useWindow();
  console.log(height);
  return (
    <header className={styles["header"]}>
      <NavLink to={urls.home} className={styles["header__logo"]}>
        Entropy
      </NavLink>

      {width > 1000 ? <Navbar /> : <Burger />}
    </header>
  );
};

export default Header;
