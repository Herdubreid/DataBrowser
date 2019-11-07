//
import { writable } from 'svelte/store';

const textMap = () => {
    const {subscribe,update} = writable(new Map())
    return {
        subscribe,
        update
    };
}

export const textMapStore = textMap();
