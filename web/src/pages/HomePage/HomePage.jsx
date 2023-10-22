import React from "react";
import styles from "./HomePage.module.scss";
import { Fade, Slide, Bounce } from "react-awesome-reveal";
import entropy from "@assets/entropy.png";
import entropy2 from "@assets/entropy3.png";
import classnames from "classnames";
import { text } from "../../utils/common";

const HomePage = () => {
  return (
    <section className={styles["page"]}>
      <div className={styles["page__container"]}>
        <Slide className={styles.page__slide}>
          <h1 className={styles["page__title"]}>Entropy</h1>
        </Slide>
        <p className={styles["page__info"]}>
          Entropy Terminal Task Manager is a tool similar to a typical task
          manager
        </p>
        <p className={styles["page__info"]}>
          But, since entropy itself is a terminal application, it has some
          features such as Performance and speed, thanks to the absence of a
          graphical interface
        </p>
        <p className={styles["page__info"]}>
          This is especially useful when every second counts.
        </p>
        <p className={styles["page__info"]}>
          Let's take a look at some of Entropy's features.
        </p>
      </div>

      {/* <p className={styles["page__description"]}>
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer aliquam
        justo in condimentum convallis. Fusce sit amet egestas massa, ultricies
        interdum risus. Etiam nec dapibus magna, eget hendrerit nisi. Vivamus
        auctor commodo libero. Curabitur eu sem fermentum, consequat nisi quis,
        posuere elit. Proin commodo tortor ac consectetur rhoncus. Mauris
        molestie lacus nec lacus dictum, pulvinar molestie tellus accumsan.
        Donec augue arcu, bibendum in mi vel, facilisis varius sapien. Sed
        volutpat tortor at nisi consequat, feugiat pellentesque turpis egestas.
        Donec vel quam aliquam, semper velit in, scelerisque velit. Sed ornare
        ex turpis, sed fringilla turpis ornare nec. Proin viverra euismod ligula
        dictum ornare. Nam finibus elit quis sapien pretium elementum. Aliquam
        urna sapien, pellentesque ac laoreet a, vehicula vitae turpis. Etiam
        maximus eros eget nulla rhoncus feugiat. Donec lobortis eros ac orci
        commodo, quis pharetra neque posuere. Pellentesque nec nibh sit amet
        quam scelerisque venenatis. Donec dictum congue libero id varius.
        Maecenas dolor velit, hendrerit vel nunc id, tristique feugiat orci.
        Phasellus malesuada porttitor augue vitae luctus. Duis eros ipsum,
        luctus nec est sit amet, bibendum egestas lacus. In hac habitasse platea
        dictumst.
      </p> */}

      <img
        src={entropy}
        alt="entropy - terminal task manager"
        className={classnames(
          styles["page__image"],
          styles["page__image--first"]
        )}
      />
      <img
        src={entropy2}
        alt="entropy - terminal task manager"
        className={classnames(
          styles["page__image"],
          styles["page__image--second"]
        )}
      />
    </section>
  );
};

export default HomePage;
