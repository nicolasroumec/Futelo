window.pwaUpdater = {
    register: function (dotnetRef) {
        if (!('serviceWorker' in navigator)) return;
        navigator.serviceWorker.ready.then(function (registration) {
            if (registration.waiting && navigator.serviceWorker.controller) {
                dotnetRef.invokeMethodAsync('OnUpdateAvailable');
                return;
            }
            registration.addEventListener('updatefound', function () {
                var newWorker = registration.installing;
                if (!newWorker) return;
                newWorker.addEventListener('statechange', function () {
                    if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                        dotnetRef.invokeMethodAsync('OnUpdateAvailable');
                    }
                });
            });
        });
    },
    applyUpdate: function () {
        navigator.serviceWorker.ready.then(function (registration) {
            if (registration.waiting) {
                registration.waiting.postMessage({ type: 'SKIP_WAITING' });
            }
        });
        navigator.serviceWorker.addEventListener('controllerchange', function () {
            window.location.reload();
        });
    }
};
