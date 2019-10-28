import './style.scss';
import { get } from 'svelte/store';
import { editorTextStore } from './stores';
import Clock from './Clock.svelte';
import Editor from './Editor.svelte';

window.clock = {
    init: (tag) => {
        const target = document.getElementsByTagName(tag)[0];
        clock = new Clock({
            target,
            props: {}
        });
    }
}
let CurrentEditor;
window.editor = {
    init: (tag) => {
        const target = document.getElementsByTagName(tag)[0];
        CurrentEditor = new Editor({
            target,
            props: {}
        });
    },
    clear: () => {
        CurrentEditor.$destroy();
    },
    getText: () => {
        return get(editorTextStore);
    },
    setText: (text) => {
        editorTextStore.set(text);
    }
}
