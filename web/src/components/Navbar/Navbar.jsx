import React from "react";
import { navbar } from "../../utils/common";
import styles from './Navbar.module.scss';

const Navbar = ({}) => (
  <NavComponent />
);

export const NavComponent = ({ onClick }) => (
  <nav className={styles["navbar"]}>
    {navbar.map((section) => (
      <a
        href={`#${section}`}
        onClick={onClick}
        className={styles["navbar__link"]}
      >
        {section}
      </a>
    ))}
  </nav>
);
export default Navbar;
