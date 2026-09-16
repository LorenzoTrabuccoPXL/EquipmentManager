import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

let siteConfiguration = {}
try {
  const { default: cfg } = await import('./.figma/make/site.json', { with: { type: 'json' } })
  siteConfiguration = cfg
} catch {
  // site.json may not exist in all environments
}

export default defineConfig(({ mode }) => {
  const emitSourcemaps = mode === 'development'

  return {
    base: process.env.FIGMA_PUBLIC_URL ? `${process.env.FIGMA_PUBLIC_URL}/` : '/',
    build: {
      sourcemap: emitSourcemaps ? 'inline' : false,
      minify: !emitSourcemaps,
    },
    plugins: [
      vue(),
      figmaSiteConfiguration(siteConfiguration),
      figmaErrorOverlayReplay(),
    ],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, './src'),
      },
    },
    server: {
      host: process.env.FIGMA_DEV_SERVER_HOST || '0.0.0.0',
      port: parseInt(process.env.PORT || '8443'),
      strictPort: true,
      watch: { ignored: ['**/.figma/**'] },
    },
    preview: {
      host: process.env.FIGMA_DEV_SERVER_HOST || '0.0.0.0',
      port: parseInt(process.env.PORT || '8443'),
    },
  }
})

function figmaSiteConfiguration(config) {
  function sanitizeHtmlValue(value) {
    return value?.replace(/[^a-zA-Z0-9_-]/g, '') || ''
  }
  function escapeHtmlText(value) {
    return value.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
  }
  function replaceHtmlCommentSlot(html, slotName, content) {
    return html.replace(`<!-- ${slotName} -->`, content)
  }

  const title = config.title ?? 'Equipment Manager'
  const description = config.description ?? ''
  const favicon = config.icons?.icon ?? ''
  const socialImage = config.openGraph?.image ?? ''
  const language = sanitizeHtmlValue(config.language) || 'en'
  const googleAnalyticsId = sanitizeHtmlValue(config.analytics?.googleAnalyticsId)
  const headStart = config.customScripts?.headStart ?? ''
  const headEnd = config.customScripts?.headEnd ?? ''
  const bodyStart = config.customScripts?.bodyStart ?? ''
  const bodyEnd = config.customScripts?.bodyEnd ?? ''
  const robotsTxt = config.robots?.index === false ? 'User-agent: *\nDisallow: /\n' : ''

  return {
    name: 'figma-site-configuration',
    configureServer(server) {
      server.middlewares.use((req, res, next) => {
        if (!robotsTxt || req.url?.split('?')[0] !== '/robots.txt') return next()
        res.setHeader('Content-Type', 'text/plain; charset=utf-8')
        res.end(robotsTxt)
      })
    },
    generateBundle() {
      if (!robotsTxt) return
      this.emitFile({ type: 'asset', fileName: 'robots.txt', source: robotsTxt })
    },
    transformIndexHtml: {
      order: 'pre',
      handler(html) {
        let result = html
        result = replaceHtmlCommentSlot(result, 'figma:lang', language)
        result = replaceHtmlCommentSlot(result, 'figma:title', escapeHtmlText(title))
        result = replaceHtmlCommentSlot(result, 'figma:head-start', headStart)
        result = replaceHtmlCommentSlot(result, 'figma:head-end', headEnd)
        result = replaceHtmlCommentSlot(result, 'figma:body-start', bodyStart)
        result = replaceHtmlCommentSlot(result, 'figma:body-end', bodyEnd)

        const tags = []
        if (description) tags.push({ tag: 'meta', attrs: { name: 'description', content: description }, injectTo: 'head' })
        if (favicon) tags.push({ tag: 'link', attrs: { rel: 'icon', href: favicon }, injectTo: 'head' })
        if (title) tags.push({ tag: 'meta', attrs: { property: 'og:title', content: title }, injectTo: 'head' })
        if (googleAnalyticsId) {
          tags.push(
            { tag: 'script', attrs: { async: true, src: `https://www.googletagmanager.com/gtag/js?id=${googleAnalyticsId}` }, injectTo: 'head' },
            { tag: 'script', children: `window.dataLayer=window.dataLayer||[];function gtag(){dataLayer.push(arguments);}gtag('js',new Date());gtag('config',${JSON.stringify(googleAnalyticsId)});`, injectTo: 'head' },
          )
        }
        return { html: result, tags }
      },
    },
  }
}

function figmaErrorOverlayReplay() {
  return {
    name: 'figma-error-overlay-replay',
    apply: 'serve',
    configureServer(server) {
      let lastError = null
      const origSend = server.ws.send.bind(server.ws)
      server.ws.send = (...args) => {
        const payload = args[0]
        if (payload && typeof payload === 'object' && !Array.isArray(payload)) {
          if (payload.type === 'error') lastError = payload
          else if (payload.type === 'update' || payload.type === 'full-reload') lastError = null
        }
        return origSend(...args)
      }
      server.ws.on('connection', (socket) => {
        if (lastError !== null) socket.send(JSON.stringify(lastError))
      })
    },
  }
}
