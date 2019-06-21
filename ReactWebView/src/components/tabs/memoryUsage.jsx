import React from "react";
import { connect } from "react-redux";
import {
  Row,
  Col,
  InputGroup,
  InputGroupAddon,
  InputGroupText,
  Input,
  Badge
} from "reactstrap";
import NumericInput from "react-numeric-input";
import { Button } from "mdbreact";
import { Link } from "react-router-dom";

var LineChart = require("react-chartjs").Line;
var lastMem = 0;
let controlConnection = new WebSocket("ws://localhost:5667/control");
controlConnection.onopen = () => {
  console.log("control channel");
};

export class MemoryUsageComponent extends React.Component {
  constructor(props) {
    super(props);
    this.alertBasedOnUsage = this.alertBasedOnUsage.bind(this);
    this.setAlert = this.setAlert.bind(this);
    this.pause = this.pause.bind(this);
    this.resume = this.resume.bind(this);
    this.audioFiles = ["1000.wav", "2000.wav", "3000.wav", "4000.wav"];
    this.audio = new Audio();
    this.state = {
      threshold: 0,
      setThreshold: false
    };
  }

  alertBasedOnUsage() {
    let usage = this.props.lastUpdates.Memory_data;
    if (this.state.setThreshold && usage > this.state.threshold) {
      this.audio = new Audio(this.audioFiles[0]);
      this.audio.play();
      if (usage == lastMem) {
        this.audio.pause();
        this.audio.currentTime = 0;
      } else {
        lastMem = usage;
      }
    } else {
      this.audio.pause();
      this.audio.currentTime = 0;
    }
  }

  setAlert(valueAsNumber, valueAsString, el) {
    this.setState({
      threshold: valueAsNumber,
      setThreshold: true
    });
  }

  componentWillUnmount() {}

  componentDidMount() {
    setInterval(this.alertBasedOnUsage, 1000);
  }

  pause() {
    controlConnection.send(
      JSON.stringify({
        command: "pause"
      })
    );
  }

  resume() {
    controlConnection.send(
      JSON.stringify({
        command: "resume"
      })
    );
  }

  render() {
    this.audio.pause();
    return (
      <React.Fragment>
        <h6>Alert Threshold: <NumericInput strict min={0} max={10000} onChange={this.setAlert} /> </h6>
        <br />
        <br />
        <Row>
          <Col sm="12">
            <h6>Memory used in MB, Total: 32GB</h6>
            <LineChart
              data={this.props.chartData}
              options={this.props.chartOptions}
              width="960"
              height="540"
            />
          </Col>
        </Row>
        <Button size="sm" color="warning" onClick={this.pause}>
          Pause
        </Button>{" "}
        <Button size="sm" color="success" onClick={this.resume}>
          Resume
        </Button>{" "}
        <Button size="sm" color="primary" outline>
          <Link to="/replayMemory">Replay</Link>{" "}
        </Button>
      </React.Fragment>
    );
  }
}

const mapStateToProps = state => {
  return {
    chartData: state.homePage.memoryUsage.chartData,
    chartOptions: state.homePage.memoryUsage.chartOptions,
    lastUpdates: state.homePage.lastUpdates
  };
};

const mapDispatchToProps = dispatch => {
  return {};
};

const MemoryUsage = connect(
  mapStateToProps,
  mapDispatchToProps
)(MemoryUsageComponent);

export default MemoryUsage;
