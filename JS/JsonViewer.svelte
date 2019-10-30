<script>
    import Prism from "./prism/prism-core";
    import './prism/prism-json';
    import { jsonTextsStore } from './stores';

    export let index = 0;

    $: code = Prism.highlight($jsonTextsStore[index], Prism.languages.json, "JSON");

</script>
<style>
    pre {
        margin: 0px;
        white-space: pre-wrap;
	    word-wrap: break-word;
        font-family: inherit;
    }
</style>

<pre>
    <code>{@html code}</code>
</pre>
