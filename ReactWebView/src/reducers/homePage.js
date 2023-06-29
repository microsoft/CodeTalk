import * as ACTION_TYPES from "../constants/actions";
import * as CONSTANTS from "../constants/strings";
import { functionsList } from "../assets/csvjson";

let CPU_data = [2, 4, 4, 1, 8, 2, 0];
let Memory_data = [12, 13.5, 11, 15.5, 25.3, 12.7, 11];
let labels = ["a", "a", "a", "a", "a", "a", "a"];

let CPU_update = 0;
let Memory_update = 0;

const initialState = {
  webSocketData: {
    CPUConsumptionTimeSeries: {},
    memoryConsumptionTimeSeries: {},
    timeslice: {}
  },
  lastUpdates: {
    CPU_data: 0,
    Memory_data: 0
  },
  cpuUsage: {
    chartData: {
      labels: labels,
      datasets: [
        {
          label: "Second Process",
          fillColor: "rgba(80,245,10,0.2)",
          strokeColor: "rgba(80,245,10,1)",
          pointColor: "rgba(80,245,10,1)",
          pointStrokeColor: "#fff",
          pointHighlightFill: "#fff",
          pointHighlightStroke: "rgba(80,245,10,1)",
          data: CPU_data
        }
      ]
    },
    chartOptions: {}
  },
  memoryUsage: {
    chartData: {
      labels: labels,
      datasets: [
        {
          label: "My Second dataset",
          fillColor: "rgba(80,245,10,0.2)",
          strokeColor: "rgba(80,245,10,1)",
          pointColor: "rgba(80,245,10,1)",
          pointStrokeColor: "#fff",
          pointHighlightFill: "#fff",
          pointHighlightStroke: "rgba(80,245,10,1)",
          data: Memory_data
        }
      ]
    },
    chartOptions: {}
  },

  tabs: {
    activeTab: CONSTANTS.MEMORY_USAGE
  },

  functionsList: functionsList,

  functionsData: {},

  alerts: {
    cpu: {},
    memory: {}
  },

  replayMemoryData: Memory_data,
  replayCPUData: CPU_data
};

export function updateCPUUsage(data) {
  if (data == "VS Joined") {
    return;
  }
  const dict = JSON.parse(data)["p"];
  CPU_data = [0];
  CPU_data.push.apply(
    CPU_data,
    Object.keys(dict).map(function(k) {
      return dict[k]["v"];
    })
  );
  CPU_update = CPU_data[CPU_data.length - 1];
}

export function updateMemoryUsage(data) {
  if (data == "VS Joined") {
    return;
  }
  const dict = JSON.parse(data)["p"];
  Memory_data = [0];
  Memory_data.push.apply(
    Memory_data,
    Object.keys(dict).map(function(k) {
      return dict[k]["v"] * 0.00000107374;
    })
  );
  Memory_update = Memory_data[Memory_data.length - 1];
}

export function download(content, fileName, contentType) {
  var a = document.createElement("a");
  var file = new Blob([content], {type: contentType});
  a.href = URL.createObjectURL(file);
  a.download = fileName;
  a.click();
}

export default function homePageReducer(state = initialState, action) {
  switch (action.type) {
    case ACTION_TYPES.TOGGLE_TABS:
      return {
        ...state,
        tabs: {
          ...state.tabs,
          activeTab: action.selectedTab
        }
      };
    case ACTION_TYPES.LOAD_FUNCTION_LIST:
      return {
        ...state,
        functionsList: action.functionsList
      };
    case ACTION_TYPES.SEND_MEMORY_USAGE:
      updateMemoryUsage(action.payload);
      return {
        ...state,
        webSocketData: {
          ...state.webSocketData,
          memoryConsumptionTimeSeries: JSON.parse(action.payload)["p"]
        },
        memoryUsage: {
          chartData: {
            labels: labels,
            datasets: [
              {
                data: Memory_data
              }
            ]
          },
          chartOptions: {}
        },
        lastUpdates: {
          ...state.lastUpdates,
          Memory_data: Memory_update
        },
        replayMemoryData: Memory_data
      };
    case ACTION_TYPES.SEND_CPU_USAGE:
      updateCPUUsage(action.payload);
      return {
        ...state,
        webSocketData: {
          ...state.webSocketData,
          CPUConsumptionTimeSeries: JSON.parse(action.payload)["p"]
        },
        cpuUsage: {
          chartData: {
            labels: labels,
            datasets: [
              {
                data: CPU_data
              }
            ]
          },
          chartOptions: {}
        },
        lastUpdates: {
          ...state.lastUpdates,
          CPU_data: CPU_update
        }
      };
    case ACTION_TYPES.SEND_TIMESLICE:
      return {
        ...state,
        webSocketData: {
          ...state.webSocketData,
        timeslice: JSON.parse(action.payload)
        }
      }
    case ACTION_TYPES.SEND_FUNCTIONS:
      return {
        ...state,
        functionsData: JSON.parse(action.payload)
      };
    case ACTION_TYPES.SET_CPU_ALERTS:
      return {
        ...state,
        alerts: {
          ...state.alerts,
          cpu: action.payload
        }
      };
      break;
    case ACTION_TYPES.SET_MEMORY_ALERTS:
      return {
        ...state,
        alerts: {
          ...state.alerts,
          memory: action.payload
        }
      };
      break;
    case ACTION_TYPES.PLAY_CPU_ALERT:
      break;
    case ACTION_TYPES.PLAY_MEMORY_ALERT:
      break;
    case ACTION_TYPES.SAVE:
      const currentStateJSON = JSON.stringify(state);
      const fileName = action.payload.concat('.json');
      download(currentStateJSON, fileName, 'text/json');
      return state;
    case ACTION_TYPES.LOAD:
      return {
        ...action.payload,
        tabs: {
          activeTab: state.tabs.activeTab
        }
      };
  }

  return state;
}
