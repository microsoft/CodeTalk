import { combineReducers } from "redux";

import homePageReduder from "./homePage";
import alertReducer from "./alert";
import replayReducer from "./replay";

const rootReducer = combineReducers({
  homePage: homePageReduder,
  alert: alertReducer,
  replay: replayReducer
});

export default rootReducer;
