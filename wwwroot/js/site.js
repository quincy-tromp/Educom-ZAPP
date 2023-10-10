
function addNewRow(task, flag = 0) {
    event.preventDefault();
    var table = getTable();
    var rowIndex = table.rows.length;
    if (task == "appointment") {
        addAppointmentTaskRow(table, rowIndex, flag);
        transformButton(table, rowIndex - 1);
    }
}

function getTable() {
    return document.getElementById("main-table").getElementsByTagName("tbody")[0];
}

function addAppointmentTaskRow(table, rowIndex, flag) {
    var newRowIndex = rowIndex + 1;

    var newRow = table.insertRow(rowIndex);
    var cell1 = newRow.insertCell(0);
    var cell2 = newRow.insertCell(1);
    var cell3 = newRow.insertCell(2);
    var cell4 = newRow.insertCell(3);
    var cell5 = newRow.insertCell(4);

    cell1.classList.add("index-cell");
    cell4.classList.add("center-cell");
    cell5.classList.add("center-cell");

    cell1.innerHTML =
    '<span>' + newRowIndex + '</span>';

    cell2.innerHTML =
    '<div class="form-group">' +
        '<input id="task-names" list="task-list" name="AppointmentTasks[' + rowIndex + '].Task.Name" asp-for="AppointmentTasks[i].Task.Name" class="form-control" value="" placeholder="Zoek taak"/>' +
    '</div>';

    cell3.innerHTML =
    '<div class="form-group">' +
        '<input id="task-info" name="AppointmentTasks[' + rowIndex + '].AdditionalInfo" asp-for="AppointmentTasks[i].AdditionalInfo" type="text" class="form-control" />' +
    '</div>';

    if (flag == 1) {
        cell4.innerHTML =
        '<div class="form-group">' +
            '<input id="is-task-done" name="AppointmentTasks[@i].IsDone" type="checkbox" asp-for="AppointmentTasks[i].IsDone" value="@Model.AppointmentTasks[i].IsDone" />' +
        '</div>';
        cell5.innerHTML =
        '<button id="add-new-task-btn" onclick="addNewRow("appointment",1)"><i class="fa fa-plus btn-icon"></i></button>' +
        '<input id="delete-task" type="hidden" asp-for="AppointmentTasks[i].IsDeleted" name="AppointmentTasks[@i].IsDeleted" />';
    }
    else {
        cell4.innerHTML = '';
        cell5.innerHTML =
        '<button id="add-new-task-btn" onclick="addNewRow("appointment")"><i class="fa fa-plus btn-icon"></i></button>' +
        '<input id="delete-task" type="hidden" asp-for="AppointmentTasks[i].IsDeleted" name="AppointmentTasks[@i].IsDeleted" />';
    }
}

function transformButton(table, rowIndex) {
    if (rowIndex < 0) return;
    var row = table.rows[rowIndex];
    var cell = row.cells[4].innerHTML =
    '<button id="delete-task" onclick="deleteTask(' + rowIndex + ')"><i class="fa fa-trash-o btn-icon"></i></button>' +
    '<input id="delete-task" type="hidden" asp-for="AppointmentTasks[i].IsDeleted" name="AppointmentTasks[@i].IsDeleted" />';
}

function deleteTask(rowIndex) {
    event.preventDefault();
    var table = document.getElementById("main-table").getElementsByTagName("tbody")[0];
    var row = table.rows[rowIndex];
    row.setAttribute("id", "disabled-row");
    var inputFields = row.getElementsByTagName("input");
    for (var i = 0; i < inputFields.length; i++) {
        if (inputFields[i].id == "delete-task") {
            inputFields[i].setAttribute("value", "true");
        }
        inputFields[i].setAttribute("id", "disabled-field");
        inputFields[i].readOnly = true;
    }
}

function AddCustomerTaskRow(table, rowIndex) {
    var newRowIndex = rowIndex + 1;

    var newRow = table.insertRow(rowIndex);
    var cell1 = newRow.insertCell(0);
    var cell2 = newRow.insertCell(1);
    var cell3 = newRow.insertCell(2);
    var cell4 = newRow.insertCell(3);
    var cell5 = newRow.insertCell(4);

    cell1.classList.add("index-cell");
    cell5.classList.add("center-cell");

    cell1.innerHTML =
    '<span>' + newRowIndex + '</span>';

    cell2.innerHTML =
    '<div class="form-group">' +
        '<input id="task-names" list="task-list" name="CustomerTasks[' + rowIndex + '].Task.Name" asp-for="CustomerTasks[i].Task.Name" class="form-control" value="" placeholder="Zoek taak"/>' +
    '</div>';

    cell3.innerHTML =
    '<div class="form-group">' +
        '<input id="task-info" name="CustomerTasks[' + rowIndex + '].AdditionalInfo" asp-for="CustomerTasks[i].AdditionalInfo" type="text" class="form-control" />' +
    '</div>';

    cell4.innerHTML = '';
    cell5.innerHTML =
    '<button id="add-new-task-btn" onclick="addNewRow()"><i class="fa fa-plus btn-icon"></i></button>' +
    '<input id="delete-task" type="hidden" asp-for="CustomerTasks[i].IsDeleted" name="CustomerTasks[@i].IsDeleted" />';
}

function aspDelete(id) {
    event.preventDefault();
    var form = document.getElementById(id);
    form.setAttribute("action", "/Appointment/Delete/");
    form.submit();
}