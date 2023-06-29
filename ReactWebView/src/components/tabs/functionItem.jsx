import React, { Component } from "react";

import "react-accessible-accordion/dist/fancy-example.css";
import InvocationRecord from "./invocationRecord";
import OverviewInvocationRecord from "./overviewInvocationRecord";

class FunctionItem extends Component {
  render() {
    var functionItem = this.props.data;
    var functionActivationRecords = functionItem.functionActivationRecords;
    var invocations = {};
    if (functionActivationRecords) {
      invocations = functionItem.functionActivationRecords.map(record => (
        <InvocationRecord record={record} />
      ));
    }
    var overview = functionItem;
    var overviewInvocationRecord = overview.overviewInvocationDetails;
    return (
      <React.Fragment>
        <tr>
          <OverviewInvocationRecord overview={functionItem} invocations={invocations} />
        </tr>
      </React.Fragment>
    );
  }
}

export default FunctionItem;
