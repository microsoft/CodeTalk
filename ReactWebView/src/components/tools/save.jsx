import React from "react";
import { connect } from "react-redux";
import {
  UncontrolledAlert,
  InputGroup,
  InputGroupAddon,
  Input,
  Button
} from "reactstrap";
import { save, load } from "../../actions/homePage";

var fileData = new FileReader();

class SaveComponent extends React.Component {
  constructor(props) {
    super(props);

    this.toggle = this.toggle.bind(this);
    this.save = this.save.bind(this);
    this.handleChangeFile = this.handleChangeFile.bind(this);
    this.handleFile = this.handleFile.bind(this);
    this.handleChange = this.handleChange.bind(this);
    this.state = {
      dropdownOpen: false,
      value: "",
      alert: {
        show: false
      }
    };
  }

  toggle() {
    this.setState({
      dropdownOpen: !this.state.dropdownOpen
    });
  }

  save() {
    this.props.save(this.state.value);
  }

  handleChange(event) {
    this.setState({ value: event.target.value });
  }

  handleFile = e => {
    const state = JSON.parse(fileData.result);
    this.props.load(state);
  };

  handleChangeFile = file => {
    try {
      fileData.onloadend = this.handleFile;
      fileData.readAsText(file);
    } catch (e) {
      this.setState({
        alert: {
          type: "danger",
          text: "Error occurred in loading the file."
        }
      });
    }
  };

  render() {
    const alert = this.state.alert.show ? (
      <UncontrolledAlert color={this.state.alert.type}>
        {this.state.alert.text}
      </UncontrolledAlert>
    ) : (
      ""
    );
    return (
      <div class="App justify-content-md-center" style={{ margin: "1%" }}>
        <div class="row justify-content-md-center">
          <div class="col-sm-4" style={{ margin: "0.7%" }}>
            <InputGroup>
              <InputGroupAddon addonType="prepend">#</InputGroupAddon>
              <Input
                required
                placeholder="Session Name"
                value={this.state.value}
                onChange={this.handleChange}
              />
              <InputGroupAddon addonType="append">
                <Button color="primary" onClick={this.save}>
                  Save
                </Button>
              </InputGroupAddon>
            </InputGroup>
          </div>
          <div
            class="col"
            style={{ display: "table-cell", verticalAlign: "middle" }}
          >
              <Button className="codeTalkButton" color="primary">Load</Button>{' '}
              <input
                type="file"
                accept=".json"
                onChange={e => this.handleChangeFile(e.target.files[0])}
                style={{ padding: "2%" }}
              />
          </div>
        </div>
      </div>
    );
  }
}

const mapStateToProps = state => {
  return {
    chartData: state.homePage.cpuUsage.chartData,
    chartOptions: state.homePage.cpuUsage.chartOptions
  };
};

const mapDispatchToProps = dispatch => {
  return {
    save: revision => dispatch(save(revision)),
    load: revision => dispatch(load(revision))
  };
};

const Save = connect(
  mapStateToProps,
  mapDispatchToProps
)(SaveComponent);

export default Save;
