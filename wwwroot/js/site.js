function aspDelete(id) {
    event.preventDefault();
    var form = document.getElementById(id);
    form.setAttribute("action", "/Appointment/Delete/");
    form.submit();
}