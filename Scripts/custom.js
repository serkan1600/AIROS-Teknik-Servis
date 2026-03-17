$(document).ready(function() {
    console.log("AIROS Web App initialized.");
    
    // Smooth scrolling
    $('a[href^="#"]').on('click', function(event) {
        var target = $(this.getAttribute('href'));
        if( target.length ) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 100
            }, 1000);
        }
    });

    // Add scroll class to navbar
    $(window).scroll(function() {
        if ($(window).scrollTop() > 50) {
            $('.navbar').addClass('shadow-sm bg-dark');
        } else {
            $('.navbar').removeClass('shadow-sm');
        }
    });
});
