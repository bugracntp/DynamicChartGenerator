$(document).ready(function () {
    $('#databaseType').on('change', function () {
        const databaseType = $(this).val();

        $('#serverName').val('');
        $('#databaseName').val('');
        $('#username').val('');
        $('#password').val('');
        $('#pgAdminPort').val('5432');
        $('#tableName').empty().append('<option value="">Select a table</option>');
        $('#pgAdminPortField').hide();
        $('#sqlFields').show();
        $('#getTablesBtn').prop('disabled', true);

        if (databaseType === '0' || databaseType === '1') { 
            $('#pgAdminPortField').hide();
        } else if (databaseType === '2') { 
            $('#pgAdminPortField').show();
        }

        checkSubmitButton();
    });

    $('#chartForm input, #chartForm select').on('change', function () {
        checkSubmitButton();
    });

    function checkSubmitButton() {
        const serverName = $('#serverName').val();
        const databaseName = $('#databaseName').val();
        const username = $('#username').val();
        const password = $('#password').val();
        const databaseType = $('#databaseType').val();
        const pgAdminPort = $('#pgAdminPort').val();
        const tableName = $('#tableName').val();

        if (serverName && databaseName && username && password && (databaseType === '3' ? pgAdminPort : true) && tableName) {
            $('#generateChartBtn').prop('disabled', false);
        } else {
            $('#generateChartBtn').prop('disabled', true);
        }

        if (serverName && databaseName && username && password && (databaseType === '3' ? pgAdminPort : true)) {
            $('#getTablesBtn').prop('disabled', false);
        } else {
            $('#getTablesBtn').prop('disabled', true);
        }
    }

    function showToast(message, isError = false) {
        const toastBody = $('#toastMessage .toast-body');
        toastBody.text(message);

        if (isError) {
            $('#toastMessage').removeClass('bg-success').addClass('bg-danger');
        } else {
            $('#toastMessage').removeClass('bg-danger').addClass('bg-success');
        }

        const toast = new bootstrap.Toast($('#toastMessage')[0]);
        toast.show();
    }

    $('#getTablesBtn').on('click', function () {
        const chartRequest = {
            ServerName: $('#serverName').val(),
            DatabaseName: $('#databaseName').val(),
            UserName: $('#username').val(),
            Password: $('#password').val(),
            DatabaseType: parseInt($('#databaseType').val()),
            Port: $('#databaseType').val() === '2' ? parseInt($('#pgAdminPort').val()) : null
        };

        $.ajax({
            url: 'http://localhost:5146/api/Chart/GetTablesFromDatabase',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(chartRequest),
            success: function (response) {
                const tableSelect = $('#tableName');
                tableSelect.empty().append('<option value="">Select a table</option>');
                response.data.forEach(function (table) {
                    tableSelect.append('<option value="' + table + '">' + table + '</option>');
                });

                showToast('Tables fetched successfully!', false);
                checkSubmitButton(); 
            },
            error: function (error) {
                const errorMessage = error.responseJSON?.message || 'Failed to fetch tables';
                showToast('Error: ' + errorMessage, true);
            }
        });
    });

    let chartInstance = null;

    $('#chartForm').on('submit', function (e) {
        e.preventDefault();

        const chartRequest = {
            DatabaseType: parseInt($('#databaseType').val()),
            ServerName: $('#serverName').val(),
            DatabaseName: $('#databaseName').val(),
            UserName: $('#username').val(),
            Password: $('#password').val(),
            Port: $('#databaseType').val() === '2' ? parseInt($('#pgAdminPort').val()) : null,
            TableName: $('#tableName').val(),
            ChartType: parseInt($('#chartType').val())
        };

        $.ajax({
            url: 'http://localhost:5146/api/Chart/GetChartData',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(chartRequest),
            success: function (response) {

                const firstItem = response.data[0];
                const keys = Object.keys(firstItem).filter(key => key.toLowerCase() !== 'id');

                const dataKey = keys.find(key => typeof firstItem[key] === 'number');
                const labelKey = keys.find(key => typeof firstItem[key] === 'string');
                
                if (!labelKey || !dataKey) {
                    showToast('Error: Could not determine keys for labels or data', true);
                    return;
                }
                const labels = response.data.map(item => item[labelKey]);
                const variables = response.data.map(item => item[dataKey]);
                const ctx = document.getElementById('myChart').getContext('2d');

                if (chartInstance) {
                    chartInstance.destroy();
                }

                chartInstance = new Chart(ctx, {
                    type: chartRequest.ChartType === 0 ? "line" : chartRequest.ChartType === 1 ? "bar" : "radar",
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Dataset',
                            data: variables,
                            backgroundColor: 'rgba(75, 192, 192, 0.2)',
                            borderColor: 'rgba(75, 192, 192, 1)',
                            borderWidth: 1
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: { position: 'top' }
                        }
                    }
                });

                showToast('Chart generated successfully!', false);
            },
            error: function (error) {
                const errorMessage = error.responseJSON?.message || 'Unexpected error occurred';
                showToast('Error: ' + errorMessage, true);
            }
        });
    });

    $('#clearChartBtn').on('click', function () {
        if (chartInstance) {
            chartInstance.destroy(); 
        }
        $('#myChart').remove(); 
        $('.card-body').append('<canvas id="myChart" width="400" height="200"></canvas>');
        showToast('Chart cleared!', false);
        $('#generateChartBtn').prop('disabled', false);
    });
});
