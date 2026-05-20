window.offlineIndicator = {
    register: function (dotnetRef) {
        window.addEventListener('online', function () {
            dotnetRef.invokeMethodAsync('OnOnline');
        });
        window.addEventListener('offline', function () {
            dotnetRef.invokeMethodAsync('OnOffline');
        });
        return navigator.onLine;
    }
};
