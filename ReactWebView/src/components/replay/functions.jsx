import React from "react";
import { connect } from "react-redux";
import { Table, Button } from "reactstrap";
import FunctionItem from "../tabs/functionItem";
import DatatablePage from "../tabs/sortableTable";

class FunctionsComponent extends React.Component {
  constructor(props) {
    super(props);
    this.onRequestFunctionDetails = this.onRequestFunctionDetails.bind(this);
    this.state = {
      functionsData: []
    };
  }

  componentDidMount() {
    let controlConnection = new WebSocket("ws://localhost:5667/control");
    controlConnection.onopen = () => {
      console.log("FF");
    };
    controlConnection.onmessage = evt => {
      this.setState({
        functionsData: JSON.parse(evt.data)
      });
    };

    this.setState({
      controlConnection: controlConnection
    });
  }

  onRequestFunctionDetails() {
    console.log("sending request");
    this.state.controlConnection.send(
      JSON.stringify({
        command: "RequestFunctionDetails",
        payload: this.props.webSocketData
      })
    );
  }

  render() {
    let data = this.state.functionsData;
    let functions = Object.keys(data).map(key => (
      <DatatablePage functions={data[key]}/>
    ));
    return (
      <div>
        <br />
        <br />
        <font size="2" face="Consolas">
          <DatatablePage functions={this.state.functionsData}/>
        </font>
      </div>
    );
  }
}

const mapStateToProps = state => {
  return {
    functionsLists: state.homePage.functionsList,
    functionsData: state.homePage.functionsData,
    webSocketData: JSON.stringify(state.homePage.webSocketData)
  };
};

const mapDispatchToProps = dispatch => {
  return {};
};

const Functions = connect(
  mapStateToProps,
  mapDispatchToProps
)(FunctionsComponent);

export default Functions;
