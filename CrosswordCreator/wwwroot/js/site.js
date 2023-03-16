function addQuestion() {
    let row = document.createElement("tr")
    row.innerHTML = "<th scope='row'></th>" +
                "<td width='60%'><input type='text' name='Question' placeholder='Question' required class='w-100'/></td>"+
        "<td width='30%'><input type='text' name='Answer' placeholder='Answer' required class='w-100'/></td>" +
        "<td><button class='btn btn-danger' onclick='removeRow(this)'>X</button></td>";
    document.getElementById('Questions').getElementsByTagName('tbody')[0].appendChild(row)
}

function submitQuestions() {
    var newRows = [];
    var $headers = $("#Questions th");
    var $rows = $("#Questions tbody tr, #Questions tfoot tr").each(function (index) {
        $cells = $(this).find("td");
        newRows[index] = {};
        $cells.each(function (cellIndex) {
            newRows[index][$($headers[cellIndex + 1]).html()] = $(this.firstChild).val();
        });
    });

    var data = {};
    data.questions = newRows;
    $.ajax({
        type: 'POST',
        url: '/Crossword/Create',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        success: function (result) {
            $("#Body").html(result);
        },
        error: function () {
            alert('Try submit one more time or fill required fields');
        }
    })
}

function removeRow(currentButton) {
    let row = currentButton.parentNode.parentNode;
    row.parentNode.removeChild(row);
}

function saveCrossword() {
    var wordsRows = [];
    var $headers = $("#Questions th");
    var $questionRows = $("#Questions tbody tr").each(function (index) {
        $cells = $(this).find("td");
        wordsRows[index] = {};
        $cells.each(function (cellIndex) {
            wordsRows[index][$($headers[cellIndex + 1]).html()] = $(this.firstChild).val();
        });
    });

    var crossword = [];
    $('#Crossword tbody tr').each(function (rowIndex) {

        crossword[rowIndex] = [];

        $(this).find('td').each(function (cellIndex) {
            var textNodes = $(this).contents().filter(function () {
                return this.nodeType === Node.TEXT_NODE;
            });
            cellText = textNodes.map(function () {
                return this.textContent.trim();
            }).get().join('');

            crossword[rowIndex][cellIndex] = cellText == '' ? '#' : cellText;
        });
    });

    var data = {};
    data.questions = wordsRows;
    data.crossword = crossword;
    $.ajax({
        type: 'POST',
        url: '/Crossword/Save',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        success: function (result) {
            $("#Body").html(result);
        },
        error: function () {
            alert('error 500');
        }
    })
}

function copyToClipboard() {
    var copyText = document.getElementById("code");
    copyText.select();
    copyText.setSelectionRange(0, 99999);
    var url = window.location.origin;
    var text = url + '/crossword/solve/' + copyText.value;
        navigator.clipboard.writeText(text);
    alert("link copied");
}

function checkCrossword() {
    let id = $('#code').val();
    let crossword = [];
    $('#Crossword tbody tr').each(function (rowIndex) {
        crossword[rowIndex] = [];

        $(this).find('td').each(function (cellIndex) {
            var value = $(this).find("input").val();
            crossword[rowIndex][cellIndex] = /^[a-zA-Z]$/.test(value) ? value : '#';
        });
    });

    let data = {};
    data.crossword = crossword;
    $.ajax({
        type: 'POST',
        url: '/Crossword/Check/' + id,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        success: function (result) {
            $("#Body").html(result);
        },
        error: function () {
            alert('error');
        }
    })
}

var movingLeft = true;
$(document).ready(function () {
    // Get all the input elements in the table
    var inputs = $('#Crossword input');

    // Add an event listener for the keyup event on the input elements
    inputs.on('keyup', function (e) {
        // Get the current cell's column and row index
        var $cell = $(this).closest('td');
        var colIndex = $cell.index();
        var rowIndex = $cell.closest('tr').index();

        // Get the next cell in the same row and the cell below in the same column
        var $nextCellInRow = $cell.next();
        var $cellBelow = $cell.closest('tr').next().find('td').eq(colIndex);

        // If the user typed a letter, try to focus on the next cell in the same row
        if (e.keyCode >= 65 && e.keyCode <= 90) {
            // If there is a next cell with a visible input element, set the focus to it
            if (movingLeft && $nextCellInRow.length && $nextCellInRow.find('input:visible').length) {
                movingLeft = true;
                $nextCellInRow.find('input:visible').focus();
            }
            // If the next cell is hidden or there is no next cell, move focus to the cell below
            else if ($cellBelow.length && $cellBelow.find('input:visible').length) {
                movingLeft = false;
                $cellBelow.find('input:visible').focus();
            }
            if (!($cellBelow.length && $cellBelow.find('input:visible').length)) {
                movingLeft = true;
            }
            // If the next cell and the cell below are hidden or there is no next cell or cell below, do nothing
        }
    });
});