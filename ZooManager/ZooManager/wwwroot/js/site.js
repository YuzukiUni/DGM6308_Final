window.keydownListener = {
    listen: function (dotNetObjectReference) {
        document.addEventListener('keydown', function (event) {
            dotNetObjectReference.invokeMethodAsync('OnKeyPress', event.key);
        });
    }
};


