$(function () {
    registerHandlers();
});

function registerHandlers() {
    $("[data-click]").each(function () {
        var e = $(this);
        var clickHandlerName = e.attr("data-click");
        var clickHandler = window[clickHandlerName];

        if (clickHandler) {
            e.click(clickHandler);
        }
    });

    $("#frm-execute").submit(function () {
        onExecuteScriptClick();

        return false;
    });
}

function onSelectFileClick() {
    $.ajax({
        url: "/dialog/open",
        data: {
            filter: "JSON files (*.json)|*.json",
        }
    })
        .done(response => {
            var firstFile = response[0];

            if (firstFile) {
                $("#txt-file-path").val(firstFile);
            }
        });
}

function onLoadFileClick() {
    var filePath = $("#txt-file-path")
        .removeClass("is-invalid")
        .val();

    var variableName = $("#txt-variable-name")
        .removeClass("is-invalid")
        .val();

    var valid = true;

    if (!filePath) {
        $("#txt-file-path").addClass("is-invalid");
        valid = false;
    }

    if (!variableName) {
        $("#txt-variable-name").addClass("is-invalid");
        valid = false;
    }

    if (!valid) {
        return;
    }

    showLoading();
    $.ajax({
        url: "/file/read",
        data: { filePath: filePath },
    })
        .done(response => {
            window[variableName] = JSON.parse(response);

            // Do not call alert, the title will show web browser URL
            showInfo("File loaded into memory. Access it using window." + variableName);
        })
        .fail(xhr => {
            showError("Error loading: " + xhr.responseText);
        })
        .always(() => {
            hideLoading();
        });
}

function onExecuteScriptClick() {
    var command = $("#txt-script-command")
        .removeClass("is-invalid")
        .val();

    if (!command) {
        $("#txt-script-command").addClass("is-invalid")
        return;
    }

    $("#txt-result").val("Executing...");
    var result = eval(command);
    
    if ($("#chk-show-result").prop("checked")) {
        if (!result) {
            $("#txt-result").val("null/undefined/empty");
        } else {
            $("#txt-result").val(result);
        }
        
    } else {
        $("#txt-result").val("Done. Do not show result.");
    }
}

function showInfo(message, title) {
    return $.ajax({
        url: "/dialog/info",
        data: {
            message: message,
            title: title,
        },
    });
}

function showError(message, title) {
    return $.ajax({
        url: "/dialog/error",
        data: {
            message: message,
            title: title,
        },
    });
}

function showLoading() {
    $("#panel-loading").removeClass("d-none");
}

function hideLoading() {
    $("#panel-loading").addClass("d-none");
}