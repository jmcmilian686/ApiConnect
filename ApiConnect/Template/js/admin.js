if (typeof jQuery === "undefined") {
    throw new Error("jQuery plugins need to be before this file");
}

$.AdminBSB = {};
$.AdminBSB.options = {
    colors: {
        red: '#F44336',
        pink: '#E91E63',
        purple: '#9C27B0',
        deepPurple: '#673AB7',
        indigo: '#3F51B5',
        blue: '#2196F3',
        lightBlue: '#03A9F4',
        cyan: '#00BCD4',
        teal: '#009688',
        green: '#4CAF50',
        lightGreen: '#8BC34A',
        lime: '#CDDC39',
        yellow: '#ffe821',
        amber: '#FFC107',
        orange: '#FF9800',
        deepOrange: '#FF5722',
        brown: '#795548',
        grey: '#9E9E9E',
        blueGrey: '#607D8B',
        black: '#000000',
        white: '#ffffff'
    },
    leftSideBar: {
        scrollColor: 'rgba(0,0,0,0.5)',
        scrollWidth: '4px',
        scrollAlwaysVisible: false,
        scrollBorderRadius: '0',
        scrollRailBorderRadius: '0',
        scrollActiveItemWhenPageLoad: true,
        breakpointWidth: 1170
    },
    dropdownMenu: {
        effectIn: 'fadeIn',
        effectOut: 'fadeOut'
    }
}

/* Left Sidebar - Function =================================================================================================
*  You can manage the left sidebar menu options
*  
*/
$.AdminBSB.leftSideBar = {
    activate: function () {
        var _this = this;
        var $body = $('body');
        var $overlay = $('.overlay');

        //Close sidebar
        $(window).click(function (e) {
            var $target = $(e.target);
            if (e.target.nodeName.toLowerCase() === 'i') { $target = $(e.target).parent(); }

            if (!$target.hasClass('bars') && _this.isOpen() && $target.parents('#leftsidebar').length === 0) {
                if (!$target.hasClass('js-right-sidebar')) $overlay.fadeOut();
                $body.removeClass('overlay-open');
            }
        });

        $.each($('.menu-toggle.toggled'), function (i, val) {
            $(val).next().slideToggle(0);
        });

        //When page load
        $.each($('.menu .list li.active'), function (i, val) {
            var $activeAnchors = $(val).find('a:eq(0)');

            $activeAnchors.addClass('toggled');
            $activeAnchors.next().show();
        });

        //Collapse or Expand Menu
        $('.menu-toggle').on('click', function (e) {
            var $this = $(this);
            var $content = $this.next();

            if ($($this.parents('ul')[0]).hasClass('list')) {
                var $not = $(e.target).hasClass('menu-toggle') ? e.target : $(e.target).parents('.menu-toggle');

                $.each($('.menu-toggle.toggled').not($not).next(), function (i, val) {
                    if ($(val).is(':visible')) {
                        $(val).prev().toggleClass('toggled');
                        $(val).slideUp();
                    }
                });
            }

            $this.toggleClass('toggled');
            $content.slideToggle(320);
        });

        //Set menu height
        _this.setMenuHeight();
        _this.checkStatuForResize(true);
        $(window).resize(function () {
            _this.setMenuHeight();
            _this.checkStatuForResize(false);
        });

        //Set Waves
        Waves.attach('.menu .list a', ['waves-block']);
        Waves.init();
    },
    setMenuHeight: function (isFirstTime) {
        if (typeof $.fn.slimScroll !== 'undefined') {
            var configs = $.AdminBSB.options.leftSideBar;
            var height = ($(window).height() - ($('.legal').outerHeight() + $('.user-info').outerHeight() + $('.navbar').innerHeight()));
            var $el = $('.list');

            $el.slimscroll({
                height: height + "px",
                color: configs.scrollColor,
                size: configs.scrollWidth,
                alwaysVisible: configs.scrollAlwaysVisible,
                borderRadius: configs.scrollBorderRadius,
                railBorderRadius: configs.scrollRailBorderRadius
            });

            //Scroll active menu item when page load, if option set = true
            if ($.AdminBSB.options.leftSideBar.scrollActiveItemWhenPageLoad) {
                var activeItemOffsetTop = $('.menu .list li.active')[0].offsetTop
                if (activeItemOffsetTop > 150) $el.slimscroll({ scrollTo: activeItemOffsetTop + 'px' });
            }
        }
    },
    checkStatuForResize: function (firstTime) {
        var $body = $('body');
        var $openCloseBar = $('.navbar .navbar-header .bars');
        var width = $body.width();

        if (firstTime) {
            $body.find('.content, .sidebar').addClass('no-animate').delay(1000).queue(function () {
                $(this).removeClass('no-animate').dequeue();
            });
        }

        if (width < $.AdminBSB.options.leftSideBar.breakpointWidth) {
            $body.addClass('ls-closed');
            $openCloseBar.fadeIn();
        }
        else {
            $body.removeClass('ls-closed');
            $openCloseBar.fadeOut();
        }
    },
    isOpen: function () {
        return $('body').hasClass('overlay-open');
    }
};
//==========================================================================================================================

/* Right Sidebar - Function ================================================================================================
*  You can manage the right sidebar menu options
*  
*/
$.AdminBSB.rightSideBar = {
    activate: function () {
        var _this = this;
        var $sidebar = $('#rightsidebar');
        var $overlay = $('.overlay');

        //Close sidebar
        $(window).click(function (e) {
            var $target = $(e.target);
            if (e.target.nodeName.toLowerCase() === 'i') { $target = $(e.target).parent(); }

            if (!$target.hasClass('js-right-sidebar') && _this.isOpen() && $target.parents('#rightsidebar').length === 0) {
                if (!$target.hasClass('bars')) $overlay.fadeOut();
                $sidebar.removeClass('open');
            }
        });

        $('.js-right-sidebar').on('click', function () {
            $sidebar.toggleClass('open');
            if (_this.isOpen()) { $overlay.fadeIn(); } else { $overlay.fadeOut(); }
        });
    },
    isOpen: function () {
        return $('.right-sidebar').hasClass('open');
    }
}
//==========================================================================================================================

/* Searchbar - Function ================================================================================================
*  You can manage the search bar
*  
*/
var $searchBar = $('.search-bar');
$.AdminBSB.search = {
    activate: function () {
        var _this = this;

        //Search button click event
        $('.js-search').on('click', function () {
            _this.showSearchBar();
        });

        //Close search click event
        $searchBar.find('.close-search').on('click', function () {
            _this.hideSearchBar();
        });

        //ESC key on pressed
        $searchBar.find('input[type="text"]').on('keyup', function (e) {
            if (e.keyCode === 27) {
                _this.hideSearchBar();
            }
        });
    },
    showSearchBar: function () {
        $searchBar.addClass('open');
        $searchBar.find('input[type="text"]').focus();
    },
    hideSearchBar: function () {
        $searchBar.removeClass('open');
        $searchBar.find('input[type="text"]').val('');
    }
}
//==========================================================================================================================

/* Navbar - Function =======================================================================================================
*  You can manage the navbar
*  
*/
$.AdminBSB.navbar = {
    activate: function () {
        var $body = $('body');
        var $overlay = $('.overlay');

        //Open left sidebar panel
        $('.bars').on('click', function () {
            $body.toggleClass('overlay-open');
            if ($body.hasClass('overlay-open')) { $overlay.fadeIn(); } else { $overlay.fadeOut(); }
        });

        //Close collapse bar on click event
        $('.nav [data-close="true"]').on('click', function () {
            var isVisible = $('.navbar-toggle').is(':visible');
            var $navbarCollapse = $('.navbar-collapse');

            if (isVisible) {
                $navbarCollapse.slideUp(function () {
                    $navbarCollapse.removeClass('in').removeAttr('style');
                });
            }
        });
    }
}
//==========================================================================================================================

/* Input - Function ========================================================================================================
*  You can manage the inputs(also textareas) with name of class 'form-control'
*  
*/
$.AdminBSB.input = {
    activate: function () {
        //On focus event
        $('.form-control').focus(function () {
            $(this).parent().addClass('focused');
        });

        //On focusout event
        $('.form-control').focusout(function () {
            var $this = $(this);
            if ($this.parents('.form-group').hasClass('form-float')) {
                if ($this.val() === '') { $this.parents('.form-line').removeClass('focused'); }
            }
            else {
                $this.parents('.form-line').removeClass('focused');
            }
        });

        //On label click
        $('body').on('click', '.form-float .form-line .form-label', function () {
            $(this).parent().find('input').focus();
        });

        //Not blank form
        $('.form-control').each(function () {
            if ($(this).val() !== '') {
                $(this).parents('.form-line').addClass('focused');
            }
        });
    }
}
//==========================================================================================================================

/* Form - Select - Function ================================================================================================
*  You can manage the 'select' of form elements
*  
*/
$.AdminBSB.select = {
    activate: function () {
        if ($.fn.selectpicker) { $('select:not(.ms)').selectpicker(); }
    }
}
//==========================================================================================================================

/* DropdownMenu - Function =================================================================================================
*  You can manage the dropdown menu
*  
*/

$.AdminBSB.dropdownMenu = {
    activate: function () {
        var _this = this;

        $('.dropdown, .dropup, .btn-group').on({
            "show.bs.dropdown": function () {
                var dropdown = _this.dropdownEffect(this);
                _this.dropdownEffectStart(dropdown, dropdown.effectIn);
            },
            "shown.bs.dropdown": function () {
                var dropdown = _this.dropdownEffect(this);
                if (dropdown.effectIn && dropdown.effectOut) {
                    _this.dropdownEffectEnd(dropdown, function () { });
                }
            },
            "hide.bs.dropdown": function (e) {
                var dropdown = _this.dropdownEffect(this);
                if (dropdown.effectOut) {
                    e.preventDefault();
                    _this.dropdownEffectStart(dropdown, dropdown.effectOut);
                    _this.dropdownEffectEnd(dropdown, function () {
                        dropdown.dropdown.removeClass('open');
                    });
                }
            }
        });

        //Set Waves
        Waves.attach('.dropdown-menu li a', ['waves-block']);
        Waves.init();
    },
    dropdownEffect: function (target) {
        var effectIn = $.AdminBSB.options.dropdownMenu.effectIn, effectOut = $.AdminBSB.options.dropdownMenu.effectOut;
        var dropdown = $(target), dropdownMenu = $('.dropdown-menu', target);

        if (dropdown.length > 0) {
            var udEffectIn = dropdown.data('effect-in');
            var udEffectOut = dropdown.data('effect-out');
            if (udEffectIn !== undefined) { effectIn = udEffectIn; }
            if (udEffectOut !== undefined) { effectOut = udEffectOut; }
        }

        return {
            target: target,
            dropdown: dropdown,
            dropdownMenu: dropdownMenu,
            effectIn: effectIn,
            effectOut: effectOut
        };
    },
    dropdownEffectStart: function (data, effectToStart) {
        if (effectToStart) {
            data.dropdown.addClass('dropdown-animating');
            data.dropdownMenu.addClass('animated dropdown-animated');
            data.dropdownMenu.addClass(effectToStart);
        }
    },
    dropdownEffectEnd: function (data, callback) {
        var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        data.dropdown.one(animationEnd, function () {
            data.dropdown.removeClass('dropdown-animating');
            data.dropdownMenu.removeClass('animated dropdown-animated');
            data.dropdownMenu.removeClass(data.effectIn);
            data.dropdownMenu.removeClass(data.effectOut);

            if (typeof callback === 'function') {
                callback();
            }
        });
    }
}
//==========================================================================================================================

/* Browser - Function ======================================================================================================
*  You can manage browser
*  
*/
var edge = 'Microsoft Edge';
var ie10 = 'Internet Explorer 10';
var ie11 = 'Internet Explorer 11';
var opera = 'Opera';
var firefox = 'Mozilla Firefox';
var chrome = 'Google Chrome';
var safari = 'Safari';

$.AdminBSB.browser = {
    activate: function () {
        var _this = this;
        var className = _this.getClassName();

        if (className !== '') $('html').addClass(_this.getClassName());
    },
    getBrowser: function () {
        var userAgent = navigator.userAgent.toLowerCase();

        if (/edge/i.test(userAgent)) {
            return edge;
        } else if (/rv:11/i.test(userAgent)) {
            return ie11;
        } else if (/msie 10/i.test(userAgent)) {
            return ie10;
        } else if (/opr/i.test(userAgent)) {
            return opera;
        } else if (/chrome/i.test(userAgent)) {
            return chrome;
        } else if (/firefox/i.test(userAgent)) {
            return firefox;
        } else if (!navigator.userAgent.match(/Version\/[\d\.]+.*Safari/)) {
            return safari;
        }

        return undefined;
    },
    getClassName: function () {
        var browser = this.getBrowser();

        if (browser === edge) {
            return 'edge';
        } else if (browser === ie11) {
            return 'ie11';
        } else if (browser === ie10) {
            return 'ie10';
        } else if (browser === opera) {
            return 'opera';
        } else if (browser === chrome) {
            return 'chrome';
        } else if (browser === firefox) {
            return 'firefox';
        } else if (browser === safari) {
            return 'safari';
        } else {
            return '';
        }
    }
}
//==========================================================================================================================

$(function () {
    $.AdminBSB.leftSideBar.activate();
    $.AdminBSB.dropdownMenu.activate();
    setTimeout(function () { $('.page-loader-wrapper').fadeOut(); }, 50);
});

$(document).ready(function () {

    $('#formLogin input').each(function () {

        $(this).addClass("form-control");

    });

    $('#formSortC input').each(function () {

        $(this).addClass("form-control");
        $(this).attr("required", "true");

    });
    $('#formSortR input').each(function () {

        $(this).addClass("form-control");
        $(this).attr("required", "true");

    });

    $('#formReSortR input').each(function () {

        $(this).addClass("form-control");
        $(this).attr("required", "true");

    });

    $("#formFields").hide();

});

$(document).on("click", "#formButton", function (e) {

    e.preventDefault();
    var user = $("#username").val();
    var pass = $("#password").val();
    var hos = $("#host").val();


    $.ajax({

        url: '/Home/Connect/',
        method: 'post',
        data: $('#formLogin').serialize(),
        success: function (data) {

            $("#formArea").html(data);
            $('#formLogin input ').each(function () {

                $(this).addClass("form-control");
            })

            $("#discLi").html("<a href='/Home/Disconnect' style='color: red'><i class='material-icons'>swap_horiz</i></a>");

        }
    });

});

//========================================Token Cred=======================================================
$("#custom").on("click", function (e) {
    e.preventDefault();
    console.log("clicked");
 

    $("#formFields").show(500);
});


$("#defaults").on("click", function (e) {
    e.preventDefault();
    console.log("clicked");
  



    $("#formFields").hide(500);
});

$("#formFields").ready(function () {
    $("#formFields").hide();
});

//======================Fields Handling Area========================================================================================


$("#btn_plus").click(function (e) {
    console.log("clicked");
    e.preventDefault();
    $('#largeModal1').modal();

});

var idDel = 0;

$(".btnDel").click(function (e) {
    e.preventDefault;
    $(this).parents().siblings().each(function () {

        if ($(this).hasClass("idElem")) {
            idDel = $(this).text();
        }

    })
    $("#defaultModal1").modal();
    console.log(idDel);

});


$("#clModal").click(function (e) {
    e.preventDefault();
    console.log("clicked");

    /* $.get('Field/Details', function (data) {
         $('#tabDetail').html(data);
     }); */
});


$("#delFinal").click(function (e) {

    e.preventDefault();
    if (idDel > 0) {

        var url1 = '/Field/Delete/' + idDel;
        $.ajax({
            // edit to add steve's suggestion.
            //url: "/ControllerName/ActionName",
            url: url1,
            method: 'POST',  // post
            success: function (data) {
                if (data === "ok") {
                    $("#updArea").html("<h4> Field Deleted Successfully!</h4>");
                    $("#delFinal").hide();
                    $("#clModal").addClass("bg-green");
                    $("#clModal").attr('onclick', 'location.href = \'/Field/Index\'');
                }

            }
        });

    }

});

// Edit Action
$(".btnEdit").click(function (e) {
    console.log("clicked");
    var id = -1;
    $(this).parents().siblings().each(function () {

        if ($(this).hasClass("idElem")) {
            id = $(this).text();
        }

    })
    var url1 = '/Field/Edit/' + id;

    $.ajax({
        // edit to add steve's suggestion.
        //url: "/ControllerName/ActionName",
        url: url1,
        success: function (data) {

            $("#largeModal1").html(data);
        }
    });
    $('#largeModal1').modal();


});

// Save Action
$("#largeModal1").on("submit", "#form-fieldedit", function (e) {
    e.preventDefault();  // prevent standard form submission

    var form = $(this);
    $.ajax({
        url: form.attr("action"),
        method: form.attr("method"),  // post
        data: form.serialize(),
        success: function (partialResult) {
            $("#largeModal1").html(partialResult);
        }
    });
});


//====================== End of Fields Handling Area========================================================================================


//=======================DataField Area=====================================================================================================



$("#addition").click(function (e) {
    e.preventDefault();

    var dict = {
        key: 0,
        value: 0
    };

    var id = $("#fldId").val();

    var mod = {
        ID: id,
        DataValue: dict

    };

    $.ajax({

        url: '/Datafield/Table/',
        data: mod,
        success: function (data) {

            $("#renderTable").html(data);
            $(".addData").each(function () {
                console.log($(this));
                $(this).prop('required', true);
            });
        }
    });


});

$("#linking").click(function (e) {
    e.preventDefault();


    var id = $("#fldId").val();

    var arr = [];
    $(".addData").each(function () {
        var Elements = {
            Name: $(this).attr('name'),
            Val: $(this).val()

        };

        arr.push(Elements);


    });



    var mod = {
        ID: id,
        DataValue: arr

    };
    console.log(mod);

    $.ajax({

        url: '/Datafield/Table/',
        data: mod,
        method: "post",
        success: function (data) {

            $("#renderTable").html(data);
            $(".addData").each(function () {
                console.log($(this));
                $(this).prop('required', true);
            });
        }
    });


});

$("#dataSave").on('click', function (e) {
    e.preventDefault();


    var arr = [];
    $(".addData").each(function () {
        var Elements = {
            Name: $(this).attr('name'),
            Val: $(this).val()

        };

        arr.push(Elements);


    });

    console.log(arr);

    var mod = {
        ID: 0,
        DataValue: arr

    };


    console.log(mod);

    $.ajax({

        url: '/Datafield/Create/',
        method: 'post',
        data: mod,
        success: function (data) {
            if (data === "ok") {
                $(".form-insert").remove();
                $("#modalOk").modal();
            } else {
                $(".form-insert").remove();
                $("#modalCont").html("<p>" + data + "</p>");
                $("#modalOk").modal();


            }
        }
    });



});

$("#sel_file").on("click", function (e) {

    e.preventDefault();
    var n = $(".addData").length;
    console.log(n);



    $("#file1").click();

    console.log($("#file1").val());



});

$("#file1").change(function () {
    $('#selectedF').text($('#file1')[0].files[0].name);
});

//==========================Batch loading=====================================================

$("#loadBatch").on("click", function (e) {

    e.preventDefault();

    $("#file3").click();

    var file = $("#file3").val();

});

//$("#file3").change(function () {
//    var files = $('#file3')[0].files;
//    if (files.length > 0) {

//        if (window.FormData !== undefined) {

//            var data = new FormData();
//            for (var x = 0; x < files.length; x++) {
//                data.append("file" + x, files[x]);
//            }

//            console.log(data);

//            $.ajax({
//                type: "POST",
//                url: '/Send/UpBatch',
//                contentType: false,
//                processData: false,
//                data: data,
//                beforeSend: function () {
//                    $('.page-loader-wrapper').fadeIn();
//                },
//                success: function (result) {
                   
//                    $('.page-loader-wrapper').fadeOut();
//                    console.log("Ok");
//                    window.location.reload();
//                },
//                error: function (xhr, status, p3, p4) {
//                    var err = "Error " + " " + status + " " + p3 + " " + p4;
//                    if (xhr.responseText && xhr.responseText[0] === "{")
//                        err = JSON.parse(xhr.responseText).Message;
//                    console.log(err);
//                    $('.page-loader-wrapper').fadeOut();
//                }
//            });

//        }


//    }
//});

//============================================================================================

//==========================Orders Sending=====================================================
$("#uplorders").on("click", function (e) {

    e.preventDefault();
    var n = $(".addData").length;
    console.log(n);



    $("#file2").click();

    console.log($("#file2").val());



});

$("#file2").change(function () {
    $('#selectedF').text($('#file2')[0].files[0].name);
    $('#dataUpl2').removeClass("hidden");
});
//===========================End of Order Sending=============================================


$("#dataUpl2").on("click", function (e) {

    e.preventDefault();

    var files = $('#file2')[0].files;




    if (files.length > 0) {

        if (window.FormData !== undefined) {

            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }

            console.log(data);

            $.ajax({
                type: "POST",
                url: '/Send/UpFile',
                contentType: false,
                processData: false,
                data: data,
                beforeSend: function () {
                    //$('.page-loader-wrapper').fadeIn();
                },
                success: function (result) {
                    //console.log(result.length);
                    //$("#OrdQty").val(result.length);
                    //var appendable = "";
                    //$.each(result, function (i, value) {
                    //    //console.log(value);
                    //    appendable = appendable + '<tr><td>' + value.Message + '</td></tr>';
                        
                    //});
                    //var tableStruct = `<table class="table table-bordered table-striped table-hover js-basic-example dataTable">
                    //    <thead>
                    //         <th>Message</th>
                    //    </thead>
                    //    <tbody id="tableBody">`+ appendable +
                        
                    //    `</tbody>
                    //</table>`;

                    //$("#appendTable2").append(tableStruct);
                    $('.page-loader-wrapper').fadeOut();
                    console.log("Ok");
                    window.location.reload();
                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] === "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                    $('.page-loader-wrapper').fadeOut();
                }
            });

        }


    }

});

$("#batchData").on("click", function (e) {
    e.preventDefault();
    $("#modalBatch").modal();

});

$("#batchCrea").on("click", function (e) {
    e.preventDefault();

    //var files = $('#file3')[0].files;
    var cont = $("#contBatch").val();



    //if (files.length > 0) {

        if (window.FormData !== undefined) {

            var dataM = new FormData();
            dataM.append("cont", cont);

            //for (var x = 0; x < files.length; x++) {
            //    data.append("file" + x, files[x]);
            //}

            console.log(dataM);

            $.ajax({
                type: "POST",
                url: '/Send/UpBatch',
                contentType: false,
                processData: false,
                data: dataM,
                beforeSend: function () {
                    //$('.page-loader-wrapper').fadeIn();
                },
                success: function (result) {
                    $('.page-loader-wrapper').fadeOut();
                    $("#updAreaResult").append("<p>" + result + "</p>");
                    $("#modalBatch").modal('toggle');
                    $("#modalResult").modal();
                    console.log("Ok");
            
                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] === "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                    $('.page-loader-wrapper').fadeOut();
                }
            });

        }


    //}
});

$("#processData").on("click", function (e) {
    e.preventDefault();

    var timer = parseInt($("#Timer").val());

    var mod = {
        Timer: timer
    };

    $.ajax({
        type: "POST",
        url: '/Send/SendTimed',
        data: mod,
        beforeSend: function () {
            $('.page-loader-wrapper').fadeIn();
        },
        success: function (data) {
            
            $("#updAreaResult").append("<p>"+data+"</p>");
            $("#modalResult").modal();
            $('.page-loader-wrapper').fadeOut();
            console.log(data);
           // window.location.reload();
        },
        error: function (xhr, status, p3, p4) {
            var err = "Error " + " " + status + " " + p3 + " " + p4;
            if (xhr.responseText && xhr.responseText[0] === "{")
                err = JSON.parse(xhr.responseText).Message;
            console.log(err);
            $('.page-loader-wrapper').fadeOut();
        }
    });


});

//===========================End of Order Sending=============================================


$("#dataUpl").on("click", function (e) {

    e.preventDefault();

    var files = $('#file1')[0].files;




    if (files.length > 0) {

        if (window.FormData !== undefined) {

            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }

            console.log(data);

            $.ajax({
                type: "POST",
                url: '/Datafield/UpFile',
                contentType: false,
                processData: false,
                data: data,
                beforeSend: function () {
                    $('.page-loader-wrapper').fadeIn();
                },
                success: function (result) {
                    console.log(result);
                    $('.page-loader-wrapper').fadeOut();
                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] === "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                    $('.page-loader-wrapper').fadeOut();
                }
            });

        }


    }

});



//+++++++++++ Linked Button Press+++++++++++++++++++++

$(".btnLinV").click(function (e) {
    e.preventDefault();
    var id = -1;
    $(this).parents().siblings().each(function () {

        if ($(this).hasClass("idElem")) {
            id = $(this).text();
        }

    });

    console.log(id);

    $.ajax({

        url: '/Datafield/LinkedList/' + id,
        method: 'post',
        success: function (data) {
            $("#updArea").html(data);
            $("#delFinalData").hide();
            $("#defaultModal").modal();

        }
    });


});

var idDelete = 0;

$(".btnDelData").click(function (e) {

    e.preventDefault();
    idDelete = 0;

    $(this).parents().siblings().each(function () {

        if ($(this).hasClass("idElem")) {
            idDelete = $(this).text();
        }

    });

    var data = "<h4> Do you want to delete the selected Data?</h4>"
    $("#updArea").html(data);
    $("#delFinalData").show();
    $("#defaultModal").modal();


});



$("#delFinalData").click(function (e) {

    e.preventDefault();

    console.log(idDelete);
    var dataS = {
        id: idDelete,
        returnUrl: window.location.href
    };

    $.ajax({

        url: '/Datafield/Delete/',
        method: 'post',
        data: dataS,
        success: function (data) {
            if (data !== "") {

                var newdata = "<h4>Data was removed<h4>"
                $("#updArea").html(newdata);
            } else {

                $("#updArea").html(data);
            }

            $("#delFinalData").hide();
            $("#defaultModal").modal();
            $("#clModalData").attr('onclick', 'location.href =\''+data+'\'');

        }
    });



});

//========================End of Data Field Area============================================================================================

//=========================Robots Area======================================================================================================


$("#btn_plusR").click(function (e) {
    console.log("clicked");
    e.preventDefault();
    $('#largeModalR').modal();

});

var idDel2 = 0;

$(".btnDelR").click(function (e) {
    e.preventDefault;
    $(this).parents().siblings().each(function () {

        if ($(this).hasClass("idElem")) {
            idDel2 = $(this).text();
        }

    });
    $("#defaultModalR").modal();
    console.log(idDel2);

});




$("#delFinalR").click(function (e) {

    e.preventDefault();
    if (idDel > 0) {

        var url1 = '/Robots/Delete/' + idDel;
        $.ajax({
            // edit to add steve's suggestion.
            //url: "/ControllerName/ActionName",
            url: url1,
            method: 'POST',  // post
            success: function (data) {
                if (data === "ok") {
                    $("#updAreaR").html("<h4> Robot Deleted Successfully!</h4>");
                    $("#delFinalR").hide();
                    $("#clModalR").addClass("bg-green");
                    $("#clModalR").attr('onclick', 'location.href = \'/Robots/Index\'');
                }

            }
        });

    }

});

// Edit Action
$(".btnEditR").click(function (e) {
    console.log("clicked");
    var id = -1;
    $(this).parents().siblings().each(function () {

        if ($(this).hasClass("idElem")) {
            id = $(this).text();
        }

    })
    var url1 = '/Robots/Edit/' + id;

    $.ajax({
        // edit to add steve's suggestion.
        //url: "/ControllerName/ActionName",
        url: url1,
        success: function (data) {

            $("#largeModalR").html(data);
        }
    });
    $('#largeModalR').modal();


});

// Save Action
$("#largeModalR").on("submit", "#form-fieldedit", function (e) {
    e.preventDefault();  // prevent standard form submission

    var form = $(this);
    $.ajax({
        url: form.attr("action"),
        method: form.attr("method"),  // post
        data: form.serialize(),
        success: function (partialResult) {
            $("#largeModalR").html(partialResult);
        }
    });
});

//========================== End of Robots Area=============================================================================================

//========================================= Wave Generation Area ===========================================================================

//Horizontal form basic
$('#wizard_horizontal').steps({
    headerTag: 'h2',
    bodyTag: 'section',
    transitionEffect: 'slideLeft',
    onInit: function (event, currentIndex) {
        setButtonWavesEffect(event);
    },
    onStepChanged: function (event, currentIndex, priorIndex) {
        setButtonWavesEffect(event);
    },
    onFinishing: function (event) {
        
        gatherData();
    }

});

//Horizontal form basic
$('#wizard_horizontal2').steps({
    headerTag: 'h2',
    bodyTag: 'section',
    transitionEffect: 'slideLeft',
    onInit: function (event, currentIndex) {
        setButtonWavesEffect(event);
    },
    onStepChanged: function (event, currentIndex, priorIndex) {
        setButtonWavesEffect(event);
        if (currentIndex === 1) {
            checkDetailQty();
        }
        
    },
    onFinishing: function (event) {

        gatherData();
    }

});

function setButtonWavesEffect(event) {
    $(event.currentTarget).find('[role="menu"] li a').removeClass('waves-effect');
    $(event.currentTarget).find('[role="menu"] li:not(.disabled) a').addClass('waves-effect');
}


                        //=======Wizzard Wave===========
$(document).on("click", "#addCustomizeW",function (e) {

    e.preventDefault();


    var field = $("#fieldIdWav").val();
    var name = $("#fieldIdWav").find('option:selected').text();
    
    if (field !== "") {

        var mod = {
            Field: field
        };


        $.ajax({

            url: '/Wave/Comboshow/',
            method: 'post',
            data: mod,
            success: function (data) {

                console.log(data);

                if (data.length > 0) {



                    var listOpt = "";

                    $.each(data, function (i, item) {
                        console.log(item);
                        listOpt += '<option value="' + item.Id + '">' + item.Value + '</option>';
                    });

                    var newList = `<div class="form-group row grupp" style="margin-bottom:5px;">
                                   <div class="col-md-3 nobott">
                                    <label>`+ name + `:</label>
                                   </div> 
                                   <div class="col-md-5 clist nobott">
                                       <select class="form-control col-md-8 show-tick valFilt" id="` + field + `">
                                                <option value="">-- Please select --</option>
                                                `+ listOpt + `
                                       </select>
                                   </div>
                                   
                                   <div class="col-md-1 nobott">
                                       <button type="button" class="  btnDelFilt btn btn-table bg-deep-orange waves-effect" >
                                                <i class="material-icons">remove</i>
                                       </button>
                                   </div>
                                   </div>`;

                    $("#wavConfPar").append(newList);

                } else {
                    console.log(mod);
                    $.ajax({
                        url: '/Wave/Nameshow/', method: 'post', data: mod, success: function (data) {

                            var newTextb = `<div class="form-group row grupp" style="margin-bottom:5px;">
                                   <div class="col-md-3 nobott">
                                    <label>`+ name + `:</label>
                                   </div> 
                                   <div class="col-md-5 nobott">
                                       <input type="text" class="form-control col-md-8 show-tick valFilt textbox" id="` + data + `" style="border-bottom: 1px solid #ddd"/>
                                      
                                   </div>
                                  
                                   <div class="col-md-1 nobott">
                                       <button type="button" class="  btnDelFilt btn btn-table bg-deep-orange waves-effect">
                                                <i class="material-icons">remove</i>
                                       </button>
                                   </div>
                                   </div>`;

                            $("#wavConfPar").append(newTextb);

                        }
                    });

                }
                
            }
        });

    }



});

$(document).on("click", ".btnDelFilt", function (e) {

    e.preventDefault();
    $(this).parents().closest('.grupp').remove();;


});
                       //=================End Wizzard Wave===========


                       //=================Wizzard Order ============
$(document).on("click", "#addCustomizeO", function (e) {

    e.preventDefault();


    var field = $("#fieldIdOrd").val();
    var name = $("#fieldIdOrd").find('option:selected').text();

    if (field !== "") {

        var mod = {
            Field: field
        }


        $.ajax({

            url: '/Wave/Comboshow/',
            method: 'post',
            data: mod,
            success: function (data) {

                console.log(data);

                if (data.length > 0) {



                    var listOpt = "";

                    $.each(data, function (i, item) {
                        console.log(item);
                        listOpt += '<option value="' + item.Id + '">' + item.Value + '</option>';
                    });

                    var newList = `<div class="form-group row grupp" style="margin-bottom:5px;">
                                   <div class="col-md-3 nobott">
                                    <label>`+ name + `:</label>
                                   </div> 
                                   <div class="col-md-5 clist nobott">
                                       <select class="form-control col-md-8 show-tick valFilt" id="` + field + `">
                                                <option value="">-- Please select --</option>
                                                `+ listOpt + `
                                       </select>
                                   </div>
                                   
                                   <div class="col-md-1 nobott">
                                       <button type="button" class="  btnDelFilt btn btn-table bg-deep-orange waves-effect" >
                                                <i class="material-icons">remove</i>
                                       </button>
                                   </div>
                                   </div>`;

                    $("#ordConfPar").append(newList);

                } else {
                    console.log(mod);
                    $.ajax({
                        url: '/Wave/Nameshow/', method: 'post', data: mod, success: function (data) {

                            var newTextb = `<div class="form-group row grupp" style="margin-bottom:5px;">
                                   <div class="col-md-3 nobott">
                                    <label>`+ name + `:</label>
                                   </div> 
                                   <div class="col-md-5 nobott">
                                       <input type="text" class="form-control col-md-8 show-tick valFilt textbox" id="` + data + `" style="border-bottom: 1px solid #ddd"/>
                                      
                                   </div>
                                  
                                   <div class="col-md-1 nobott">
                                       <button type="button" class="  btnDelFilt btn btn-table bg-deep-orange waves-effect">
                                                <i class="material-icons">remove</i>
                                       </button>
                                   </div>
                                   </div>`;

                            $("#ordConfPar").append(newTextb);

                        }
                    })

                }

            }
        });

    }



});




                       //=================End Wizzard Order========

                       //====================== Wizzard Container ======

$(document).on("click", "#addCustomizeC", function (e) {

    e.preventDefault();


    var field = $("#fieldIdCont").val();
    var name = $("#fieldIdCont").find('option:selected').text();

    if (field !=="") {

        var mod = {
            Field: field
        };


        $.ajax({

            url: '/Wave/Comboshow/',
            method: 'post',
            data: mod,
            success: function (data) {

                console.log(data);

                if (data.length > 0) {



                    var listOpt = "";

                    $.each(data, function (i, item) {
                        console.log(item);
                        listOpt += '<option value="' + item.Id + '">' + item.Value + '</option>';
                    });

                    var newList = `<div class="form-group row grupp" style="margin-bottom:5px;">
                                   <div class="col-md-3 nobott">
                                    <label>`+ name + `:</label>
                                   </div> 
                                   <div class="col-md-5 clist nobott">
                                       <select class="form-control col-md-8 show-tick valFilt" id="` + field + `">
                                                <option value="">-- Please select --</option>
                                                `+ listOpt + `
                                       </select>
                                   </div>
                                   
                                   <div class="col-md-1 nobott">
                                       <button type="button" class="  btnDelFilt btn btn-table bg-deep-orange waves-effect" >
                                                <i class="material-icons">remove</i>
                                       </button>
                                   </div>
                                   </div>`;

                    $("#contConfPar").append(newList);

                } else {
                    console.log(mod);
                    $.ajax({
                        url: '/Wave/Nameshow/', method: 'post', data: mod, success: function (data) {

                            var newTextb = `<div class="form-group row grupp" style="margin-bottom:5px;">
                                   <div class="col-md-3 nobott">
                                    <label>`+ name + `:</label>
                                   </div> 
                                   <div class="col-md-5 nobott">
                                       <input type="text" class="form-control col-md-8 show-tick valFilt textbox" id="` + data + `" style="border-bottom: 1px solid #ddd"/>
                                      
                                   </div>
                                  
                                   <div class="col-md-1 nobott">
                                       <button type="button" class="  btnDelFilt btn btn-table bg-deep-orange waves-effect">
                                                <i class="material-icons">remove</i>
                                       </button>
                                   </div>
                                   </div>`;

                            $("#contConfPar").append(newTextb);

                        }
                    });

                }

            }
        });

    }



});





                       //===================== End Wizzard Container===

                       //==================== Wizzard Line===================
$(document).on("click", "#addCustomizeL", function (e) {

    e.preventDefault();


    var field = $("#fieldIdLine").val();
    var name = $("#fieldIdLine").find('option:selected').text();

    if (field !== "") {

        var mod = {
            Field: field
        };


        $.ajax({

            url: '/Wave/Comboshow/',
            method: 'post',
            data: mod,
            success: function (data) {

                console.log(data);

                if (data.length > 0) {



                    var listOpt = "";

                    $.each(data, function (i, item) {
                        console.log(item);
                        listOpt += '<option value="' + item.Id + '">' + item.Value + '</option>';
                    });

                    var newList = `<div class="form-group row grupp" style="margin-bottom:5px;">
                                   <div class="col-md-3 nobott">
                                    <label>`+ name + `:</label>
                                   </div> 
                                   <div class="col-md-5 clist nobott">
                                       <select class="form-control col-md-8 show-tick valFilt" id="` + field + `">
                                                <option value="">-- Please select --</option>
                                                `+ listOpt + `
                                       </select>
                                   </div>
                                   
                                   <div class="col-md-1 nobott">
                                       <button type="button" class="  btnDelFilt btn btn-table bg-deep-orange waves-effect" >
                                                <i class="material-icons">remove</i>
                                       </button>
                                   </div>
                                   </div>`;

                    $("#lineConfPar").append(newList);

                } else {
                    console.log(mod);
                    $.ajax({
                        url: '/Wave/Nameshow/', method: 'post', data: mod, success: function (data) {

                            var newTextb = `<div class="form-group row grupp" style="margin-bottom:5px;">
                                   <div class="col-md-3 nobott">
                                    <label>`+ name + `:</label>
                                   </div> 
                                   <div class="col-md-5 nobott">
                                       <input type="text" class="form-control col-md-8 show-tick valFilt textbox" id="` + data + `" style="border-bottom: 1px solid #ddd"/>
                                      
                                   </div>
                                  
                                   <div class="col-md-1 nobott">
                                       <button type="button" class="  btnDelFilt btn btn-table bg-deep-orange waves-effect">
                                                <i class="material-icons">remove</i>
                                       </button>
                                   </div>
                                   </div>`;

                            $("#lineConfPar").append(newTextb);

                        }
                    });

                }

            }
        });

    }



});



                       //==================== End Wizzard Line ==============


function gatherData() {

    var arr = [];

    $(".valRow").each(function () {
        var name = "";
        $(this).siblings().each(function () {
            if ($(this).hasClass("idRow")) {
                name = $(this).html().trim();

            }
        });
        if ($(this).html() !== "") {

            if ($(this).hasClass("text")) {

                var Elements = {
                    Name: name,
                    Val: $(this).html(),
                    Type: "text",
                    Percent: 100
                };
                arr.push(Elements);

            } else {
                var Elements2 = {
                    Name: name,
                    Val: $(this).attr("id"),
                    Type: "list",
                    //Percent: $(this).parent().siblings().find('.valPerc').val()
                    Percent: 100

                };
                arr.push(Elements2);
            }


            

        }
       


    });
    var noWave = false;
    var savFile = false;
    if ($("#NoWave").is(':checked')) {
        noWave = true;
    }
    if ($("#SavFile").is(':checked')) {
        savFile = true;
    }
    var model = {
        SavFile: savFile,
        NoWave: noWave,
        NOrd: $("#AmntOrd").val(),
        NCont: $("#AmntCont").val(),
        NLine: $("#AmntLine").val(),
        FiltValues: arr

    };

    $.ajax({
        url: '/Wave/WaveGen/',
        method: 'post',  // post
        data: model,
        beforeSend: function () {
            $('.page-loader-wrapper').fadeIn();
        },
        success: function (response) {
            if (response !== "error") {

                $("#updAreaW").append("<h4>" + response + "</h4>");
                $("#waveCModal").modal();
                $("#clModalW").attr('onclick', 'location.href = \'/Wave/Index\'');

            }
            console.log(response);
            $('.page-loader-wrapper').fadeOut();
        }
    });
}



//========================================= End of Wave Generation Area ====================================================================

//========================================== Grid ==========================================================================================


var editorDrop = `<input class="form-control " placeholder="col-sm-3" type="text">`;
var prevVal = "";
$(".btnEditRow").on('click', function () {
    console.log("clicked");
    $(this).siblings().each(function () {
        if ($(this).hasClass("btnEditRow") || $(this).hasClass("btnDelRow"))
        {
            $(this).addClass("hidden");
        } else if ($(this).hasClass("btnSavRow") || $(this).hasClass("btnCanRow"))
        {
            $(this).removeClass("hidden");
        }
        
    });
    $(this).addClass("hidden");
    var idR = "";
    var mod = {
        Field: 0
    }
    

    $(this).parent().siblings().each(function () {
        if ($(this).hasClass("ID")) {
            idR = $(this).html();
            mod.Field = idR;
        }
       
        if ($(this).hasClass("valRow")) {

            var val = $(this).html();
            prevVal = val;
            var $updArea = $(this);
            $(this).empty();
            //$(this).append(`<input class="form- control " placeholder="Value" type="text" value="`+val+`">`);
            
            $.ajax({

                url: '/Wave/Comboshow/',
                method: 'post',
                data: mod,
                success: function (data) {

                    console.log(data);

                    if (data.length > 0) {



                        var listOpt = "";

                        $.each(data, function (index, value) {
                            console.log($(this));
                            listOpt += '<option value="' + value.Id + '">' + value.Value + '</option>';
                        });

                        var newList = `<select class="form-control col-md-8 show-tick valFilt focusing" id="` + idR + `">
                                                  `+ listOpt + `
                                       </select>`;

                        $updArea.append(newList);

                    } else {
                        console.log(mod);
                        $.ajax({
                            url: '/Wave/Nameshow/',
                            method: 'post',
                            data: mod,
                            success: function (data) {

                                var newTextb = `<input type="text" class="form-control col-md-8 show-tick valFilt focusing textbox" id="` + data + `" style="border-bottom: 1px solid #ddd" value= "` + prevVal+`"/>  `;

                                $updArea.append(newTextb);

                            }
                        })

                    }

                }
            });

        }

    });

});


$(".btnCanRow").on("click", function () {
    $(this).siblings().each(function () {
        if ($(this).hasClass("btnEditRow") || $(this).hasClass("btnDelRow")) {
            $(this).removeClass("hidden");
        } else if ($(this).hasClass("btnSavRow") || $(this).hasClass("btnCanRow")) {
            $(this).addClass("hidden");
        }

    });
    $(this).addClass("hidden");


    $(this).parent().siblings().each(function () {

        if ($(this).hasClass("valRow")) {
            
            $(this).empty();

            $(this).append(prevVal);

        }

    });
});

$(".btnSavRow").on("click", function () {
       saveRow($(this));
});

function saveRow(selector) {
    $(selector).siblings().each(function () {
        if ($(this).hasClass("btnEditRow") || $(this).hasClass("btnDelRow")) {
            $(this).removeClass("hidden");
        } else if ($(this).hasClass("btnSavRow") || $(this).hasClass("btnCanRow")) {
            $(this).addClass("hidden");
        }

    });
    $(selector).addClass("hidden");


    $(selector).parent().siblings().each(function () {

        if ($(this).hasClass("valRow")) {
            var value = "";
            if ($(this).children().first().is("select")) {
                value = $(this).children().first().find(":selected").text();
                var id = $(this).children().first().find(":selected").val();
                $(this).attr("ID", id);
                $(this).addClass("list");
            } else {
                value = $(this).children().first().val();
                $(this).addClass("text");
            }

            $(this).empty();

            $(this).append(value);

        }

    });
};

$(document).on("focusout", ".focusing", function (e) {
    e.preventDefault();
    console.log("no focus");
    console.log($(this));
    $(this).parent().siblings().each(function () {
        if ($(this).hasClass("contRow")) {
            $(this).children().each(function () {
                if ($(this).hasClass("btnEditRow") || $(this).hasClass("btnDelRow")) {
                    $(this).removeClass("hidden");
                } else if ($(this).hasClass("btnSavRow") || $(this).hasClass("btnCanRow")) {
                    $(this).addClass("hidden");
                }

            });
        }
       

    });
   // $(this).addClass("hidden");

    var parent = $(this).parent();
   

        if ($(parent).hasClass("valRow")) {
            var value = "";
            if ($(parent).children().first().is("select")) {
                value = $(parent).children().first().find(":selected").text();
                var id = $(parent).children().first().find(":selected").val();
                $(parent).attr("ID", id);
                $(parent).addClass("list");
            } else {
                value = $(parent).children().first().val();
                $(parent).addClass("text");
            }

            $(parent).empty();

            $(parent).append(value);

        }

  

});

$(".btnDelRow").on("click", function () {
    console.log("clicked");
    $(this).parent().siblings().each(function () {

        if ($(this).hasClass("valRow")) {

            $(this).empty();

        }

    });

});

//========================================== End of Grid ===================================================================================
function loaded(selector, callback) {
    //trigger after page load.
    $(function () {
        callback($(selector));
    });
    //trigger after page update eg ajax event or jquery insert.
    $(document).on('DOMNodeInserted', selector, function () {
        callback($(this));
    });
}
var rangeSlider = document.getElementById('slider_range');


//====================== Detail Qty ========================================================
function checkDetailQty() {

    $(".idRow").each(function () {
        var value = "";
        var insert = `<tr>

                                                            <td class="ID hidden">26</td>
                                                            <td class="idRow"> wmsLocationId</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td class="ID hidden">27</td>
                                                            <td class="idRow"> wmsContainerId</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td class="ID hidden">28</td>
                                                            <td class="idRow"> wmsPickZone</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td class="ID hidden">29</td>
                                                            <td class="idRow"> sku</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td class="ID hidden">30</td>
                                                            <td class="idRow"> skuDesc</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td class="ID hidden">31</td>
                                                            <td class="idRow"> requestedQty</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td class="ID hidden">32</td>
                                                            <td class="idRow"> actualQty</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td class="ID hidden">33</td>
                                                            <td class="idRow"> productCode</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td class="ID hidden">34</td>
                                                            <td class="idRow"> aggregatedWeight</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td class="ID hidden">35</td>
                                                            <td class="idRow"> unitWeight</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td class="ID hidden">36</td>
                                                            <td class="idRow"> user</td>
                                                            <td class="valRow"></td>
                                                            <td class="contRow">
                                                                <button type="button" class=" btnEditRow btn btn-table btn-success waves-effect">
                                                                    <i class="material-icons">edit</i>
                                                                </button>
                                                                <button type="button" class=" btnDelRow btn btn-table bg-deep-orange waves-effect">
                                                                    <i class="material-icons ">delete</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnSavRow btn btn-table bg-green waves-effect">
                                                                    <i class="material-icons">check</i>
                                                                </button>
                                                                <button type="button" class=" hidden btnCanRow btn btn-table bg-red waves-effect">
                                                                    <i class="material-icons ">cancel</i>
                                                                </button>
                                                            </td>
                                                        </tr>
`;
     
          if ($(this).html().trim() === "detailCount")
          {
              value = $(this).siblings(".valRow").html();
              console.log(value);
              for (var i = 1; i < value; i++) {
                  $("#TableBodyContC").append(insert);
              }
          }


    });

}



//=============================================================================================


