
$(document).ready(function () {
    $('#hidelIdHolder').val('0');

    $(document).ajaxStart(function () {
        $(".content").addClass('body-blure');
    });
    $(document).ajaxStop(function () {
        $(".content").removeClass('body-blure');
    });

    $('#btnSubmit').val("Save");

    let previousSelectTrID = 0;
    var globalActiveRow = 0;



    $('#btnSearch').click(function () {
        getList();
    });


    function getList() {
        let searchInput = $('#txtSearchDetails').val();
        if (searchInput == "") { alert('enter input'); }
        let SearchInputViewModel = {
            searchInput: searchInput,
        }
        $.ajax({
            type: "POST",
            url: '/Orax/FilterWithDetails',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(SearchInputViewModel),
            success: function (result) {

                var tblRows = "";
                $.each(result.filterData, function (a, b) {
                    if (b.group3 == null) { b.group3 = "" };
                    if (b.group1 == "n") { b.group1 = "" };

                    //assign class
                    let group1Class = "",group2Class="",group3Class = "", _globalActive="";
                    if (b.group1 == "") { group1Class = "" } else if (b.group1 == "need") { group1Class = "group-need" } else if (b.group1 == "want") { group1Class = "group-want" } else if (b.group1 == "save") { group1Class = "group-save" };
                    if (b.group2 == "food-5k") { group2Class = "group-food" } else if (b.group2 == "beer-1.5k") { group2Class = "group-beer" }
                    if (b.group3 == "repeat") { group3Class = "group-repeat" }

                    //assign global acvive  
                    if (b.id == globalActiveRow) { _globalActive = "dynamic-bg-tr" }

                    var tblRow = '<tr class="act-row hei-35 ' + _globalActive + '" id="' + b.id + ' "> '

                        + '<td class="col3">' + b.createdDate + ' <span class="spn-clr">' + dateToDay(b.createdDate) + '</span> </td>'
                        + '<td class="col1">' + b.price + ' | <a id="btnView" class="' + b.id + '" > <i class="fa-solid fa-eye"></i></a> |</td>'
                        + '<td class="col2">' + b.details + '</td>'
                        + '<td class="wid-8 ' + group2Class + '">' + b.group2 + '</td>'
                        + '<td class="wid-8 ' + group1Class + '">' + b.group1 + '</td>'
                        + '<td class="wid-8 ' + group3Class + '">' + b.group3 + '</td>'
                        + '</tr>';
                    tblRows = tblRows + tblRow;
                });

                $("#tblExpensive").empty();
                $("#tblExpensive").append(tblRows);

                assignClass();

                $('#tblExpensive > #' + previousSelectTrID).addClass('dynamic-bg-tr');
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    }

    $(document).on('click', '#btnView', function () {
        $('#hidelIdHolder').val('0');
        var id = $(this).attr('class');

        let CreateExpensiveViewModel = {
            id: parseInt(id),
            details: '',
            price: 0,
        }
        $.ajax({
            url: '/Orax/GetExpensive',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(CreateExpensiveViewModel),
            success: function (data) {
                $('#hidelIdHolder').val(id);
                $('#inputDetails').val(data.details);
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    });

    $('#btnSubmit').click(function () {
        updateExpensiveData();
    });

    function updateExpensiveData() {
        debugger
        let id = $('#hidelIdHolder').val();
        let details = $('#inputDetails').val();

        let CreateExpensiveViewModel = {
            id: parseInt(id),
            details: details,
        }

        $.ajax({
            url: '/Orax/UpdateBySearch',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(CreateExpensiveViewModel),
            success: function (data) {
                if (data == 'success') {
                    getList();
                    $('#inputDetails').val('');
                    $('#hidelIdHolder').val('0');
                }
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    }


    function dateToDay(inputDate) {
        var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        var d = new Date(inputDate);
        return days[d.getDay()];
    }



    //remove not in used
    function assignClass() {
        $(".box-default").each(function () {
            var calssData = $(this).html();
            if (calssData == "save") {
                $(this).addClass('box-save');
            }
            else if (calssData == "want") {
                $(this).addClass('box-want');
            }
            else if (calssData == "need") {
                $(this).addClass('box-need');
            }
            else {
                $(this).addClass('box-non');
            }
        });

        $(".box-default2").each(function () {
            var calssData = $(this).html();
            if (calssData != "null") {
                $(this).addClass('box-assigned');
            }
            else {
                $(this).addClass('box-non');
            }
        });

        $(".box-default-3").each(function () {
            var calssData = $(this).html();
            if (calssData != "null") {
                if (calssData == "repeat") {
                    $(this).addClass('box-assigned-repeat');
                }
                $(this).addClass('box-assigned-3');
            }
            else {
                $(this).addClass('box-non-3');
            }
        });
    }

});