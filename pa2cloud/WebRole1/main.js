"use strict";
$(function() {
    console.log("jquery loaded!");
    searchTree("Abe Lin");
    $("#input").keyup(function (event) {
        searchTree($(this).val());
    });
    function searchTree(inputText) {
        $("#jsonTestData").html("")
        $.ajax({
            type: "POST",
            url: "WebService1.asmx/Query",
            data: "{ query: '" + inputText.toLowerCase() + "' }",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var json = JSON.parse(msg.d)
                var html = "";
                for (var i = 0; i < json.length; i++) {
                    html += "<p class='result'>" + json[i] + "</p>"
                    console.log(json[i]);
                }
                console.log(JSON.parse(msg.d));
                $("#jsonTestData").html(html);
            }
        });
    }
});