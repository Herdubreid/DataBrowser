import './style.scss';
import { get } from 'svelte/store';
import { textMapStore } from './stores';
import CqlEditor from './CqlEditor.svelte';
import JsonViewer from './JsonViewer.svelte';

const cqlEditors = new Map();
const cqlEditorId = (id) => `${id}-edit`;
window.cqlEditor = {
    init: (id, text) => {
        const target = document.getElementById(cqlEditorId(id));
        textMapStore.update(m => {
            m.set(cqlEditorId(id), text);
            return m;
        });
        cqlEditors.set(id, new CqlEditor({
            target,
            props: {
                id: cqlEditorId(id)
            }
        }));
    },
    clear: (id) => {
        get(textMapStore).delete(cqlEditorId(id));
        cqlEditors.get(id).$destroy();
    },
    getText: (id) => {
        var m = get(textMapStore);
        return m.get(cqlEditorId(id));
    },
    setText: (id, text) => {
        textMapStore.update(m => {
            m.set(cqlEditorId(id), text);
            return m;
        });
    }
}

const jsonViewers = new Map();
const jsonViewerId = (id) => `${id}-view`;
window.jsonViewer = {
    init: (id, text) => {
        const target = document.getElementById(jsonViewerId(id));
        textMapStore.update(m => {
            m.set(jsonViewerId(id), text);
            return m;
        });
        jsonViewers.set(id, new JsonViewer({
            target,
            props: {
                id: jsonViewerId(id)
            }
        }));
    },
    clear: (id) => {
        get(textMapStore).delete(jsonViewer(id));
        jsonViewers.get(id).$destroy();
    },
    setText: (id, text) => {
        textMapStore.update(m => {
            m.set(jsonViewerId(id), text);
            return m;
        });
    }
}
