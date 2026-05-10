---
name: reusable-ui-components
description: Create or revise generic reusable Svelte UI components for Navigator under frontend/src/lib/components/ui. Use when Codex needs to add UI primitives, compound UI components with child components/context, shared component APIs, styling conventions, accessibility behavior, or exports for the reusable UI library.
---

# Reusable UI Components

## Scope

Use this skill for generic reusable UI components that belong in `frontend/src/lib/components/ui`. Keep components domain-agnostic: they may support dashboard, map, form, dialog, chart, and navigation use cases, but should not know about specific Navigator entities, routes, statistics, or API DTOs.

## Placement

- Place standalone primitives directly in `frontend/src/lib/components/ui` as `PascalCase.svelte`.
- Place components with related child components, context, helpers, or barrel exports in `frontend/src/lib/components/ui/<component-name>/`.
- Name compound component folders in lowercase kebab-case, matching the public component concept, for example `accordion`, `toggle-group`, or `tooltip`.
- Use `PascalCase.svelte` for component files and `<component-name>-context.svelte.ts` for Svelte context modules.
- Add an `index.ts` barrel for compound components when callers should import a component family from one folder. Prefer existing local aliases such as `Root`, `Item`, `Trigger`, and `Content` when they fit.

## Component Shape

- Use Svelte 5 runes and TypeScript.
- Define a local `type Props = { ... }` near the top of each component.
- Read props with `$props()` and use `$bindable()` for values that the parent should be able to bind.
- Use `Snippet` for child content and named render areas. Type snippet arguments when children receive state such as `{ isOpen, isDisabled }`.
- Accept `class?: ClassValue` when styling extension is useful, and merge classes with Svelte class arrays.
- Use `script module lang="ts"` to export component-specific public types from a Svelte file when that keeps the API close to the component.
- Prefer clear callback props such as `onchange`, `onselect`, `onclose`, or `onvaluechange` for meaningful state transitions.
- Keep component state minimal and derive display state with `$derived` or `$derived.by` when possible.

## Compound Components

- Use Svelte context for shared state across child components.
- Store context logic in a sibling `*-context.svelte.ts` module.
- Use a unique `Symbol(...)` as the context key.
- Model context as a class when it owns behavior, registration, or derived state.
- Pass parent props into context through getters/setters so bound parent state remains authoritative.
- Throw explicit usage errors in child components when required parent context is missing.
- Register and unregister child items with `$effect` and `untrack` when the parent context tracks available items.
- Generate stable DOM ids from `$props.id()` on the root and safe item identifiers when child components need `aria-controls` or `aria-labelledby`.

## Styling

- Use Tailwind utility classes and existing semantic tokens such as `bg-background`, `bg-secondary`, `text-foreground`, `text-muted-foreground`, `border-border`, `bg-accent`, and `text-accent-foreground`.
- Prefer state-driven class arrays over string concatenation.
- Expose narrow class override props only where callers need them, for example `class`, `barClass`, `legendClass`, or `iconClass`.
- Include disabled, hover, active, selected, loading, and empty states when they are natural for the component.
- Use `data-*` attributes such as `data-state`, `data-disabled`, or `data-active` for state that styling or tests may need.
- Keep layout dimensions stable for repeated UI elements such as buttons, grids, bars, skeletons, icons, and counters.
- Use `@lucide/svelte` icons for familiar UI actions instead of custom inline SVGs.

## Accessibility

- Preserve native semantics first: use `button`, `input`, `dialog`, and semantic attributes before custom roles.
- Add ARIA only when it describes real relationships or state, such as `aria-expanded`, `aria-controls`, `aria-labelledby`, `aria-label`, `role="region"`, or `role="img"`.
- Ensure disabled state prevents interaction and communicates disabled styling.
- Keep keyboard and focus behavior intact when wrapping native controls.
- Avoid hiding meaningful content behind visual-only affordances; use labels, titles, or ARIA labels where the component would otherwise be ambiguous.

## API Design

- Keep reusable component props generic and composable. Prefer data, state, snippets, formatting callbacks, and visual variants over domain-specific nouns.
- Prefer small variant unions for visual modes, for example `primary | secondary | destructive | tertiary`.
- Use generic type parameters for reusable selection components when callers provide item shapes.
- Keep formatting concerns configurable with formatter callbacks instead of hard-coding display strings.
- Avoid introducing new abstractions unless they reduce duplication across multiple UI primitives or match an existing pattern in `ui`.

## Workflow

1. Inspect nearby components in `frontend/src/lib/components/ui` before adding or revising a component.
2. Decide whether the component is a standalone primitive or a compound component family.
3. Shape the public API first: props, bindable values, snippets, callbacks, and exported types.
4. Implement behavior with native elements, Svelte runes, and context only when needed.
5. Apply semantic Tailwind tokens and state classes that fit the existing UI library.
6. Add or update compound `index.ts` exports when the component family should be imported as a grouped API.
7. Run the repository's relevant frontend validation command when available, such as typecheck, lint, test, or build.
