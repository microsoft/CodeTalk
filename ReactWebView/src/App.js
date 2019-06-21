import React, { Component } from 'react';
import { BrowserRouter as Router, Route} from "react-router-dom";
// import logo from './logo.svg';
import './App.css';
import HomePage from './components/pages/homePage';
import Documentation from './components/pages/documentation';
import Memory from './components/replay/memory';
import CPU from './components/replay/cpu';
import Header from './components/tools/header';
import bindKeyboardShortcuts from './js/keyboardManager';

class App extends Component {
  constructor(props){
    super(props);
    this.unlockAudioContext = this.unlockAudioContext.bind(this);
  }

  unlockAudioContext(audioCtx) {
    if (audioCtx.state === 'suspended') {
      var events = ['touchstart', 'touchend', 'mousedown', 'keydown'];
      var unlock = function unlock() {
        events.forEach(function (event) {
          document.body.removeEventListener(event, unlock)
        });
        audioCtx.resume();
      };
  
      events.forEach(function (event) {
        document.body.addEventListener(event, unlock, false)
      });
    }
  }

  render() {
    const audioCtx = new (window.AudioContext || window.webkitAudioContext)();
    this.unlockAudioContext(audioCtx);
    bindKeyboardShortcuts();
    return (
      <Router>
        <div className="App">
          <Header/>
          <Route exact path="/" component={HomePage} />
          <Route path="/Documentation" component={Documentation} />
          <Route path='/replayMemory' component={Memory} />
          <Route path='/replayCPU' component={CPU} />
        </div>
      </Router>
    );
  }
}

export default App;