import './style.scss';
import { get } from 'svelte/store';
import { cqlTextsStore, jsonTextsStore } from './stores';
import CqlEditor from './CqlEditor.svelte';
import JsonViewer from './JsonViewer.svelte';

let cqlEditors = [];
window.cqlEditor = {
    init: (id, text) => {
        const target = document.getElementById(id);
        let index = cqlEditors.length;
        cqlTextsStore.update(t => {
            t.push(text);
            return t;
        });
        cqlEditors.push(new CqlEditor({
            target,
            props: {
                index
            }
        }));
        return index;
    },
    clear: (index) => {
        cqlTextsStore.update(t => {
            t[index] = "";
            return t;
        });
        editor.$destroy();
    },
    getText: (index) => {
        return get(cqlTextsStore)[index];
    },
    setText: (index, text) => {
        cqlTextsStore.update(t => {
            t[index] = text;
            return t;
        });
    }
}

let jsonViewers = [];
window.jsonViewer = {
    init: (id, text) => {
        const target = document.getElementById(id);
        let index = jsonViewers.length;
        jsonTextsStore.update(t => {
            t.push(text);
            return t;
        });
        jsonViewers = new JsonViewer({
            target,
            props: {
                index
            }
        });
        return index;
    },
    clear: (index) => {
        jsonTextsStore.update(t => {
            t[index] = "";
            return t;
        });
        jsonViewers[index].$destroy();
    },
    setText: (index, text) => {
        jsonTextsStore.update(t => {
            t[index] = text;
            return t;
        });
    }
}
