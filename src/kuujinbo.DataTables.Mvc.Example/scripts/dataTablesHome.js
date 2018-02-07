var customFilter = function() {
    return {
        setupCustomFilters: function() {
            // return array => resolved && rejected jqXHR objects
            $.when([
                '/home/CustomPositionFilter',
                '/home/CustomOfficeFilter'
            ]
            .map(function(url) { return $.get(url); }))
            .always(function(arrayJqXHR) {
                arrayJqXHR.forEach(function(element, index, array) {
                    // resolved
                    element.then(function(data, textStatus, jqXHR) {
                        if (index === 0) {
                            configTable.addValuePicker(2, data);
                        }
                        else {
                            configTable.addValuePicker(3, data);
                        }
                    },
                    // rejected
                    function(jqXHR, textStatus, errorThrown) {
                        configTable.jqModalError(
                            'There was an error looking up Position or Office. Please try again.'
                        );
                    });
                });
            });
        }
    };
}();

customFilter.setupCustomFilters();


$('#jquery-data-table tbody').on('click', 'tr', function () {
    var data = table.row(this).data();
    console.log('You clicked on ' + data[0] + '\'s row');
});