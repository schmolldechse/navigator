import type { Config } from "tailwindcss";

export default {
	content: ["./src/**/*.{html,js,svelte,ts}"],

	theme: {
		extend: {
			colors: {
				text: "var(--text)",
				background: "var(--background)",
				primary: "var(--primary)",
				"primary-dark": "var(--primary-dark)",
				"primary-darker": "var(--primary-darker)",
				secondary: "var(--secondary)",
				accent: "var(--accent)"
			},
			fontFamily: {
				montserrat: ["Montserrat", "sans-serif"]
			},
			fontWeight: {}
		}
	},

	plugins: []
} satisfies Config;
