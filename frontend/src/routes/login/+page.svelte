<svelte:head>
    <title>Navigator - Login</title>
</svelte:head>

<div class="flex h-screen flex-col items-center justify-center">
    <button onclick={() => (window.location.href = 'http://localhost:8080/api/v1/auth/login/github')}>Login</button>
</div>