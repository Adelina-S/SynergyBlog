// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//Код проверки повторения пароля для формы регистрации
function CheckPassword(login, password)
{
    if (login.length == 0 || login.length>20) {
        alert("Поле Login должно содержать от 1 до 20 символов!");
        return false;
    }
    var confirm = document.getElementById("confirm").value;
    if (password.length <=4 || password.length>20) {
        alert("Пароль должен содержать от 5 до 20 символов!");
        return false;
    }
    if (password != confirm) {
        alert("Пароль не совпадает с подтверждением!");
        return false;
    }
    return true;
}
//Код регистрации, он же вызывает проверку пароля
function RegisterJS() {
    var login = document.getElementById("login").value;
    var password = document.getElementById("password").value;
    var checkResult = CheckPassword(login, password);
    if (checkResult == false) return;
    $.post('/Home/Register', { 'login': login, 'password': password },
        function (data) {
            if (data.length != 0)
                alert(data);
            else
                window.location.replace('/Home/Login');
        });
}