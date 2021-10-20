$(function () {

    $("#search").on('keyup', function () {
        const text = $(this).val();
        $('table tr:gt(0)').each(function () {
            const tr = $(this);
            const name = tr.find("td:eq(1)").text();
            name.toLowerCase().includes(text.toLowerCase()) ? tr.show() : tr.hide();
        });
    });


    $("#clear").on('click', function () {
        $('#search').val('');
        $('tr').show();
    });

    $(".deposit-button").on('click', function () {
        const contributorId = $(this).data('contributorid');
        $('[name="contributorId"]').val(contributorId);

        const tr = $(this).closest('tr');
        const name = tr.find('td:eq(1)').text();
        $('#deposit-name').text(name);

        $(".deposit").modal();
    });

    $('#new-contributor').on('click', () => {

        const form = $(".new-contrib form");
        form.attr("action", "/contributors/new");

        const title = form.parent().find('h5');
        title.html('New Contributor');

        form.find("#edit-id").remove();
        $('#contributor_first_name').val('');
        $('#contributor_last_name').val('');
        $('#contributor_cell_number').val('');
        $('#contributor_created_at').val((new Date()).toISOString().split('T')[0]);
        $('#contributor_always_include').prop('checked', '');

        $('#initialDepositDiv').show();
        $('.new-contrib').modal();
    });

    $(".edit-contrib").on("click", function () {
        const firstName = $(this).data('first-name');
        const lastName = $(this).data('last-name');
        const cell = $(this).data('cell');
        const id = $(this).data('id');
        const createdDate = $(this).data('createddate');
        const alwaysInclude = $(this).data('alwaysinclude');

        const form = $(".new-contrib form");
        form.attr("action", "/contributors/edit");

        const title = form.parent().find('h5');
        title.html('Edit Contributor');

        form.find("#edit-id").remove();
        const hidden = `<input type="hidden" id="edit-id" name="id" value="${id}" />`;
        form.append(hidden);

        $('#initialDepositDiv').hide();
        $('#contributor_first_name').val(firstName);
        $('#contributor_last_name').val(lastName);
        $('#contributor_cell_number').val(cell);
        $('#contributor_created_at').val(createdDate);

        if (!!alwaysInclude) {
            $('#contributor_always_include').prop('checked', 'checked');
        }

        $('#initialDepositDiv').hide();
        $(".new-contrib").modal();
    });
});