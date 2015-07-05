/// <reference path="~/Scripts/jquery-2.1.4.js"/>
/// <reference path="~/Scripts/jquery-2.1.4.intellisense.js"/>
/// <reference path="~/Scripts/DataTables/jquery.dataTables.js"/>
/// <reference path="~/Scripts/handlebars.js"/>
/// <reference path="~/Scripts/bootbox.js" />

$(document).ready(function () {

    if (typeof $.fn.dataTable != "undefined") {

        $.fn.dataTableExt.templateCache = {
            get: function (selector) {
                if (!this.templates) { this.templates = {}; }

                if (!this.templates[selector]) {
                    var source = $(selector).html();
                    this.templates[selector] = Handlebars.compile(source);
                }

                return this.templates[selector];
            }
        };

        // set the defaults for dataTables initialization
        $.extend(true, $.fn.dataTable.defaults, {
            processing: true,
            serverSide: true,

            ajax: {
                type: "POST",
                data: function (d) {
                    // add data from custom filters
                    $($('.table-filter form').serializeArray()).each(function (index, item) {
                        d.push(item);
                    });
                }
            },

            rowCallback: function (row, data, index) {
                var $row = $(row);
                var $table = $(this);

                // assumes id property is the first
                $row.attr('data-id', data[0]);

                // assumes action column is last column in table
                var $actions = $row.find('td').last();
                $actions.attr('nowrap', 'nowrap');

                // process template cells
                $table.find('th[data-template]').each(function (index, column) {
                    var $column = $(column);
                    var templateName = $column.data('template');

                    if (templateName) {
                        var template = $.fn.dataTableExt.templateCache.get('#' + templateName);
                        var html = template(data);

                        var $tdCell = $row.find('td').eq($column.index());
                        $tdCell.html(html);
                    }
                });

                // enable confirmation dialog
                $('a[data-confirm]', $row).each(function (index, anchor) {
                    $(anchor).click(function (e) {
                        e.preventDefault();
                        var $anchor = $(this);
                        bootbox.confirm($anchor.attr("data-confirm"), function (result) {
                            if (result) {
                                window.location.replace($anchor.attr("href"));
                            }
                        });
                    });
                });

                return row;
            },

            drawCallback: function (settings) {
                // process detail rows
                $(settings.nTHead).find('tr[data-detailtemplate]').each(function (index, headerRow) {
                    var $dt = $(this).closest('table').DataTable();

                    var $headerRow = $(headerRow);
                    var templateName = $headerRow.data('detailtemplate');

                    if (templateName) {
                        var template = $.fn.dataTableExt.templateCache.get('#' + templateName);

                        // loop over each row and add detail row
                        $.each(settings.aoData, function (index, data) {
                            $('td.details-control', data.nTr).click(function () {
                                var $tr = $(this).closest('tr');
                                var $row = $dt.row($tr);

                                if ($row.child.isShown()) {
                                    $row.child.hide();
                                    $tr.removeClass('shown');
                                }
                                else {
                                    var $html = template($row.data());
                                    $row.child($html).show();
                                    $tr.addClass('shown');
                                }
                            });
                        });
                    }
                });
            }
        });

        var dataTableFilterTimeout;
        $.fn.dataTableExt.oApi.fnSetFilteringEnterPress = function () {
            var that = this;
            this.each(function (i) {
                $.fn.dataTableExt.iApiIndex = i;
                var anControl = $('input', that.fnSettings().aanFeatures.f);
                anControl.unbind().bind('keyup change', function (e) {
                    window.clearTimeout(dataTableFilterTimeout);
                    if (anControl.val().length == 0 || anControl.val().length > 2 || e.keyCode == 13) {
                        dataTableFilterTimeout = setTimeout(function () {
                            that.fnFilter(anControl.val());
                        }, 250); // number of milliseconds to wait before firing filter
                    }
                });
                return this;
            });
            return this;
        };
    }
});