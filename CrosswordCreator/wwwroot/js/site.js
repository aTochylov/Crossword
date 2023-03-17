function addQuestion() {
    let row = document.createElement("tr")
    row.innerHTML = "<th scope='row'></th>" +
                "<td width='60%'><input type='text' name='Question' placeholder='Question' required class='form-control w-100'/></td>"+
        "<td width='30%'><input type='text' name='Answer' placeholder='Answer' required class='form-control w-100'/></td>" +
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
            window.location = window.location.origin + '/crossword/solve/' + result;
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
    var inputs = $('#Crossword input');

    inputs.on('keyup', function (e) {

        var $cell = $(this).closest('td');
        var colIndex = $cell.index();
        var rowIndex = $cell.closest('tr').index();

        var $nextCellInRow = $cell.next();
        var $cellBelow = $cell.closest('tr').next().find('td').eq(colIndex);

        if (e.keyCode >= 65 && e.keyCode <= 90) {
            if (movingLeft && $nextCellInRow.length && $nextCellInRow.find('input:visible').length) {
                movingLeft = true;
                $nextCellInRow.find('input:visible').focus();
            }

            else if ($cellBelow.length && $cellBelow.find('input:visible').length) {
                movingLeft = false;
                $cellBelow.find('input:visible').focus();
            }
            if (!($cellBelow.length && $cellBelow.find('input:visible').length)) {
                movingLeft = true;
            }
        }
    });
});