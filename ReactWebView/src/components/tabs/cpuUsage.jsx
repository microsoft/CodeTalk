import React from "react";
import { connect } from "react-redux";
import { Row, Col, Button } from "reactstrap";

var LineChart = require("react-chartjs").Line;

export class CPUUsageComponent extends React.Component {
  constructor(props) {
    super(props);
    this.alertBasedOnUsage = this.alertBasedOnUsage.bind(this);
  }

  alertBasedOnUsage(){
    let usage = this.props.lastUpdates.CPU_data;
    console.log("cpu :", usage);
  }

  componentDidMount(){
    setInterval(this.alertBasedOnUsage, 2000);
  }

  render() {
    return (
      <React.Fragment>
      <Row>
        <Col sm="12">
          <p> % CPU Usage</p>
          <LineChart
            data={this.props.chartData}
            options={this.props.chartOptions}
            width="960"
            height="540"
          />
        </Col>
      </Row>
      </React.Fragment>
    );
  }
}

const mapStateToProps = state => {
  return {
    chartData: state.homePage.cpuUsage.chartData,
    chartOptions: state.homePage.cpuUsage.chartOptions,
    lastUpdates: state.homePage.lastUpdates
  };
};

const mapDispatchToProps = dispatch => {
  return {};
};

const CPUUsage = connect(
  mapStateToProps,
  mapDispatchToProps
)(CPUUsageComponent);

export default CPUUsage;
