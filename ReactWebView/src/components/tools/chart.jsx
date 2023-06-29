import React from "react";
import {Howl} from 'howler';
import { connect } from "react-redux";

class AudioChartComponent extends React.Component {
  constructor(props) {
    super(props);
    this.x = 0;
    this.xstep = 10;
    this.state = {
      playing: false,
    };
    this.handlePlay = this.handlePlay.bind(this);
    this.handlePause = this.handlePause.bind(this);
  }

  handlePlay() {
    this.setState({
      playing: true
    });
  }

  handlePause() {
    this.setState({
      playing: false
    });
  }

  render() {
    const data = this.props.chartData;
    let lastNum = 0;
    if (data && data == "VS Joined") {
    } else {
      const array = data["datasets"]["0"]["data"];
      lastNum = array[array.length - 1];
      this.x += this.xstep;
    }
    var sound = new Howl({
      src: ["sin.wav"],
      loop: true,
    });
    var id = sound.play();
    sound.volume((lastNum/30)*(lastNum/30)*(lastNum/30), id);
    // sound.pos(lastNum * 40, 40, this.x, id);
    // console.log(lastNum * 40, 40, this.x, id);
    return <div />;
  }
}

const mapStateToProps = state => {
  return {
    chartData: state.homePage.cpuUsage.chartData,
    chartOptions: state.homePage.cpuUsage.chartOptions
  };
};

const mapDispatchToProps = dispatch => {
  return {};
};

const AudioChart = connect(
  mapStateToProps,
  mapDispatchToProps
)(AudioChartComponent);

export default AudioChart;
