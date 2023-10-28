import React from "react";
import styles from "./Footer.module.scss";
import classnames from "classnames";

const Footer = () => {
  return (
    <footer className={styles["footer"]}>
      <h4 className={styles["footer__title"]}>Entropy</h4>
      <p className={styles["footer__text"]}>Entropy version 0.0.0</p>
      <p
        className={classnames(
          styles["footer__text"],
          styles["footer__text--rights"]
        )}
      >
        @2023. Entropy. All rights reserved
      </p>

      {/* <div className={styles["footer__navbar"]}>
        <a className={styles["footer__link"]} href="">
          about
        </a>
        <a className={styles["footer__link"]} href="">
          features
        </a>
        <a className={styles["footer__link"]} href="">
          gallery
        </a>
      </div> */}
    </footer>
  );
};

export default Footer;
