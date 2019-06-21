import * as ACTION_TYPES from "../constants/actions";

export function loadDataForReplay(data) {
    return {
        type: ACTION_TYPES.LOAD_DATA_FOR_REPLAY,
        payload: data
    }
}

export function playNext(index){
    return {
        type: ACTION_TYPES.PLAY_NEXT,
        payload: index
    }
}

export function playPrevious(index){
    return {
        type: ACTION_TYPES.PLAY_PREVIOUS,
        payload: index
    }
}