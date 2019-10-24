import './style.scss';
import Clock from './Clock.svelte';

window.clock = {
    init: (tag) => {
        const target = document.getElementsByTagName(tag)[0];
        Clock = new Clock({
            target,
            props: {}
        });
    }
}
