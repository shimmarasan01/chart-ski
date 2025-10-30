const API_BASE = "http://localhost:5121/api";
let uploadedData = null;
let uploadedFileName = null;
let chartInstance = null;

$(document).ready(function () {

    const $fileInput = $("#fileInput");
    const $uploadBtn = $("#uploadBtn");
    const $uploadResult = $("#uploadResult");
    const $labelSelect = $("#labelSelect");
    const $valueSelect = $("#valueSelect");
    const $chartType = $("#chartType");
    const $visualiseBtn = $("#visualiseBtn");
    const $visualiseStatus = $("#visualiseStatus");
    const $chartContainer = $("#chartContainer");
    const chartCanvas = $("#chartCanvas")[0].getContext("2d");

    $uploadBtn.on("click", function () {
        const file = $fileInput[0].files[0];
        if (!file) {
            alert("❗ No file is found");
            return;
        }

        $uploadResult.text("");
        uploadedData = null;
        uploadedFileName = file.name.replace(/\.[^/.]+$/, "");

        const form = new FormData();
        form.append("file", file);
        form.append("includeFullData", "true");

        $.ajax({
            url: `${API_BASE}/fileupload`,
            method: "POST",
            data: form,
            processData: false,
            contentType: false,
            success: function (json) {
                alert("✅ File has been uploaded");
                $uploadResult.text(JSON.stringify(json, null, 2));
                uploadedData = json.data;

                // Populate dropdowns
                $labelSelect.empty();
                $valueSelect.empty();
                $.each(json.columns, function (_, col) {
                    $labelSelect.append(`<option value="${col}">${col}</option>`);
                    $valueSelect.append(`<option value="${col}">${col}</option>`);
                });
            },
            error: function (xhr) {
                alert("❌ Failed to upload file");
                $uploadResult.text(xhr.responseText);
            }
        });
    });

    $visualiseBtn.on("click", function () {
        if (!uploadedData) {
            alert("❗ No data is found");
            return;
        }

        const labelColumn = $labelSelect.val();
        const selectedValues = $valueSelect.val();
        const type = $chartType.val();

        if (!labelColumn || !selectedValues || selectedValues.length === 0) {
            alert("❗ Please select label and at least one dataset column");
            return;
        }

        $visualiseStatus.text("⏳ Generating chart...");

        const payload = {
            rows: uploadedData,
            labelColumn: labelColumn,
            valueColumns: selectedValues
        };

        $.ajax({
            url: `${API_BASE}/chart`,
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function (json) {
                $visualiseStatus.text("✅ Chart is ready");
                renderChart(type, json.labels, json.datasets);
                // Scroll down to chart
                $('html, body').animate({
                    scrollTop: $chartContainer.offset().top - 50
                }, 600);
            },
            error: function (xhr) {
                $visualiseStatus.text("❌ Error generating visualisation");
                console.error(xhr);
            }
        });
    });

    function renderChart(type, labels, datasets) {
        $chartContainer.show();

        // Random colours
        datasets.forEach(ds => {
            ds.backgroundColor = `rgba(${Math.floor(Math.random() * 255)},${Math.floor(Math.random() * 255)},${Math.floor(Math.random() * 255)},0.5)`;
            ds.borderColor = "rgba(0,0,0,0.8)";
        });

        if (chartInstance) chartInstance.destroy();

        chartInstance = new Chart(chartCanvas, {
            type: type,
            data: { labels, datasets },
            options: {
                responsive: true,
                plugins: {
                    legend: { position: 'bottom' },
                    title: { display: true, text: uploadedFileName ? uploadedFileName : '' }
                }
            }
        });
    }

});