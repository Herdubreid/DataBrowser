<script>
    import { onMount } from 'svelte';
    import Prism from "./prism/prism-core";
    import './prism/prism-celinql';
    import { textMapStore } from './stores';

    export let caller;
    export let id;

    let editId = `${id}-edit`;
    let textareaEl;
    let codeEl;
    let mounted = false;

    let text = $textMapStore.get(editId);
    
    $: code = Prism.highlight(text, Prism.languages.celinql, "CelinQL")
        || "<em style='color: lightgray;'>Enter Command</em>";
    
    $: textHeight = mounted && text.length > 0
        ? textareaEl.scrollHeight > textareaEl.clientHeight
            ? `${textareaEl.scrollHeight}px`
            : (codeEl.clientHeight + 24) < textareaEl.clientHeight
                ? `${codeEl.clientHeight}px`
                : `${textareaEl.clientHeight}px`
        : '1px';

    const setText = () => {
        $textMapStore.set(editId, text);
        caller.invokeMethodAsync("TextChanged", id, text);
    };

    onMount(() => {
        mounted = true;
    });
</script>

<style>
    div.col {
        overflow-y: auto;
	}
    textarea,
    code {
        margin: 0px;
        padding: 0px;
        border: 0;
        left: 0;
        word-break: break-all;
        white-space: break-spaces;
        overflow: visible;
        position: absolute;
        font-family: inherit;
        font-size: 16px;
    }
    textarea {
        width: 100%;
        min-height: 100%;
        background: transparent !important;
        z-index: 2;
        resize: none;
        -webkit-text-fill-color: transparent;
    }
    textarea:focus {
        outline: 0;
        border: 0;
        box-shadow: none;
    }
    code {
        z-index: 1;
    }
    pre {
        margin: 0px;
        white-space: pre-wrap;
	    word-wrap: break-word;
        font-family: inherit;
    }
</style>

<div class="col">
    <div>
        <textarea bind:value={text} on:blur={setText} spellcheck="false" style="height: {textHeight}" bind:this={textareaEl} class="editor" />
        <pre>
            <code bind:this={codeEl}>{@html code}</code>
        </pre>
    </div>
</div>
