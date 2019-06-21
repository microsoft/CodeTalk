import * as Mousetrap from "mousetrap";
import store from "../store";
import * as actions from "../actions/homePage";
import * as CONSTANTS from "../constants/strings";
import * as soundManager from "./soundManager";

export default function bindKeyboardShortcuts() {
  let controlConnection = new WebSocket("ws://localhost:5667/control");
  controlConnection.onopen = () => {
    console.log("control channel");
  };

  Mousetrap.bind("ctrl+` ctrl+c", function(e) {
    e.preventDefault();
    store.dispatch(actions.toggleTab(CONSTANTS.CPU_USAGE));
  });

  Mousetrap.bind("ctrl+` ctrl+m", function(e) {
    e.preventDefault();
    store.dispatch(actions.toggleTab(CONSTANTS.MEMORY_USAGE));
  });

  Mousetrap.bind("ctrl+` ctrl+/", function(e) {
    e.preventDefault();
    store.dispatch(actions.toggleTab(CONSTANTS.FUNCTIONS_LIST));
  });

  Mousetrap.bind("ctrl+z", function(e) {
    e.preventDefault();
    soundManager.playSound();
    try {
      console.log(window);
      window.replay.playNext();
    } catch (e) {
      console.log(e);
    }
  });

  Mousetrap.bind("ctrl+` ctrl+.", function(e) {
    e.preventDefault();
    controlConnection.send(
      JSON.stringify({
        command: "pause"
      })
    );
  });

  Mousetrap.bind("ctrl+` ctrl+,", function(e) {
    e.preventDefault();
    controlConnection.send(
      JSON.stringify({
        command: "resume"
      })
    );
  });

  Mousetrap.bind("ctrl+` ctrl+a", function(e) {
    e.preventDefault();
    try {
      window.replay.autoPlay();
    } catch (e) {
      console.log(e);
    }
  });

  Mousetrap.bind("ctrl+` ctrl+p", function(e) {
    e.preventDefault();
    try {
      window.replay.pauseAutoPlay();
    } catch (e) {
      console.log(e);
    }
  });

  Mousetrap.bind("ctrl+` ctrl+x", function(e) {
    e.preventDefault();
    try {
      window.replay.playNext();
    } catch (e) {
      console.log(e);
    }
  });

  Mousetrap.bind("ctrl+` ctrl+v", function(e) {
    e.preventDefault();
    try {
      window.replay.playPrevious();
    } catch (e) {
      console.log(e);
    }
  });

  Mousetrap.bind("ctrl+` ctrl+r", function(e) {
    e.preventDefault();
    try {
      window.location.href = '/replayMemory';
    } catch (e) {
      console.log(e);
    }
  });
}
