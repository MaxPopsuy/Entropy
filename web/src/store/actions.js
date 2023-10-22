import { createAction } from "@reduxjs/toolkit";
import { TEST_TYPE } from "@store/types";

const TEST_ACTION = createAction(TEST_TYPE);

export { TEST_ACTION };
