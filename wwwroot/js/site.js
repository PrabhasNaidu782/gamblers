// Auto-dismiss alerts after 4 seconds
setTimeout(function () {
    document.querySelectorAll('.alert-dismissible').forEach(function (el) {
        var bsAlert = bootstrap.Alert.getOrCreateInstance(el);
        bsAlert.close();
    });
}, 4000);
