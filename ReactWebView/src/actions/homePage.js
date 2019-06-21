import * as ACTION_TYPES from "../constants/actions";

export function toggleTab(selectedTab) {
    return {
        type: ACTION_TYPES.TOGGLE_TABS,
        selectedTab: selectedTab
    }
}

export function updateChart(){
    return {
        type: ACTION_TYPES.UPDATE_CHARTS
    }
}

export function loadFunctionsList(functionsList){
    return {
        type: ACTION_TYPES.LOAD_FUNCTION_LIST,
        functionsList: functionsList
    }
}

export function sendMemoryUsage(payload){
    return {
        type: ACTION_TYPES.SEND_MEMORY_USAGE,
        payload: payload
    }
}

export function sendCPUUsage(payload){
    return {
        type: ACTION_TYPES.SEND_CPU_USAGE,
        payload: payload
    }
}

export function sendFunctions(payload){
    return {
        type: ACTION_TYPES.SEND_FUNCTIONS,
        payload: payload
    }
}

export function sendTimeslice(payload){
    return {
        type: ACTION_TYPES.SEND_TIMESLICE,
        payload: payload
    }
}


export function save(payload){
    return {
        type: ACTION_TYPES.SAVE,
        payload: payload
    }
}

export function load(payload){
    return {
        type: ACTION_TYPES.LOAD,
        payload: payload
    }
}