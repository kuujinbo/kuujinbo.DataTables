/// <reference path="./../../src/kuujinbo.DataTables.Mvc.Example/scripts/jquery.dataTables.js" />
/// <reference path="./../../src/kuujinbo.DataTables.Mvc.Example/scripts/dataTables.bootstrap.js" />
/// <reference path="./../../src/kuujinbo.DataTables.Mvc.Example/scripts/jquery-ui-1.12.1.js" />
/// <reference path="./../../src/kuujinbo.DataTables.Mvc.Example/scripts/binary/jquery-binary.js" />
/// <reference path="./../../src/kuujinbo.DataTables.Mvc.Example/scripts/TableConfig.js" />
'use strict';

describe('configTable', function() {
    var configTable = null;
    beforeEach(function() {
        configTable = new TableConfig();
        configTable.init();
    });

    describe('selectors and DOM', function() {
        it('initializes the table objects', function() {
            expect(configTable).toBeDefined();
            expect(configTable.getTableId()).toEqual('#jquery-data-table');
            expect(configTable.getCheckAllId()).toEqual('#datatable-check-all');

            // setTable() && setConfigValues() return 'this' to allow chaining
            expect(
                configTable.setTable({}).setConfigValues({})
            ).toEqual(configTable);
            /* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
             *  .setConfigValues() resets InfoEditDelete link cache
             * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            */
            expect(configTable.getInfoEditDelete()).toEqual('');

            expect(configTable.getLoadingElement()).toEqual(
                "<h1 class='dataTablesLoading'>Loading data "
                + "<span class='glyphicon glyphicon-refresh spin-infinite' /></h1>"
            );
        });

        it('creates CSS selectors', function() {
            expect(configTable.getActionButtonSelector()).toEqual('#data-table-actions button.btn');
            expect(configTable.getSearchFilterSelector()).toEqual('th input[type=text], th select');
            expect(configTable.getCheckedSelector()).toEqual('input[type="checkbox"]:checked');
            expect(configTable.getUncheckedSelector()).toEqual('input[type="checkbox"]:not(:checked)');
        });

        it('creates class names', function() {
            expect(configTable.getSelectedRowClass()).toEqual('datatable-select-row');

            var spinClasses = configTable.getSpinClasses();
            expect(spinClasses.length).toEqual(3);
            expect(spinClasses[0]).toEqual('glyphicon');
            expect(spinClasses[1]).toEqual('glyphicon-refresh');
            expect(spinClasses[2]).toEqual('spin-infinite');
        });
    });

    describe('getInfoEditDelete', function() {
        var infoEditDeleteId = 'infoEditDelete';

        it('does not display links when all URLs are empty', function() {
            configTable.setConfigValues({});
            setFixtures(
                '<div id="' + infoEditDeleteId + '">'
                + configTable.getInfoEditDelete()
                + '</div>'
            );
            var links = document.querySelector('#' + infoEditDeleteId);

            expect(links.children.length).toEqual(0);
            expect(links.textContent).toEqual('');
        });

        it('displays the info link when info URL is set', function() {
            configTable.setConfigValues({ infoRowUrl: '/info' });
            setFixtures(
                '<div id="' + infoEditDeleteId + '">'
                + configTable.getInfoEditDelete()
                + '</div>'
            );

            var links = document.querySelector('#' + infoEditDeleteId);

            expect(links.children.length).toEqual(1);
            expect(links.children[0].tagName.toLowerCase()).toEqual('span');
            expect(links.children[0].dataset.action).toEqual('info');
        });

        it('displays the edit link when info URL is set', function() {
            configTable.setConfigValues({ editRowUrl: '/edit' });
            setFixtures(
                '<div id="' + infoEditDeleteId + '">'
                + configTable.getInfoEditDelete()
                + '</div>'
            );

            var links = document.querySelector('#' + infoEditDeleteId);

            expect(links.children.length).toEqual(1);
            expect(links.children[0].tagName.toLowerCase()).toEqual('span');
            expect(links.children[0].dataset.action).toEqual('edit');
        });

        it('displays the delete link when info URL is set', function() {
            configTable.setConfigValues({ deleteRowUrl: '/edit' });
            setFixtures(
                '<div id="' + infoEditDeleteId + '">'
                + configTable.getInfoEditDelete()
                + '</div>'
            );

            var links = document.querySelector('#' + infoEditDeleteId);

            expect(links.children.length).toEqual(1);
            expect(links.children[0].tagName.toLowerCase()).toEqual('span');
            expect(links.children[0].dataset.action).toEqual('delete');
        });

        it('displays all links when all URLs are set', function() {
            var configValues = {
                dataUrl: '/',
                infoRowUrl: '/info',
                editRowUrl: '/edit',
                deleteRowUrl: '/delete'
            };
            configTable.setConfigValues(configValues);
            setFixtures(
                '<div id="' + infoEditDeleteId + '">'
                + configTable.getInfoEditDelete()
                + '</div>'
            );

            var links = document.querySelector('#' + infoEditDeleteId);
            var deleteLink = links.querySelector('span[data-action=delete]');

            expect(links.children.length).toEqual(3);
            expect(links.querySelector('span[data-action=info]')).not.toBeNull();
            expect(links.querySelector('span[data-action=edit]')).not.toBeNull();

            expect(deleteLink).not.toBeNull();
            // empty span for XHR processing spinner
            expect(deleteLink.children.length).toEqual(1);
            expect(deleteLink.children[0].tagName.toLowerCase()).toEqual('span');
        });
    });

    describe('clearCheckAll', function() {
        it('unchecks all checkboxes', function() {
            var template = document.createElement('div');
            template.innerHTML = '<input id="'
                + configTable.getCheckAllId()
                + '" type="checkbox" />';
            document.body.appendChild(template);
            var checkbox = template.firstChild;

            configTable.clearCheckAll();

            expect(checkbox.checked).toEqual(false);
        });
    });

    describe('clearSearchFilters', function() {
        it('clears all filter values', function() {
            setFixtures(
                '<tfoot><tr>'
                + "<th><input type='text' value='00' /></th>"
                + "<th><input type='text' value='11' /></th>"
                + '<th><select name="select">'
                    + '<option value=""></option>'
                    + '<option selected="selected" value="true">Yes</option>'
                    + '<option value="false">No</option>'
                + '</select></th>'
                + '</tr></tfoot>'
            );
            spyOn(configTable, 'clearSearchColumns');

            var filters = document.querySelectorAll(
                configTable.getSearchFilterSelector()
            );
            configTable.clearSearchFilters();

            expect(filters.length).toEqual(3);
            expect(filters[0].value).toEqual('');
            expect(filters[1].value).toEqual('');
            expect(filters[2].value).toEqual('');
            expect(configTable.clearSearchColumns).toHaveBeenCalledTimes(1);
        });
    });

    describe('getXsrfToken', function() {
        it('returns null when the hidden field is not in the DOM', function() {
            expect(configTable.getXsrfToken()).toBeNull()
        });

        it('returns the XSRF token when the hidden field is in DOM', function() {
            setFixtures(
                "<input name='__RequestVerificationToken' type='hidden' value='XXX' />"
            );

            var xsrf = configTable.getXsrfToken();

            expect(xsrf).not.toBeNull();
            expect(xsrf.__RequestVerificationToken).toEqual('XXX');
        });
    });

    describe('search', function() {
        beforeEach(function() {
            spyOn(configTable, 'setSearchColumn');
            spyOn(configTable, 'drawAndGoToPage1');
        });

        it('does not execute search when textboxes are empty or whitespace', function() {
            setFixtures(
                '<th>'
                + "<input type='text' placeholder='Search' data-column-number='1' />"
                + "<input type='text' placeholder='Search' data-column-number='2' value='   ' />"
                + '</th>'
            );
            var resultTextboxes = document.querySelectorAll('input[type=text]');

            configTable.search();

            expect(configTable.setSearchColumn.calls.count()).toEqual(2);
            expect(configTable.drawAndGoToPage1.calls.count()).toEqual(0);
            expect(resultTextboxes.length).toEqual(2);
            expect(resultTextboxes[0].value).toEqual('');
            expect(resultTextboxes[1].value).toEqual('');
        });

        it('executes search when any textbox is not empty or whitespace', function() {
            spyOn(configTable, 'clearCheckAll');
            setFixtures(
                '<th>'
                + "<input type='text' placeholder='Search' data-column-number='1' />"
                + "<input type='text' placeholder='Search' data-column-number='2' value='   ' />"
                + "<input type='text' placeholder='Search' data-column-number='3' value='03' />"
                + "<input type='text' placeholder='Search' data-column-number='4' value='04' />"
                + '</th>'
            );
            var resultTextboxes = document.querySelectorAll('input[type=text]');

            configTable.search();

            expect(configTable.setSearchColumn.calls.count()).toEqual(4);
            expect(configTable.clearCheckAll.calls.count()).toEqual(1);
            expect(configTable.drawAndGoToPage1.calls.count()).toEqual(1);
            expect(resultTextboxes.length).toEqual(4);
            expect(resultTextboxes[0].value).toEqual('');
            expect(resultTextboxes[1].value).toEqual('');
            expect(resultTextboxes[2].value).toEqual('03');
            expect(resultTextboxes[3].value).toEqual('04');
        });

        it('does not execute search when the default select option is selected', function () {
            setFixtures(
                '<th>'
                + "<select name='select'>"
                + "<option value='' selected='selected'></option>"
                + "<option value='true'>Yes</option>"
                + "<option value='false'>No</option>"
                + "</select></th>"
            );
            var result = document.querySelectorAll('select');

            configTable.search();

            expect(configTable.setSearchColumn.calls.count()).toEqual(1);
            expect(configTable.drawAndGoToPage1.calls.count()).toEqual(0);
            expect(result.length).toEqual(1);
        });

        it('executes search when any selected select is not empty or whitespace', function() {
            spyOn(configTable, 'clearCheckAll');
            setFixtures(
                "<th><select name='select'>"
                + "<option value=''></option>"
                + "<option value='true' selected='selected'>Yes</option>"
                + "<option value='false'>No</option>"
                + '</select></th>'
                + "<th><select name='select'>"
                + "<option value='' selected='selected'></option>"
                + "<option value='true'>Yes</option>"
                + "<option value='false'>No</option>"
                + '</select></th>'
                + "<th><select name='select'>"
                + "<option value=''></option>"
                + "<option value='true' selected='selected'>Yes</option>"
                + "<option value='false'>No</option>"
                + '</select></th>'
            );
            var result = document.querySelectorAll('input[type=text]');

            configTable.search();

            expect(configTable.setSearchColumn.calls.count()).toEqual(3);
            expect(configTable.clearCheckAll.calls.count()).toEqual(1);
            expect(configTable.drawAndGoToPage1.calls.count()).toEqual(1);
        });
    });

    describe('saveAs', function() {
        beforeEach(function() {
            spyOn(configTable, 'getAjaxParams').and.returnValue({ saveAs: true });
            spyOn(configTable, 'getXsrfToken');
            spyOn(configTable, 'getConfigValues').and.returnValue({ dataUrl: '/data' });
            spyOn($.fn, 'downloadFile');
        });

        it('downloads binary content', function() {
            configTable.saveAs();

            expect(configTable.getAjaxParams.calls.count()).toEqual(1);
            expect(configTable.getXsrfToken.calls.count()).toEqual(1);
            expect(configTable.getConfigValues.calls.count()).toEqual(1);
            expect($.fn.downloadFile.calls.count()).toEqual(1);
        });
    });

    // add / remove processing spinner
    describe('showSpin', function() {
        var spinClasses;
        beforeEach(function() {
            spinClasses = configTable.getSpinClasses();
        });

        it('adds the spin classes', function() {
            setFixtures('<div><span></span></div>');
            var domContainer = document.querySelector('div');

            configTable.showSpin(domContainer, true);
            var span = domContainer.querySelector('span');

            expect(spinClasses.length).toEqual(3);
            for (var i = 0; i < spinClasses.length; ++i) {
                expect(span.classList.contains(spinClasses[i])).toEqual(true);
            }
        });

        it('removes the spin classes', function() {
            setFixtures(
                '<div><span class="' + spinClasses.join(' ') + '"></span></div>'
            );
            var domContainer = document.querySelector('div');

            configTable.showSpin(domContainer);
            var span = domContainer.querySelector('span');

            for (var i = 0; i < spinClasses.length; ++i) {
                expect(span.classList.contains(spinClasses[i])).toEqual(false);
            }
        });
    });

    describe('sendXhr', function() {
        var deferred, element;
        beforeEach(function() {
            deferred = new jQuery.Deferred();
            element = document.createElement('button');
            spyOn(jQuery, 'ajax').and.returnValue(deferred);
            spyOn(configTable, 'showSpin');
            spyOn(configTable, 'getXsrfToken');
        });

        it('calls jQuery.ajax()', function() {
            var expectedArgs = {
                url: '/', headers: undefined, data: {}, type: 'POST'
            };
            configTable.sendXhr(element, '/', {});

            expect(jQuery.ajax.calls.count()).toEqual(1);
            expect(jQuery.ajax).toHaveBeenCalledWith(expectedArgs);
        });

        it('calls showSpin before sending XHR', function() {
            configTable.sendXhr(element, '/', {});

            // mock XHR has **NOT** returned
            expect(deferred.state()).toEqual("pending");
            expect(configTable.showSpin.calls.count()).toEqual(1);
            expect(configTable.showSpin).toHaveBeenCalledWith(element, true);
            expect(configTable.getXsrfToken).toHaveBeenCalledTimes(1);
        });

        it('calls jqModalError and showSpin when promise is rejected', function() {
            spyOn(configTable, 'jqModalError');
            var httpResponseMsg = 'HTTP response error';
            var jqXHR = { responseJSON: httpResponseMsg };
            configTable.sendXhr(element, '/', {});

            deferred.reject(jqXHR);

            // ajax.fail()
            expect(configTable.jqModalError.calls.count()).toEqual(1);
            expect(configTable.jqModalError).toHaveBeenCalledWith(httpResponseMsg);
            expect(configTable.showSpin.calls.count()).toEqual(2);
            expect(configTable.getXsrfToken).toHaveBeenCalledTimes(1);
            // ajax.always()
            expect(configTable.showSpin).toHaveBeenCalledWith(element);
        });

        it('calls jqModalError with default error message when promise is rejected and 500 error', function() {
            spyOn(configTable, 'jqModalError');
            var jqXHR = { status: 500 };
            configTable.sendXhr(element, '/', {});

            deferred.reject(jqXHR);

            // ajax.fail()
            expect(configTable.jqModalError.calls.count()).toEqual(1);
            expect(configTable.jqModalError).toHaveBeenCalledWith(configTable.getXhrErrorMessage());
        });

        it('calls jqModalError with XMLHttpRequest.statusText when promise is rejected and not 500 error', function() {
            spyOn(configTable, 'jqModalError');
            var statusText = 'a custom HTTP response message';
            var jqXHR = { status: 400, statusText: statusText };
            configTable.sendXhr(element, '/', {});

            deferred.reject(jqXHR);

            // ajax.fail()
            expect(configTable.jqModalError.calls.count()).toEqual(1);
            expect(configTable.jqModalError).toHaveBeenCalledWith(statusText);
        });

        it('calls jqModalOK, showSpin, and draw when promise is fulfilled for bulk action', function() {
            spyOn(configTable, 'jqModalOK');
            spyOn(configTable, 'draw');
            var httpResponseMsg = 'HTTP response success';
            configTable.sendXhr(element, '/', {});

            deferred.resolve(httpResponseMsg);

            // ajax.done()
            expect(configTable.jqModalOK.calls.count()).toEqual(1);
            expect(configTable.jqModalOK).toHaveBeenCalledWith(httpResponseMsg);
            expect(configTable.draw).toHaveBeenCalledTimes(1);
            expect(configTable.showSpin.calls.count()).toEqual(2);
            expect(configTable.getXsrfToken).toHaveBeenCalledTimes(1);
            // ajax.always()
            expect(configTable.showSpin).toHaveBeenCalledWith(element);
        });

        it('calls jqPartialViewModalOK and showSpin when promise is fulfilled for partial view action', function() {
            spyOn(configTable, 'jqPartialViewModalOK');
            var partialHtmlResponse = '<h1>Partial View</h1>';
            var buttonText = 'Submit';
            element.textContent = buttonText;

            configTable.sendXhr(element, '/', null, 'GET');

            deferred.resolve(partialHtmlResponse);

            // ajax.done()
            expect(configTable.jqPartialViewModalOK.calls.count()).toEqual(1);
            expect(configTable.jqPartialViewModalOK)
                .toHaveBeenCalledWith(partialHtmlResponse, buttonText);
            expect(configTable.showSpin.calls.count()).toEqual(2);
            expect(configTable.getXsrfToken).toHaveBeenCalledTimes(1);
            // ajax.always()
            expect(configTable.showSpin).toHaveBeenCalledWith(element);
        });
    });

    describe('init', function() {
        var id = 'id';
        it('calls the setup functions', function() {
            spyOn(configTable, 'getTableId').and.returnValue(id);
            spyOn(configTable, 'addListeners');
            configTable.init();

            expect(configTable.getTableId.calls.count()).toEqual(1);
            expect(configTable.addListeners.calls.count()).toEqual(1);
            expect(configTable.addListeners).toHaveBeenCalledWith(id);
        });
    });

    /* ========================================================================
       verify event listeners are registered in init()
       event listener functions themsleves are tested in isolation below
       ========================================================================
    */
    describe('EventTarget registration', function() {
        var tableId;
        beforeEach(function() {
            tableId = configTable.getTableId();
            setFixtures(
            "<div id='data-table-actions'>"
            + "<a id='IGNORE-link' href='#'>link</a>"
            + "<button id='BUTTON-ACTION' class='btn'>a</button>"
            + "<button id='IGNORE-button'>b</button>"
            + '</div>'
            + "<table id='jquery-data-table'>"
                + "<thead><tr>"
                    + "<th><input id='datatable-check-all' type='checkbox'></th>"
                    + "<th>Col 00</th>"
                    + "<th>Col 01</th>"
                    + "<th>Col 02</th>"
                    + "<th></th>"
                + "</tr></thead>"
                + "<tfoot><tr>"
                    + "<th></th>"
                    + "<th data-is-searchable='true'><input type='text' value='00' /></th>"
                    + "<th data-is-searchable='true'><input type='text' value='00' /></th>"
                    + "<th style='white-space: nowrap;'>"
                    + '<span title="Search" class="btn search-icons glyphicon glyphicon-search"></span>'
                    + '<span title="Clear Search and Reload" class="btn search-icons glyphicon glyphicon-repeat"></span>'
                    + '<span title="Save As..." class="btn btn-default glyphicon glyphicon-download-alt" id="datatable-save-as"></span>'
                    + "</th>"
                + "</tr></tfoot>"
                + "<tbody><tr>"
                    + "<td><input type='checkbox' /></td>"
                    + "<td>Row 1 data cell 00</td>"
                    + "<td>Row 1 data cell 01</td>"
                    + "<td>Row 1 data cell 02</td>"
                    + "<td>"
                        + "<span class='glyphicon-edit'></span>"
                        + "<span class='glyphicon-remove-circle'><span></span></span>"
                    + "</td>"
                + "</tr></tbody>"
            + "</table>"
            );
        });

        it('calls the action button click handlers', function () {
            spyOn(configTable, 'clickActionButton');

            configTable.init();
            // not a button => ignore
            var linkIgnore = document.querySelector('#IGNORE-link');
            linkIgnore.dispatchEvent(new Event('click'));
            // button without class 'btn' => ignore
            var buttonIgnore = document.querySelector('#IGNORE-button');
            buttonIgnore.dispatchEvent(new Event('click'));
            // target match => handle event
            var actionButtonMatch = document.querySelector('#BUTTON-ACTION');
            actionButtonMatch.dispatchEvent(new Event('click'));

            expect(configTable.clickActionButton.calls.count()).toEqual(1);
        });

        it('calls the checkAll checkbox click handler', function () {
            spyOn(configTable, 'clickCheckAll');

            configTable.init();
            var checkAll = document.querySelector(configTable.getCheckAllId());
            checkAll.dispatchEvent(new Event('click'));

            expect(checkAll.tagName).toMatch(/^input$/i);
            expect(checkAll.getAttribute('type')).toBe('checkbox');
            expect(configTable.clickCheckAll.calls.count()).toEqual(1);
        });

        it('calls the table click handler', function () {
            spyOn(configTable, 'clickTable');

            configTable.init();
            // addEventListener() with last parameter === false
            // so no need to test child elements
            var table = document.querySelector(tableId);
            table.dispatchEvent(new Event('click'));

            expect(table.tagName).toMatch(/^table$/i);
            expect(configTable.clickTable.calls.count()).toEqual(1);
        });

        it('calls the search icon click handlers', function () {
            spyOn(configTable, 'clickSearch');

            configTable.init();
            var searchIcons = document.querySelectorAll('tfoot span.search-icons');
            searchIcons[0].dispatchEvent(new Event('click'));
            searchIcons[1].dispatchEvent(new Event('click'));

            expect(searchIcons.length).toEqual(2);
            expect(configTable.clickSearch.calls.count()).toEqual(2);
        });

        it('calls the saveAs icon click handler', function () {
            spyOn(configTable, 'clickSaveAs');

            configTable.init();
            var saveAs = document.querySelector(configTable.getSaveAsId());
            saveAs.dispatchEvent(new Event('click'));

            expect(configTable.clickSaveAs.calls.count()).toEqual(1);
        });

        it('calls the search input fieldse keyup handler', function() {
            spyOn(configTable, 'keyupSearch');

            configTable.init();
            var searchBoxes = document.querySelectorAll(tableId + ' tfoot input[type=text]');
            searchBoxes[0].dispatchEvent(new Event('keyup'));
            searchBoxes[1].dispatchEvent(new Event('keyup'));

            expect(searchBoxes.length).toEqual(2);
            expect(configTable.keyupSearch.calls.count()).toBe(2);
        });
    });

    /* ========================================================================
       event listener functions - verify EventTarget and correct behavior
       ========================================================================
    */
    describe('clickActionButton', function() {
        var template, event;
        beforeEach(function() {
            template = document.createElement('div');
            event = {
                preventDefault: jasmine.createSpy()
            };
            spyOn(configTable, 'sendXhr');
        });

        it('shows a modal error when a button does not have a data URL', function() {
            spyOn(configTable, 'getSelectedRowIds')
            spyOn(configTable, 'jqModalError');
            template.innerHTML = '<button class="btn btn-primary">Batch Update<span></span></button>';
            event.target = template.firstChild;

            var result = configTable.clickActionButton(event);

            expect(result).toEqual(false);
            expect(event.preventDefault).toHaveBeenCalledTimes(1);
            expect(configTable.getSelectedRowIds).not.toHaveBeenCalled();
            expect(configTable.sendXhr).not.toHaveBeenCalled();
            expect(configTable.jqModalError).toHaveBeenCalledWith(
                configTable.getInvalidUrlMessage()
            );
        });

        it('shows a modal error when no rows are selected for a bulk action', function () {
            spyOn(configTable, 'getSelectedRowIds').and.returnValue([]);
            spyOn(configTable, 'jqModalError');
            template.innerHTML = '<button class="btn btn-primary" data-url="/action">Batch Update<span></span></button>';
            event.target = template.firstChild;

            var result = configTable.clickActionButton(event);

            expect(result).toEqual(false);
            expect(event.preventDefault).toHaveBeenCalledTimes(1);
            expect(configTable.getSelectedRowIds).toHaveBeenCalledTimes(1);
            expect(configTable.sendXhr).not.toHaveBeenCalled();
            expect(configTable.jqModalError.calls.mostRecent().args[0]).toMatch('<h2>No Records Selected</h2>');
        });

        it('sends XHR when rows are selected for a bulk action', function() {
            spyOn(configTable, 'getSelectedRowIds').and.returnValue([1, 2]);
            template.innerHTML = '<button class="btn btn-primary" data-url="/action">Batch Update<span></span></button>';
            event.target = template.firstChild;

            var result = configTable.clickActionButton(event);

            expect(result).toEqual(false);
            expect(event.preventDefault).toHaveBeenCalledTimes(1);
            expect(configTable.getSelectedRowIds).toHaveBeenCalledTimes(1);
            expect(configTable.sendXhr).toHaveBeenCalledWith(
                event.target, '/action', { ids: [1, 2] }
            );
        });

        it('sends XHR GET with null data for a modal view action', function() {
            template.innerHTML = "<button class='btn btn-primary' data-url='/action'"
                + ' data-modal="">'
                + '<span></span></button>';
            event.target = template.firstChild;

            var result = configTable.clickActionButton(event);

            expect(result).toEqual(false);
            expect(event.preventDefault).toHaveBeenCalledTimes(1);
            expect(configTable.sendXhr).toHaveBeenCalledWith(
                event.target, '/action', null, 'GET'
            );
        });
    });

    // click 'datatable-check-all' checkbox - [un]check all checkboxes 
    describe('clickCheckAll', function() {
        var event, checked, unchecked, checkAllId;
        beforeEach(function() {
            event = {};
            checked = configTable.getCheckedSelector();
            unchecked = configTable.getUncheckedSelector();
            checkAllId = configTable.getCheckAllId();
            setFixtures(
                "<div><input id='datatable-check-all' type='checkbox' /></div>"
                + "<div id='WANTED'>"
                + "<input type='checkbox' />"
                + "<input type='checkbox' checked='checked' />"
                + '</div>'
            );
        });

        it('unchecks all checkboxes when clickAll is unchecked', function() {
            event.target = document.querySelector(checkAllId);

            configTable.clickCheckAll(event);
            var checkboxes = document.querySelectorAll('#WANTED input[type=checkbox]');

            expect(checkboxes.length).toBe(2);
            for (var i = 0; i < checkboxes.length; ++i) {
                expect(checkboxes[i].checked).toBe(false);
            }
        });

        it('checks all checkboxes when clickAll is checked', function() {
            var checkAll = document.querySelector(checkAllId);
            checkAll.checked = true;
            event.target = checkAll;

            configTable.clickCheckAll(event);
            var checkboxes = document.querySelectorAll('#WANTED input[type=checkbox]');

            expect(checkboxes.length).toBe(2);
            for (var i = 0; i < checkboxes.length; ++i) {
                expect(checkboxes[i].checked).toBe(true);
            }
        });
    });

    describe('clickSearch', function() {
        var template, event;
        beforeEach(function() {
            template = document.createElement('div');
            event = {};
        });

        it('does not execute search for a non matching event target', function() {
            spyOn(configTable, 'search');
            spyOn(configTable, 'clearSearchFilters');
            spyOn(configTable, 'reload');
            template.innerHTML = "<span class='NO-MATCH' title='Search'></span>";
            event.target = template.firstChild;

            configTable.clickSearch(event);
            expect(configTable.search).not.toHaveBeenCalled();
            expect(configTable.clearSearchFilters).not.toHaveBeenCalled();
            expect(configTable.reload).not.toHaveBeenCalled();
        });

        it('executes search when icon is clicked', function() {
            spyOn(configTable, 'search');
            template.innerHTML = "<span class='search-icons glyphicon glyphicon-search' title='Search'></span>";
            event.target = template.firstChild;

            configTable.clickSearch(event);
            expect(configTable.search).toHaveBeenCalledTimes(1);
        });

        it('clears the search and reloads data when icon is clicked', function() {
            spyOn(configTable, 'clearSearchFilters');
            spyOn(configTable, 'reload');
            template.innerHTML = "<span class='search-icons glyphicon glyphicon-repeat title='Clear Search'></span>";
            event.target = template.firstChild;

            configTable.clickSearch(event);
            expect(configTable.clearSearchFilters).toHaveBeenCalledTimes(1);
            expect(configTable.reload).toHaveBeenCalledTimes(1);
        });
    });

    describe('clickTable link', function() {
        var event, config, recordId;
        beforeEach(function() {
            var infoLink =
                "<span class='glyphicon glyphicon-info-sign blue link-icons' data-action='"
                + configTable.getInfoAction()
                + "' title='Information'></span>";

            var editLink =
                "<span class='glyphicon glyphicon-edit green link-icons' data-action='"
                + configTable.getEditAction()
                + "' title='Edit'></span>";

            var deleteLink =
                "<span class='glyphicon glyphicon-remove-circle red link-icons' data-action='"
                + configTable.getDeleteAction()
                + "' title='Delete'><span></span></span>";

            event = {};
            config = configTable.getConfigValues();
            setFixtures('<table><tr><td>'
                + [infoLink, editLink, deleteLink].join(' ')
                + "<span class='NO-MATCH'><span></span></span>"
                + '</td></tr></table>'
            );
            recordId = 1;
            spyOn(configTable, 'getRowData').and.returnValue(recordId);
            spyOn(configTable, 'getConfigValues').and.returnValue(config);
        });

        it('ignores a non-matching event target', function() {
            spyOn(configTable, 'sendXhr');
            spyOn(configTable, 'redirect');
            spyOn(configTable, 'clearCheckAll');

            event.target = document.querySelector('span.NO-MATCH');
            configTable.clickTable(event);

            expect(event.target.tagName.toLowerCase()).toEqual('span');
            expect(configTable.sendXhr).not.toHaveBeenCalled();
            expect(configTable.redirect).not.toHaveBeenCalled();
            expect(configTable.getConfigValues).not.toHaveBeenCalled();
            expect(configTable.getRowData).not.toHaveBeenCalled();
            expect(configTable.clearCheckAll).not.toHaveBeenCalled();
        });

        it('redirects to the info page', function() {
            spyOn(configTable, 'redirect');

            event.target = document.querySelector('span[data-action=info]');
            // event.target = document.querySelector('span.glyphicon-edit');
            var row = document.querySelector('tr');
            configTable.clickTable(event);

            expect(event.target.tagName.toLowerCase()).toEqual('span');
            expect(configTable.getConfigValues).toHaveBeenCalledTimes(1);
            expect(configTable.getRowData).toHaveBeenCalledWith(row);
            expect(configTable.redirect).toHaveBeenCalledWith(
                config.infoRowUrl + '/' + recordId
            );
        });

        it('redirects to the edit page', function() {
            spyOn(configTable, 'redirect');

            event.target = document.querySelector('span[data-action=edit]');
            var row = document.querySelector('tr');
            configTable.clickTable(event);

            expect(event.target.tagName.toLowerCase()).toEqual('span');
            expect(configTable.getConfigValues).toHaveBeenCalledTimes(1);
            expect(configTable.getRowData).toHaveBeenCalledWith(row);
            expect(configTable.redirect).toHaveBeenCalledWith(
                config.editRowUrl + '/' + recordId
            );
        });

        it('deletes the selected record', function() {
            spyOn(configTable, 'sendXhr');
            spyOn(configTable, 'clearCheckAll');

            var span = document.querySelector('span[data-action=delete]');
            var row = document.querySelector('tr');
            event.target = span;
            configTable.clickTable(event);

            expect(event.target.tagName.toLowerCase()).toEqual('span');
            expect(configTable.sendXhr).toHaveBeenCalledWith(
                span, config.deleteRowUrl, { id: recordId }
            );
            expect(configTable.getConfigValues).toHaveBeenCalledTimes(1);
            expect(configTable.getRowData).toHaveBeenCalledWith(row);
            expect(configTable.clearCheckAll).toHaveBeenCalledTimes(1);
        });
    });

    describe('clickTable checkbox', function() {
        var event, selectedClass;
        beforeEach(function() {
            event = {};
            selectedClass = configTable.getSelectedRowClass();
            spyOn(configTable, 'getSelectedRowClass')
                .and.returnValue(selectedClass);
        });

        it('adds the selected row class when checkbox is checked', function() {
            setFixtures('<tr>'
                + "<td><input type='checkbox' checked='checked' /></td>"
                + "<td></td>"
                + '</tr>'
            );
            var row = document.querySelector('tr');
            var checkbox = row.querySelector('input[type="checkbox"]:checked');
            event.target = checkbox;

            configTable.clickTable(event);

            expect(event.target.type).toEqual('checkbox');
            expect(event.target.checked).toEqual(true);
            expect(configTable.getSelectedRowClass).toHaveBeenCalledTimes(1);
            expect(row.classList.contains(selectedClass)).toEqual(true);
        });

        it('removes the selected row class when checkbox is not checked', function() {
            setFixtures('<tr class="' + selectedClass + '">'
                + "<td><input type='checkbox' /></td>"
                + "<td></td>"
                + '</tr>'
            );
            var row = document.querySelector('tr');
            var checkbox = row.querySelector('input[type="checkbox"]:not(:checked)');
            event.target = checkbox;

            configTable.clickTable(event);

            expect(event.target.type).toEqual('checkbox');
            expect(event.target.checked).toEqual(false);
            expect(configTable.getSelectedRowClass).toHaveBeenCalledTimes(1);
            expect(row.classList.length).toEqual(0);
        });
    });

    describe('keyupSearch', function() {
        var event;
        beforeEach(function() {
            spyOn(configTable, 'search');
            event = {};
        });

        it('executes search when KeyboardEvent.key is [Enter]', function() {
            event.key = 'Enter';

            configTable.keyupSearch(event);
            expect(configTable.search).toHaveBeenCalledTimes(1);
        });

        it('does not execute search when KeyboardEvent.key is not [Enter]', function() {
            event.key = 'Escape';

            configTable.keyupSearch(event);
            expect(configTable.search).not.toHaveBeenCalled();
        });
    });

    /* ========================================================================
        value picker       
       ========================================================================
    */
    describe('value picker', function() {
        var col1 = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        var col2 = ['zero', 'one', 'two', 'three', 'four'];
        var pickerCount = col1.length + col2.length
        var selectedClass = '', table = null,
            valuePickers = null, valuePickerCount = 0;

        var clickAllValuePickerItems = function(elementList) {
            for (var i = 0; i < elementList.length; ++i) {
                // select **all** picker items => fill input text field
                var itemList = valuePickers[i].querySelectorAll('div.pickerItem');
                for (var j = 0; j < itemList.length; ++j) {
                    itemList[j].dispatchEvent(new Event('click', { bubbles: true }));
                    // event handler set on parent element node  ^^^^^^^^^^^^^^^^^
                }
            }
        }

        beforeEach(function() {
            selectedClass = configTable.getSelectedSelector();
            setFixtures(
                "<table id='jquery-data-table'><tfoot></tr>"
                + "<th rowspan='1' colspan='1'></th>"
                + "<th><input type='text' data-column-number='1' /></th>"
                + "<th><input type='text' data-column-number='2' /></th>"
                + "<th><input type='text' data-column-number='3' /></th>"
                + "<th><input type='text' data-column-number='4' /></th>"
                + '</tr><tfoot></table>'
            );

            spyOn(configTable, 'enterSearchInput').and.callThrough();
            spyOn(configTable, 'resetValuePicker');
            spyOn(configTable, 'togglePickerItem').and.callThrough();
            spyOn(configTable, 'getSelectedSelector').and.returnValue(selectedClass);

            configTable.addValuePicker(1, col1);
            configTable.addValuePicker(2, col2);
            configTable.addValuePicker(3, 'this should be ignored, since not array');

            table = document.querySelector(configTable.getTableId());
            valuePickers = table.querySelectorAll('div.valuePicker');
            valuePickerCount = valuePickers.length;
        });

        it('creates the value pickers with correct ids', function() {
            expect(valuePickers.length).toEqual(2);
            expect(valuePickers[0].id).toEqual('valuePickerId__0');
            expect(valuePickers[1].id).toEqual('valuePickerId__1');
        });

        it('hides the value pickers when created', function() {
            expect(valuePickers[0].style.display).toEqual('none');
            expect(valuePickers[1].style.display).toEqual('none');
        });

        it('adds overflowY style when the value picker child item count is greater than 10', function() {
            configTable.addValuePicker(4, [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);

            // querySelectorAll() => non-live NodeList of element objects, so re-query
            var localValuePickers = table.querySelectorAll('div.valuePicker');

            expect(localValuePickers.length).toEqual(3);

            // gt 10
            expect(localValuePickers[0].children[0].style.overflowY).toEqual('auto');
            // lt 10
            expect(localValuePickers[1].children[0].style.overflowY).toEqual('');
            // eq 10
            expect(localValuePickers[2].children[0].style.overflowY).toEqual('');
        });

        it('creates the value picker items', function() {
            var columns = [col1.length, col2.length];

            for (var i = 0; i < valuePickerCount; ++i) {
                expect(valuePickers[i].querySelectorAll('div.pickerItem').length)
                    .toEqual(columns[i]);
            }
        });

        it('creates the value picker input button', function() {
            for (var i = 0; i < valuePickerCount; ++i) {
                var button = valuePickers[i].querySelectorAll('input[type=button]');

                expect(button.length).toEqual(1);
                expect(button[0].value).toEqual('Add / Clear');
            }
        });

        it('displays the value picker when entering the search field', function () {
            for (var i = 0; i < valuePickerCount; ++i) {
                var selector = "th > input[data-column-number='" + (i + 1) + "']";
                document.querySelector(selector).dispatchEvent(new Event('focus'));

                expect(valuePickers[i].style.display).toEqual('block');
            }

            expect(configTable.enterSearchInput.calls.count()).toEqual(2);
        });

        it('adds and removes the selected class when a picker item is clicked', function() {
            // first click add 'select' class to each item
            clickAllValuePickerItems(valuePickers);
            expect(configTable.togglePickerItem.calls.count()).toEqual(pickerCount);
            expect(configTable.getSelectedSelector.calls.count()).toEqual(pickerCount);
            for (var i = 0; i < valuePickerCount; ++i) {
                var elementList = valuePickers[i].querySelectorAll('div.pickerItem');
                for (var j = 0; j < elementList.length; ++j) {
                    expect(elementList[j].classList.contains(selectedClass)).toEqual(true);
                }
            }

            // second click **REMOVE** 'select' class from each item
            clickAllValuePickerItems(valuePickers);
            expect(configTable.togglePickerItem.calls.count()).toEqual(pickerCount * 2);
            expect(configTable.getSelectedSelector.calls.count()).toEqual(pickerCount * 2);
            for (var i = 0; i < valuePickerCount; ++i) {
                var elementList = valuePickers[i].querySelectorAll('div.pickerItem');
                for (var j = 0; j < elementList.length; ++j) {
                    expect(elementList[j].classList.contains(selectedClass)).toEqual(false);
                }
            }
        });

        describe('value picker button', function() {
            var multiValueFilterSeparator = '|'
            beforeEach(function() {
                configTable.setConfigValues({multiValueFilterSeparator: multiValueFilterSeparator});
                clickAllValuePickerItems(valuePickers);

                for (var i = 0; i < valuePickerCount; ++i) {
                    valuePickers[i].querySelector('input[type=button]')
                        .dispatchEvent(new Event('click'));
                }
                spyOn(configTable, 'valuePickerButtonClick').and.callThrough();
            });

            it('populates the search fields with each selected item', function() {
                var textInputs = document.querySelectorAll('th > input[data-column-number]');

                // HTML fixture
                expect(textInputs.length).toEqual(4);

                // first two search inputs have selected value picker text
                expect(textInputs[0].value).toEqual(col1.join(multiValueFilterSeparator));
                expect(textInputs[1].value).toEqual(col2.join(multiValueFilterSeparator));

                // last two search inputs do **not** have value picker, and are empty
                expect(textInputs[2].value).toEqual('');
                expect(textInputs[3].value).toEqual('');
            });

            it('resets value picker items and hides the value picker', function() {
                expect(configTable.resetValuePicker.calls.count()).toEqual(valuePickers.length);
                for (var i = 0; i < valuePickerCount; ++i) {
                    expect(valuePickers[i].style.display).toEqual('none');
                }
            });
        });
    });
});