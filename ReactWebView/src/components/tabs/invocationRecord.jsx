import React, { Component } from 'react';

class InvocationRecord extends Component {

    render() {
        var record = this.props.record;
        return (
            <tr>
                <td>{record.functionLevelResourceConsumptionStatistics.CPUConsumptionStatistics.mean}</td>
                <td>{record.functionLevelResourceConsumptionStatistics.CPUConsumptionStatistics.max}</td>
                <td>{record.functionLevelResourceConsumptionStatistics.CPUConsumptionStatistics.standardDeviation}</td>

                <td>{record.functionLevelResourceConsumptionStatistics.memoryConsumptionStatistics.mean}</td>
                <td>{record.functionLevelResourceConsumptionStatistics.memoryConsumptionStatistics.max}</td>
                <td>{record.functionLevelResourceConsumptionStatistics.memoryConsumptionStatistics.standardDeviation}</td>
            </tr>
        );
    }
}

export default InvocationRecord;