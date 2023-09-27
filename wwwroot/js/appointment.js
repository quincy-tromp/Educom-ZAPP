
function addTableRow(table, rowIndex) {
    var newRowIndex = rowIndex + 1;

    var newRow = table.insertRow(rowIndex);
    var cell1 = newRow.insertCell(0);
    var cell2 = newRow.insertCell(1);
    var cell3 = newRow.insertCell(2);
    var cell4 = newRow.insertCell(3);
    var cell5 = newRow.insertCell(4);

    cell1.innerHTML =
    "<span>" + newRowIndex + "</span>";

    cell2.innerHTML =
    "<div class=\"form-group\">" +
        "<input id=\"task-names\" list=\"task-list\" name=\"AppointmentTasks[" + rowIndex + "].Task.Name\" asp-for=\"@task.Task.Name\" class=\"form-control\" value=\"\" placeholder=\"Zoek taak\"/>" +
    "</div>";

    cell3.innerHTML =
    "<div class=\"form-group\">" +
        "<input id=\"task-info\" name=\"AppointmentTasks[" + rowIndex + "].AdditionalInfo\" asp-for=\"@task.AdditionalInfo\" type=\"text\" class=\"form-control\" />" +
    "</div>";

    cell4.innerHTML =
    "<div class=\"form-group\">" +
        "<input id=\"is-task-done\" name=\"AppointmentTasks[" + rowIndex + "].IsDone\" type=\"checkbox\" asp-for=\"@task.IsDone\" disabled class=\"form-control\" />" +
    "</div>";

    cell5.innerHTML =
    "<button id=\"addRow\" onclick=\"addNewTask()\">Add</button>";
}

function removeButton(table, rowIndex) {
    var row = table.rows[rowIndex];
    var button = row.cells[4].getElementsByTagName("button");
    button.remove();
}

function addNewTask() {
    var table = document.getElementById("appointment-table").getElementsByTagName("tbody")[0];
    var rowIndex = table.rows.length;
    addTableRow(table, rowIndex);
    removeButton(table, rowIndex);
}

//document.getElementById("addRow").addEventListener("click", addNewTask);