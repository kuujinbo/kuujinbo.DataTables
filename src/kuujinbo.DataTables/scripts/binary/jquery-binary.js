(function ($) {
    $.fn.downloadFile = function (url, data, headers, requestType, before, fail, always) {
        if (before && typeof before === 'function') before();

        // explicitly add for ASP.NET MVC; e.g. Angular 'feature' leaves this out
        if (headers) headers['X-Requested-With'] = 'XMLHttpRequest';
        $.ajax({
            url: url,
            data: data,
            type: requestType || 'POST',
            headers: headers,
            dataType: 'binary'
        })
        .done(function(data, textStatus, jqXHR) {
            var type = jqXHR.getResponseHeader('Content-Type');
            var filename = jqXHR.getResponseHeader('Content-Disposition');
            filename = filename && filename.indexOf('attachment') > -1
                ? filename.replace(/(?:[^=])+=/, '')
                : 'file.bin';
            var blob = new Blob([data], { type: type });
            saveAs(blob, filename);
        })
        .fail(function(jqXHR, textStatus, errorThrown) {
            if (fail && typeof fail === 'function') {
                fail(jqXHR.responseJSON);
            }
            else { // last resort if caller didn't supply a fail callback
                alert('error');
            }
        })
        .always(function () {
            if (always && typeof always === 'function') always();
        });
        return false;
    };
}(jQuery));