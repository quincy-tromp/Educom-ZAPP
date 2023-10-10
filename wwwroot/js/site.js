
function addNewRow(task, flag = 0) {
    event.preventDefault();
    var table = getTable();
    var rowIndex = table.rows.length;
    AddTaskRow(table, rowIndex, task, flag);
    transformButton(table, rowIndex - 1, task);
}

function getTable() {
    return document.getElementById("main-table").getElementsByTagName("tbody")[0];
}

function AddTaskRow(table, rowIndex, task, flag) {
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
        '<input id="task-names" list="task-list" name="' + task + 'Tasks[' + rowIndex + '].Task.Name" asp-for="' + task + 'Tasks[i].Task.Name" class="form-control" value="" placeholder="Zoek taak"/>' +
    '</div>';

    cell3.innerHTML =
    '<div class="form-group">' +
        '<input id="task-info" name="' + task + 'Tasks[' + rowIndex + '].AdditionalInfo" asp-for="' + task + 'Tasks[i].AdditionalInfo" type="text" class="form-control" />' +
    '</div>';


    if (flag == 1) {
        cell4.innerHTML =
        '<div class="form-group">' +
            '<input id="is-task-done" name="' + task + 'Tasks[@i].IsDone" type="checkbox" asp-for="' + task + 'Tasks[i].IsDone" value="@Model.' + task + 'Tasks[i].IsDone" />' +
        '</div>';
        cell5.innerHTML =
        '<button id="add-new-task-btn" onclick="addNewRow(' + task + ',1)"><i class="fa fa-plus btn-icon"></i></button>' +
        '<input id="delete-task" type="hidden" asp-for="' + task + 'Tasks[i].IsDeleted" name="' + task + 'Tasks[@i].IsDeleted" />';
    }
    else {
        cell4.innerHTML = '';
        cell5.innerHTML =
        '<button id="add-new-task-btn" onclick="addNewRow(' + task + ')"><i class="fa fa-plus btn-icon"></i></button>' +
        '<input id="delete-task" type="hidden" asp-for="' + task + 'Tasks[i].IsDeleted" name="' + task + 'Tasks[@i].IsDeleted" />';
    }
}

function transformButton(table, rowIndex, task) {
    if (rowIndex < 0) return;
    var row = table.rows[rowIndex];
    var cell = row.cells[4].innerHTML =
    '<button id="delete-task" onclick="deleteTask(' + rowIndex + ')"><i class="fa fa-trash-o btn-icon"></i></button>' +
    '<input id="delete-task" type="hidden" asp-for="' + task + 'Tasks[i].IsDeleted" name="' + task + 'Tasks[@i].IsDeleted" />';
}

function deleteTask(rowIndex) {
    event.preventDefault();
    var table = getTable();
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

function aspDelete(id, task) {
    event.preventDefault();
    var form = document.getElementById(id);
    form.setAttribute('action', '/' + task + '/Delete/');
    form.submit();
}