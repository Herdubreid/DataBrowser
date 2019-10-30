//
import { writable } from 'svelte/store';

const cqlTexts = () => {
    const { subscribe, update } = writable([]);
    return {
        subscribe,
        update
    }
}

const jsonTexts = () => {
    const { subscribe, update } = writable([]);
    return {
        subscribe,
        update
    }
}

export const cqlTextsStore = cqlTexts();
export const jsonTextsStore = jsonTexts();
