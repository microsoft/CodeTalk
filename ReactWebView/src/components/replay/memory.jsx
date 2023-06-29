import React from "react";
import { connect } from "react-redux";
import { Row, Col, Button, Progress } from "reactstrap";
import NumericInput from "react-numeric-input";
import DatatablePage from "../tabs/sortableTable";
import {
  loadDataForReplay,
  playNext,
  playPrevious
} from "../../actions/replay";
import "../../index.css";
import { Alert } from "mdbreact";
import Save from "../tools/save";

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
    this.reset = this.reset.bind(this);
    this.onDismiss = this.onDismiss.bind(this);
    this.onRequestFunctionDetails = this.onRequestFunctionDetails.bind(this);
    this.state = {
      index: 0,
      nextButton: true,
      previousButton: false,
      autoPlayButton: true,
      step: 1,
      speed: 5,
      functionsData: [],
      alert: { show: false, text: "Fetching statistics", type: "success" }
    };
  }

  componentDidMount() {
    this.props.loadDataForReplay(this.props.data);
    this.playNext();

    let controlConnection = new WebSocket("ws://localhost:5667/control");
    controlConnection.onopen = () => {
      console.log("ReplaySocket");
    };

    controlConnection.onmessage = evt => {
      console.log(JSON.parse(evt.data));
      this.setState({
        functionsData: JSON.parse(evt.data)
      });
    };

    this.setState({
      controlConnection: controlConnection
    });
  }

  onDismiss() {
    this.setState({ alert: { show: false } });
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

  reset() {
    this.pauseAutoPlay();
    this.setState(
      {
        index: 0,
        nextButton: true
      },
      () => this.playNext()
    );
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
      if (
        this.state.index > this.state.step &&
        this.state.index <= this.props.data.length
      ) {
        this.setState({
          previousButton: true,
          nextButton: true
        });
      }
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
      intervalId: intervalId,
      pauseAutoPlayButton: true,
      autoPlayButton: false
    });
  }

  pauseAutoPlay() {
    clearInterval(this.state.intervalId);
    this.setState({
      autoPlayButton: true,
      pauseAutoPlayButton: false
    });
  }

  onRequestFunctionDetails() {
    try {
      var webSocketDataLocal = JSON.parse(
        JSON.stringify(this.props.webSocketData)
      );
      var memoryConsumptionTimeSeries =
        webSocketDataLocal["memoryConsumptionTimeSeries"];

      webSocketDataLocal[
        "memoryConsumptionTimeSeries"
      ] = memoryConsumptionTimeSeries.slice(0, this.state.index);
      this.state.controlConnection.send(
        JSON.stringify({
          command: "RequestFunctionDetails",
          payload: JSON.stringify(webSocketDataLocal)
        })
      );
      this.setState({
        alert: {}
      });
    } catch (e) {
      this.setState({
        alert: {
          show: true,
          text: "An error occured while fetching Function Statistics.",
          type: "danger"
        }
      });
    }
  }

  render() {
    const previousButton = this.state.previousButton ? (
      <Button
        className="codeTalkButton"
        size="sm"
        color="primary"
        onClick={this.playPrevious}
      >
        Previous
      </Button>
    ) : (
      <Button
        className="codeTalkButton"
        size="sm"
        color="primary"
        onClick={this.playPrevious}
        disabled
      >
        Previous
      </Button>
    );

    const nextButton = this.state.nextButton ? (
      <Button
        className="codeTalkButton"
        size="sm"
        color="primary"
        onClick={this.playNext}
      >
        Next
      </Button>
    ) : (
      <Button
        className="codeTalkButton"
        size="sm"
        color="primary"
        onClick={this.playNext}
        disabled
      >
        Next
      </Button>
    );

    const autoPlayButton =
      this.state.autoPlayButton &&
      this.state.index <= this.props.data.length ? (
        <Button
          className="codeTalkButton"
          size="sm"
          color="success"
          onClick={this.autoPlay}
        >
          AutoPlay
        </Button>
      ) : (
        ""
      );

    const pauseAutoPlayButton = this.state.pauseAutoPlayButton ? (
      <Button
        className="codeTalkButton"
        size="sm"
        color="warning"
        onClick={this.pauseAutoPlay}
      >
        Pause
      </Button>
    ) : (
      ""
    );

    const resetButton = (
      <Button
        className="codeTalkButton"
        size="sm"
        color="danger"
        onClick={this.reset}
      >
        Reset
      </Button>
    );

    const rf = (
      <Button
        className="codeTalkButton"
        size="sm"
        color="primary"
        block
        onClick={() => this.onRequestFunctionDetails()}
      >
        Request Function Statistics
      </Button>
    );

    const progress = (
      ((this.state.index - 1 > 0 ? this.state.index - 1 : 0) /
        this.props.data.length) *
      100
    ).toFixed(1);

    const alert = this.state.alert.show ? (
      <Alert
        className="alert"
        color={this.state.alert.type}
        isOpen={this.state.alert.show}
        toggle={this.onDismiss}
      >
        {this.state.alert.text}
      </Alert>
    ) : (
      ""
    );
    return (
      <div>
        <Save />
        <div className="horizontalDiv">
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
              value={this.state.step}
              onChange={this.setStep}
            />{" "}
            {previousButton} {nextButton}
            <br /> <br />
            Autoplay Speed:{" "}
            <NumericInput
              strict
              default={2}
              min={1}
              max={10}
              value={this.state.speed}
              onChange={this.setSpeed}
            />{" "}
            {autoPlayButton} {pauseAutoPlayButton} {resetButton}
          </div>
          <div style={{ margin: "2% 0%" }}>
            {rf} {alert}
            <font size="2" face="Consolas">
              <DatatablePage functions={this.state.functionsData} />
            </font>
          </div>
        </div>
      </div>
    );
  }
}

const mapStateToProps = state => {
  return {
    data: state.homePage.replayMemoryData,
    chartData: state.replay.storage.chartData,
    chartOptions: state.replay.storage.chartOptions,
    webSocketData: state.homePage.webSocketData
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
