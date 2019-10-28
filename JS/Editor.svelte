<script>
    import Prism from "./prism/prism-core";
    import './prism/prism-celinql';
    import { editorTextStore } from './stores';

    let textarea;
    $: code = Prism.highlight($editorTextStore, Prism.languages.celinql, "CelinQL")
        || "<em style='color: lightgray;'>Enter Command</em>";
    $: textHight = textarea === undefined ? 0
        : $editorTextStore.length > 0 ? textarea.scrollHeight : 0;
</script>

<style>
    .container {
        height: 100%;
        position: relative;
        top: 0;
        overflow: auto;
	}
    textarea,
    code {
        margin: 0px;
        padding: 0px;
        font-size: 20px;
        min-height: 100%;
        border: 0;
        left: 0;
        overflow: visible;
        position: absolute;
    }
    textarea {
        width: 100%;
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
    }
</style>

<div class="container">
    <div>
        <textarea bind:value={$editorTextStore} spellcheck="false" style="height:{textHight}px" bind:this={textarea} class="editor" />
        <pre>
            <code>{@html code}</code>
        </pre>
    </div>
</div>
