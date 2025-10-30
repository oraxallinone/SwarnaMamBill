$(document).ready(function () {

    // #region Page Load Initialization 
    // Load all expense groups in dropdown on page load
    getAllGroups();

    // Set default date and month values
    const d = new Date();
    let _nowMonth = d.getMonth() + 1;
    let _month = d.getMonth() + 2;
    let __nowMonth = "0" + _nowMonth.toString();
    var _todayDate = `${d.getFullYear()}-${__nowMonth}-${d.getDate()}`;

    // Set default values for dropdowns and date
    $('#ddlMonth').prop('selectedIndex', _month);
    $('#createdDate').val(_todayDate);
    $('#hidelIdHolder').val('0');
    $('#btnSubmit').val("Save");
    $('#tbldashboardCalc').hide();

    // Load initial expense list if month is selected
    let monthSelected = $('select#ddlMonth option:selected').val();
    if (monthSelected != "-- select --") {
        getList();
    }
    // #endregion

    // #region Ajax Loading Indicators
    // Show/hide loading blur effect during ajax calls  
    $(document).ajaxStart(function () {
        $(".content").addClass('body-blure');
    });
    $(document).ajaxStop(function () {
        $(".content").removeClass('body-blure');
    });
    // #endregion

    // #region Main Form Event Handlers
    // Month dropdown change - reload expenses
    $('#ddlMonth').change(function () {
        getList();
        getTotal();
        getAllGroups();
    });

    // Group1 dropdown change (Need/Want/Save)
    $('#ddlGroup1').change(function () {
        $("#ddlGroup2  option:first").prop("selected", "selected");
        $("#ddlGroup3  option:first").prop("selected", "selected");
        getList();
        getTotal();
    });

    // Group2 dropdown change (Expense Categories)
    $(document).on("change", "#ddlGroup2", function () {
        $("#ddlGroup1  option:first").prop("selected", "selected");
        $("#ddlGroup3  option:first").prop("selected", "selected");
        getList();
        getTotal();
    });

    // Group3 dropdown change (Repeat/Oil etc)
    $('#ddlGroup3').change(function () {
        $("#ddlGroup1  option:first").prop("selected", "selected");
        $("#ddlGroup2  option:first").prop("selected", "selected");
        getList();
        getTotal();
    });

    // Submit button click handler
    $('#btnSubmit').click(function () {
        let check = $('#hidelIdHolder').val();
        if (check != '0') {
            updateExpensiveData()
        }
        else {
            saveExpensiveData();
        }
    });

    // Details input enter key handler
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
    // #endregion

    // #region Modal Related Event Handlers
    // Open edit modal when More button clicked
    $(document).on("click", ".btnMore", function () {
        var id = $(this).attr("id");
        let CreateExpensiveViewModel = {
            id: parseInt(id),
            details: '',
            price: 0,
        }
        // Load expense details in modal
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

    // Modal save button click handler
    $(document).on("click", "#btnSavePopup", function () {
        // Update expense groups from modal
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
    // #endregion

    // #region Helper Functions
    // Format date to YYYY-MM-DD
    function formatDate(dateTimeStr) {
        return dateTimeStr.substring(0, 10);
    }

    // Convert date to day name
    function dateToDay(inputDate) {
        var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        var d = new Date(inputDate);
        return days[d.getDay()];
    }

    // Reset modal dropdown selections
    function resetAllModelDDL() {
        $("#modalDdlGroup1").prop("selectedIndex", 0);
        $('#modalDdlGroup2').empty();
        $("#modalDdlGroup3").prop("selectedIndex", 0);
    }

    // Get active expense groups for modal
    function getAllActiveGroups() {
        // ... existing getAllActiveGroups code ...
    }

    // Get all expense groups for main form
    function getAllGroups() {
        // ... existing getAllGroups code ...
    }

    // Calculate total expenses
    function getTotal() {
        // ... existing getTotal code ...
    }
    // #endregion

    // #region Data Operations
    // Get expense list based on filters
    function getList() {
        // ... existing getList code ...
    }

    // Save new expense
    function saveExpensiveData() {
        // ... existing saveExpensiveData code ...
    }

    // Update existing expense
    function updateExpensiveData() {
        // ... existing updateExpensiveData code ...
    }
    // #endregion

    // #region Unused Functions - Can be removed
    /*
    Following functions appear to be unused and can be removed:
    - assignClass()
    - The group1, group2, group3 change event handlers
    These seem to be legacy code that's no longer needed
    */
    // #endregion

});