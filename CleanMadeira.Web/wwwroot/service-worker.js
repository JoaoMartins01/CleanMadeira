const CACHE_NAME = "cleanmadeira-static-v3";

const STATIC_FILES = [
    "/manifest.json",
    "/css/site.css",
    "/css/layout.css",
    "/js/layout.js",
    "/images/icons/icon-192.png",
    "/images/icons/icon-512.png"
];

self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => cache.addAll(STATIC_FILES))
    );

    self.skipWaiting();
});

self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames
                    .filter(name => name !== CACHE_NAME)
                    .map(name => caches.delete(name))
            );
        })
    );

    self.clients.claim();
});

self.addEventListener("fetch", event => {
    const request = event.request;

    // Nunca intercetar POST, PUT, DELETE, etc.
    if (request.method !== "GET") {
        return;
    }

    const url = new URL(request.url);

    // Não guardar páginas de autenticação ou páginas MVC dinâmicas
    if (
        url.pathname.startsWith("/Account") ||
        url.pathname.startsWith("/Conta") ||
        request.mode === "navigate"
    ) {
        event.respondWith(fetch(request));
        return;
    }

    // Cache apenas para ficheiros estáticos
    event.respondWith(
        caches.match(request).then(cachedResponse => {
            return cachedResponse || fetch(request);
        })
    );
});