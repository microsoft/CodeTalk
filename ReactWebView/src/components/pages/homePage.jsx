import React from "react";
import { connect } from "react-redux";
import { TabContent, TabPane, Nav, NavItem, NavLink } from "reactstrap";
import CPUUsage from "../tabs/cpuUsage";
import MemoryUsage from "../tabs/memoryUsage";
import FunctionsList from "../tabs/functionsList";
import AudioChart from "../tools/chart";
import Save from "../tools/save";
import * as CONSTANTS from "../../constants/strings";
import { toggleTab, updateChart, sendMemoryUsage, sendCPUUsage, sendFunctions, sendTimeslice } from "../../actions/homePage";

export class HomePageComponent extends React.Component {
  constructor(props) {
    super(props);
    this.toggle = this.toggle.bind(this);
  }

  componentDidMount(){
    this.Mconnection = new WebSocket('ws://localhost:5667/memory');
    this.Mconnection.onopen = () => {
    }
    this.Mconnection.onmessage = evt => { 
      this.props.sendMemoryUsage(evt.data);
      try{
        this.Tconnection.send(
          JSON.stringify({
            command: "RequestTimeslice"
          })
        )
      }
      catch(e){}
    };

    this.Cconnection = new WebSocket('ws://localhost:5667/cpu');
    this.Cconnection.onopen = () => {
    }
    this.Cconnection.onmessage = evt => { 
      this.props.sendCPUUsage(evt.data);
      try{
        this.Tconnection.send(
          JSON.stringify({
            command: "RequestTimeslice"
          })
        )
      }
      catch(e){}
    };

    this.Tconnection = new WebSocket('ws://localhost:5667/timeslice');
    this.Tconnection.onopen = () => {
    }
    this.Tconnection.onmessage = evt => { 
      this.props.sendTimeslice(evt.data);
    };
  }

  toggle(selectedTab) {
    this.props.toggleTab(selectedTab);
  }

  render() {
    return (
      <div>
        <Save/>
        <Nav tabs role="tablist">
          <NavItem>
            <NavLink
              onClick={() => {
                this.toggle(CONSTANTS.CPU_USAGE);
              }}
            >
              {CONSTANTS.CPU_USAGE}
            </NavLink>
          </NavItem>
          <NavItem>
            <NavLink
              onClick={() => {
                this.toggle(CONSTANTS.MEMORY_USAGE);
              }}
            >
              {CONSTANTS.MEMORY_USAGE}
            </NavLink>
          </NavItem>
          <NavItem>
            <NavLink
              onClick={() => {
                this.toggle(CONSTANTS.FUNCTIONS_LIST);
              }}
            >
              {CONSTANTS.FUNCTIONS_LIST}
            </NavLink>
          </NavItem>
        </Nav>
        <TabContent activeTab={this.props.activeTab}>
          <TabPane tabId={CONSTANTS.CPU_USAGE}>
            <br />
            <CPUUsage />
            <AudioChart />
          </TabPane>
          <TabPane tabId={CONSTANTS.MEMORY_USAGE}>
            <br />
            <MemoryUsage />
          </TabPane>
          <TabPane tabId={CONSTANTS.FUNCTIONS_LIST}>
            <br />
            <FunctionsList />
          </TabPane>
        </TabContent>
      </div>
    );
  }
}

const mapStateToProps = state => {
  return {
    activeTab: state.homePage.tabs.activeTab
  };
};

const mapDispatchToProps = dispatch => {
  return {
    toggleTab: tab => dispatch(toggleTab(tab)),
    updateCharts: () => dispatch(updateChart()),
    sendMemoryUsage: data => dispatch(sendMemoryUsage(data)),
    sendCPUUsage: data => dispatch(sendCPUUsage(data)),
    sendFunctions: data => dispatch(sendFunctions(data)),
    sendTimeslice: data => dispatch(sendTimeslice(data))
  };
};

const HomePage = connect(
  mapStateToProps,
  mapDispatchToProps
)(HomePageComponent);

export default HomePage;
