import { createReducer } from "@reduxjs/toolkit";
import { combineReducers } from "redux";
import { TEST_ACTION } from "@store/actions";

const initialState = { items: ["item-1", "item-2", "item-3"] };

const test = createReducer(initialState, {
  [TEST_ACTION]: (_, action) => action.payload,
});

export default combineReducers({ test: test });
