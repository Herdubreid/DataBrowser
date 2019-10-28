//
import { writable } from 'svelte/store';

const editorText = () => {
    const { subscribe, set } = writable('');
    return {
        subscribe,
        set
    }
}

export const editorTextStore = editorText();
