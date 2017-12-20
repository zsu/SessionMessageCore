$().ready(function () {
    handleAjaxMessages();
    displayMessages();
});
function displayMessages() {
    var messagewrapper = $('#messagewrapper');
    var messageboxstatusbar = $('#messageboxstatusbar');
    var messageboxmodal = $('#messageboxmodal');
    if (messagewrapper.children().length > 0) {
        if (messageboxstatusbar && messageboxstatusbar.children().length > 0) {
            if(typeof toastr != 'undefined')
                {
                  messageboxstatusbar.hide();
		  /*toastr.options = {
		closeButton: false,
                newestOnTop: false,
                progressBar: false,
                positionClass: 'toast-top-right',
                timeOut: '5000',
                extendedTimeOut: '0',
				showMethod: 'fadeIn',
				hideMethod: 'fadeOut',
				closeMethod: 'fadeOut'};*/
		messageboxstatusbar.children().each(function() {
                  var caption=$(this).attr('caption');
                  var message=$(this).html();
                  switch($(this).attr('type'))
                    {
                        case 'Success':
                            toastr.success(message,caption);                        
                            break;
                        case 'Error':
                            toastr.error(message,caption);                        
                            break;
                        case 'Warning':
                            toastr.warning(message,caption);                        
                            break;
                        case 'Info':
                            toastr.info(message,caption);                        
                            break;

                  }
                  $(this).remove();
                });
                }
              else
              {
                var timeoutId;
                messagewrapper.mouseenter(function () {
                    if (timeoutId) {
                        clearTimeout(timeoutId);
                        messagewrapper.stop(true).css('opacity', 1).show();
                    }
                }).mouseleave(function () {
                    timeoutId = setTimeout(function () {
                        messagewrapper.slideUp('slow');
                    }, 5000);
                });
                messagewrapper.show();
                // display status message for 5 sec only
                timeoutId = setTimeout(function () {
                    messagewrapper.slideUp('slow');
                    clearMessages();
                }, 5000);
                $(document).click(function () {
                    clearMessages();
                });
               }
        }
        if (messageboxmodal && messageboxmodal.children().length > 0) {
            messageboxmodal.dialog({
                bgiframe: true,
                autoOpen: false,
                modal: true,
                title: 'Message',
                close: function (event, ui) {
                    try {
                        $(this).dialog('destroy').remove();
                        clearMessages();
                        return true;
                    }
                    catch (e) {
                        return true;
                    }
                }
            });
            messageboxmodal.dialog('open');
            messagewrapper.show();
        }
    }
    else {
        messagewrapper.hide();
    }
}

function clearMessages() {
    $('#messagewrapper').fadeOut(500, function () {
        $('#messagewrapper').empty();
    });
}

function handleAjaxMessages() {
    $(document).ajaxSuccess(function (event, request) {
        if (request.getResponseHeader('FORCE_REDIRECT') !== null) {
            window.location = request.getResponseHeader('FORCE_REDIRECT');
            return;
        }
        checkAndHandleMessageFromHeader(request);
    })
    //.ajaxError(function (event, request) {
    //    if (request.getResponseHeader('FORCE_REDIRECT') !== null) {
    //        window.location = request.getResponseHeader('FORCE_REDIRECT');
    //        return;
    //    }
    //    var responseMessage, exception;
    //    try {//Error handling for POST calls
    //        var jsonResult = JSON.parse(request.responseText);
    //        if (jsonResult && jsonResult.Message) {
    //            responseMessage = jsonResult.Message;
    //            if (jsonResult.ExceptionMessage) {
    //                exception = 'Message: ' + jsonResult.Message + ' Exception: ' + jsonResult.ExceptionMessage;
    //                if (jsonResult.StackTrace)
    //                    exception += jsonResult.StackTrace;
    //            }
    //        }
    //    }

    //    catch (ex) {//Error handling for GET calls
    //        if (request.responseText)
    //            responseMessage = request.responseText;
    //        else
    //            responseMessage = 'Status: '' + request.statusText + ''. Error code: ' + request.status;
    //    }
    //    if (exception)
    //        log.error(exception);
    //    else
    //        log.error(responseMessage);
    //    //var message = '<div id='messagebox' behavior=' + '2' + ' class='messagebox ' + 'error' + ''>' + responseMessage + '</div>';
    //    //displayMessage(message, 'error', 2);
    //});
}

function checkAndHandleMessageFromHeader(request) {
    var msg = request.getResponseHeader('X-Message');
    if (msg) {
        displayMessage(msg);
    }
    msg = request.getResponseHeader('X-ModalMessage');
    if (msg) {
        displayModalMessage(msg);
    }
}
function displayMessage(message) {
    var jsonResult = JSON.parse(message);
    if (jsonResult) {
        clearMessages();
        jQuery('<div/>', {
            id: 'messageboxstatusbar',
            class: 'messagebox'
        }).appendTo('#messagewrapper');
        var messageboxstatusbar = $('#messageboxstatusbar');
        //var messageboxmodal = $('#messageboxmodal');
        var loaded = false;
        //if ((messageboxstatusbar && messageboxstatusbar.children().length > 0) || (messageboxmodal && messageboxmodal.children().length > 0)) {
        //    loaded = true;
        //}
        $.each(jsonResult, function (i, item) {
            if ((messageboxstatusbar && messageboxstatusbar.children().length > 0) && item.Key) {
                if (messageboxstatusbar.children('div[key=' + item.Key + ']').length != 0)
                    return true;
            }
            jQuery('<div/>', {
                class: 'messagebox ' + item.Type,
                text: item.Message,
                key: item.Key,
                type:item.Type,
                caption:item.Caption
            }).appendTo('#messageboxstatusbar');
        });
        displayMessages();
    }
}
function displayModalMessage(message) {
    var jsonResult = JSON.parse(message);
    if (jsonResult) {
        jQuery('<div/>', {
            id: 'messageboxmodal',
            class: 'messagebox'
        }).appendTo('#messagewrapper');
        //var messageboxstatusbar = $('#messageboxstatusbar');
        var messageboxmodal = $('#messageboxmodal');
        var loaded = false;
        //if ((messageboxstatusbar && messageboxstatusbar.children().length > 0) || (messageboxmodal && messageboxmodal.children().length > 0)) {
        //    loaded = true;
        //}
        $.each(jsonResult, function (i, item) {
            if ((messageboxmodal && messageboxmodal.children().length > 0) && item.Key) {
                if (messageboxmodal.children('div[key=' + item.Key + ']').length != 0)
                    return true;
            }
            jQuery('<div/>', {
                class: 'messagebox ' + item.Type.toLowerCase(),
                text: item.Message,
                key: item.Key
            }).appendTo('#messageboxmodal');
        });
        displayMessages();
    }
}