import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// https://astro.build/config
export default defineConfig({
	integrations: [
		starlight({
			title: 'CounterStrikeSharp Docs',
			customCss: [
				'@fontsource/dm-sans/400.css',
				'@fontsource/dm-sans/500.css',
				'@fontsource/dm-sans/700.css',
				'@fontsource/jetbrains-mono/400.css',
				'@fontsource/jetbrains-mono/600.css',
				'./src/styles/custom.css',
			],
			social: {
				github: 'https://github.com/roflmuffin/CounterStrikeSharp',
			},
			sidebar: [
				{
					label: 'Guides',
					autogenerate: { directory: 'guides' }
				},
			],
			editLink: {
				baseUrl: "https://github.com/roflmuffin/CounterStrikeSharp/edit/main/docs/",
			  },
		}),
	],
	site: 'https://docs.cssharp.dev',
	markdown: {
		shikiConfig: {
		  theme: 'github-dark-dimmed',
		  wrap: true,
		},
	  },
	redirects: {
		'/': '/guides/getting-started',
	},
});
