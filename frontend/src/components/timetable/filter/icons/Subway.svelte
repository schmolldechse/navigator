<script lang="ts">
	let {width = "25px", height = "25px"} = $props();
</script>

<style>
    .cls-1 {
        @apply fill-[#2C68DA];
    }

    .cls-2 {
        @apply fill-text stroke-text;
        stroke-width: .656px;
    }
</style>

<svg {width} {height} xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
    <path class="cls-1" d="M29 3.5a.5.5 0 0 0-.5-.5h-25a.5.5 0 0 0-.5.5v25a.5.5 0 0 0 .5.5h25a.5.5 0 0 0 .5-.5z"/>
    <path class="cls-2"
          d="M9.25 6.78h-.282v11.556c0 2.215.692 3.916 1.94 5.06 1.245 1.142 3.005 1.697 5.079 1.697s3.84-.555 5.09-1.696c1.255-1.144 1.953-2.845 1.953-5.061V6.78h-3.434v10.593c0 1.139-.045 2.288-.513 3.154a2.44 2.44 0 0 1-1.045 1.034c-.489.255-1.15.413-2.051.413-.904 0-1.565-.158-2.052-.413a2.4 2.4 0 0 1-1.036-1.032c-.46-.867-.497-2.015-.497-3.156V6.78z"/>
</svg>