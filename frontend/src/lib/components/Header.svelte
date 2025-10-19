<script lang="ts">
    let { pageName }: { pageName: string } = $props();
</script>

<header class="fixed top-0 left-0 right-0 h-16 bg-card border-b border-border z-50 flex items-center px-6 gap-x-4">
    <img src="/logo.png" alt="Navigator Logo" width="48px" height="48px" class="w-12 h-12" />
    <h1 class="text-2xl font-bold tracking-tight text-primary uppercase">{pageName}</h1>
</header>