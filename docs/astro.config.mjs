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
      head: [
        {
          tag: 'script',
          attrs: {
            src: 'https://gc.zgo.at/count.js',
            'data-goatcounter': 'https://cssharp.goatcounter.com/count',
            async: true,
          },
        },
      ],
      social: {
        github: 'https://github.com/roflmuffin/CounterStrikeSharp',
      },
      sidebar: [
        {
          label: 'Guides',
          autogenerate: { directory: 'guides' },
        },
        {
          label: 'Features',
          autogenerate: { directory: 'features' },
        },
        {
          label: 'Reference',
          autogenerate: { directory: 'reference' },
        },
      ],
      editLink: {
        baseUrl:
          'https://github.com/roflmuffin/CounterStrikeSharp/edit/main/docs/',
      },
    }),
  ],
  base: '/',
  site: 'https://docs.cssharp.dev',
  markdown: {
    shikiConfig: {
      wrap: false,
    },
  },
});
