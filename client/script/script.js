$(document).ready(function () {
    // Veritabanı tipi değiştiğinde formu güncelle
    $('#databaseType').on('change', function () {
        const databaseType = $(this).val();

        // Input alanlarını sıfırlayalım
        $('#serverName').val('');
        $('#databaseName').val('');
        $('#username').val('');
        $('#password').val('');
        $('#pgAdminPort').val('5432');
        $('#tableName').empty().append('<option value="">Select a table</option>');

        // Bütün özel alanları gizleyelim
        $('#pgAdminPortField').hide();
        $('#sqlFields').show();
        $('#getTablesBtn').prop('disabled', true);

        // İlgili veritabanı tipine göre alanları göster
        if (databaseType === '0' || databaseType === '1') { // SQL Server ve MySQL
            $('#pgAdminPortField').hide();
        } else if (databaseType === '2') { // PGAdmin
            $('#pgAdminPortField').show();
        }

        // Butonun aktif olup olmadığını kontrol et
        checkSubmitButton();
    });

    // Kullanıcı veritabanı bilgilerini girdikçe butonu aktif et
    $('#chartForm input, #chartForm select').on('change', function () {
        checkSubmitButton();
    });

    // Butonun aktif olup olmadığını kontrol et
    function checkSubmitButton() {
        const serverName = $('#serverName').val();
        const databaseName = $('#databaseName').val();
        const username = $('#username').val();
        const password = $('#password').val();
        const databaseType = $('#databaseType').val();
        const pgAdminPort = $('#pgAdminPort').val();
        const tableName = $('#tableName').val();

        // Eğer tableName boşsa, submit butonunu devre dışı bırak
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

    // Toast mesajını gösteren fonksiyon
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

    // Veritabanı tablolarını alma
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
                console.log(response.data);
                response.data.forEach(function (table) {
                    tableSelect.append('<option value="' + table + '">' + table + '</option>');
                });

                showToast('Tables fetched successfully!', false);
                checkSubmitButton(); // Tablo listesi alındıktan sonra butonları kontrol et
            },
            error: function (error) {
                const errorMessage = error.responseJSON?.message || 'Failed to fetch tables';
                showToast('Error: ' + errorMessage, true);
            }
        });
    });

    let chartInstance = null;

    // Grafik verisi gönderimi
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
                const labels = response.data.map(item => item.Label);
                const variables = response.data.map(item => item.Data);
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

    // "Clear Chart" butonunun işlevi
    $('#clearChartBtn').on('click', function () {
        if (chartInstance) {
            chartInstance.destroy(); // Grafiği sil
        }
        $('#myChart').remove(); // Grafik alanını temizle
        $('.card-body').append('<canvas id="myChart" width="400" height="200"></canvas>'); // Yeni bir canvas ekle
        showToast('Chart cleared!', false);
        $('#generateChartBtn').prop('disabled', false); // "Generate Chart" butonunu tekrar aktif et
    });
});
