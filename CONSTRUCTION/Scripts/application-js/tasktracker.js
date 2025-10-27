$(document).ready(function () {
    // Load existing tasks on page load
    loadTasks();

    // Submit task button click handler
    $('#btnSubmitTask').click(function () {
        addTask();
    });

    // Load tasks function
    function loadTasks() {
        $.ajax({
            url: '/Orax/GetTasks', // Adjust the URL to your backend endpoint
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                const taskTableBody = $('#tblTasks tbody');
                taskTableBody.empty(); // Clear existing tasks

                $.each(data, function (index, task) {
                    taskTableBody.append(`
                        <tr>
                            <td>${task.TaskDetails}</td>
                            <td>${ convertDotNetDate(task.DueDate)}</td>
                            <td>
                                <button class="btn btn-warning btnEdit" data-id="${task.id}">Edit</button>
                                <button class="btn btn-danger btnDelete" data-id="${task.id}">Delete</button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    }

    // Edit task event handler
    $(document).on('click', '.btnEdit', function () {
        const taskId = $(this).data('id');
        // Load task details and populate the input fields for editing
        $.ajax({
            url: `/Orax/GetTask/${taskId}`, // Adjust the URL to your backend endpoint
            type: 'GET',
            success: function (task) {
                $('#txtTaskDetails').val(task.details);
                $('#txtDatetoComplete').val(task.dueDate);
                $('#btnSubmitTask').data('id', taskId).text('Update Task'); // Change button text to Update
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    });

    // Delete task event handler
    $(document).on('click', '.btnDelete', function () {
        const taskId = $(this).data('id');
        if (confirm('Are you sure you want to delete this task?')) {
            $.ajax({
                url: `/Orax/DeleteTask/${taskId}`, // Adjust the URL to your backend endpoint
                type: 'DELETE',
                success: function () {
                    loadTasks(); // Reload tasks after deletion
                },
                error: function (xhr) {
                    alert('Error: ' + xhr.statusText);
                }
            });
        }
    });


    // Add task event handler
    function addTask() {
        debugger
        let taskDetails = $('#txtTaskDetails').val();
        let dateToComplete = $('#txtDatetoComplete').val();

        if (!taskDetails || !dateToComplete) {
            alert('Please fill in all fields');
            return;
        }

        let taskData = {
            TaskDetails: taskDetails,
            DueDate: dateToComplete
        };
        $.ajax({
            url: '/Orax/AddTask',
            type: 'POST',
            data: JSON.stringify(taskData),
            contentType: 'application/json',
            success: function (result) {
                debugger
                if (result == 'success') {
                    $('#txtTaskDetails').val('');
                    $('#txtDatetoComplete').val('');
                    loadTasks();
                } else {
                    alert('Error: ' + result);
                }
            },
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            }
        });
    }



    function convertDotNetDate(dotNetDate) {
        // Extract the timestamp using regex
        if (dotNetDate == null) {
            return "";
        }
        const match = /\/Date\((\d+)\)\//.exec(dotNetDate);

        if (!match) {
            throw new Error("Invalid .NET date format");
        }

        // Convert extracted timestamp to a number
        const timestamp = parseInt(match[1], 10);

        // Create a Date object
        const date = new Date(timestamp);

        // Return as a human-readable string (localized)
        return date.toLocaleString();
    }
});