function include(scriptUrl) {
    document.write('<script src="' + scriptUrl + '"></script>');
}

function isIE() {
    var myNav = navigator.userAgent.toLowerCase();
    return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
};

/* cookie.JS
 ========================================================*/
include('js/jquery.cookie.js');

/* Easing library
 ========================================================*/
include('js/jquery.easing.1.3.js');

/* PointerEvents
 ========================================================*/
;
(function ($) {
    if (isIE() && isIE() < 11) {
        include('js/pointer-events.js');
        $('html').addClass('lt-ie11');
        $(document).ready(function () {
            PointerEventsPolyfill.initialize({});
        });
    }
})(jQuery);

/* Scroll To
 =============================================*/
; (function ($) {
    include('js/scrollTo.js');
})(jQuery);

/* ToTop
 ========================================================*/
;
(function ($) {
    var o = $('html');
    if (o.hasClass('desktop')) {
        include('js/jquery.ui.totop.js');

        $(document).ready(function () {
            $().UItoTop({
                easingType: 'easeOutQuart',
                containerClass: 'toTop fa fa-arrow-circle-up'
            });
        });
    }
})(jQuery);

/* EqualHeights
 ========================================================*/
;
(function ($) {
    var o = $('[data-equal-group]');
    if (o.length > 0) {
        include('js/jquery.equalheights.js');
    }
})(jQuery);

/* Copyright Year
 ========================================================*/
;
(function ($) {
    var currentYear = (new Date).getFullYear();
    $(document).ready(function () {
        $("#copyright-year").text((new Date).getFullYear());
    });
})(jQuery);

/* WOW
 ========================================================*/
;
(function ($) {
    var o = $('html');

    if ((navigator.userAgent.toLowerCase().indexOf('msie') == -1) || (isIE() && isIE() > 9)) {
        if (o.hasClass('desktop')) {
            include('js/wow.js');

            $(document).ready(function () {
                new WOW().init();
            });
        }
    }
})(jQuery);


/**
 * @module       Vide
 * @description  Enables Vide.js Plugin
 */
;
(function ($) {
    var o = $(".vide");
    if (o.length) {
        include('js/jquery.vide.js');
        $(document).ready(function () {
            o.wrapInner('<div class="vide__body"></div>');
        });
    }
})(jQuery);

/* Orientation tablet fix
 ========================================================*/
$(function () {
    // IPad/IPhone
    var viewportmeta = document.querySelector && document.querySelector('meta[name="viewport"]'),
        ua = navigator.userAgent,

        gestureStart = function () {
            viewportmeta.content = "width=device-width, minimum-scale=0.25, maximum-scale=1.6, initial-scale=1.0";
        },

        scaleFix = function () {
            if (viewportmeta && /iPhone|iPad/.test(ua) && !/Opera Mini/.test(ua)) {
                viewportmeta.content = "width=device-width, minimum-scale=1.0, maximum-scale=1.0";
                document.addEventListener("gesturestart", gestureStart, false);
            }
        };

    scaleFix();
    // Menu Android
    if (window.orientation != undefined) {
        var regM = /ipod|ipad|iphone/gi,
            result = ua.match(regM);
        if (!result) {
            $('.sf-menus li').each(function () {
                if ($(">ul", this)[0]) {
                    $(">a", this).toggle(
                        function () {
                            return false;
                        },
                        function () {
                            window.location.href = $(this).attr("href");
                        }
                    );
                }
            })
        }
    }
});
var ua = navigator.userAgent.toLocaleLowerCase(),
    regV = /ipod|ipad|iphone/gi,
    result = ua.match(regV),
    userScale = "";
if (!result) {
    userScale = ",user-scalable=0"
}
document.write('<meta name="viewport" content="width=device-width,initial-scale=1.0' + userScale + '">');

/*/!* Onepage scroll
=============================================*!/
;(function ($) {
    include('js/jquery.onepage-scroll.js');
})(jQuery);

$(document).ready(function(){
    $(".main").onepage_scroll({
        "pagination": true
    });
});*/

/*/!* Vide
========================================================*!/
;(function ($) {
    var o = $('.vide');
    if (o.length > 0) {
        include('js/jquery.vide.js');
    }
})(jQuery);

/!**
 * @module       RD Parallax 3
 * @description  Enables RD Parallax 3 Plugin
 *!/
;
(function ($) {
    var o = $('.rd-parallax');
    if (o.length) {
        include('js/jquery.rd-parallax.js');
        $(document).ready(function () {
            o.each(function () {
                var p = $(this);
                if (!p.parents(".swiper-slider").length) {
                    p.RDParallax({
                        direction: ($('html').hasClass("smoothscroll") || $('html').hasClass("smoothscroll-all")) && !isIE() ? "normal" : "inverse"
                    });
                }
            });
        });
    }
})(jQuery);*/

/**
 * @module       RD Parallax 3
 * @description  Enables RD Parallax 3 Plugin
 */
;
(function ($) {
    var o = $('.rd-parallax');
    if (o.length) {
        include('js/jquery.rd-parallax.js');
        $(document).ready(function () {
            o.each(function () {
                var p = $(this);
                if (!p.parents(".swiper-slider").length) {
                    p.RDParallax({
                        direction: ($('html').hasClass("smoothscroll") || $('html').hasClass("smoothscroll-all")) && !isIE() ? "normal" : "inverse"
                    });
                }
            });
        });
    }
})(jQuery);

/**
 * @module       RD Mailform
 * @description  Enables RD Mailform Plugin
 */
;



(function ($) {
    $(document).ready(function () {
        //$('.go-to-shop').bind("click", function (evt) {
        //    evt.preventDefault();
        //    evt.stopPropagation();
        //    var infoContainer = $(".mfInfo");
        //    infoContainer.addClass('success');
        //    infoContainer.first().text('Juz niedlugo do pobrania!');
        //    setTimeout(function () {
        //        infoContainer.removeClass('success');
        //    }, 2000);
        //});


        function emailIsMatch(email) {
            if (email.length === 0) {
                return false;
            }
            var pattern = new RegExp("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$");
            return pattern.test(email);
        }



        $("#contactForm").submit(function (e) {
            var infoContainer = $(".mfInfo");
            infoContainer.addClass('sending');
            infoContainer.first().text('Wysylanie');
            var url = "api/Feedback/MailFromSite";
            var model = {
                Name: $("#name").val(),
                Email: $("#email").val(),
                Message: $("#message").val()
            }

            if (model.Name.length === 0 || !emailIsMatch(model.Email) || model.Message.length === 0) {
                infoContainer.removeClass('sending');
                infoContainer.addClass('fail');
                infoContainer.first().text('Formularz nie zostal wypelniony poprawnie');
                setTimeout(function () {
                    infoContainer.removeClass('fail');
                }, 2000);
            }
            else {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: JSON.stringify(model),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, statusName, responseObject) {
                        infoContainer.removeClass('sending');
                        infoContainer.removeClass('fail');
                        if (responseObject.status === 200) {
                            infoContainer.addClass('success');
                            infoContainer.first().text('Email zostal wyslany! Dziekujemy.');
                            $("#name").val(''),
                            $("#email").val(''),
                            $("#message").val('')
                            setTimeout(function () {
                                infoContainer.removeClass('success');
                            }, 3000);
                        }
                        else {
                            infoContainer.addClass('fail');
                            infoContainer.first().text('Wysylanie sie nie powiodlo...');
                            setTimeout(function () {
                                infoContainer.removeClass('success');
                            }, 2500);
                        }
                    }
                });
            }
            e.preventDefault();
        });
    });
})(jQuery);


//(function ($) {
//    var o = $('.rd-mailform');
//    if (o.length > 0) {
//        include('js/mailform/jquery.rd-mailform.min.js');
//        $(document).ready(function () {
//            var o = $('.rd-mailform');

//            if (o.length) {
//                o.rdMailForm({
//                    validator: {
//                        'constraints': {
//                            '@LettersOnly': {
//                                message: 'Please use letters only!'
//                            },
//                            '@NumbersOnly': {
//                                message: 'Please use numbers only!'
//                            },
//                            '@NotEmpty': {
//                                message: 'Field should not be empty!'
//                            },
//                            '@Email': {
//                                message: 'Enter valid e-mail address!'
//                            },
//                            '@Phone': {
//                                message: 'Enter valid phone number!'
//                            },
//                            '@Date': {
//                                message: 'Use MM/DD/YYYY format!'
//                            },
//                            '@SelectRequired': {
//                                message: 'Please choose an option!'
//                            }
//                        }
//                    }
//                }, {
//                    'MF000': 'Sent',
//                    'MF001': 'Recipients are not set!',
//                    'MF002': 'Form will not work locally!',
//                    'MF003': 'Please, define email field in your form!',
//                    'MF004': 'Please, define type of your form!',
//                    'MF254': 'Something went wrong with PHPMailer!',
//                    'MF255': 'There was an error submitting the form!'
//                });
//            }
//        });
//    }
//})(jQuery);


/* Owl Carousel
========================================================*/
; (function ($) {
    var o = $('.owl-carousel');
    if (o.length > 0) {
        include('js/owl.carousel.min.js');
        $(document).ready(function () {
            o.owlCarousel({
                margin: 30,
                smartSpeed: 450,
                loop: true,
                dots: true,
                dotsEach: 1,
                nav: true,
                navClass: ['owl-prev fa fa-angle-left', 'owl-next fa fa-angle-right'],
                responsive: {
                    0: { items: 1 },
                    768: { items: 1 },
                    980: { items: 1 }
                }
            });
        });
    }
})(jQuery);