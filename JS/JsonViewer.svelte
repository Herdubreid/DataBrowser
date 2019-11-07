<script>
    import Prism from "./prism/prism-core";
    import './prism/prism-json';
    import { textMapStore } from './stores';

    export let id;

    $: code = Prism.highlight($textMapStore.get(id), Prism.languages.json, "JSON")
        || "<em style='color: lightgray;'>No Content</em>";

</script>
<style>
    .col {
        overflow: auto;
    }
    code {
        left: 0;
        position: absolute;
        font-family: inherit;
    }
    pre {
        margin: 0px;
        white-space: pre-wrap;
	    word-wrap: break-word;
        font-family: inherit;
    }
</style>

<div class="col">
    <pre>
        <code>{@html code}</code>
    </pre>
</div>
