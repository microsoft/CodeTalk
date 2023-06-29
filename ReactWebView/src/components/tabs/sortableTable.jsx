import React from 'react';
import { MDBDataTable } from 'mdbreact';

export default class DatatablePage extends React.Component {
  constructor(props){
    super(props);
  }
    render(){
      var functions = Object.keys(this.props.functions).map(key => ( this.props.functions[key] )) || [];
      var rows = functions.map(item => (
        {
          fname: item.name,
          numInvocations: item.invocationCounter,
          meanCPU: item.overviewInvocationDetails.functionLevelResourceConsumptionStatistics.CPUConsumptionStatistics.mean.toFixed(2),
          peakCPU: item.overviewInvocationDetails.functionLevelResourceConsumptionStatistics.CPUConsumptionStatistics.max.toFixed(2),
          stddevCPU: isNaN(item.overviewInvocationDetails.functionLevelResourceConsumptionStatistics.CPUConsumptionStatistics.standardDeviation) ? NaN : 
          item.overviewInvocationDetails.functionLevelResourceConsumptionStatistics.CPUConsumptionStatistics.standardDeviation.toFixed(2),
          meanMemory: (item.overviewInvocationDetails.functionLevelResourceConsumptionStatistics.memoryConsumptionStatistics.mean * 0.00000107374).toFixed(2),
          peakMemory: (item.overviewInvocationDetails.functionLevelResourceConsumptionStatistics.memoryConsumptionStatistics.max * 0.00000107374).toFixed(2),
          stddevMemory: isNaN(item.overviewInvocationDetails.functionLevelResourceConsumptionStatistics.memoryConsumptionStatistics.standardDeviation) ? NaN : 
          (item.overviewInvocationDetails.functionLevelResourceConsumptionStatistics.CPUConsumptionStatistics.standardDeviation * 0.00000107374).toFixed(2),
      }
      ));

    const columns = [
      {
        label: 'Function Name',
        field: 'fname',
        sort: 'asc',
        width: 150
      },
      {
        label: 'Number of Invocations',
        field: 'numInvocations',
        sort: 'asc',
        width: 150
      },
      {
        label: 'Mean CPU (%)',
        field: 'meanCPU',
        sort: 'asc',
        width: 150
      },
      {
        label: 'Peak CPU (%)',
        field: 'peakCPU',
        sort: 'asc',
        width: 150
      },
      {
        label: 'CPU Std. Dev. (%)',
        field: 'stddevCPU',
        sort: 'asc',
        width: 150
      },
      {
        label: 'Mean Memory (MB)',
        field: 'meanMemory',
        sort: 'asc',
        width: 150
      },
      {
        label: 'Peak Memory (MB)',
        field: 'peakMemory',
        sort: 'asc',
        width: 150
      },
      {
        label: 'Memory Std. Dev. (MB)',
        field: 'stddevMemory',
        sort: 'asc',
        width: 150
      },
    ]
  const data = {
    columns: columns,
    rows: rows
  };

  return (
    <MDBDataTable
      striped
      bordered
      small
      data={data}
      sorting="true"
    />
  );
    };
}
