
$(document).ready(function () {
    //#start:- variables
    $('#hidelIdHolder').val('0');
    $('#btnSubmit').val("Save");
    let previousSelectTrID = 0;
    var globalActiveRow = 0;
    $('#tbldashboardCalc').hide();
    //#end:- variables


    //#start:- Page load
    activeAjaxLoaderOnPageLoad();
    getAllGroups();
    AssignDateOnPageLoad();
    loadDetailsOnPageLoad();
    //#end:- Page Load


    //#start:- Events
    $('#ddlMonth').change(function () {
        getList();
        getTotal();
        getAllGroups();
    });

    $('#ddlGroup1').change(function () {
        $("#ddlGroup2  option:first").prop("selected", "selected");
        $("#ddlGroup3  option:first").prop("selected", "selected");
        getList();
        getTotal();
    });

    $(document).on("change", "#ddlGroup2", function () {
        $("#ddlGroup1  option:first").prop("selected", "selected");
        $("#ddlGroup3  option:first").prop("selected", "selected");
        getList();
        getTotal();
    });

    $('#ddlGroup3').change(function () {
        $("#ddlGroup1  option:first").prop("selected", "selected");
        $("#ddlGroup2  option:first").prop("selected", "selected");
        getList();
        getTotal();
    });

    $('#inputDetails').keypress(function (e) {
        var key = e.which;
        if (key == 13) {
            let check = $('#hidelIdHolder').val();
            if (check != '0') {
                updateExpensiveData()
            }
            else {
                saveExpensiveData();
            }
        }
    });

    $('#btnSubmit').click(function () {
        let check = $('#hidelIdHolder').val();
        if (check != '0') {
            updateExpensiveData()
        }
        else {
            saveExpensiveData();
        }
    });
    //#end:- Events


    //#start:- Dynamic Event
    $(document).on("click", ".act-row", function () {
        var id = $(this).attr("id").trim();
        globalActiveRow = id;
    });

    $(document).on("click", ".btnMore", function () {
        var id = $(this).attr("id");

        let CreateExpensiveViewModel = {
            id: parseInt(id),
            details: '',
            price: 0,
        }

        $.ajax({
            url: '/Orax/GetModelExpensive',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(CreateExpensiveViewModel),
            success: function (data) {
                $("#productModal").modal("show");
                resetAllModelDDL();

                $('#budgetId').val(data.id);
                $('#modalDate').val(data.createdDate);
                $('#modalVale').val(data.price);
                $('#modalDetails').val(data.details);
                $('#modalGroup2').html(data.group2);
                $('#modalGroup1').html(data.group1);
                $('#modalGroup3').html(data.group3);

                getAllActiveGroups();
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });


    });

    $(document).on("click", "#btnSavePopup", function () {
        let budgetID = parseInt($('#budgetId').val());
        let group1 = $('select#modalDdlGroup1 option:selected').val();
        let group2 = $('select#modalDdlGroup2 option:selected').val();
        let group2Text = $('select#modalDdlGroup2 option:selected').text();
        let group3 = $('select#modalDdlGroup3 option:selected').val();

        let CreateExpensiveViewModel = {
            id: budgetID,
            group1: group1,
            group2: group2,
            group2Text: group2Text,
            group3: group3,
            details: '',
            price: parseInt(0),
            createdDate: new Date(createdDate)
        }

        $.ajax({
            url: '/Orax/UpdateGroupAll',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(CreateExpensiveViewModel),
            success: function (data) {
                if (data == 'success') {
                    getList();
                    getTotal();
                    $('#budgetId').val('0');
                    $("#productModal").modal("hide");
                }
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });

    });

    $(document).on('change', '#modalDdlGroup2', function () {
        let popup_grp2 = $('select#modalDdlGroup2 option:selected').val();

        if (popup_grp2 == 6 || popup_grp2 == 10) { $('#modalDdlGroup1').val('need'); }//food-5k oil-4k
        if (popup_grp2 == 1) { $('#modalDdlGroup1').val('want'); }//beer-1.5k
    });

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
                $('#createdDate').val(data.createdDate);
                $('#inputPrice').val(data.price);
                $('#inputDetails').val(data.details);
                $('#btnSubmit').val("Update?");
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    });

    $(document).on('change', '.group1', function () {
        var id = $(this).attr('id');

        let CreateExpensiveViewModel = {
            id: parseInt(id),
            group1: $(this).find(":selected").val(),
            group2: '',
            details: '',
            price: 0,
            createdDate: new Date(createdDate)
        }
        $.ajax({
            url: '/Orax/UpdateGroup1',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(CreateExpensiveViewModel),
            success: function (data) {
                if (data == 'success') {
                    getList();
                    getTotal();


                }
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });

    });

    $(document).on('change', '.group2', function () {
        var id = $(this).attr('id');
        id3 = id.split('_');

        let CreateExpensiveViewModel = {
            id: parseInt(id3[0]),
            group1: '',
            group2: $(this).find(":selected").val(),
            details: '',
            price: 0,
            createdDate: new Date(createdDate)
        }
        $.ajax({
            url: '/Orax/UpdateGroup2',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(CreateExpensiveViewModel),
            success: function (data) {
                if (data == 'success') {
                    getList();
                    getTotal();
                }
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });

    });

    $(document).on('change', '.group3', function () {
        var id = $(this).attr('id');
        id3 = id.split('_');
        let CreateExpensiveViewModel = {
            id: parseInt(id3[0]),
            group1: '',
            group2: '',
            group3: $(this).find(":selected").val(),
            details: '',
            price: 0,
            createdDate: new Date(createdDate)
        }
        $.ajax({
            url: '/Orax/UpdateGroup3',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(CreateExpensiveViewModel),
            success: function (data) {
                if (data == 'success') {
                    getList();
                    getTotal();
                }
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });

    });

    $(document).on('click', '.btnRemove', function () {
        confirm('want to delete?');
        var id = $(this).attr('id')
        let CreateExpensiveViewModel = {
            id: parseInt(id),
            group1: '',
            group2: '',
            details: '',
            price: 0,
            createdDate: new Date(createdDate)
        }
        $.ajax({
            url: '/Orax/removeRecord',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(CreateExpensiveViewModel),
            success: function (data) {
                if (data == 'success') {
                    alert('removed');
                    getList();
                    getTotal();
                }
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });

    });

    $(document).on('click', '.checkRow', function () {
        let total = 0;
        let checkCount = 0;
        $('.checkRow').each(function () {

            if ($(this).prop('checked') == true) {
                let id = $(this).attr('id');
                total = total + parseInt(id);
                $("#totalSpan").html(total);
                checkCount = 1;
            }
            if (checkCount == 0) {
                $("#totalSpan").html(0);
            }
        });
        total = 0;
        checkCount = 0;
    });

    $(document).on('click', '#tblExpensive > tr', function () {
        $('#tblExpensive > tr').removeClass('dynamic-bg-tr');
        $(this).addClass('dynamic-bg-tr');
        previousSelectTrID = $(this).attr('id');
    });
    //#end:- Dynamic Events



    //#start:- functions
    function activeAjaxLoaderOnPageLoad() {

        $(document).ajaxStart(function () {
            $(".content").addClass('body-blure');
        });
        $(document).ajaxStop(function () {
            $(".content").removeClass('body-blure');
        });
    }

    function AssignDateOnPageLoad() {
        const d = new Date();
        let month = d.getMonth();
        let date = d.getDate();
        let _nowMonth = d.getMonth() + 1;
        let _month = d.getMonth() + 2;
        let __nowMonth = "0" + _nowMonth.toString();
        var _todayDate = `${d.getFullYear()}-${__nowMonth}-${d.getDate()}`;
        $('#ddlMonth').prop('selectedIndex', _month);
        $('#createdDate').val(_todayDate)
    }

    function loadDetailsOnPageLoad() {
        let monthSelected = $('select#ddlMonth option:selected').val();
        if (monthSelected != "-- select --") {
            getList();
        }
    }
    
    function assignClass() { //remove not in used
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

    function getList() {
        let forYear = $('select#ddlYear option:selected').val();
        let forMonth = $('select#ddlMonth option:selected').val();
        let forGroup1 = $('select#ddlGroup1 option:selected').val();
        let forGroup2 = $('select#ddlGroup2 option:selected').text();
        let forGroup3 = $('select#ddlGroup3 option:selected').val();
        if (forGroup2 == "-- select one --") { forGroup2 = "" }

        if (forGroup2 != "" && forGroup1 != "" && forGroup3 != "") {
            $("#tblExpensive").empty();
            return;
        }
        if (forMonth == "-- select --") {
            $("#tblExpensive").empty();
            return;
        }
        let GetExpensiveViewModel = {
            forYear: forYear,
            forMonth: forMonth,
            group1: forGroup1,//20 30 50
            group2: forGroup2,// 1.5k
            group3: forGroup3//sip
        }
        $.ajax({
            type: "POST",
            url: '/Orax/GetAllExpensive2',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(GetExpensiveViewModel),
            success: function (result) {
                $('#tbldashboardCalc').show();
                $('.total-fix').html(Math.floor(result.monthMaster[0].salary).toLocaleString('en-IN'));
                $('.total-fix').attr('title', result.monthMaster[0].salary);

                var tblRows = "";
                $.each(result.monthTransList, function (a, b) {
                    if (b.group3 == null) { b.group3 = "" };
                    if (b.group1 == "n") { b.group1 = "" };

                    //assign class
                    let group1Class = "", group2Class = "", group3Class = "", _globalActive = "";
                    if (b.group1 == "") { group1Class = "" } else if (b.group1 == "need") { group1Class = "group-need" } else if (b.group1 == "want") { group1Class = "group-want" } else if (b.group1 == "save") { group1Class = "group-save" };
                    if (b.group2 == "food-5k") { group2Class = "group-food" } else if (b.group2 == "beer-1.5k") { group2Class = "group-beer" }
                    if (b.group3 == "repeat") { group3Class = "group-repeat" }

                    //assign global acvive  
                    if (b.id == globalActiveRow) { _globalActive = "dynamic-bg-tr" }

                    var tblRow = '<tr class="act-row hei-35 ' + _globalActive + '" id="' + b.id + ' "> '

                        + '<td class="col3">' + formatDate(b.createdDate) + ' <span class="spn-' + dateToDay(b.createdDate) + '">' + dateToDay(b.createdDate) + '</span> </td>'
                        + '<td class="col1">' + b.price + ' | <a id="btnView" class="' + b.id + '" > <i class="fa-solid fa-eye"></i></a> | <input type="checkbox" id="' + b.price + '" class="checkRow" /> </td>'
                        + '<td class="wid-4 center-algn"> <a id="' + b.id + '" class="btnMore" href="#"> <i class="fa-solid fa-minimize"></i></a> </td>'
                        + '<td class="col2">' + b.details + '</td>'
                        + '<td class="wid-8 ' + group2Class + '">' + b.group2 + '</td>'
                        + '<td class="wid-8 ' + group1Class + '">' + b.group1 + '</td>'
                        + '<td class="wid-8 ' + group3Class + '">' + b.group3 + '</td>'
                        + '<td class="wid-8 center-algn "> <a id="' + b.id + 'remove" class="btnRemove clr-red" href="#"> <i class="fa-solid fa-trash"></i></a> </td>'

                        + '</tr>';
                    tblRows = tblRows + tblRow;
                });

                $("#tblExpensive").empty();
                $("#tblExpensive").append(tblRows);

                getTotal();
                assignClass();

                $('#tblExpensive > #' + previousSelectTrID).addClass('dynamic-bg-tr');
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    }

    function formatDate(dateTimeStr) {
        return dateTimeStr.substring(0, 10);
    }

    function resetAllModelDDL() {
        $("#modalDdlGroup1").prop("selectedIndex", 0);
        $('#modalDdlGroup2').empty();
        $("#modalDdlGroup3").prop("selectedIndex", 0);
    }

    function getAllActiveGroups() {
        $.ajax({
            url: '/Orax/GetDDLGroup2OnlyActive',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            success: function (data) {
                $('#modalDdlGroup2').empty();
                $('#modalDdlGroup2').append($("<option></option>").attr("value", "0").text("-- select one --"));

                $.each(data, function (key, value) {
                    let _obgcolour = "";
                    if (value.group2int == 6) { _obgcolour = "bg-yellow" }
                    $('#modalDdlGroup2').append($("<option class='" + _obgcolour + "'></option>").attr("value", value.group2int).text(value.groupName));
                });
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    }

    function getAllGroups() {
        $.ajax({
            url: '/Orax/GetDDLGroup2All',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            success: function (data) {
                $('#ddlGroup2').empty();
                $('#ddlGroup2').append($("<option></option>").attr("value", "0").text("-- select one --"));

                $.each(data, function (key, value) {
                    $('#ddlGroup2').append($("<option></option>").attr("value", value.group2int).text(value.groupName));
                });
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    }

    function saveExpensiveData() {
        //var group2 = $('select#optiongroup2 option:selected').val();
        var details = $('#inputDetails').val();
        var price = $('#inputPrice').val();
        var createdDate = $('#createdDate').val();
        if (details == '' || price == '' || createdDate == '') {
            alert("empty");
            return false;
        }

        let CreateExpensiveViewModel = {
            id: parseInt(0),
            group1: '',
            group2: '',
            details: details,
            price: parseInt(price),
            createdDate: new Date(createdDate)
        }

        $.ajax({
            url: '/Orax/AddExpensive',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(CreateExpensiveViewModel),
            success: function (data) {
                if (data == 'success') {
                    getList();
                    getTotal();
                    $('#inputPrice').val('');
                    $('#inputDetails').val('');
                    $('#inputPrice').focus();
                    $('#hidelIdHolder').val('0');
                }
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    }

    function updateExpensiveData() {
        let id = $('#hidelIdHolder').val();
        let price = $('#inputPrice').val();
        let details = $('#inputDetails').val();

        let CreateExpensiveViewModel = {
            id: parseInt(id),
            details: details,
            price: parseInt(price),
        }

        $.ajax({
            url: '/Orax/UpdateExpensive',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(CreateExpensiveViewModel),
            success: function (data) {
                if (data == 'success') {
                    getList();
                    getTotal();
                    $('#inputPrice').val('');
                    $('#inputDetails').val('');
                    $('#inputPrice').focus();
                    $('#btnSubmit').val("Save");
                    $('#createdDate').val('');
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

    function getTotal() {

        var forMonth = $('select#ddlMonth option:selected').val();

        if (forMonth != "-- select --") {
            let thisMonthSalary = $('.total-fix').attr('title');
            let total = 0.0;
            $('.col1').each(function (a, b) { total = total + parseFloat(b.innerText); });

            let foramtedTotal = Math.floor(total).toLocaleString('en-IN')
            $('.total-spend').html(foramtedTotal);
            $('.total-spend').attr('title', total);

            let foramtedData = Math.floor(thisMonthSalary - total).toLocaleString('en-IN')
            $('.total_remaining').html(foramtedData);
            $('.total_remaining').attr('title', thisMonthSalary - total);
        }
    }
    //#end:- funcitons

});