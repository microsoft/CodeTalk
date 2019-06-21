import React, { Component } from "react";
import { Button, Modal, ModalHeader, ModalBody, ModalFooter, Table } from 'reactstrap';

class OverviewInvocationRecord extends Component {
    constructor(props) {
        super(props);
        this.state = {
          modal: false
        };
    
        this.toggle = this.toggle.bind(this);
      }
    
      toggle() {
        this.setState(prevState => ({
          modal: !prevState.modal
        }));
      }
      
  render() {
    var overview = this.props.overview;
    var overviewInvocationRecord = overview.overviewInvocationDetails;
    return (
      <React.Fragment>
        <td> {overview.name}</td>
        <td> 
        <Button size="sm" color="primary" onClick={this.toggle}>{overview.invocationCounter}</Button>
        <Modal size="lg" style={{width: "90vw"}} isOpen={this.state.modal} toggle={this.toggle} className={this.props.className}>
          <ModalHeader toggle={this.toggle}>{overview.name} </ModalHeader>
          <ModalBody>
          <Table size="sm" hover striped >
            <thead>
              <tr>
                <th colspan="3">CPU Usage</th>
                <th colspan="3">Memory Usage</th>
              </tr>
              <tr>
                <th>Mean</th>
                <th>Peak</th>
                <th>Std. Dev.</th>
                <th>Mean</th>
                <th>Peak</th>
                <th>Std. Dev.</th>
              </tr>
            </thead>
            <tbody>
              {this.props.invocations}
            </tbody>
          </Table>
          </ModalBody>
          <ModalFooter>
            <Button color="secondary" onClick={this.toggle}>Close</Button>
          </ModalFooter>
        </Modal>
        </td>
        <td>
          {
            overviewInvocationRecord.functionLevelResourceConsumptionStatistics
              .CPUConsumptionStatistics.mean
          }
        </td>
        <td>
          {
            overviewInvocationRecord.functionLevelResourceConsumptionStatistics
              .CPUConsumptionStatistics.max
          }
        </td>
        <td>
          {
            overviewInvocationRecord.functionLevelResourceConsumptionStatistics
              .CPUConsumptionStatistics.standardDeviation
          }
        </td>

        <td>
          {
            overviewInvocationRecord.functionLevelResourceConsumptionStatistics
              .memoryConsumptionStatistics.mean
          }
        </td>
        <td>
          {
            overviewInvocationRecord.functionLevelResourceConsumptionStatistics
              .memoryConsumptionStatistics.max
          }
        </td>
        <td>
          {
            overviewInvocationRecord.functionLevelResourceConsumptionStatistics
              .memoryConsumptionStatistics.standardDeviation
          }
        </td>
      </React.Fragment>
    );
  }
}

export default OverviewInvocationRecord;
