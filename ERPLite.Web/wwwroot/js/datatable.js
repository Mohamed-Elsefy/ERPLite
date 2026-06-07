$(document).ready(function () {
    if ($('.datatable').length > 0) {
        $('.datatable').DataTable({
            responsive: true,
            pageLength: 10,
            lengthMenu: [5, 10, 25, 50],
            language: {
                search: "_INPUT_",
                searchPlaceholder: "Search records..."
            }
        });
    }
});