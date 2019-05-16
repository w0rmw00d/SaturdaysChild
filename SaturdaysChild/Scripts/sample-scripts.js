﻿/***************************************************
 * Scripts for Home/Samples. Functionality includes
 * animation of progress bar, and plotting and
 * interactivity for the interactive graph.
 ***************************************************/

// data for interactive graph in Home/Samples
var graphData = [
    { // visitors
        color: '#15a9e0',
        data: [[6, 1300], [7, 1600], [8, 1900], [9, 2100], [10, 2500], [11, 2200], [12, 2000], [13, 1950], [14, 1900], [15, 2000]]
    },
    { // returning visitors
        data: [[6, 500], [7, 600], [8, 550], [9, 600], [10, 800], [11, 900], [12, 800], [13, 850], [14, 830], [15, 1000]],
        color: '#ff3240',
        points: { radius: 4, fillColor: '#ff3240' }
    }
];

// variable for previous point location on graph in Home/Samples
var prevPoint = null;

// appends new tooltip to current mouse position showing current graph value
function showTooltip(x, y, contents) {
    $('<div id="tooltip">' + contents + '</div>').css({ top: y - 16, left: x + 20 })
        .appendTo('body').fadeIn();
}

// runs on page load
$(document).ready(function () {
    // toggles visibility of warning modal
    $('#modal-compatibility').modal('show');
    // animates the progress bar on Home/Samples
    $('.progress-bar').animate({ 'width': "100%" }, 5500);
    // plotting graph lines
    $.plot($('#graph-lines'), graphData, {
        series: {
            points: {
                show: true,
                radius: 5
            },
            lines: {
                show: true
            },
            shadowSize: 0
        },
        grid: {
            color: '#646464',
            borderColor: 'transparent',
            borderWidth: 20,
            hoverable: true
        },
        xaxis: {
            tickColor: 'transparent'
        },
        yaxis: {
            tickSize: 1000
        }
    });
    // plotting graph bars
    $.plot($('#graph-bars'), graphData, {
        series: {
            bars: {
                show: true,
                barWidth: .9,
                align: 'center'
            },
            shadowSize: 0
        },
        grid: {
            color: '#646464',
            borderColor: 'transparent',
            borderWidth: 20,
            hoverable: true
        },
        xaxis: {
            tickColor: 'transparent',
            tickSize: 1
        },
        yaxis: {
            tickSize: 1000
        }
    });
    // hides graph bars on page load
    $('#graph-bars').hide();
    // on click event for graph lines, toggles 'active' descriptor on click and invokes fading animation
    $('#lines').on('click', function (e) {
        $('#bars').removeClass('active');
        $('#graph-bars').fadeOut();
        $(this).addClass('active');
        $('#graph-lines').fadeIn();
        e.preventDefault();
    });
    // on click event for graph bars, toggles 'active' descriptor on click and invokes fading animation
    $('#bars').on('click', function (e) {
        $('#lines').removeClass('active');
        $('#graph-lines').fadeOut();
        $(this).addClass('active');
        $('#graph-bars').fadeIn().removeClass('hidden');
        e.preventDefault();
    });
    // binding tooltip to mouse position when hovering over graph points
    $('#graph-lines, #graph-bars').bind('plothover', function (event, pos, item) {
        if (item) {
            if (previousPoint != item.dataIndex) {
                previousPoint = item.dataIndex;
                $('#tooltip').remove();
                var x = item.datapoint[0],
                    y = item.datapoint[1];
                showTooltip(item.pageX, item.pageY, y + ' visitors at ' + x + '00 hours');
            }
        } else {
            $('#tooltip').remove();
            previousPoint = null;
        }
    });
});