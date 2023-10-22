import React, { useState } from "react";
import { NavComponent } from "../Navbar/Navbar";
import styles from "./Burger.module.scss";
import classnames from "classnames";

const Burger = () => {
  let [active, setActive] = useState(false);
  return (
    <div className={styles["burger"]}>
      <button
        className={styles["burger__button"]}
        onClick={() => setActive(!active)}
      >
        {active ? <span>&times;</span> : <span>&#9776;</span>}
      </button>

      <div
        className={classnames(
          styles["burger__container"],
          active && styles["burger__container--active"]
        )}
      >
        <NavComponent onClick={() => setActive(true)} />
      </div>
    </div>
  );
};
export default Burger;
