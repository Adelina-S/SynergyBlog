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
    var userName = document.getElementById("userName").value;
    var checkResult = CheckPassword(login, password);
    if (checkResult == false) return;
    $.post('/Home/Register', { 'login': login, 'password': password, 'name':userName },
        function (data) {
            if (data.length != 0)
                alert(data);
            else
                window.location.replace('/Home/Login');
        });
}
function ShowPosts() {
    $.post('/Home/GetMessages', { 'page': page }, function (data) {
        if (data.length != 0) {
            $("#postList").append(data);
            page += 1;
        }
        else {
            $('#nextPage').hide();
        }
    });
}
    function ShowMyPosts() {
        $.post('/Home/GetMyMessages', { 'page': page }, function (data) {
            if (data.length != 0) {
                $("#postList").append(data);
                page += 1;
            }
            else {
                $('#nextPage').hide();
            }
        });
}
function ShowFilter() {
    var user = document.getElementById("filterUser").value;
    var tags = document.getElementById("filterTags").value;
    $.post('/Home/Filter', { 'filterUser': user, 'filterTags': tags, 'page': page }, function (data) {
        if (data.length != 0) {
            $("#postList").append(data);
            page += 1;
        }
        else {
            $('#nextPage').hide();
        }
    });
}
function Subscribe(button, messageId) {
    $.post('/Home/Subscribe', { 'messageId': messageId }, function (data) {
        if (data.length == 0) {
            button.innerHTML = 'Отписаться от автора'
        }
        else {
            alert(data);
        }
    });
}
function UnSubscribe(button, messageId) {
    $.post('/Home/UnSubscribe', { 'messageId': messageId }, function (data) {
        if (data.length == 0) {
            button.innerHTML = 'Подписаться на автора'
        }
        else {
            alert(data);
        }
    });
}
    function ShowMySubscribes() {
        $.post('/Home/GetMySubscribes', { 'page': page }, function (data) {
            if (data.length != 0) {
                $("#subscribeList").append(data);
                page += 1;
            }
            else {
                $('#nextPage').hide();
            }
        });
}
function CreateMessage() {
    var title = document.getElementById("Title").value;
    var text = document.getElementById("Text").value;
    var tags = document.getElementById("Tags").value;
    var isHidden = document.getElementById("IsHidden").value;
    $.post('/Home/CreateMessage', { 'title': title, 'Text':text, 'IsHidden':isHidden, 'Tags':tags }, function (data) {
        if (data.length != 0) {
            alert(data);
        }
        else {
            window.location.replace('/Home/MyMessages');
        }
    });

}