
        $(function () {
            // init dates
            const today = new Date().toISOString().slice(0, 10);
        const ytd = new Date(new Date().getFullYear(), 0, 1).toISOString().slice(0, 10);
        $('#dateTo').val(today); $('#dateFrom').val(today);

        // account autocomplete
        $('#accountPicker').autocomplete({
            minLength: 2,
        source: function (req, res) {
            $.getJSON('/api/accounts/lookup', { q: req.term }, function (data) {
                res($.map(data, function (x) { return { label: x.text, value: x.text, id: x.id }; }));
            });
                },
        select: function (e, ui) {$('#accountId').val(ui.item.id); }
            });

        
        const txModalEl = document.getElementById('txModal');
        const txModal = new bootstrap.Modal(txModalEl);

        
        const table = $('#statementTable').DataTable({
            processing: true,
        serverSide: true,
        searching: true,
        orderMulti: false,
        lengthMenu: [10, 25, 50, 100],
        ajax: {
            url: '/api/statements/load',
        type: 'POST',
        data: function (d) {
            d.accountId = $('#accountId').val();
        d.dateFrom = $('#dateFrom').val();
        d.dateTo = $('#dateTo').val();
                    },
                    beforeSend: () => $('body').addClass('loading'),
                    complete: (xhr) => {
            $('body').removeClass('loading');
        const totals = xhr.responseJSON && xhr.responseJSON.totals;
        if (totals) {
            $('#ftPrev').text(totals.FirstPreviousBalance);
        $('#ftDebit').text(totals.TotalDebits);
        $('#ftCredit').text(totals.TotalCredits);
        $('#ftFinal').text(totals.AccountFinalBalance);
                        }
                    }
                },
        columns: [
        {data: 'AccountId', name: 'AccountId' },
        {data: 'AccountName', name: 'AccountName' },
        {
            data: 'TxnDate', name: 'TxnDate',
        render: function (d) {
                            if (!d) return '';
        const dt = new Date(d);
        return isNaN(dt) ? d : dt.toLocaleDateString();
                        }
                    },
        {data: 'PreviousBalance', name: 'PreviousBalance' },
        {data: 'Debit', name: 'Debit' },
        {data: 'Credit', name: 'Credit' },
        {data: 'FinalBalance', name: 'FinalBalance' },
        {
            data: 'BalanceHistoryId', orderable: false, searchable: false,
        render: function (id) {
                            return `<button class="btn btn-sm btn-outline-primary view-tx" data-id="${id}">Details</button>`;
                        }
                    }
        ]
            });


        //$("#printBtn").on("click", function () {
        //    window.print();
        //});

            $("#printBtn").on("click", function () {
                var divToPrint = document.getElementById("statementTable");
                var newWin = window.open("");

                newWin.document.write(`
    <html>
      <head>
        <title>Print Table</title>
        <style>
          @media print {
            @page {
              size: A4 landscape;
              margin: 10mm;
            }
          }
          table {
            border-collapse: collapse;
            width: 100%;
          }
          table, th, td {
            border: 1px solid black;
            padding: 2px;
          }
        </style>
      </head>
      <body>
        ${divToPrint.outerHTML}
      </body>
    </html>
  `);

                newWin.document.close();
                newWin.print();
                newWin.close();
            });



        $('#btnSearch').on('click', function () {
                if (!$('#accountId').val()) {alert('Select account'); return; }
        if (!$('#dateFrom').val() || !$('#dateTo').val()) {alert('Select dates'); return; }
        table.ajax.reload();
            });

        $('#btnYTD').on('click', function () {
            $('#dateFrom').val(ytd);
        table.ajax.reload();
            });

        $('#statementTable').on('click', '.view-tx', function () {
                const id = $(this).data('id');
        if (!id) return;

       
        txModal.show();

        // Clear Data
        $('#detailAccountId,#detailAccountName,#detailTxnDate,#detailPrevious,#detailDebit,#detailCredit,#detailFinal,#detailDescription').text('...');

        // Get Details From API
        $.get(`/api/statements/details/${id}`, function (d) {
            $('#detailAccountId').text(d.AccountId);
        $('#detailAccountName').text(d.AccountName ?? '');
        $('#detailTxnDate').text(d.TxnDate ? new Date(d.TxnDate).toLocaleString() : '');
        $('#detailPrevious').text(d.PreviousBalance);
        $('#detailDebit').text(d.Debit);
        $('#detailCredit').text(d.Credit);
        $('#detailFinal').text(d.FinalBalance);
        $('#detailDescription').html(d.Description ?? '');

                });
            });

        $('#btnExport').on('click', function () {
                const id = $('#accountId').val();
        const f = $('#dateFrom').val();
        const t = $('#dateTo').val();
        if (!id) {alert('Select account'); return; }
        window.location = `/api/statements/export?accountId=${id}&dateFrom=${f}&dateTo=${t}`;
            });

        // Loading Style
        const style = `<style>.loading:before{content:"";position:fixed;inset:0;background:#0003;z-index:9999}
            .loading:after{content:"Loading...";position:fixed;top:50%;left:50%;transform:translate(-50%,-50%);background:#fff;padding:12px 16px;border-radius:8px;z-index:10000}</style>`;
        document.head.insertAdjacentHTML('beforeend', style);
        });
