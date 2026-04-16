//----------------------------------------------------------------
// >>> TABLE OF CONTENTS:
//----------------------------------------------------------------

// 01. Mobile Menu
// 02. Header Dropdown Menu
// 03. Select List (Dropdown)
// 04. Facts Counter
// 05. Category Filter (MixItUp Plugin)
// 06. Vertical Tabs
// 07. Blog Tags (Tooltip)
// 08. Owl Carousel
// 09. Sidebar Accordion
// 10. Responsive Tabs
// 11. Responsive Table
// 12. Form Fields (Value Disappear on Focus)
// 13. Bootstrap Carousel Swipe (Testimonials Carousel)
// 14. Bx Carousel
// 15. Contact Form Submit/Validation
// 16. Masonry

//----------------------------------------------------------------

$(function () {
    'use strict';

    //Mobile Menu
    //--------------------------------------------------------
    var bodyObj = $('body');
    var MenuObj = $("#menu");
    var mobileMenuObj = $('#mobile-menu');

    if (typeof Harvey !== 'undefined' && $.fn.mmenu) {
        bodyObj.wrapInner('<div id="wrap"></div>');

        var toggleMenu = {
            elem: MenuObj,
            mobile: function () {
                mobileMenuObj.mmenu({
                    slidingSubmenus: false,
                    position: 'right',
                    zposition: 'front'
                }, {
                    pageSelector: '#wrap'
                });

                this.elem.hide();
            },
            desktop: function () {
                mobileMenuObj.trigger("close.mm");
                this.elem.show();
            }
        };

        Harvey.attach('screen and (max-width:991px)', {
            setup: function () {
            },
            on: function () {
                toggleMenu.mobile();
            },
            off: function () {
            }
        });

        Harvey.attach('screen and (min-width:992px)', {
            setup: function () {
            },
            on: function () {
                toggleMenu.desktop();
            },
            off: function () {
            }
        });
    }

    //Header Dropdown Menu
    //--------------------------------------------------------
    var megaMenuHasChildren = $('.dropdown');
    var megaMenuDropdownMenu = $('.dropdown-menu');

    megaMenuHasChildren.on({
        mouseenter: function () {
            if (navigator.userAgent.match(/iPad/i) !== null) {
                $(this).find(megaMenuDropdownMenu).stop(true, true).slideDown('400');
            } else {
                $(this).find(megaMenuDropdownMenu).stop(true, true).delay(400).slideDown();
            }
        }, mouseleave: function () {
            if (navigator.userAgent.match(/iPad/i) !== null) {
                $(this).find(megaMenuDropdownMenu).stop(true, true).slideUp('400');
            } else {
                $(this).find(megaMenuDropdownMenu).stop(true, true).delay(400).slideUp();
            }
        }
    });

    //Select List (Dropdown)
    //--------------------------------------------------------
    var selectObj = $('select');
    var selectListObj = $('ul.select-list');
    selectObj.each(function () {
        var $this = $(this), numberOfOptions = $(this).children('option').length;

        $this.addClass('select-hidden');
        $this.wrap('<div class="select"></div>');
        $this.after('<div class="select-styled"></div>');

        var $styledSelect = $this.next('div.select-styled');
        $styledSelect.text($this.children('option').eq(0).text());

        var $list = $('<ul />', {
            'class': 'select-list'
        }).insertAfter($styledSelect);

        for (var i = 0; i < numberOfOptions; i++) {
            $('<li />', {
                text: $this.children('option').eq(i).text(),
                rel: $this.children('option').eq(i).val()
            }).appendTo($list);
        }

        var $listItems = $list.children('li');

        $styledSelect.on('click', function (e) {
            e.stopPropagation();
            $('div.select-styled.active').not(this).each(function () {
                $(this).removeClass('active').next(selectListObj).hide();
            });
            $(this).toggleClass('active').next(selectListObj).toggle();
        });

        $listItems.on('click', function (e) {
            e.stopPropagation();
            $styledSelect.text($(this).text()).removeClass('active');
            $this.val($(this).attr('rel'));
            $list.hide();
        });

        $(document).on('click', function () {
            $styledSelect.removeClass('active');
            $list.hide();
        });

    });

    //Facts Counter
    //--------------------------------------------------------
    var counterObj = $('.fact-counter');
    if (counterObj.length && $.fn.counterUp) {
        counterObj.counterUp({
            delay: 10,
            time: 500
        });
    }

    //Category Filter (MixItUp Plugin)
    //--------------------------------------------------------
    var folioFilterObj = $('#category-filter');
    if (folioFilterObj.length && $.fn.mixItUp) {
        folioFilterObj.mixItUp();
    }

    //Vertical Tabs
    //--------------------------------------------------------
    var tabObject = $(".tabs-menu li");
    var tabContent = $(".tabs-list .tab-content");
    tabObject.on('click', function (e) {
        e.preventDefault();
        $(this).siblings('li.active').removeClass("active");
        $(this).addClass("active");
        var index = $(this).index();
        tabContent.removeClass("active");
        tabContent.eq(index).addClass("active");
    });

    //Blog Tags (Tooltip)
    //--------------------------------------------------------
    var tagObj = $('[data-toggle="blog-tags"]');
    if (tagObj.length && $.fn.tooltip) {
        tagObj.tooltip();
    }

    //Owl Carousel
    //--------------------------------------------------------
    var owlObj = $('.owl-carousel');
    if (owlObj.length && $.fn.owlCarousel) {
        owlObj.owlCarousel({
            loop: false,
            margin: 30,
            nav: false,
            dots: true,
            responsiveClass: true,
            responsive: {
                0: {
                    items: 1
                },
                600: {
                    items: 1
                },
                1000: {
                    items: 2
                }
            }
        });
    }

    var owlEventObj = $('.owl-carousel-event');
    if (owlEventObj.length && $.fn.owlCarousel) {
        owlEventObj.owlCarousel({
            loop: false,
            margin: 30,
            nav: false,
            dots: true,
            responsiveClass: true,
            responsive: {
                0: {
                    items: 1
                },
                768: {
                    items: 2
                },
                1200: {
                    items: 3
                }
            }
        });
    }

    //Sidebar Accordion
    //--------------------------------------------------------
    var secondaryObj = $('#secondary [data-accordion]');
    var multipleObj = $('#multiple [data-accordion]');
    var singleObj = $('#single[data-accordion]');

    if (secondaryObj.length && $.fn.accordion) {
        secondaryObj.accordion({
            singleOpen: true
        });
    }

    if (multipleObj.length && $.fn.accordion) {
        multipleObj.accordion({
            singleOpen: false
        });
    }

    if (singleObj.length && $.fn.accordion) {
        singleObj.accordion({
            transitionEasing: 'cubic-bezier(0.455, 0.030, 0.515, 0.955)',
            transitionSpeed: 200
        });
    }

    //Responsive Tabs
    //--------------------------------------------------------
    var restabObj = $('#responsiveTabs');
    if (restabObj.length && $.fn.responsiveTabs) {
        restabObj.responsiveTabs({
            startCollapsed: 'accordion'
        });
    }

    //Responsive Tables
    //--------------------------------------------------------
    var tableObj = $('.table');
    var shoptableObj = $('.shop_table');
    if (tableObj.length && $.fn.basictable) {
        tableObj.basictable({
            breakpoint: 991
        });
    }

    if (shoptableObj.length && $.fn.basictable) {
        shoptableObj.basictable({
            breakpoint: 991
        });
    }

    //Form Fields (Value Disappear on Focus)
    //--------------------------------------------------------
    var requiredFieldObj = $('.input-required');

    requiredFieldObj.find('input').on('focus',function(){
        if(!$(this).parent(requiredFieldObj).find('label').hasClass('hide')){
            $(this).parent(requiredFieldObj).find('label').addClass('hide');
        }
    });
    requiredFieldObj.find('input').on('blur',function(){
        if($(this).val() === '' && $(this).parent(requiredFieldObj).find('label').hasClass('hide')){
            $(this).parent(requiredFieldObj).find('label').removeClass('hide');
        }
    });

    //Bootstrap Carousel Swipe (Testimonials Carousel)
    //--------------------------------------------------------
    var testimonialsObj = $("#testimonials");
    if (testimonialsObj.length && $.fn.swiperight && $.fn.swipeleft) {
        testimonialsObj.swiperight(function () {
            $(this).carousel('prev');
        });
        testimonialsObj.swipeleft(function () {
            $(this).carousel('next');
        });
    }

    //Bx Carousel
    //--------------------------------------------------------

    //Popular Items Detail V1

    var popularSlidesD1 = 2;
    var popularWidthD1 = 370;
    var popularMarginD1 = 54;

    if($(window).width() <= 1199) {
        popularSlidesD1 = 2;
        popularWidthD1 = 330;
        popularMarginD1 = 37;
    }
    if($(window).width() <= 991) {
        popularSlidesD1 = 2;
        popularWidthD1 = 350;
        popularMarginD1 = 20;
    }
    if($(window).width() <= 767) {
        popularSlidesD1 = 1;
        popularWidthD1 = 320;
        popularMarginD1 = 0;
    }

    var popularItemObjD1 = $('.popular-items-detail-v1');
    if (popularItemObjD1.length && $.fn.bxSlider) {
        popularItemObjD1.bxSlider({
            minSlides: 1,
            maxSlides: popularSlidesD1,
            slideWidth: popularWidthD1,
            slideMargin: popularMarginD1,
            responsive: true,
            touchEnabled: true,
            controls: false,
            infiniteLoop: true,
            shrinkItems: true
        });
    }

    //Popular Items Detail V2

    var popularSlidesD2 = 3;
    var popularWidthD2 = 360;
    var popularMarginD2 = 30;

    if($(window).width() <= 1199) {
        popularSlidesD2 = 3;
        popularWidthD2 = 300;
        popularMarginD2 = 20;
    }
    if($(window).width() <= 991) {
        popularSlidesD2 = 2;
        popularWidthD2 = 350;
        popularMarginD2 = 20;
    }
    if($(window).width() <= 767) {
        popularSlidesD2 = 1;
        popularWidthD2 = 320;
        popularMarginD2 = 0;
    }

    var popularItemObjD2 = $('.popular-items-detail-v2');
    if (popularItemObjD2.length && $.fn.bxSlider) {
        popularItemObjD2.bxSlider({
            minSlides: 1,
            maxSlides: popularSlidesD2,
            slideWidth: popularWidthD2,
            slideMargin: popularMarginD2,
            responsive: true,
            touchEnabled: true,
            controls: false,
            infiniteLoop: true,
            shrinkItems: true
        });
    }

    //Contact Form Submit/Validation
    //--------------------------------------------------------
    var emailerrorvalidation = 0;
    var formObj = $('#contact');
    var contactFormObj = $('#submit-contact-form');
    var firstNameFieldObj = $("#first-name");
    var lastNameFieldObj = $("#last-name");
    var emailFieldObj = $("#email");
    var phoneFieldObj = $("#phone");
    var messageFieldObj = $("#message");
    var successObj = $('#success');
    var errorObj = $('#error');

    if (contactFormObj.length) {
        contactFormObj.on('click', function () {
            var emailaddress = emailFieldObj.val();
            function validateEmail(emailaddress) {
                var filter = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,4}$/;
                if (filter.test(emailaddress)) {
                    return true;
                } else {
                    return false;
                }
            }

            var data = {
                firstname: firstNameFieldObj.val(),
                lastname: lastNameFieldObj.val(),
                email: emailFieldObj.val(),
                phone: phoneFieldObj.val(),
                message: messageFieldObj.val()
            };
            if (data.firstname === '' || data.lastname === '' || data.email === '' || data.phone === '' || data.message === '') {
                alert("All fields are mandatory");
            } else {
                if (validateEmail(emailaddress)) {
                    if (emailerrorvalidation === 1) {
                        alert('Nice! your Email is valid, you can proceed now.');
                    }
                    emailerrorvalidation = 0;
                    $.ajax({
                        type: "POST",
                        url: "contact.php",
                        data: data,
                        cache: false,
                        success: function () {
                            successObj.fadeIn(1000);
                            formObj[0].reset();
                        },
                        error: function () {
                            errorObj.fadeIn(1000);
                        }
                    });
                } else {
                    emailerrorvalidation = 1;
                    alert('Oops! Invalid Email Address');
                }
            }
            return false;
        });
    }
});

$( window ).load(function() {
    //Masonry
    //--------------------------------------------------------
    var girdFieldObj = $('.grid');
    if (girdFieldObj.length && $.fn.masonry) {
        girdFieldObj.masonry({
            itemSelector: '.grid-item',
            percentPosition: true
        });
    }
});