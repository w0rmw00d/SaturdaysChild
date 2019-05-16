/***********************************************
 * Scripts for _Layout. Functionality includes
 * collapse of list in top nav and popup events.
 **********************************************/
// runs on page load
$(document).ready(function () {
    // handles popover events for all views
    $('[data-toggle="popover"]').popover();
    // toggles visibility for ul in top nav on click of menu logo
    $('#top-logo').on('click', function () {
        $('#top-nav-list').toggle();
        $('#search-form').toggle();
    });
});