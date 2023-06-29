import * as ACTION_TYPES from "../constants/actions";
import * as CONSTANTS from "../constants/strings";
let values = [12, 13.5, 11, 15.5, 25.3, 12.7, 11];
let index = 0;
let labels = [];
var initialState = {
  processedData: [],
  labels: [],
  storage: {
    chartData: {
      labels: [],
      datasets: [
        {
          label: "My Second dataset",
          fillColor: "rgba(80,245,10,0.2)",
          strokeColor: "rgba(80,245,10,1)",
          pointColor: "rgba(80,245,10,1)",
          pointStrokeColor: "#fff",
          pointHighlightFill: "#fff",
          pointHighlightStroke: "rgba(80,245,10,1)",
          data: []
        }
      ]
    },
    chartOptions: {}
  },
  tabs: {
    activeTab: CONSTANTS.REPLAY_CPU
  }
};

let arrayData = [];

function splitForReplay(data) {
    arrayData = data;
}

function getNextIndex(index, state) {
  if (index < state.processedData.length) {
    return index + 1;
  }
  return index;
}

function getPrevIndex(index) {
  if (index > 0) {
    index = index - 1;
  }
  return index;
}

export default function replayReducer(state = initialState, action) {
  var newState = { ...state };
  switch (action.type) {
    case ACTION_TYPES.LOAD_DATA_FOR_REPLAY:
      splitForReplay(action.payload);  
      return state;

    case ACTION_TYPES.PLAY_NEXT:
      var nextIndex = getNextIndex(action.payload, state);
      return {
        ...state,
        storage: {
          ...state.storage,
          chartData: {
            datasets: [
              {
                ...state.storage.chartData.datasets[0],
                data: [0].concat(arrayData.slice(0, nextIndex))
              }
            ],
            labels: [0].concat(arrayData.slice(0, nextIndex)).map(item => Math.floor(item))
          }
        }
      };

    case ACTION_TYPES.PLAY_PREVIOUS:
      const prevIndex = getPrevIndex(action.payload) - 1;
      return {
        ...state,
        storage: {
          ...state.storage,
          chartData: {
            datasets: [
              {
                ...state.storage.chartData.datasets[0],
                data: [0].concat(arrayData.slice(0, prevIndex))
              }
            ],
            labels: [0].concat(arrayData.slice(0, prevIndex)).map(item => Math.floor(item))
          }
        }
      };
  }
  return newState;
}
