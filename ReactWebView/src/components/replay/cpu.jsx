import React from "react";
import { connect } from "react-redux";
import { Row, Col, Button, Progress } from "reactstrap";
import NumericInput from "react-numeric-input";
import {
  loadDataForReplay,
  playNext,
  playPrevious
} from "../../actions/replay";

var LineChart = require("react-chartjs").Line;

export class MemoryComponent extends React.Component {
  constructor(props) {
    super(props);
    window.replay = this;
    this.playNext = this.playNext.bind(this);
    this.playPrevious = this.playPrevious.bind(this);
    this.autoPlay = this.autoPlay.bind(this);
    this.pauseAutoPlay = this.pauseAutoPlay.bind(this);
    this.updateButtonRenderState = this.updateButtonRenderState.bind(this);
    this.setStep = this.setStep.bind(this);
    this.setSpeed = this.setSpeed.bind(this);
    this.state = {
      index: 0,
      nextButton: true,
      previousButton: false,
      step: 1,
      speed: 1
    };
  }

  componentWillMount() {
    this.props.loadDataForReplay(this.props.data);
    this.playNext();
  }

  setStep(valueAsNumber, valueAsString, el) {
    this.setState({
      step: valueAsNumber
    });
  }

  setSpeed(valueAsNumber, valueAsString, el) {
    this.setState({
      speed: valueAsNumber
    });
  }

  updateButtonRenderState() {
    if (this.state.index > this.props.data.length) {
      clearInterval(this.state.intervalId);
      this.setState({
        nextButton: false,
        index: this.props.data.length + 1
      });
    }
    if (this.state.index <= this.state.step) {
      this.setState({
        previousButton: false,
        index: 1
      });
    }
  }

  playNext() {
    this.props.playNext(this.state.index);
    this.setState(
      prevState => ({
        index: prevState.index + this.state.step,
        previousButton: true
      }),
      () => this.updateButtonRenderState()
    );
  }

  playPrevious() {
    this.props.playPrevious(this.state.index);
    this.setState(
      prevState => ({
        index: prevState.index - this.state.step,
        nextButton: true
      }),
      () => this.updateButtonRenderState()
    );
  }

  autoPlay() {
    let intervalId = setInterval(this.playNext, 1000 / this.state.speed);
    this.setState({
      intervalId: intervalId
    });
  }

  pauseAutoPlay() {
    clearInterval(this.state.intervalId);
  }

  render() {
    const previousButton = this.state.previousButton ? (
      <Button color="primary" onClick={this.playPrevious}>
        Previous
      </Button>
    ) : (
      <Button color="primary" onClick={this.playPrevious} disabled>
        Previous
      </Button>
    );

    const nextButton = this.state.nextButton ? (
      <Button color="primary" onClick={this.playNext}>
        Next
      </Button>
    ) : (
      <Button color="primary" onClick={this.playNext} disabled>
        Next
      </Button>
    );

    const autoPlayButton = (
      <Button color="success" onClick={this.autoPlay}>
        AutoPlay
      </Button>
    );

    const pauseAutoPlayButton = (
      <Button color="warning" onClick={this.pauseAutoPlay}>
        Pause AutoPlay
      </Button>
    );

    const progress = (
      ((this.state.index - 1 > 0 ? this.state.index - 1 : 0) /
        this.props.data.length) *
      100
    ).toFixed(1);

    return (
      <div>
        <div style={{ margin: "2% 10%" }} className="text-center">
          <h6>Replay Progress: {progress}%</h6>
          <Progress color="success" value={progress} />
        </div>
        <Row>
          <Col sm="12">
            <LineChart
              data={this.props.chartData}
              options={this.props.chartOptions}
              width="960"
              height="540"
            />
          </Col>
        </Row>
        Step Size:{" "}
        <NumericInput
          strict
          min={1}
          max={this.props.data.length}
          onChange={this.setStep}
        />{" "}
        {previousButton} {nextButton}
        <br /> <br />
        Autoplay Speed:{" "}
        <NumericInput strict default={2} min={1} max={10} onChange={this.setSpeed} />{" "}
        {autoPlayButton} {pauseAutoPlayButton}
      </div>
    );
  }
}

const mapStateToProps = state => {
  return {
    data: state.homePage.replayCPUData,
    chartData: state.replay.storage.chartData,
    chartOptions: state.replay.storage.chartOptions
  };
};

const mapDispatchToProps = dispatch => {
  return {
    loadDataForReplay: data => dispatch(loadDataForReplay(data)),
    playNext: index => dispatch(playNext(index)),
    playPrevious: index => dispatch(playPrevious(index))
  };
};

const Memory = connect(
  mapStateToProps,
  mapDispatchToProps
)(MemoryComponent);

export default Memory;
