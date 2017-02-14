function TableConfig() {
    this._table = null;
    this._configValues = {};
    this._infoEditDelete = '';
    // MS @Html.AntiForgeryToken() **IGNORES** HTML4 naming standards:
    // https://www.w3.org/TR/html4/types.html#type-id ('name' token)
    this._xsrf = '__RequestVerificationToken';
    this._idNo = 0;
    // per column mutli-value filter
    this._ieGTE10 = document.documentMode >= 10;
}

TableConfig.prototype = {
    constructor: TableConfig,

    jqPartialViewModal: $('#datatable-partial-modal').dialog({
        autoOpen: false, width: 'auto'
        // DO NOT SET THE FOLLOWING; IE's focus() is HORRIBLY broken
        //, modal: true
    }),
    jqPartialViewModalOK: function(html, title) {
        this.jqPartialViewModal.html(html)
            .dialog({ title: title })
            .dialog('open');
    },
    jqBulkActionModal: $('#datatable-success-error-modal').dialog({
        autoOpen: false, height: 276, width: 476
    }),
    jqModalOK: function(msg) {
        var success = 'Request Processed Successfully';
        var html = "<h1><span class='glyphicon glyphicon-ok green'></span></h1>"
            + '<div>' + (msg || success) + '</div>';
        this.jqBulkActionModal.html(html)
            .dialog({ title: success })
            .dialog('open');
    },
    jqModalError: function(msg) {
        var err = 'Error Processing Your Request'
        var html = "<h1><span class='glyphicon glyphicon-flag red'></span></h1>"
            + '<div>' + (msg || err) + '</div>';
        this.jqBulkActionModal.html(html)
            .dialog({ title: err })
            .dialog('open');
    },
    /* -----------------------------------------------------------------
        selectors and DOM elements
    ----------------------------------------------------------------- */
    getTableId: function() { return '#jquery-data-table'; },
    getSaveAsId: function() { return '#datatable-save-as'; },
    getCheckAllId: function() { return '#datatable-check-all'; },
    setTable: function(table) {
        this._table = table;
        return this;
    },
    getConfigValues: function() { return this._configValues; },
    setConfigValues: function(config) {
        this._configValues = config;
        // reset InfoEditDelete link cache
        this._infoEditDelete = null;
        return this;
    },
    getLoadingElement: function() {
        return "<h1 class='dataTablesLoading'>"
            + 'Loading data'
            + " <span class='glyphicon glyphicon-refresh spin-infinite' />"
            + '</h1>';
    },
    getSpinClasses: function() {
        return 'glyphicon glyphicon-refresh spin-infinite'.split(/\s+/);
    },
    getSelectedRowClass: function() {
        return 'datatable-select-row';
    },
    getInvalidUrlMessage: function() {
        return '<h2>Invalid URL</h2>Please contact the application administrators.';
    },
    getXhrErrorMessage: function() {
        return 'There was a problem processing your request. If the problem continues, please contact the application administrators';
    },
    getActionButtonSelector: function() { return '#data-table-actions button.btn'; },
    getSearchFilterSelector: function() { return 'th input[type=text], th select'; },
    getCheckedSelector: function() { return 'input[type="checkbox"]:checked'; },
    getUncheckedSelector: function() { return 'input[type="checkbox"]:not(:checked)'; },

    getInfoAction: function() { return 'info'; },
    getEditAction: function() { return 'edit'; },
    getDeleteAction: function() { return 'delete'; },
    getInfoEditDelete: function() {
        // calculate once then cache value
        if (this._infoEditDelete) return this._infoEditDelete;

        var infoLink = this.getConfigValues().infoRowUrl
            ? "<span class='glyphicon glyphicon-info-sign blue link-icons' data-action='"
                + this.getInfoAction()
                + "' title='Information'></span>"
            : '';

        var editLink = this.getConfigValues().editRowUrl
            ? "<span class='glyphicon glyphicon-edit green link-icons' data-action='"
                + this.getEditAction()
                + "' title='Edit'></span>"
            : '';

        var deleteLink = this.getConfigValues().deleteRowUrl
            ? "<span class='glyphicon glyphicon-remove-circle red link-icons' data-action='"
                + this.getDeleteAction()
                + "' title='Delete'><span></span></span>"
            : '';

        this._infoEditDelete = [infoLink, editLink, deleteLink].join(' ').trim();
        return this._infoEditDelete;
    },
    /* -----------------------------------------------------------------
        DataTables wrappers
    ----------------------------------------------------------------- */
    getAjaxParams: function() { return this._table.ajax.params(); },
    clearSearchColumns: function() { this._table.search('').columns().search(''); },
    draw: function() { this._table.draw(false); },
    drawAndGoToPage1: function() { this._table.draw(); },
    getRowData: function(row) {
        return this._table.row(row).data()[0];
    },
    reload: function() { this._table.ajax.reload(); },
    setSearchColumn: function(element) {
        this._table.column(element.dataset.columnNumber).search(element.value);
    },
    /* -----------------------------------------------------------------
        helper functions
    ----------------------------------------------------------------- */
    clearCheckAll: function() {
        // ajax call only updates tbody
        var n = document.querySelector(this.getCheckAllId());
        if (n !== null) n.checked = false;
    },
    clearSearchFilters: function() {
        var elements = document.querySelectorAll(this.getSearchFilterSelector());
        for (i = 0; i < elements.length; ++i) elements[i].value = '';

        this.clearSearchColumns();
    },
    getSelectedRowIds: function() {
        var selectedIds = [];
        var self = this;
        this._table.rows().every(function(rowIdx, tableLoop, rowLoop) {
            var cb = this.node()
                .querySelector(self.getCheckedSelector());

            if (cb !== null && cb.checked) selectedIds.push(this.data()[0]);
        });
        return selectedIds;
    },
    getXsrfToken: function() {
        var token = document.querySelector('input[name=' + this._xsrf + ']');
        if (token !== null) {
            var xsrf = {};
            xsrf[this._xsrf] = token.value;
            return xsrf;
        }
        return null;
    },
    redirect: function(url) {
        document.location.href = url;
    },
    search: function() {
        var searchCount = 0;
        var elements = document.querySelectorAll(this.getSearchFilterSelector());
        for (i = 0; i < elements.length; ++i) {
            var searchText = elements[i].value;
            // search only if non-whitespace
            if (searchText !== '' && !/^\s+$/.test(searchText)) {
                ++searchCount;
                this.setSearchColumn(elements[i]);
            }
                /* explicitly clear individual input, or will save last value 
                   if user backspaces.
                */
            else {
                elements[i].value = '';
                this.setSearchColumn(elements[i]);
            }
        }
        if (searchCount > 0) {
            this.clearCheckAll();
            this.drawAndGoToPage1();
        }
    },
    saveAs: function(before, fail, always) {
        var params = this.getAjaxParams();
        params.saveAs = true;
        var config = this.getConfigValues();
        params.columnNames = JSON.stringify(config.columnNames);

        // return binary content via XHR => see ~/Scripts/jQueryAjax/
        $().downloadFile(
            config.dataUrl,
            params,
            this.getXsrfToken(),
            null,
            before, fail, always
        );
    },
    // handle back button - explicitly set form field value(s)
    setSearchState: function() {
        var elements = document.querySelectorAll(this.getSearchFilterSelector());
        var columns = this._table.state().columns;
        for (var i = 1; i < columns.length - 1; ++i) {
            var searchState = columns[i].search.search;
            if (searchState && !elements[i - 1].value) {
                elements[i - 1].value = searchState;
            }
        }
        // return this._table.state();
    },
    showSpin: function(element, doAdd) {
        var span = element.querySelector('span');
        if (span) {
            if (doAdd) {
                this.getSpinClasses()
                    .forEach(function(i) { span.classList.add(i) });
            } else {
                this.getSpinClasses()
                    .forEach(function(i) { span.classList.remove(i) });
            }
        }
    },
    sendXhr: function(element, url, requestData, requestType) {
        var self = this;
        self.showSpin(element, true);
        $.ajax({
            url: url,
            headers: self.getXsrfToken(),
            data: requestData, // bulk action button => record Id array
            type: requestType || 'POST'
        })
        .done(function(data, textStatus, jqXHR) {
            if (requestData !== null) {
                self.draw();
                self.jqModalOK(data);
            } else {
                self.jqPartialViewModalOK(
                    data,               // HTML from partial view
                    element.textContent // button text for modal title
                );
            }
        })
        .fail(function(jqXHR, textStatus, errorThrown) {
            self.jqModalError(
                jqXHR.responseJSON
                || (jqXHR.status !== 500 ? jqXHR.statusText : self.getXhrErrorMessage())
            );
        })
        .always(function() {
            self.showSpin(element)
        });
    },
    /* -----------------------------------------------------------------
        event listeners
    ----------------------------------------------------------------- */
    clickActionButton: function(e) {
        e.preventDefault();
        var target = e.target;
        var url = target.dataset.url;
        var isModal = target.hasAttribute('data-modal');

        if (url) {
            if (isModal) {
                this.sendXhr(target, url, null, 'GET');
            } else {
                var ids = this.getSelectedRowIds();
                if (ids.length > 0) {
                    this.sendXhr(target, url, { ids: ids });
                } else {
                    this.jqModalError(
                        '<h2>No Records Selected</h2>'
                        + 'Select one or more records to process the '
                        + (target.textContent || 'selected')
                        + ' action.'
                    );
                }
            }
        } else {
            this.jqModalError(this.getInvalidUrlMessage());
        }

        return false;
    },
    // send binary content via XHR
    clickSaveAs: function(e) {
        // explicitly show/hide 'processing' element; since XHR is not
        // sent via jQuery DataTables API, need to handle this here
        var before, always;
        var n = document.querySelector('div.dataTables_processing');
        if (n !== null) {
            before = function() { n.style.display = 'block'; }
            always = function() { n.style.display = 'none'; }
        }
        // and handle response errors
        var fail = function(msg) {
            this.jqModalError(msg || this.getXhrErrorMessage());
        }

        this.saveAs(before, fail, always);
    },
    clickCheckAll: function(e) {
        if (e.target.checked) {
            var elements = document.querySelectorAll(
                this.getUncheckedSelector()
            );
            for (i = 0; i < elements.length; ++i) elements[i].checked = true;
        } else {
            var elements = document.querySelectorAll(
                this.getCheckedSelector()
            );
            for (i = 0; i < elements.length; ++i) elements[i].checked = false;
        }
    },
    // search icons in <span>
    clickSearch: function(e) {
        var target = e.target;
        if (target.classList.contains('glyphicon-search')) {
            this.search();
        } else if (target.classList.contains('glyphicon-repeat')) {
            this.clearSearchFilters();
            this.reload();
        }
    },
    clickTable: function(e) {
        var target = e.target;
        var action = target.dataset.action;
        var self = this;

        // single checkbox click
        if (target.type === 'checkbox') {
            var row = target.parentNode.parentNode;
            if (row && row.tagName.toLowerCase() === 'tr') {
                var cl = self.getSelectedRowClass();
                if (target.checked) {
                    row.classList.add(cl);
                } else {
                    row.classList.remove(cl);
                }
            }
        } else if (action) { // info, edit, & delete links
            var row = target.parentNode.parentNode;
            if (action === self.getDeleteAction()) {
                // delete record from dataset...
                self.sendXhr(
                    target,
                    self.getConfigValues().deleteRowUrl,
                    { id: self.getRowData(row) }
                );
                self.clearCheckAll();
            } else if (action === self.getEditAction()) {
                self.redirect(
                    self.getConfigValues().editRowUrl
                    + '/'
                    + self.getRowData(row)
                );
            } else if (action === self.getInfoAction()) {
                self.redirect(
                    self.getConfigValues().infoRowUrl
                    + '/'
                    + self.getRowData(row)
                );
            }
        }
    },
    // search when ENTER key pressed in <input> text
    keyupSearch: function(e) {
        if (e.key === 'Enter') this.search();
    },
    addListeners: function(tableId) {
        // allow ENTER in search boxes, otherwise possible form submit
        document.onkeypress = function(e) {
            if ((e.which === 13) && (e.target.type === 'text')) { return false; }
        };

        // action buttons
        var buttons = document.querySelectorAll(
            this.getActionButtonSelector()
        );
        for (i = 0 ; i < buttons.length ; i++) {
            buttons[i].addEventListener('click', this.clickActionButton.bind(this), false);
        }

        // saveAs button
        var saveAsButton = document.querySelector(this.getSaveAsId());
        if (saveAsButton != null) {
            saveAsButton.addEventListener('click', this.clickSaveAs.bind(this), false);
        }

        // 'check all' checkbox
        var checkAll = document.querySelector(this.getCheckAllId());
        if (checkAll != null) checkAll.addEventListener('click', this.clickCheckAll.bind(this), false);

        // datatable clicks
        var clickTable = document.querySelector(tableId);
        if (clickTable != null) clickTable.addEventListener('click', this.clickTable.bind(this), false);

        // search icons
        var searchIcons = document.querySelectorAll('tfoot span.search-icons');
        for (var i = 0; i < searchIcons.length; i++) {
            searchIcons[i].addEventListener('click', this.clickSearch.bind(this), false);
        }

        // search input fields
        var footerSearchBoxes = document.querySelectorAll(tableId + ' tfoot input[type=text]');
        for (var i = 0; i < footerSearchBoxes.length; i++) {
            footerSearchBoxes[i]
                .addEventListener('keyup', this.keyupSearch.bind(this), false);
        }
    },
    init: function() {
        var tableId = this.getTableId();

        this.addListeners(tableId);
    },
    /* -----------------------------------------------------------------
        value picker UI element: server processing - per-column 
        multi-value filter
    ----------------------------------------------------------------- */
    getValuePickerId: function() {
        return 'valuePickerId__' + this._idNo++;
    },
    // flag value picker selected items
    getSelectedSelector: function() {
        return 'dataTableSelected';
    },
    // flag value picker selected items
    getPickerSelectedSelector: function() {
        return 'div.' + this.getSelectedSelector();;
    },
    // store widget id => multiple widgets in DOM
    getValuePickerIdName: function() { return '_valuePickerIdName_'; },
    // store column search term input field selector
    getSearchColumnIndexName: function() { return '_searchColumnIndexName_'; },

    // add value picker:
    // [1] columnIndex; zero-based
    // [2] stringArray: string items that populate value picker
    addValuePicker: function(columnIndex, stringArray) {
        var selector = "th > input[data-column-number='" + columnIndex + "']";

        var searchInput = document.querySelector(selector);
        if (searchInput !== null
            && stringArray !== null
            && Array.isArray(stringArray)
            && stringArray.length > 0) {
            var newId = this.getValuePickerId();
            searchInput[this.getValuePickerIdName()] = newId;
            searchInput[this.getSearchColumnIndexName()] = selector;

            searchInput.addEventListener('focus', this.enterSearchInput.bind(this), false);
            searchInput.addEventListener('blur', this.leaveSearchInput.bind(this), false);

            // value picker DOM element
            var div = document.createElement('div');
            // selectable items w/extra container for scrolling
            var inner = "<div>";
            stringArray.forEach(function(item) {
                // class style => jQueryDataTables.css
                inner += "<div class='pickerItem'>" + item + '</div>';
            });
            inner += "</div><input type='button' style='margin:8px;' value='Add / Clear' />";
            div.innerHTML = inner;
            div.addEventListener(
                'click', this.togglePickerItem.bind(this), false
            );

            var button = div.querySelector('input');
            button.addEventListener(
                'click', this.valuePickerButtonClick.bind(this), false
            );
            button[this.getSearchColumnIndexName()] =
                searchInput[this.getSearchColumnIndexName()];

            // need this to make <div> receive focus
            if (this._ieGTE10) div.tabIndex = '0';

            div.id = newId;
            // class style => jQueryDataTables.css
            div.classList.add('valuePicker');
            div.style.minWidth = searchInput.getBoundingClientRect().width + 'px';
            div.style.display = 'none';
            var items = div.querySelector('div');
            if (stringArray.length > 10) {
                items.style.overflowY = 'auto';
                items.style.height = '276px';
            };
            searchInput.parentNode.appendChild(div);

            // handle value picker show/hide - **ONLY** works for IE
            div.addEventListener('blur', this.leaveSearchInput.bind(this), false);
        }
    },
    // show value picker when corresponding search text input receives focus
    enterSearchInput: function(e) {
        var valuepickerId = e.target[this.getValuePickerIdName()];

        var el = document.querySelector('#' + valuepickerId);
        if (document.querySelector('#' + valuepickerId) !== null) {
            el.style.display = 'block';
        }
    },
    // hide value picker:
    // [1] corresponding search text input loses focus
    // [2] value picker loses focus
    // NOTES:
    // --------------------------------------------------------------------
    // https://developer.mozilla.org/en-US/docs/Web/API/Document/activeElement
    // https://developer.mozilla.org/en-US/docs/Web/Events/blur
    // [1] IE10 >= sets document.activeElement to element focus **MOVES TO**;
    // allows show/hide based on user page interaction
    // [2] all other browsers set document.activeElement to document.body;
    // must manually close value picker
    leaveSearchInput: function(e) {
        if (this._ieGTE10) {
            var focusEl = document.activeElement;
            var target = e.target;
            var targetName = target.tagName.toLowerCase();
            var valuepickerId = '';
            if (targetName === 'input') {
                valuepickerId = target[this.getValuePickerIdName()];
            } else if (targetName === 'div') {
                valuepickerId = target.previousSibling[this.getValuePickerIdName()];
            }

            if (valuepickerId || focusEl !== null) {
                var picker = document.querySelector('#' + valuepickerId);
                if (picker !== null && picker.contains(focusEl)) { return; }
                this.resetValuePicker(valuepickerId);
            }
        }
    },
    // reset any selected items back to **NOT** selected
    resetValuePicker: function(selector) {
        var picker = document.querySelector('#' + selector);
        if (picker !== null) {
            picker.style.display = 'none';
            var selected = picker.querySelectorAll(this.getPickerSelectedSelector());
            if (selected !== null && selected.length > 0) {
                var selectedClass = this.getSelectedSelector();
                for (var i = 0; i < selected.length; ++i) selected[i].classList.remove(selectedClass);
            }
        }
    },
    // [1] get value picker selected values
    // [2] join into single value with separator character for server-side processing
    valuePickerButtonClick: function(e) {
        var target = e.target;
        var filterField = document.querySelector(target[this.getSearchColumnIndexName()]);
        if (filterField !== null) {
            var valuePickerId = filterField[this.getValuePickerIdName()];
            var el = document.querySelector('#' + valuePickerId);
            if (el !== null) {
                var values = [];
                var childEls = el.querySelectorAll(this.getPickerSelectedSelector());
                for (var i = 0; i < childEls.length; i++) {
                    values.push(childEls[i].textContent);
                }
                filterField.value = values.length > 0
                    ? values.join(this.getConfigValues().multiValueFilterSeparator) : '';
            }

            this.resetValuePicker(valuePickerId);
        }
    },
    togglePickerItem: function(e) {
        var target = e.target;
        if (target.classList.contains('pickerItem')) {
            target.classList.toggle(this.getSelectedSelector());
            if (this._ieGTE10) target.parentNode.parentNode.focus();
        }
    }
};