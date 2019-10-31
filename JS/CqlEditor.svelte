<script>
    import Prism from "./prism/prism-core";
    import './prism/prism-celinql';
    import { cqlTextsStore } from './stores';

    export let index = 0;

    let textareaEl;
    let codeEl;
    
    $: code = Prism.highlight($cqlTextsStore[index], Prism.languages.celinql, "CelinQL")
        || "<em style='color: lightgray;'>Enter Command</em>";
    
    $: textHeight = $cqlTextsStore[index].length > 0
        ? textareaEl.scrollHeight > textareaEl.clientHeight
            ? `${textareaEl.scrollHeight}px`
            : (codeEl.clientHeight + 24) < textareaEl.clientHeight
                ? `${codeEl.clientHeight}px`
                : `${textareaEl.clientHeight}px`
        : '1px';
</script>

<style>
    .col {
        overflow: auto;
	}
    textarea,
    code {
        margin: 0px;
        padding: 0px;
        border: 0;
        left: 0;
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
        <textarea bind:value={$cqlTextsStore[index]} spellcheck="false" style="height: {textHeight}" bind:this={textareaEl} class="editor" />
        <pre>
            <code bind:this={codeEl}>{@html code}</code>
        </pre>
    </div>
</div>
